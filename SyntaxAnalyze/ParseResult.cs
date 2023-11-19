namespace SyntaxAnalyze;

public class ParseResult
{
    public string nameV = "Undefined";
    public TypesExpr typeV = TypesExpr.Undefined;
    public double valueV;

    public ParseResult()
    {
    }
    
    public ParseResult(string name, TypesExpr type, double value)
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