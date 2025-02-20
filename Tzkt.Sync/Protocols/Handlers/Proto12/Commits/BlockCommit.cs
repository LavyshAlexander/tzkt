﻿using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tzkt.Data.Models;

namespace Tzkt.Sync.Protocols.Proto12
{
    class BlockCommit : ProtocolCommit
    {
        public Block Block { get; private set; }

        public BlockCommit(ProtocolHandler protocol) : base(protocol) { }

        public virtual async Task Apply(JsonElement rawBlock)
        {
            var header = rawBlock.Required("header");
            var metadata = rawBlock.Required("metadata");

            var level = header.RequiredInt32("level");
            var proposer = Cache.Accounts.GetDelegate(metadata.RequiredString("proposer"));
            var producer = Cache.Accounts.GetDelegate(metadata.RequiredString("baker"));
            var protocol = await Cache.Protocols.GetAsync(rawBlock.RequiredString("protocol"));
            var events = BlockEvents.None;

            if (protocol.IsCycleStart(level))
                events |= BlockEvents.CycleBegin;
            else if (protocol.IsCycleEnd(level))
                events |= BlockEvents.CycleEnd;

            if (protocol.FirstLevel == level)
                events |= BlockEvents.ProtocolBegin;
            else if (protocol.Hash != metadata.RequiredString("next_protocol"))
                events |= BlockEvents.ProtocolEnd;

            if (metadata.RequiredArray("deactivated").Count() > 0)
                events |= BlockEvents.Deactivations;

            if (level % protocol.BlocksPerSnapshot == 0)
                events |= BlockEvents.BalanceSnapshot;

            var payloadRound = header.RequiredInt32("payload_round");
            var balanceUpdates = metadata.RequiredArray("balance_updates").EnumerateArray();
            var rewardUpdate = balanceUpdates.FirstOrDefault(x => x.RequiredString("kind") == "minted" && x.RequiredString("category") == "baking rewards");
            var bonusUpdate = balanceUpdates.FirstOrDefault(x => x.RequiredString("kind") == "minted" && x.RequiredString("category") == "baking bonuses");

            Block = new Block
            {
                Id = Cache.AppState.NextOperationId(),
                Hash = rawBlock.RequiredString("hash"),
                Cycle = protocol.GetCycle(level),
                Level = level,
                ProtoCode = protocol.Code,
                Protocol = protocol,
                Timestamp = header.RequiredDateTime("timestamp"),
                PayloadRound = payloadRound,
                BlockRound = payloadRound,
                Proposer = proposer,
                ProposerId = proposer.Id,
                ProducerId = producer.Id,
                Events = events,
                Reward = rewardUpdate.ValueKind == JsonValueKind.Undefined ? 0 : -rewardUpdate.RequiredInt64("change"),
                Bonus = bonusUpdate.ValueKind == JsonValueKind.Undefined ? 0 : -bonusUpdate.RequiredInt64("change"),
                LBEscapeVote = header.RequiredBool("liquidity_baking_escape_vote"),
                LBEscapeEma = metadata.RequiredInt32("liquidity_baking_escape_ema")
            };

            #region determine block round
            if (Block.ProposerId != Block.ProducerId)
            {
                var blockRound = (await Cache.BakingRights.GetAsync(Block.Cycle, Block.Level))
                    .Where(x => x.Type == BakingRightType.Baking)
                    .OrderBy(x => x.Round)
                    .SkipWhile(x => x.Round < Block.PayloadRound)
                    .FirstOrDefault(x => x.BakerId == Block.ProducerId)?
                    .Round ?? -1;

                if (blockRound == -1)
                {
                    var cycle = await Db.Cycles.FirstAsync(x => x.Index == Block.Cycle);
                    var sampler = await Sampler.CreateAsync(Proto, Block.Cycle);
                    blockRound = RightsGenerator.EnumerateBakingRights(sampler, cycle, Block.Level, 9_999_999)
                        .SkipWhile(x => x.Round < Block.PayloadRound)
                        .First(x => x.Baker == Block.ProducerId)
                        .Round;
                }

                Block.BlockRound = blockRound;
            }
            #endregion

            Db.TryAttach(proposer);
            proposer.Balance += Block.Reward;
            proposer.StakingBalance += Block.Reward;
            proposer.BlocksCount++;

            #region set baker active
            var newDeactivationLevel = proposer.Staked ? GracePeriod.Reset(Block) : GracePeriod.Init(Block);
            if (proposer.DeactivationLevel < newDeactivationLevel)
            {
                if (proposer.DeactivationLevel <= Block.Level)
                    await UpdateDelegate(proposer, true);

                Block.ResetBakerDeactivation = proposer.DeactivationLevel;
                proposer.DeactivationLevel = newDeactivationLevel;
            }
            #endregion

            Db.TryAttach(producer);
            producer.Balance += Block.Bonus;
            producer.StakingBalance += Block.Bonus;
            if (producer.Id != proposer.Id)
            {
                producer.BlocksCount++;

                #region set proposer active
                newDeactivationLevel = producer.Staked ? GracePeriod.Reset(Block) : GracePeriod.Init(Block);
                if (producer.DeactivationLevel < newDeactivationLevel)
                {
                    if (producer.DeactivationLevel <= Block.Level)
                        await UpdateDelegate(producer, true);

                    Block.ResetProposerDeactivation = producer.DeactivationLevel;
                    producer.DeactivationLevel = newDeactivationLevel;
                }
                #endregion
            }

            Db.TryAttach(protocol); // if we don't attach it, ef will recognize it as 'added'
            if (Block.Events.HasFlag(BlockEvents.ProtocolEnd))
                protocol.LastLevel = Block.Level;

            Db.Blocks.Add(Block);
            Cache.Blocks.Add(Block);
        }

        public virtual async Task Revert(Block block)
        {
            Block = block;
            Block.Protocol ??= await Cache.Protocols.GetAsync(block.ProtoCode);
            Block.Proposer ??= Cache.Accounts.GetDelegate(block.ProposerId);
            
            var proposer = Block.Proposer;
            Db.TryAttach(proposer);
            proposer.Balance -= Block.Reward;
            proposer.StakingBalance -= Block.Reward;
            proposer.BlocksCount--;

            #region reset baker activity
            if (Block.ResetBakerDeactivation != null)
            {
                if (Block.ResetBakerDeactivation <= Block.Level)
                    await UpdateDelegate(proposer, false);

                proposer.DeactivationLevel = (int)Block.ResetBakerDeactivation;
            }
            #endregion

            var producer = Cache.Accounts.GetDelegate(block.ProducerId);
            Db.TryAttach(producer);
            producer.Balance -= Block.Bonus;
            producer.StakingBalance -= Block.Bonus;
            if (producer.Id != proposer.Id)
            {
                producer.BlocksCount--;

                #region reset proposer activity
                if (Block.ResetProposerDeactivation != null)
                {
                    if (Block.ResetProposerDeactivation <= Block.Level)
                        await UpdateDelegate(producer, false);

                    producer.DeactivationLevel = (int)Block.ResetProposerDeactivation;
                }
                #endregion
            }

            Db.Blocks.Remove(Block);
        }
    }
}
