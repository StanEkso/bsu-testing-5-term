namespace SyntaxAnalyze;

public abstract class Token
{
    public TokenType _type;
    
    protected Token(){}

    protected Token(TokenType type)
    {
        this._type = type;
    }

    public TokenType Type
    {
        get => _type;
        set => _type = value;
    }
}