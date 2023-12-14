namespace SyntaxAnalyze;

public class Value
{
    private string _name = string.Empty;
    private ExpressionType _type = ExpressionType.Undefined;
    private dynamic? _contentValue;

    public Value()
    {
    }

    public Value(int contentValue)
    {
        this._contentValue = contentValue;
        this._type = ExpressionType.Int;
    }
    
    public Value(float contentValue)
    {
        this._contentValue = contentValue;
        this._type = ExpressionType.Float;
    }
    
    public Value(string contentValue)
    {
        this._contentValue = contentValue;
        this._type = ExpressionType.String;
    }
    
    public Value(bool contentValue)
    {
        this._contentValue = contentValue;
        this._type = ExpressionType.Bool;
    }

    public string Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }

    public ExpressionType Type
    {
        get => _type;
        set => _type = value;
    }

    public dynamic? ContentValue
    {
        get => _contentValue;
        set => this._contentValue = value;
    }
}