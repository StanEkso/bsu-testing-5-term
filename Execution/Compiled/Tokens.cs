using Execution.Compiled;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Execution.Compiled;

public enum TokenType
{
    Goto,
    GotoIf,
    Operation,
    //Constant,
    TokenTypedValue, // alternative to Constant
    //ConstantInt,
    //ConstantDouble,
    //ConstantString,
    SetGlobalVar,
    SetLocalVar,
    //RefLocalVar,
    GetGlobalVarValue,
    GetLocalVarValue,
    Ret,
    Call,
    EndOfExpression, // needed as marker with priority 0 to evaluate operations in stack (...expression... {  } )
    PrepareCall // needed as marker with priority = -10 to stop pop operation until return from function
}

public class Token
{
    public TokenType Type { get; private set; }

    public Token(TokenType type)
    { Type = type; }
}

public class TokenTypedValue : Token // not OOP style
{
    public TypedValue typedValue = new TypedValue(); // struct, not object!

    public TokenTypedValue() : base(TokenType.TokenTypedValue) { }

    public TokenTypedValue(TypedValue typedValue) : base(TokenType.TokenTypedValue)
    {
        this.typedValue = typedValue;
    }

    public TokenTypedValue(int value) : base(TokenType.TokenTypedValue) => typedValue.SetValue(value); 
    public TokenTypedValue(double value) : base(TokenType.TokenTypedValue) => typedValue.SetValue(value);
    public TokenTypedValue(string value) : base(TokenType.TokenTypedValue) => typedValue.SetValue(value); 
    public TokenTypedValue(bool value) : base(TokenType.TokenTypedValue) => typedValue.SetValue(value); 
}

/***
public class TokenConstantType : Token // alternative to TokenTypedType, much more OOP style
{
    public readonly ExpressionType valueType;

    public TokenConstantType(ExpressionType valueType) : base(TokenType.Constant)
    {
        this.valueType = valueType;
    }
}

public class TokenConstant<T> : TokenConstantType
{
    public readonly T? value;

    public TokenConstant(T value, ExpressionType valueType) : base(valueType)
    {
        this.value = value;
    }
}

//public class TokenInt : TokenConstant
//{
//    public readonly int value;
//    public TokenInt(int value) : base(ExpressionType.Int) => this.value = value; 
//}

//public class TokenDouble : TokenConstant
//{
//    public readonly double value;

//    public TokenDouble(double value) : base(ExpressionType.Double) => this.value = value; 
//}

//public class TokenString : TokenConstant
//{
//    public readonly string value;

//    public TokenString(string value) : base(ExpressionType.Str) => this.value = value; 
//}

//public class TokenBool : TokenConstant
//{
//    public readonly bool value;

//    public TokenBool(bool value) : base(ExpressionType.Bool) => this.value = value;
//}
//
****/

public class TokenOperation : Token
{
    public string Operation { get; private set; }

    public TokenOperation(string value) : base(TokenType.Operation)
    {
        Operation = value;
    }

}

public class TokenCall : Token
{
    //public readonly string name; // { get; private set; }
    //public readonly VariableDef def; // { get; private set; }

    public readonly int toToken; // { get; private set; }

    public TokenCall(int toToken) : base(TokenType.Call)
    {
        this.toToken = toToken;
    }
}

public class TokenVar : Token
{
    public readonly string name; // { get; private set; }
    public readonly VariableDef def; // { get; private set; }

    public TokenVar(string name, VariableDef def, TokenType type) : base(type)
    {
        this.name = name;
        this.def = def;
    }
}

public class TokenFunc : Token
{
    public readonly string name; // { get; private set; }
    public readonly FuncDef def; // { get; private set; }

    public TokenFunc(string name, FuncDef def, TokenType type) : base(type)
    {
        this.name = name;
        this.def = def;
    }
}

public class TokenGoto : Token
{
    //public readonly Token ToToken;
    public int toToken;

    public TokenGoto(TokenType type, int toToken) : base(type)
    {
        this.toToken = toToken;
    }
}

