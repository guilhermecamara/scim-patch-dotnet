using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ScimPatch.Extensions;

namespace ScimPatch
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
            switch (operation.OperationType)
            {
                case OperationType.Add:
                    return CreateAddOperations(operation, instance); 
                case OperationType.Copy:
                    break;
                case OperationType.Move:
                    break;
                case OperationType.Remove:
                    return CreateRemoveOperations(operation, instance);
                case OperationType.Replace:
                    break;
                case OperationType.Test:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            throw new NotImplementedException();
        }

        private static IList<OperationNode> CreateAddOperations(Operation operation, object instance)
        {
            // Strategy
            var operationStrategy = new DefaultAddOperationStrategy();
            
            // Source
            PropertyInfo? sourcePropertyInfo = null;
            
            // Target
            var (targetObjects, lastPath) = GetTargetObjects(operation.Path, instance);

            var (targetPropertyName, filter) = lastPath.GetRootPath();

            if (!string.IsNullOrEmpty(filter))
                throw new InvalidOperationException("Last path cannot have filter for add operations");
            
            var operationNodes = new List<OperationNode>();
            foreach (var targetObject in targetObjects)
            {
                var targetPropertyInfo = targetObject.GetType().GetProperty(targetPropertyName)
                                         ?? throw new ArgumentException(targetPropertyName);
                
                // Value
                object? value = null;
                if (Utils.IsIList(targetPropertyInfo.PropertyType))
                {
                    var type = targetPropertyInfo.PropertyType.GetGenericArguments()[0];
                    value = operation.Value!.ToObject(type);
                }
                else
                {
                    value = operation.Value!.ToObject(targetPropertyInfo.PropertyType);
                }
                
                // Nodes
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

        private static IList<OperationNode> CreateRemoveOperations(Operation operation, object instance)
        {
            // Strategy
            var operationStrategy = new DefaultRemoveOperationStrategy();
            
            // Source
            PropertyInfo? sourcePropertyInfo = null;
            
            // Target
            var (targetObjects, lastPath) = GetTargetObjects(operation.Path, instance);

            var (targetPropertyName, filter) = lastPath.GetRootPath();

            var operationNodes = new List<OperationNode>();
            foreach (var targetObject in targetObjects)
            {
                var targetPropertyInfo = targetObject.GetType().GetProperty(targetPropertyName)
                                         ?? throw new ArgumentException(targetPropertyName);
                
                // Value
                // This is always null for remove except on list with filter on last path
                object? value = null;
                
                if (!string.IsNullOrEmpty(filter))
                {
                    var filteredTargetObjects = targetObject.GetProperties(new string[] { lastPath });

                    // When the target is a list, and there is a filter in last path
                    // the value will be the filtered objects
                    // so that list.Remove(o) removes the filtered objects
                    if (Utils.IsIList(targetPropertyInfo.PropertyType))
                    {
                        // Nodes
                        foreach (var filteredTargetObject in filteredTargetObjects)
                        {
                            operationNodes.Add(new OperationNode(
                                operationStrategy!,
                                targetObject,
                                targetPropertyInfo!,
                                sourcePropertyInfo,
                                filteredTargetObject
                            ));
                        }
                    }
                }
                else
                {
                    operationNodes.Add(new OperationNode(
                        operationStrategy!,
                        targetObject,
                        targetPropertyInfo!,
                        sourcePropertyInfo,
                        value
                    ));
                }
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

            object sourceObject = objects.Single();

            if (sourceObject.GetType().IsNonStringEnumerable())
                sourceObject = ((IEnumerable<object>)sourceObject).First<object>();
            
            return (sourceObject, lastPath);
        }
    }
}