namespace SyntaxAnalyze;

public class Variable
{
    public ExpressionType? type;
    public dynamic? var;

    public Variable()
    {
        type = ExpressionType.Undefined;
    }

    public Variable(dynamic value, ExpressionType t)
    {
        var = value;
        type = t;
    }

    public void SetVariable(dynamic value) => var = value;

    public void SetType(ExpressionType t) => type = t;
}