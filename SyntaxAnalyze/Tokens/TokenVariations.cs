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
    private int _numberOfTokensAfter;

    public TokenSetLocalVariable(): base(TokenType.SetLocalVariable)
    {
    }

    public TokenSetLocalVariable(string name) : base(TokenType.SetLocalVariable)
    {
        this._name = name;
        _numberOfTokensAfter = -1;
    }
    public TokenSetLocalVariable(string name, int numberOfTokensAfter) : base(TokenType.SetLocalVariable)
    {
        this._name = name;
        _numberOfTokensAfter = numberOfTokensAfter;
    }

    public string Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }
    
    public int NumberOfTokensAfter
    {
        get => _numberOfTokensAfter;
        set => _numberOfTokensAfter = value;
    }
}

public class TokenGoto : Token
{
    private int _TokenToGo;
    private int _numOfTokens;

    public TokenGoto()
    {
    }
    
    public TokenGoto(int tokenToGo)
    {
        _TokenToGo = tokenToGo;
    }

    public TokenGoto(int tokenToGo, int numOfTokens)
    {
        _TokenToGo = tokenToGo;
        this._numOfTokens = numOfTokens;
    }

    public int PositionToGo
    {
        get => _TokenToGo;
        set => _TokenToGo = value;
    }

    public int NumOfTokens
    {
        get => _numOfTokens;
        set => _numOfTokens = value;
    }
}