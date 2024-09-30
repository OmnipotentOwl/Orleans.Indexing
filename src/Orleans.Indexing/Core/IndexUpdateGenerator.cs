using System.Reflection;

namespace Orleans.Indexing
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [GenerateSerializer]
    [Alias("Orleans.Indexing.IndexUpdateGenerator")]
    internal class IndexUpdateGenerator : IIndexUpdateGenerator
    {
        [Id(0)]
        private PropertyInfo prop;
        [Id(1)]
        private object nullValue;

        public IndexUpdateGenerator(PropertyInfo prop)
        {
            this.prop = prop;
            this.nullValue = IndexUtils.GetNullValue(prop);
        }

        public IMemberUpdate CreateMemberUpdate(object gProps, object befImg)
        {
            object aftImg = gProps == null ? null : this.ExtractIndexImage(gProps);
            return new MemberUpdate(befImg, aftImg);
        }

        public IMemberUpdate CreateMemberUpdate(object aftImg)
            => new MemberUpdate(null, aftImg);

        public object ExtractIndexImage(object gProps)
        {
            var currentValue = this.prop.GetValue(gProps);
            return currentValue == null || this.nullValue == null
                ? currentValue
                : (currentValue.Equals(this.nullValue) ? null : currentValue);
        }
    }
}
