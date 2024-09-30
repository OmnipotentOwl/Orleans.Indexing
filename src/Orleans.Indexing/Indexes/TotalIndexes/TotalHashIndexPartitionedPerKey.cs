namespace Orleans.Indexing
{
    /// <summary>
    /// A simple implementation of a partitioned and persistent hash-index
    /// </summary>
    /// <typeparam name="K">type of hash-index key</typeparam>
    /// <typeparam name="V">type of grain interface that is being indexed</typeparam>
    [Serializable]
    [GenerateSerializer]
    [TransactionalIndexVariant(typeof(TotalHashIndexPartitionedPerKeyTransactional<,>))]
    [Alias("Orleans.Indexing.TotalHashIndexPartitionedPerKey`2")]
    public class TotalHashIndexPartitionedPerKey<K, V> : HashIndexPartitionedPerKey<K, V, ITotalHashIndexPartitionedPerKeyBucket<K, V>>,
                                                         ITotalIndex where V : class, IIndexableGrain
    {
        public TotalHashIndexPartitionedPerKey(IServiceProvider serviceProvider, string indexName, bool isUniqueIndex)
            : base(serviceProvider, indexName, isUniqueIndex)
        {
        }
    }

    [Serializable]
    [GenerateSerializer]
    [Alias("Orleans.Indexing.TotalHashIndexPartitionedPerKeyTransactional`2")]
    public class TotalHashIndexPartitionedPerKeyTransactional<K, V> : HashIndexPartitionedPerKey<K, V, ITotalHashIndexPartitionedPerKeyBucketTransactional<K, V>>,
                                                                      ITotalIndex where V : class, IIndexableGrain
    {
        public TotalHashIndexPartitionedPerKeyTransactional(IServiceProvider serviceProvider, string indexName, bool isUniqueIndex)
            : base(serviceProvider, indexName, isUniqueIndex, isTransactional:true)
        {
        }
    }
}
