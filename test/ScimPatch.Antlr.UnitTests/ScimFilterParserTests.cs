using Antlr4.Runtime;

namespace ScimPatch.Antlr.UnitTests;

public class Tests
{
    class SyntaxErrorListener : BaseErrorListener
    {
        public bool HasSyntaxErrors { get; private set; } = false;

        public override void SyntaxError(
            IRecognizer recognizer, 
            IToken offendingSymbol, 
            int line, 
            int charPositionInLine, 
            string msg, 
            RecognitionException e)
        {
            HasSyntaxErrors = true;
        }
    }
    
    [Test]
    public void ParseSCIMFilter_DoesNotThro2w()
    {
        // Sample SCIM filter input
        string input = "!! asd 21 e12e 412 username ???";

        // Create an input stream from the input string
        AntlrInputStream inputStream = new AntlrInputStream(input);

        // Create a lexer and parser
        ScimFilterLexer lexer = new ScimFilterLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        ScimFilterParser parser = new ScimFilterParser(commonTokenStream);
        var errorListener = new SyntaxErrorListener();
        parser.RemoveErrorListeners(); 
        parser.AddErrorListener(errorListener);  
        
        // Act
        _ = parser.parse();
        
        // Assert
        Assert.IsTrue(errorListener.HasSyntaxErrors, "The parser reported syntax errors.");
    }
    
    [Test]
    public void ParseSCIMFilter_DoesNotThrow()
    {
        // Sample SCIM filter input
        string input = "username eq \"john\" and (email pr or name co \"Doe\")";

        // Create an input stream from the input string
        AntlrInputStream inputStream = new AntlrInputStream(input);

        // Create a lexer and parser
        ScimFilterLexer lexer = new ScimFilterLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        ScimFilterParser parser = new ScimFilterParser(commonTokenStream);
        var errorListener = new SyntaxErrorListener();
        parser.RemoveErrorListeners(); 
        parser.AddErrorListener(errorListener);  
        
        // Act
        _ = parser.parse();
        
        // Assert
        Assert.IsFalse(errorListener.HasSyntaxErrors, "The parser reported syntax errors.");
    }
}
