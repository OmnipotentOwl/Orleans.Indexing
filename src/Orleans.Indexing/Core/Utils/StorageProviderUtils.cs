namespace Orleans.Indexing.Core.Utils;

public class StorageProviderUtils
{
    /// <summary>
    /// ETag of value "*" to match any etag for conditional table operations (update, nerge, delete).
    /// </summary>
    public const string ANY_ETAG = "*";

    public static int PositiveHash(int hash, int hashRange)
    {
        int positiveHash = ((hash % hashRange) + hashRange) % hashRange;
        return positiveHash;
    }
}