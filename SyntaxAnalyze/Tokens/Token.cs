namespace SyntaxAnalyze.Tokens;

public class Token
{
    public Tokens Type
    {
        get;
        set;
    }
    
    public Token(Tokens type)
    {
        Type = type;
    }
}
