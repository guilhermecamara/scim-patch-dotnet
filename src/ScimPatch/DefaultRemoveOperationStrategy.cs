using System.Collections;
using System.Threading.Tasks;
using ScimPatch.Extensions;

namespace ScimPatch
{
    /// <summary>
    /// Add operation as defined at <see cref="https://datatracker.ietf.org/doc/html/rfc7644#section-3.5.2.2"/>
    /// </summary>
    public class DefaultRemoveOperationStrategy : IOperationStrategy
    {
        public Task ApplyAsync(IOperationNode operationNode)
        {
            if (Utils.IsIList(operationNode.TargetProperty.PropertyType))
            {
                if (operationNode.Value == null)
                {
                    operationNode.TargetProperty.SetValue(
                        operationNode.Instance,
                        operationNode.TargetProperty.PropertyType.GetDefaultValue());
                }
                else
                {
                    var list = (IList)operationNode.TargetProperty.GetValue(operationNode.Instance);
                    list.Remove(operationNode.Value);
                }
            }
            else
            {
                operationNode.TargetProperty.SetValue(
                    operationNode.Instance,
                    operationNode.TargetProperty.PropertyType.GetDefaultValue());
            }
            return Task.CompletedTask;
        }

        public Task RevertAsync(IOperationNode operationNode)
        {
            if (Utils.IsIList(operationNode.TargetProperty.PropertyType))
            {
                if (operationNode.Value == null)
                {
                    operationNode.TargetProperty.SetValue(operationNode.Instance, operationNode.PreviousValue);
                }
                else
                {
                    var list = (IList)operationNode.TargetProperty.GetValue(operationNode.Instance);
                    list.Add(operationNode.Value);
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