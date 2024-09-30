namespace Orleans.Indexing
{
    /// <summary>
    /// This class is a wrapper around another IMemberUpdate which adds an update mode.
    /// </summary>
    [Serializable]
    [GenerateSerializer]
    [Alias("Orleans.Indexing.MemberUpdateOverriddenMode")]
    internal class MemberUpdateOverriddenMode : IMemberUpdate
    {
        [Id(0)]
        private IMemberUpdate _update;

        [Id(1)]
        public IndexUpdateMode UpdateMode { get; }

        public MemberUpdateOverriddenMode(IMemberUpdate update, IndexUpdateMode indexUpdateMode)
        {
            this._update = update;
            this.UpdateMode = indexUpdateMode;
        }

        public object GetBeforeImage() => this._update.GetBeforeImage();

        public object GetAfterImage() => this._update.GetAfterImage();

        public IndexOperationType OperationType => this._update.OperationType;

        public override string ToString() => MemberUpdate.ToString(this);
    }
}
