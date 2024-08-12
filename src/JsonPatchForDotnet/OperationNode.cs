using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JsonPatchForDotnet
{
    public class OperationNode : IOperationNode
    {
        public IOperationStrategy OperationStrategy { get; set; }
        
        public object Instance { get; private set; }
        
        public PropertyInfo? SourceProperty { get; set; }
        
        public PropertyInfo TargetProperty { get; set; }
        
        public object? PreviousValue { get; private set; }

        public object? Value { get; private set; }
        
        public Exception? OperationException { get; set; }

        public OperationNode(
            IOperationStrategy operationStrategy,
            object instance,
            PropertyInfo targetPropertyInfo,
            PropertyInfo? sourcePropertyInfo = null,
            object? value = null)
        {
            OperationStrategy = operationStrategy;
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

        public static IList<OperationNode> FromOperation(Operation operation, object instance)
        {
            var operationNodes = new List<OperationNode>();
            
            IOperationStrategy? operationStrategy = null;
            PropertyInfo? sourcePropertyInfo = null;
            object? value = null;
            
            var (targetObjects, targetPropertyName) = GetTargetObjects(operation.Path, instance);
            var targetPropertyInfo = instance.GetType().GetProperty(targetPropertyName)
                                     ?? throw new ArgumentException(targetPropertyName);
            
            if (operation.OperationType == OperationType.Move
                || operation.OperationType == OperationType.Copy)
            {
                var (sourceObject, sourcePropertyName) = GetSourceObject(operation.From!, instance);
                sourcePropertyInfo = sourceObject.GetType().GetProperty(sourcePropertyName);
            }

            if (operation.OperationType == OperationType.Add
                || operation.OperationType == OperationType.Replace
                || operation.OperationType == OperationType.Test)
            {
                if (Utils.IsIList(targetPropertyInfo.PropertyType))
                {
                    var type = targetPropertyInfo.PropertyType.GetGenericArguments()[0];
                    value = operation.Value!.ToObject(type);
                }
                else
                {
                    value = operation.Value!.ToObject(targetPropertyInfo.PropertyType);
                }
            }
            
            switch (operation.OperationType)
            {
                case OperationType.Add:
                    operationStrategy = new DefaultAddOperationStrategy();
                    break; 
                case OperationType.Copy:
                    break;
                case OperationType.Move:
                    break;
                case OperationType.Remove:
                    break;
                case OperationType.Replace:
                    break;
                case OperationType.Test:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var targetObject in targetObjects)
            {
                operationNodes.Add(new OperationNode(
                    operationStrategy!,
                    targetObject,
                    targetPropertyInfo!,
                    sourcePropertyInfo,
                    value
                ));
            }

            return operationNodes;
        }

        internal static (IList<object>, string) GetTargetObjects(string operationPath, object instance)
        {
            var paths = operationPath.Split('.').ToArray();
            var lastPath = paths.Last();
            paths = paths.SkipLast(1).ToArray();

            var objects = (paths.Length > 0)
                ? instance.GetProperties(paths)
                : new object[] { instance };
            
            return (objects, lastPath);        
        }
        
        internal static (object, string) GetSourceObject(string operationPath, object instance)
        {
            var paths = operationPath.Split('.').ToArray();
            var lastPath = paths.Last();
            paths = paths.SkipLast(1).ToArray();

            var objects = (paths.Length > 0)
                ? instance.GetProperties(paths)
                : new object[] { instance };
            
            return (objects.Single(), lastPath);
        }
    }
}