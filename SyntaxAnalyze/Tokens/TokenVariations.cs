namespace SyntaxAnalyze;

public class TokenValue : Token
{
    private Value _value;

    public TokenValue() : base(TokenType.Value)
    {
    }
    
    public TokenValue(Value value) : base(TokenType.Value)
    {
        Value = value;
    }

    public Value Value
    {
        get => _value;
        set => _value = value ?? throw new ArgumentNullException(nameof(value));
    }
}

public class TokenOperation : Token
{
    private string _operation;

    public TokenOperation() : base(TokenType.Operation)
    {
    }
    public TokenOperation(string operation) : base(TokenType.Operation)
    {
        _operation = operation;
    }

    public string Operation
    {
        get => _operation;
        set => _operation = value ?? throw new ArgumentNullException(nameof(value));
    }
}

public class TokenLocalVariable : Token
{
    private string _name;
    private Value _value;

    public TokenLocalVariable(): base(TokenType.LocalVariable)
    {
    }

    public TokenLocalVariable(string name, Value value) : base(TokenType.LocalVariable)
    {
        this._name = name;
        _value = value;
    }

    public string Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Value Value
    {
        get => _value;
        set => _value = value ?? throw new ArgumentNullException(nameof(value));
    }
}

public class TokenGetLocalVariable : Token
{
    private string _name;

    public TokenGetLocalVariable(): base(TokenType.GetLocalVariable)
    {
    }

    public TokenGetLocalVariable(string name) : base(TokenType.GetLocalVariable)
    {
        this._name = name;
    }

    public string Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }
}

public class TokenSetLocalVariable : Token
{
    private string _name;
    private Value _newValue;

    public TokenSetLocalVariable(): base(TokenType.SetLocalVariable)
    {
    }

    public TokenSetLocalVariable(string name, Value value) : base(TokenType.SetLocalVariable)
    {
        this._name = name;
        _newValue = value;
    }

    public string Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }
    
    public Value NewValue
    {
        get => _newValue;
        set => _newValue = value ?? throw new ArgumentNullException(nameof(value));
    }
}