public class CompiledCode
{
    public readonly IList<Token> tokens = new List<Token>();

    public int LastIndex { get => tokens.Count - 1; }
    //public int startIndex = -1; // undefined

    public void AddReturn()
    {
        tokens.Add(new Token(TokenType.Ret));
    }
    public void AddEndOfExpression()
    {
        tokens.Add(new Token(TokenType.EndOfExpression));
    }
    public void AddGoto(int toToken)
    {
        tokens.Add(new TokenGoto(TokenType.Goto, toToken));
    }
    public void AddGotoIf(int toToken)
    {
        tokens.Add(new TokenGoto(TokenType.GotoIf, toToken));
    }
    public int AddUndefinedGoto() // useful to set up goto later
    {
        AddGoto(-1); // -1 just placeholder
        return LastIndex;
    }
    public int AddUndefinedGotoIf() // useful to set up goto later
    {
        AddGotoIf(-1); // -1 just placeholder
        return LastIndex;
    }
    public void DefineGoto(int fromIndex, int toIndex)
    {
        ((TokenGoto)(tokens[fromIndex])).toToken = toIndex;
    }
    public void DefineGotoHereFrom(int fromIndex)
    {
        ((TokenGoto)(tokens[fromIndex])).toToken = LastIndex + 1;
    }

    public void AddOperation(string operation)
    {
        tokens.Add(new TokenOperation(operation));
    }

    //public void AddGetGlobalVarValue(string name, VariableDef def)
    //{
    //    tokens.Add(new TokenVar(name, def, TokenType.GetGlobalVarValue));
    //}

    //public void AddSetGlobalVar(string name, VariableDef def)
    //{
    //    tokens.Add(new TokenVar(name, def, TokenType.SetGlobalVar));
    //}

    //public void AddGetLocalVar(string name, VariableDef def)
    //{
    //    tokens.Add(new TokenVar(name, def, TokenType.GetLocalVarValue));
    //}
    //public void AddSetLocalVar(string name, VariableDef def)
    //{
    //    tokens.Add(new TokenVar(name, def, TokenType.SetLocalVar));
    //}
    public void AddGetVarValue(string name, VariableDef def)
    {
        if (def is GlobalVariableDef)
            tokens.Add(new TokenVar(name, def, TokenType.GetGlobalVarValue));
        else if (def is LocalVariableDef)
            tokens.Add(new TokenVar(name, def, TokenType.GetLocalVarValue));
    }
    public void AddSetVar(string name, VariableDef def)
    {
        if (def is GlobalVariableDef)
            tokens.Add(new TokenVar(name, def, TokenType.SetGlobalVar));
        else if (def is LocalVariableDef)
            tokens.Add(new TokenVar(name, def, TokenType.SetLocalVar));
    }

    public void AddCall(FuncDef def)
    {
        tokens.Add(new TokenCall(def.CodeIndex));
    }
    //public void AddPrepareCall() => tokens.Add(new Token(TokenType.PrepareCall));

    public void AddInt(int value) => tokens.Add(new TokenTypedValue(value));
    public void AddDouble(double value) => tokens.Add(new TokenTypedValue(value));
    public void AddString(string value) => tokens.Add(new TokenTypedValue(value));
    public void AddBool(bool value) => tokens.Add(new TokenTypedValue(value));

    /**
    public void AddString(string value)
    {
        tokens.Add(new TokenConstant<string>(value, ExpressionType.Str));
        //this.tokens.Add(new TokenString(value));
    }

    public void AddInt(int value)
    {
        tokens.Add(new TokenConstant<int>(value, ExpressionType.Int));
        //this.tokens.Add(new TokenInt(value));
    }

    public void AddDouble(double value)
    {
        tokens.Add(new TokenConstant<double>(value, ExpressionType.Double));
        //this.tokens.Add(new TokenDouble(value));
    }
    public void AddBool(bool value)
    {
        tokens.Add(new TokenConstant<bool>(value, ExpressionType.Bool));
        //this.tokens.Add(new TokenBool(value));
    }
    **/
}

