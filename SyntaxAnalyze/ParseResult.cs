namespace SyntaxAnalyze;

public class ParseResult
{
    public string nameV = "Undefined";
    public ExpressionType typeV = ExpressionType.Undefined;
    public double valueV;

    public ParseResult()
    {
    }
    
    public ParseResult(string name, ExpressionType type, double value)
    {
        this.nameV = name;
        this.typeV = type;
        this.valueV = value;
    }

    public ParseResult Clone()
    {
        return new ParseResult(this.nameV, this.typeV, this.valueV);
    }
}