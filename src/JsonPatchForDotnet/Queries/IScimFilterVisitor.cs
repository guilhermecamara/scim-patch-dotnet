namespace JsonPatchForDotnet.Queries
{
    using Antlr4.Runtime.Tree;

    public interface IScimFilterVisitor<Return>
    {
        Return VisitExpression(IParseTree tree);
    }
}