namespace Orleans.Indexing.Facet
{
    /// <summary>
    /// A wrapper around a user-defined state, TGrainState, which indicates whether the grain has been persisted.
    /// </summary>
    /// <typeparam name="TGrainState">the type of user state</typeparam>
    [Serializable]
    [GenerateSerializer]
    [Alias("Orleans.Indexing.Facet.IndexedGrainStateWrapper`1")]
    public class IndexedGrainStateWrapper<TGrainState>
        where TGrainState: new()
    {
        /// <summary>
        /// Indicates whether the grain was read from storage (used on startup to set null values).
        /// </summary>
        [Id(0)]
        public bool AreNullValuesInitialized;

        /// <summary>
        /// The actual user state.
        /// </summary>
        [Id(1)]
        public TGrainState UserState = (TGrainState)Activator.CreateInstance(typeof(TGrainState));

        internal void EnsureNullValues(IReadOnlyDictionary<string, object> propertyNullValues)
        {
            if (!this.AreNullValuesInitialized)
            {
                foreach (var propInfo in typeof(TGrainState).GetProperties())
                {
                    var nullValue = IndexUtils.GetNullValue(propInfo);
                    if (nullValue != null || propertyNullValues.TryGetValue(propInfo.Name, out nullValue))
                    {
                        propInfo.SetValue(this.UserState, nullValue);
                    }
                }
                this.AreNullValuesInitialized = true;
            }
        }
    }
}
