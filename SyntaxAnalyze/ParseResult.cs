namespace SyntaxAnalyze;

public class ParseResult
{
    public ParseResult()
    {
        this.Name = "Undefined";
    }
    
    public ParseResult(string name, ExpressionType type, double value)
    {
        this.Name = name;
        this.Type = type;
        this.Value = value;
    }

    public string Name
    {
        get;
        set;
    }

    public ExpressionType Type
    {
        get;
        set;
    }

    public double Value
    {
        get;
        set;
    }

    public ParseResult Clone()
    {
        return new ParseResult(this.Name, this.Type, this.Value);
    }
}