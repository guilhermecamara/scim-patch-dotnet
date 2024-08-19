using Antlr4.Runtime.Tree;

namespace ScimPatchForDotnet.Queries
{
    public interface IScimFilterVisitor<Return>
    {
        Return VisitExpression(IParseTree tree);
    }
}