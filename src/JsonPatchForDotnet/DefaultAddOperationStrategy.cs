using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace JsonPatchForDotnet
{
    public class DefaultAddOperationStrategy : IOperationStrategy
    {
        public Task ApplyAsync(IOperationNode operationNode)
        {
            operationNode.TargetProperty.SetValue(operationNode.Instance, operationNode.Value);
            return Task.CompletedTask;
        }

        public Task RevertAsync(IOperationNode operationNode)
        {
            operationNode.TargetProperty.SetValue(operationNode.Instance, operationNode.PreviousValue);
            return Task.CompletedTask;
        }
    }
}