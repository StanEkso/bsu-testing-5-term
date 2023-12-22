using System.Runtime.InteropServices;

namespace Execution.Compiled;

public enum ExpressionType
{
    Undefined,
    Int,
    Double,
    Str,
    Bool
}

//[StructLayout(LayoutKind.Explicit)]
//public struct SampleUnion
//{
//    [FieldOffset(0)] public float bar;
//    [FieldOffset(4)] public int killroy;
//    [FieldOffset(4)] public float fubar;
//}

public struct TypedValue
{
    public int intValue = 0;
    public double doubleValue = 0;
    public string? stringValue;
    public bool boolValue = false;

    public ExpressionType type = ExpressionType.Undefined;

    public TypedValue()
    {
        type = ExpressionType.Undefined;
    }

    public TypedValue(TypedValue source)
    {
        this.type = source.type;
        this.intValue = source.intValue;
        this.doubleValue = source.doubleValue;
        this.stringValue = source.stringValue;
        this.boolValue = source.boolValue;
    }

    public void SetValue(int value)
    {
        intValue = value;
        type = ExpressionType.Int;
    }
    public void SetValue(double value)
    {
        doubleValue = value;
        type = ExpressionType.Double;
    }
    public void SetValue(string value)
    {
        stringValue = value;
        type = ExpressionType.Str;
    }
    public void SetValue(bool value)
    {
        boolValue = value;
        type = ExpressionType.Bool;
    }

    //public void CopyFrom(TypedValue source) // no!!! use 'constructor' TypedValue(TypedValue source)
    //{
    //    this.type = source.type;
    //    this.intValue = source.intValue;
    //    this.doubleValue = source.doubleValue;
    //    this.stringValue = source.stringValue;  
    //    this.boolValue = source.boolValue;
    //}
    
    /****
    public void SetFrom(TokenConstantType token)
    {
        if (token is TokenConstant<int> ti)
        {
            intValue = ti.value;
            type = ExpressionType.Int;
        }
        else if (token is TokenConstant<double> td)
        {
            doubleValue = td.value;
            type = ExpressionType.Double;
        }
        else if (token is TokenConstant<string> ts)
        {
            stringValue = ts.value;
            type = ExpressionType.Str;
        }
        else if (token is TokenConstant<bool> tb)
        {
            boolValue = tb.value;
            type = ExpressionType.Bool;
        }
    }
    ****/
}

public static class TypeResolver
{
    public static ExpressionType ResultingOperationType(string operation, ExpressionType type1, ExpressionType type2)
    {
        if (type1 == ExpressionType.Int || type1 == ExpressionType.Double)
        { 
            if (operation == "Unary-"
                || operation == "Unary+"
               )
            return type1;
        }

        if (type1 == ExpressionType.Int && type2 == ExpressionType.Double)
            type1 = ExpressionType.Double;
        else if (type1 == ExpressionType.Double && type2 == ExpressionType.Int)
            type2 = ExpressionType.Double;

        if (type1 == type2)
        {
            if (operation == "=="
             || operation == "!="
             || operation == "<="
             || operation == ">="
             || operation == "<"
             || operation == ">"
                )
            {
                return ExpressionType.Bool;
            }

            if (operation == "+") // plus or string concat   
            {
                return type1;
            }
        }

        if (type1 == ExpressionType.Int || type1 == ExpressionType.Double) 
        {
            if (operation == "Unary++" || operation == "Unary--")
            {
                return type1;
            }

            if (type2 == ExpressionType.Int || type2 == ExpressionType.Double)
            {
                if (operation == "+"
                    || operation == "-"
                    || operation == "*"
                    || operation == "/"
                    )
                {
                    if (type1 == ExpressionType.Double || type2 == ExpressionType.Double)
                        return ExpressionType.Double;
                    else
                        return ExpressionType.Int;
                }

                if (operation == "%")
                {
                    if (type1 == ExpressionType.Int)
                        return type1;
                }
            }
        }

        if (type1 == ExpressionType.Bool)
        {
            if (operation == "!")
            {
                return type1;
            }
            if (type2 == ExpressionType.Bool)
            {
                if (operation == "&&" || operation == "||")
                {
                    return type1;
                }
            }
        }

        //StopOnError("qqqError");
        return ExpressionType.Undefined;
    }

}