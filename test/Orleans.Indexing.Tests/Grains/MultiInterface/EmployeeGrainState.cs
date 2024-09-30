using System;

namespace Orleans.Indexing.Tests.MultiInterface
{
    [Serializable]
    [GenerateSerializer]
    [Alias("Orleans.Indexing.Tests.MultiInterface.EmployeeGrainState")]
    public class EmployeeGrainState : IEmployeeGrainState
    {
        #region IPersonProperties
        [Id(0)]
        public string Name { get; set; }
        [Id(1)]
        public int Age { get; set; }
        #endregion IPersonProperties

        #region IJobProperties
        [Id(2)]
        public string Title { get; set; }
        [Id(3)]
        public string Department { get; set; }
        #endregion IJobProperties

        #region IEmployeeProperties
        [Id(4)]
        public int EmployeeId { get; set; }
        #endregion IJobProperties

        #region IEmployeeGrainState - not indexed
        [Id(5)]
        public int Salary { get; set; }
        #endregion IEmployeeGrainState - not indexed
    }
}
