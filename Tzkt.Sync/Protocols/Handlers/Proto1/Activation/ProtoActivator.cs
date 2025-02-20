﻿using System.Text.Json;
using System.Threading.Tasks;
using Tzkt.Data.Models;

namespace Tzkt.Sync.Protocols.Proto1
{
    partial class ProtoActivator : ProtocolCommit
    {
        public ProtoActivator(ProtocolHandler protocol) : base(protocol) { }

        public async Task Activate(AppState state, JsonElement rawBlock)
        {
            if (state.Level == 1) // bootstrap
            {
                var (protocol, parameters) = BootstrapProtocol(rawBlock);

                var accounts = await BootstrapAccounts(protocol, parameters);
                var (bakingRights, endorsingRights) = await BootstrapBakingRights(protocol, accounts);
                BootstrapCycles(protocol, accounts);
                BootstrapDelegatorCycles(protocol, accounts);
                BootstrapBakerCycles(protocol, accounts, bakingRights, endorsingRights);
                BootstrapSnapshotBalances(accounts);
                BootstrapVoting(protocol, accounts);
                await BootstrapCommitments(parameters);
                await ActivateContext(state);
            }
            else // upgrade
            {
                await UpgradeProtocol(state);
                await MigrateContext(state);
            }
        }

        public async Task Deactivate(AppState state)
        {
            if (state.Level == 1) // clear
            {
                await DeactivateContext(state);
                await ClearCommitments();
                await ClearVoting();
                await ClearSnapshotBalances();
                await ClearBakerCycles();
                await ClearDelegatorCycles();
                await ClearCycles();
                await ClearBakingRights();
                await ClearAccounts();
                await ClearProtocol();
            }
            else // downgrade
            {
                await RevertContext(state);
                await DowngradeProtocol(state);
            }
        }

        protected virtual Task ActivateContext(AppState state) => Task.CompletedTask;
        protected virtual Task DeactivateContext(AppState state) => Task.CompletedTask;
        protected virtual Task MigrateContext(AppState state) => Task.CompletedTask;
        protected virtual Task RevertContext(AppState state) => Task.CompletedTask;
    }
}
