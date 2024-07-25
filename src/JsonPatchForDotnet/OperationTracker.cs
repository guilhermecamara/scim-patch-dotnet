using System.Collections.Generic;

namespace JsonPatchForDotnet
{
    public static class OperationTracker
    {
        public static IList<OperationNode> FromJson(object instance, string json)
        {
            var operationsTracker = new List<OperationNode>();

            var operations = Operations.FromJson(json);

            foreach (var operation in operations)
            {
                operationsTracker.AddRange(OperationNode.FromOperation(operation, instance));
            }

            return operationsTracker;
        }
    }
}