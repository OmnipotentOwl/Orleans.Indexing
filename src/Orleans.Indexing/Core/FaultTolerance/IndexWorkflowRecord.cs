namespace Orleans.Indexing
{
    [Serializable]
    [GenerateSerializer]
    [Alias("Orleans.Indexing.IndexWorkflowRecord")]
    internal class IndexWorkflowRecord
    {
        /// <summary>
        /// The grain being indexed, which its ID is the first part of the workflowID
        /// </summary>
        [Id(0)]
        internal IIndexableGrain Grain { get; }

        /// <summary>
        /// The sequence number of update on the Grain, which is the second part of the workflowID
        /// </summary>
        [Id(1)]
        internal Guid WorkflowId { get; }

        /// <summary>
        /// The list of updated values to all updated indexed properties of the Grain
        /// </summary>
        [Id(2)]
        internal IReadOnlyDictionary<string, IMemberUpdate> MemberUpdates { get; }

        internal IndexWorkflowRecord(Guid workflowId, IIndexableGrain grain, IReadOnlyDictionary<string, IMemberUpdate> memberUpdates)
        {
            this.Grain = grain;
            this.WorkflowId = workflowId;
            this.MemberUpdates = memberUpdates;
        }

        public override bool Equals(object other)
            => other is IndexWorkflowRecord otherW && this.WorkflowId.Equals(otherW.WorkflowId);

        public override int GetHashCode() => this.WorkflowId.GetInvariantHashCode();

        public override string ToString() => string.Format("<Grain: {0}, WorkflowId: {1}>", this.Grain, this.WorkflowId);
    }
}
