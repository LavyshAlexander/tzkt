﻿using Microsoft.EntityFrameworkCore;

using Tzkt.Data.Models;

namespace Tzkt.Data
{
    public class TzktContext : DbContext
    {
        #region app state
        public DbSet<AppState> AppState { get; set; }
        #endregion

        #region accounts
        public DbSet<Commitment> Commitments { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Delegate> Delegates { get; set; }
        public DbSet<User> Users { get; set; }
        #endregion

        #region blocks
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Protocol> Protocols { get; set; }
        public DbSet<Software> Software { get; set; }
        #endregion

        #region operations
        public DbSet<ActivationOperation> ActivationOps { get; set; }
        public DbSet<BallotOperation> BallotOps { get; set; }
        public DbSet<DelegationOperation> DelegationOps { get; set; }
        public DbSet<DoubleBakingOperation> DoubleBakingOps { get; set; }
        public DbSet<DoubleEndorsingOperation> DoubleEndorsingOps { get; set; }
        public DbSet<DoublePreendorsingOperation> DoublePreendorsingOps { get; set; }
        public DbSet<EndorsementOperation> EndorsementOps { get; set; }
        public DbSet<PreendorsementOperation> PreendorsementOps { get; set; }
        public DbSet<NonceRevelationOperation> NonceRevelationOps { get; set; }
        public DbSet<OriginationOperation> OriginationOps { get; set; }
        public DbSet<ProposalOperation> ProposalOps { get; set; }
        public DbSet<RevealOperation> RevealOps { get; set; }
        public DbSet<TransactionOperation> TransactionOps { get; set; }
        public DbSet<RegisterConstantOperation> RegisterConstantOps { get; set; }
        public DbSet<SetDepositsLimitOperation> SetDepositsLimitOps { get; set; }

        public DbSet<EndorsingRewardOperation> EndorsingRewardOps { get; set; }
        public DbSet<MigrationOperation> MigrationOps { get; set; }
        public DbSet<RevelationPenaltyOperation> RevelationPenaltyOps { get; set; }
        #endregion

        #region voting
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<VotingPeriod> VotingPeriods { get; set; }
        public DbSet<VotingSnapshot> VotingSnapshots { get; set; }
        #endregion

        #region baking
        public DbSet<Cycle> Cycles { get; set; }
        public DbSet<BakerCycle> BakerCycles { get; set; }
        public DbSet<DelegatorCycle> DelegatorCycles { get; set; }
        public DbSet<BakingRight> BakingRights { get; set; }
        public DbSet<SnapshotBalance> SnapshotBalances { get; set; }
        public DbSet<FreezerUpdate> FreezerUpdates { get; set; }
        #endregion

        #region quotes
        public DbSet<Quote> Quotes { get; set; }
        #endregion

        #region statistics
        public DbSet<Statistics> Statistics { get; set; }
        #endregion

        #region scripts
        public DbSet<Script> Scripts { get; set; }
        public DbSet<Storage> Storages { get; set; }
        #endregion

        #region bigmaps
        public DbSet<BigMap> BigMaps { get; set; }
        public DbSet<BigMapKey> BigMapKeys { get; set; }
        public DbSet<BigMapUpdate> BigMapUpdates { get; set; }
        #endregion

        #region tokens
        public DbSet<Token> Tokens { get; set; }
        public DbSet<TokenBalance> TokenBalances { get; set; }
        public DbSet<TokenTransfer> TokenTransfers { get; set; }
        #endregion

        public TzktContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region app state
            modelBuilder.BuildAppStateModel();
            #endregion

            #region accounts
            modelBuilder.BuildCommitmentModel();
            modelBuilder.BuildAccountModel();
            modelBuilder.BuildContractModel();
            modelBuilder.BuildDelegateModel();
            modelBuilder.BuildUserModel();
            #endregion

            #region block
            modelBuilder.BuildBlockModel();
            modelBuilder.BuildProtocolModel();
            modelBuilder.BuildSoftwareModel();
            #endregion

            #region operations
            modelBuilder.BuildActivationOperationModel();
            modelBuilder.BuildBallotOperationModel();
            modelBuilder.BuildDelegationOperationModel();
            modelBuilder.BuildDoubleBakingOperationModel();
            modelBuilder.BuildDoubleEndorsingOperationModel();
            modelBuilder.BuildDoublePreendorsingOperationModel();
            modelBuilder.BuildEndorsementOperationModel();
            modelBuilder.BuildPreendorsementOperationModel();
            modelBuilder.BuildNonceRevelationOperationModel();
            modelBuilder.BuildOriginationOperationModel();
            modelBuilder.BuildProposalOperationModel();
            modelBuilder.BuildRevealOperationModel();
            modelBuilder.BuildTransactionOperationModel();
            modelBuilder.BuildRegisterConstantOperationModel();
            modelBuilder.BuildSetDepositsLimitOperationModel();

            modelBuilder.BuildEndorsingRewardOperationModel();
            modelBuilder.BuildMigrationOperationModel();
            modelBuilder.BuildRevelationPenaltyOperationModel();
            #endregion

            #region voting
            modelBuilder.BuildProposalModel();
            modelBuilder.BuildVotingPeriodModel();
            modelBuilder.BuildVotingSnapshotModel();
            #endregion

            #region baking
            modelBuilder.BuildCycleModel();
            modelBuilder.BuildBakerCycleModel();
            modelBuilder.BuildDelegatorCycleModel();
            modelBuilder.BuildBakingRightModel();
            modelBuilder.BuildSnapshotBalanceModel();
            modelBuilder.BuildFreezerUpdateModel();
            #endregion

            #region quotes
            modelBuilder.BuildQuoteModel();
            #endregion

            #region statistics
            modelBuilder.BuildStatisticsModel();
            #endregion

            #region scripts
            modelBuilder.BuildScriptModel();
            modelBuilder.BuildStorageModel();
            #endregion

            #region bigmaps
            modelBuilder.BuildBigMapModel();
            modelBuilder.BuildBigMapKeyModel();
            modelBuilder.BuildBigMapUpdateModel();
            #endregion

            #region tokens
            modelBuilder.BuildTokenModel();
            modelBuilder.BuildTokenBalanceModel();
            modelBuilder.BuildTokenTransferModel();
            #endregion
        }
    }
}
