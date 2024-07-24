using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace JsonPatchForDotnet
{
    public interface IOperationNode
    {
        public object Instance { get; }

        /// <summary>
        /// The value of the SourceProperty may be used instead of the Value property. 
        /// </summary>
        public PropertyInfo? SourceProperty { get; }
        
        /// <summary>
        /// Property of Instance that Value will be applied to through the operation strategy.
        /// </summary>
        public PropertyInfo TargetProperty { get; }
        
        /// <summary>
        /// Value of TargetPropertyInfo before applying this operation.
        /// This is what is used when reverting the operation.
        /// </summary>
        public object? PreviousValue { get; }

        /// <summary>
        /// Value that will be used in the operation.
        /// </summary>
        public object? Value { get; }
        
        /// <summary>
        /// If Apply is not successful, then this will contain the exception that was thrown.
        /// </summary>
        Exception? OperationException { get; }
    }
}