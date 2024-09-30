using System.Runtime.Serialization;

namespace Orleans.Indexing
{
    /// <summary>
    /// This exception is thrown when an indexing configuration exception is encountered.
    /// </summary>
    [Serializable]
    [GenerateSerializer]
    [Alias("Orleans.Indexing.IndexConfigurationException")]
    public class IndexConfigurationException : IndexException
    {
        public IndexConfigurationException(string message) : base(message)
        {
        }

        [Obsolete]
        protected IndexConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
