using System.Collections.Generic;

namespace JsonPatchForDotnet
{
    public class OperationTracker
    {
        private IList<OperationNode> Nodes { get; set; }
    }
}