using System.Threading.Tasks;

namespace JsonPatchForDotnet
{
    public interface IOperationStrategy
    {
        Task ApplyAsync(IOperationNode operationNode);
        Task RevertAsync(IOperationNode operationNode);
    }
}