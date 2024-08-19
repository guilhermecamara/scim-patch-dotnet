using Antlr4.Runtime.Tree;

namespace ScimPatch.Queries
{
    public interface IScimFilterVisitor<Return>
    {
        Return VisitExpression(IParseTree tree);
    }
}