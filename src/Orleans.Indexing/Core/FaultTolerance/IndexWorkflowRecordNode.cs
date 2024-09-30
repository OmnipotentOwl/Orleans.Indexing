using System.Runtime.CompilerServices;
using System.Text;

namespace Orleans.Indexing
{
    /// <summary>
    /// A node in the linked list of workflowRecords.
    /// 
    /// This linked list makes the traversal more efficient.
    /// </summary>
    [Serializable]
    [GenerateSerializer]
    [Alias("Orleans.Indexing.IndexWorkflowRecordNode")]
    internal class IndexWorkflowRecordNode
    {
        [Id(0)]
        internal IndexWorkflowRecord WorkflowRecord;
        [Id(1)]
        internal IndexWorkflowRecordNode Prev = null;
        [Id(2)]
        internal IndexWorkflowRecordNode Next = null;

        /// <summary>
        /// This constructor creates a punctuation node
        /// </summary>
        public IndexWorkflowRecordNode() : this(null)
        {
        }

        public IndexWorkflowRecordNode(IndexWorkflowRecord workflow)
        {
            this.WorkflowRecord = workflow;
        }

        public void Append(IndexWorkflowRecordNode elem, ref IndexWorkflowRecordNode tail)
        {
            var tmpNext = this.Next;
            if (tmpNext != null)
            {
                elem.Next = tmpNext;
                tmpNext.Prev = elem;
            }
            elem.Prev = this;
            this.Next = elem;

            if (tail == this)
            {
                tail = elem;
            }
        }

        public IndexWorkflowRecordNode AppendPunctuation(ref IndexWorkflowRecordNode tail)
        {
            // We never append a punctuation to an existing punctuation; it should never be requested.
            if (this.IsPunctuation) throw new WorkflowIndexException("Adding a punctuation to a workflow queue that already has a punctuation is not allowed.");

            var punctuation = new IndexWorkflowRecordNode();
            this.Append(punctuation, ref tail);
            return punctuation;
        }

        public void Remove(ref IndexWorkflowRecordNode head, ref IndexWorkflowRecordNode tail)
        {
            if (this.Prev == null) head = this.Next;
            else
                this.Prev.Next = this.Next;

            if (this.Next == null) tail = this.Prev;
            else
                this.Next.Prev = this.Prev;

            this.Clean();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Clean()
        {
            this.WorkflowRecord = null;
            this.Next = null;
            this.Prev = null;
        }

        internal bool IsPunctuation => this.WorkflowRecord == null;

        public override string ToString()
        {
            int count = 0;
            var res = new StringBuilder();
            IndexWorkflowRecordNode curr = this;
            do
            {
                ++count;
                res.Append(curr.IsPunctuation ? "::Punc::" : curr.WorkflowRecord.ToString()).Append(",\n");
                curr = curr.Next;
            } while (curr != null);
            res.Append("Number of elements: ").Append(count);
            return res.ToString();
        }

        public string ToStringReverse()
        {
            int count = 0;
            var res = new StringBuilder();
            IndexWorkflowRecordNode curr = this;
            do
            {
                ++count;
                res.Append(curr.IsPunctuation ? "::Punc::" : curr.WorkflowRecord.ToString()).Append(",\n");
                curr = curr.Prev;
            } while (curr != null);
            res.Append("Number of elements: ").Append(count);
            return res.ToString();
        }
    }
}
