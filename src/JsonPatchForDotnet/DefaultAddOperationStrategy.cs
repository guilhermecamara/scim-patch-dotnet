using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonPatchForDotnet
{
    public class DefaultAddOperationStrategy : IOperationStrategy
    {
        public Task ApplyAsync(IOperationNode operationNode)
        {
            if (operationNode.Value == null) 
                throw new ArgumentNullException(nameof(operationNode.Value));
            
            if (Utils.IsIList(operationNode.TargetProperty.PropertyType))
            {
                var type = operationNode.TargetProperty.PropertyType.GetGenericArguments()[0];
                var listType = typeof(List<>);
                var list = (IList?)operationNode.TargetProperty.GetValue(operationNode.Instance);
                if (list == null)
                {
                    var genericListType = listType.MakeGenericType(type);
                    list = (IList)Activator.CreateInstance(genericListType);
                    operationNode.TargetProperty.SetValue(operationNode.Instance, list);
                }
                list.Add(operationNode.Value);
            }
            else
            {
                operationNode.TargetProperty.SetValue(operationNode.Instance, operationNode.Value);
            }
            return Task.CompletedTask;
        }

        public Task RevertAsync(IOperationNode operationNode)
        {
            if (Utils.IsIList(operationNode.TargetProperty.PropertyType))
            {
                if (operationNode.PreviousValue != null)
                {
                    var list = (IList)operationNode.TargetProperty.GetValue(operationNode.Instance);
                    list.RemoveAt(list.Count - 1);
                }
                else
                {
                    operationNode.TargetProperty.SetValue(operationNode.Instance, null);
                }
            }
            else
            {
                operationNode.TargetProperty.SetValue(operationNode.Instance, operationNode.PreviousValue);
            }
            return Task.CompletedTask;
        }
    }
}