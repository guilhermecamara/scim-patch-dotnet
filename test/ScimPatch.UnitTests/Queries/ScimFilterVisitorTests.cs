using Antlr4.Runtime;
using ScimPatch.Antlr;
using ScimPatch.Queries;

namespace ScimPatch.UnitTests.Queries;

[TestClass]
public class ScimFilterVisitorTests
{
    // A simple test resource class
    public class TestResource
    {
        public string Property { get; set; } = null!;
        public int Number { get; set; }
        public bool Flag { get; set; }
    }

    private ScimFilterParser Setup(string filter)
    {
        var inputStream = new AntlrInputStream(filter);
        var speakLexer = new ScimFilterLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(speakLexer);
        var speakParser = new ScimFilterParser(commonTokenStream);
        return speakParser;   
    }
    
    [TestMethod]
    [DataRow("Property eq \"abc\"", true)]
    [DataRow("Property eq \"efg\"", false)]
    [DataRow("Number eq 2", true)]
    [DataRow("Number gt 1", true)]
    [DataRow("Number lt 0", false)]
    [DataRow("Number ge 2", true)]
    [DataRow("Flag eq \"true\"", false)]
    [DataRow("not(Flag eq \"true\")", true)]
    public void EqOperators_ShouldParseAndVisitCorrectly(string filter, bool expectedResult)
    {
        // Arrange
        var parser = Setup(filter);
        var res = new TestResource
        {
            Property = "abc",
            Number = 2,
            Flag = false
        };
        var visitor = new ScimFilterVisitor<TestResource>();
        var filterContext = parser.filter();
        
        // Act
        var expression = visitor.Visit(filterContext);
        var compiledResult = (Func<TestResource, bool>)expression.Compile();
        var result = compiledResult.Invoke(res);
        
        // Assert
        Assert.IsNotNull(expression);
        Assert.AreEqual(expectedResult, result);
    }
}