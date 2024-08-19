using System.Threading.Tasks;

namespace ScimPatch
{
    public interface IOperationStrategy
    {
        Task ApplyAsync(IOperationNode operationNode);
        Task RevertAsync(IOperationNode operationNode);
    }
}