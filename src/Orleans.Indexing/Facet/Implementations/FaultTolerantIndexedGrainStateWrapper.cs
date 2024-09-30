namespace Orleans.Indexing.Facet
{
    /// <summary>
    /// A wrapper around a user-defined state, TGrainState, which extends IndexableGrainStateWrapper to add fields for fault-tolerant indexing
    /// </summary>
    /// <typeparam name="TGrainState">the type of user state</typeparam>
    [Serializable]
    [GenerateSerializer]
    [Alias("Orleans.Indexing.Facet.FaultTolerantIndexedGrainStateWrapper`1")]
    public class FaultTolerantIndexedGrainStateWrapper<TGrainState> : IndexedGrainStateWrapper<TGrainState>
        where TGrainState : new()
    {
        /// <summary>
        /// Points to the in-flight indexing workflowsIds
        /// </summary>
        [Id(0)]
        internal HashSet<Guid> ActiveWorkflowsSet = null;

        /// There's a fixed mapping (e.g., a hash function) from a GrainReference to an <see cref="IndexWorkflowQueueGrainService"/>
        /// instance. Each Indexable Grain Interface IG has a property workflowQueue whose value, [grain-interface-type-name + sequence number],
        /// identifies the <see cref="IndexWorkflowQueueGrainService"/> grain that processes index updates on IG's behalf.
        [Id(1)]
        internal IDictionary<Type, IIndexWorkflowQueue> WorkflowQueues = null;
    }
}
