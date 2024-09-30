using System;

namespace Orleans.Indexing.Tests
{
    [Serializable]
    [GenerateSerializer]
    [Alias("Orleans.Indexing.Tests.TestMultiIndexState")]
    public class TestMultiIndexState : ITestMultiIndexState
    {
        #region ITestMultiIndexState
        [Id(0)]
        public int UniqueInt { get; set; }
        [Id(1)]
        public string UniqueString { get; set; }
        [Id(2)]
        public int NonUniqueInt { get; set; }
        [Id(3)]
        public string NonUniqueString { get; set; }
        #endregion ITestMultiIndexState

        #region Not Indexed
        [Id(4)]
        public string UnIndexedString { get; set; }
        #endregion Not Indexed
    }
}
