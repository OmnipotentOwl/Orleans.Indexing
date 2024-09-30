using Orleans.Concurrency;
using Orleans.Runtime;

namespace Orleans.Indexing
{
    [Reentrant]
    internal class ReincarnatedIndexWorkflowQueue : Grain, IIndexWorkflowQueue
    {
        internal static TimeSpan ACTIVE_FOR_A_DAY = TimeSpan.FromDays(1);
        private IndexWorkflowQueueBase _base;

        internal SiloIndexManager SiloIndexManager => IndexManager.GetSiloIndexManager(ref this.__siloIndexManager, this.ServiceProvider);
        private SiloIndexManager __siloIndexManager;

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            this.DelayDeactivation(ACTIVE_FOR_A_DAY);
            return base.OnActivateAsync(cancellationToken);
        }

        public Task Initialize(IIndexWorkflowQueue oldParentGrainService)
        {
            if (this._base == null)
            {
                GrainReference oldParentGrainServiceRef = oldParentGrainService.AsWeaklyTypedReference();
                string[] parts = oldParentGrainServiceRef.GetPrimaryKeyString().Split('-');
                if (parts.Length != 2)
                {
                    throw new WorkflowIndexException("The primary key for IndexWorkflowQueueGrainService should only contain a single special character '-', while it contains multiple." +
                                                     " The primary key is '" + oldParentGrainServiceRef.GetPrimaryKeyString() + "'");
                }

                Type grainInterfaceType = this.SiloIndexManager.CachedTypeResolver.ResolveType(parts[0]);
                int queueSequenceNumber = int.Parse(parts[1]);

                SystemTargetGrainId.TryParse(oldParentGrainServiceRef.GrainId, out var systemGrainId);

                this._base = new IndexWorkflowQueueBase(this.SiloIndexManager, grainInterfaceType, queueSequenceNumber,
                                                   systemGrainId.GetSiloAddress(),
                                                   isDefinedAsFaultTolerantGrain: true /*otherwise it shouldn't have reached here!*/,
                                                   parentFunc: () => this.AsWeaklyTypedReference(), recoveryGrainReference:oldParentGrainServiceRef);
            }
            return Task.CompletedTask;
        }

        public Task AddAllToQueue(Immutable<List<IndexWorkflowRecord>> workflowRecords)
            =>
                this._base.AddAllToQueue(workflowRecords);

        public Task AddToQueue(Immutable<IndexWorkflowRecord> workflowRecord)
            =>
                this._base.AddToQueue(workflowRecord);

        public Task<Immutable<List<IndexWorkflowRecord>>> GetRemainingWorkflowsIn(HashSet<Guid> activeWorkflowsSet)
            =>
                this._base.GetRemainingWorkflowsIn(activeWorkflowsSet);

        public Task<Immutable<IndexWorkflowRecordNode>> GiveMoreWorkflowsOrSetAsIdle()
            =>
                this._base.GiveMoreWorkflowsOrSetAsIdle();

        public Task RemoveAllFromQueue(Immutable<List<IndexWorkflowRecord>> workflowRecords)
            =>
                this._base.RemoveAllFromQueue(workflowRecords);
    }
}
