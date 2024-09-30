using System;

namespace Orleans.Indexing.Tests
{
    [Serializable]
    [GenerateSerializer]
    [Alias("Orleans.Indexing.Tests.PlayerGrainState")]
    public class PlayerGrainState : IPlayerProperties
    {
        [Id(0)]
        public string Email { get; set; }
        [Id(1)]
        public int Score { get; set; }
        [Id(2)]
        public string Location { get; set; }
    }
}
