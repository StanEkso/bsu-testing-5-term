namespace DomainLibrary;

public class Token
{
    public Token(TokenType type, int? position)
    {
        this.Type = type;
        this.Position = position;
    }

    public Token(TokenType type, string name)
    {
        this.Type = type;
        this.Name = name;
    }

    public TokenType Type
    {
        get;
        private set;
    }

    public int? Position
    {
        get;
        private set;
    }

    public string? Name
    {
        get;
        private set;
    }
}