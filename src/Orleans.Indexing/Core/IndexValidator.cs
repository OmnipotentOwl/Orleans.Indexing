namespace Orleans.Indexing
{
    public static class IndexValidator
    {
        public static void Validate(Type[] types)
        {
            var _ = ApplicationPartsIndexableGrainLoader.GetIndexRegistry(null, types);
        }
    }
}
