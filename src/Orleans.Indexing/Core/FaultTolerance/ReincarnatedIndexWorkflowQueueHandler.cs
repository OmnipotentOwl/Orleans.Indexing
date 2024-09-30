using Orleans.Concurrency;

namespace Orleans.Indexing
{
    [Reentrant]
    internal class ReincarnatedIndexWorkflowQueueHandler : Grain, IIndexWorkflowQueueHandler
    {
        private IIndexWorkflowQueueHandler _base;

        internal SiloIndexManager SiloIndexManager => IndexManager.GetSiloIndexManager(ref this.__siloIndexManager, this.ServiceProvider);
        private SiloIndexManager __siloIndexManager;

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            this.DelayDeactivation(ReincarnatedIndexWorkflowQueue.ACTIVE_FOR_A_DAY);
            return base.OnActivateAsync(cancellationToken);
        }

        public Task Initialize(IIndexWorkflowQueue oldParentGrainService)
        {
            if (this._base == null)
            {
                var oldParentGrainServiceRef = oldParentGrainService.AsWeaklyTypedReference();
                var parts = oldParentGrainServiceRef.GetPrimaryKeyString().Split('-');
                if (parts.Length != 2)
                {
                    throw new WorkflowIndexException("The primary key for IndexWorkflowQueueGrainService should only contain a single special character '-', while it contains multiple." +
                                                     " The primary key is '" + oldParentGrainServiceRef.GetPrimaryKeyString() + "'");
                }

                Type grainInterfaceType = this.SiloIndexManager.CachedTypeResolver.ResolveType(parts[0]);
                int queueSequenceNumber = int.Parse(parts[1]);

                SystemTargetGrainId.TryParse(oldParentGrainServiceRef.GrainId, out var systemGrainId);

                this._base = new IndexWorkflowQueueHandlerBase(this.SiloIndexManager, grainInterfaceType, queueSequenceNumber,
                                                          systemGrainId.GetSiloAddress(),
                                                          isDefinedAsFaultTolerantGrain: true /*otherwise it shouldn't have reached here!*/,
                                                          () => this.AsWeaklyTypedReference());
            }
            return Task.CompletedTask;
        }

        public Task HandleWorkflowsUntilPunctuation(Immutable<IndexWorkflowRecordNode> workflowRecordsHead)
            =>
                this._base.HandleWorkflowsUntilPunctuation(workflowRecordsHead);
    }
}
