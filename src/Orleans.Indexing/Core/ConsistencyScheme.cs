namespace Orleans.Indexing
{
    [Flags]
    internal enum ConsistencyScheme
    {
        Workflow = 1,
        FaultTolerantWorkflow = 3,
        NonFaultTolerantWorkflow = 5,

        Transactional = 1024
    }
}
