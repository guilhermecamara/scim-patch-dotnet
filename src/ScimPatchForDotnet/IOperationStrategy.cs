using System.Threading.Tasks;

namespace ScimPatchForDotnet
{
    public interface IOperationStrategy
    {
        Task ApplyAsync(IOperationNode operationNode);
        Task RevertAsync(IOperationNode operationNode);
    }
}