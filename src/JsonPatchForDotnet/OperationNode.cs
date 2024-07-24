using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace JsonPatchForDotnet
{
    public class OperationNode : IOperationNode
    {
        public object Instance { get; private set; }
        
        public PropertyInfo? SourceProperty { get; set; }
        
        public PropertyInfo TargetProperty { get; set; }
        
        public object? PreviousValue { get; private set; }

        public object? Value { get; private set; }
        
        public Exception? OperationException { get; set; }

        public OperationNode(
            object instance,
            PropertyInfo targetPropertyInfo,
            PropertyInfo? sourcePropertyInfo,
            object? value)
        {
            Instance = instance;
            TargetProperty = targetPropertyInfo;
            SourceProperty = sourcePropertyInfo;
            Value = value;
            PreviousValue = targetPropertyInfo.GetValue(instance);
        }
        
        public virtual async Task<bool> TryApplyAsync()
        {
            try
            {
                await OperationStrategy.ApplyAsync(this);
                return true;
            }
            catch (Exception e)
            {
                OperationException = e;
                return false;
            }
        }

        public virtual async Task<bool> TryRevertAsync()
        {
            try
            {
                await OperationStrategy.RevertAsync(this);
                return true;
            }
            catch (Exception e)
            {
                OperationException = e;
                return false;
            }
        }
        
        public IOperationStrategy OperationStrategy { get; set; }
    }
}