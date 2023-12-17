using static System.Runtime.InteropServices.JavaScript.JSType;

using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using System;
using Execution.Compiled;
using System.Reflection.Metadata.Ecma335;

namespace Execution;

public class Calculator
{
    private string? error;
    public string? Error { get => error; }

    public readonly CompiledCode compiledCode;

    public Calculator(CompiledCode compiledCode)
    {
        this.compiledCode = compiledCode;
    }

    private static void Assign(Stack<Token> operands, TokenVar token)
    {
        var left = operands.Pop();

        token.def.VarValue = new(GetTypedValue(left, operands)); // struct, constructor new just set values

        //token?.def.VarValue = GetTypedValue(left, operands); // struct, copy by value

        //token?.def.TypedValue.CopyFrom(GetTypedValue(left, operands)); // struct, copy by value
        //token?.def.typedValue.SetFrom((left as TokenConstantType));
    }

    public static void ComputeOnTheTop(Stack<Token> operands, Stack<string> operators)
    {
        string op = operators.Pop();

        Token? val2 = null;

        if (!IsUnaryOperatioin(op))
        {
            val2 = operands.Pop();
        }
        var val1 = operands.Pop();

        operands.Push(Operate(val1, val2, op, operands));
    }

    public TypedValue? Compute()
    {
        if (compiledCode?.tokens == null)
            return null;

        Stack<Token> operands = new();
        Stack<string> operators = new();

        string suspect;// = "";

        TypedValue retval = new(); // struct

        int ip = 0; // instruction pointer

        while (ip < compiledCode.tokens.Count)
        {
            Token token = compiledCode.tokens[ip];
            if (token == null)
                return null;

            suspect = "";

            if (token.Type == TokenType.Ret)
            {
                if (operators.Count > 0 && operators.Peek() == "PrepareCall")
                {
                    operators.Pop();
                    var retvaltoken = (TokenTypedValue)operands.Pop();
                    ip = ((TokenTypedValue)operands.Pop()).typedValue.intValue;
                    operands.Push(retvaltoken);
                    continue;
                }
                else { 
                    break; // top level return;
                }
            }
            else if (token.Type == TokenType.Goto)
            {
                ip = ((TokenGoto)token).toToken;
                continue;
            }
            else if (token.Type == TokenType.GotoIf)
            {
                var val1 = ((TokenTypedValue)operands.Pop()).typedValue.boolValue;
                if (!val1)
                {
                    ip = ((TokenGoto)token).toToken;
                    continue;
                }
            }
            else if (token.Type == TokenType.SetGlobalVar)
            {
                Assign(operands, (TokenVar)token);
                ip++;
                continue;
            }
            else if (token.Type == TokenType.Operation)
            {
                suspect = ((TokenOperation)token).Operation;
            }
            else if (token.Type == TokenType.Call) {
                operands.Push(new TokenTypedValue(ip + 1)); 
                ip = ((TokenCall)token).toToken;
                continue;
            }

            if (suspect == "(" || suspect == "PrepareCall")
            {
                operators.Push(suspect);
            }
            else if (token.Type == TokenType.TokenTypedValue) 
            {
                operands.Push(token);
            }
            else if (token.Type == TokenType.GetGlobalVarValue)
            {
                // for correct calc with side effects we replace ref with value
                var v = GetTypedValue(token, operands); 
                operands.Push(new TokenTypedValue(v)); 
            }
            else if (suspect == ")")
            {
                while (operators.Count != 0 && operators.Peek() != "(")
                {
                    ComputeOnTheTop(operands, operators);
                }

                if (operators.Count != 0) operators.Pop();
            }
            else // EndOfExpression
            {
                var currentOperatorPriority = GetOperationPriority(suspect);

                while (operators.Count != 0 && GetOperationPriority(operators.Peek()) >= currentOperatorPriority)
                {
                    ComputeOnTheTop(operands, operators);
                }

                if (suspect != "")
                    operators.Push(suspect);
            }
            ip++;
        }

        while (operators.Count != 0 && operands.Count > 1)
        {
            ComputeOnTheTop(operands, operators);
        }

        if (operators.Count != 0)
        {
            string op = operators.Pop();
            throw new InvalidOperationException($"operators stack is not empty at the end: {op}");
        }

        //TypedValue retval = new(); // struct
        // TokenTypedValue tokenretval;

        if (operands.Count != 0)
        {
            retval = GetTypedValue(operands.Pop(), operands);
        }
        else
        {
            retval.SetValue(0); 
            //tokenretval = new TokenConstant<int>(0, ExpressionType.Int);
        }
        if (operands.Count > 1)
        {
            throw new InvalidOperationException($"operands stack is not empty at the end: ((op))");
        }

        return retval;
    }


    private static bool IsUnaryOperatioin(string operation)
    {
        if (operation == "!" || operation.StartsWith("Unary"))
            return true;
        return false;
    }

    private static TypedValue GetTypedValue(Token token, Stack<Token> operands)
    {
        TypedValue typedValue;

        if (token is TokenVar tvar)
        {
            typedValue = tvar.def.VarValue;
        }
        else if (token is TokenTypedValue tv)
        {
            typedValue = tv.typedValue;
        }
        else
        {
            throw new InvalidOperationException($"Invalid token type (it is not operand): {token.Type}. ");
        }
        return typedValue;
    }

    private static TokenTypedValue Operate(Token left, Token? right, string operation, Stack<Token> operands)
    {
        TypedValue typedValue1 = GetTypedValue(left, operands);
        TypedValue typedValue2;
        if (right != null)
        {
            typedValue2 = GetTypedValue(right, operands);
        }
        else
        {
            typedValue2 = new TypedValue(); // just init struct fields;
        }

        var res = TypedValueOperate(typedValue1, typedValue2, operation);

        var resToken = new TokenTypedValue(res);
        return resToken;
    }

    private static TypedValue TypedValueOperate(TypedValue typedValue1, TypedValue typedValue2, string operation)
    {
        var resultType = TypeResolver.ResultingOperationType(operation, typedValue1.type, typedValue2.type);
        if (resultType == ExpressionType.Undefined)
            throw new InvalidOperationException($"Incompatible types: {typedValue1.type} {typedValue2.type}. Operation: {operation} ");

        bool err = false;

        TypedValue res = new();

        if (typedValue1.type == ExpressionType.Double || typedValue2.type == ExpressionType.Double)
        {
            if (typedValue1.type == ExpressionType.Int)
            {
                typedValue1.doubleValue = typedValue1.intValue;
            }
            else if (typedValue2.type == ExpressionType.Int)
            {
                typedValue2.doubleValue = typedValue2.intValue;
            }
        }

        if (resultType == ExpressionType.Bool)
        {
            if (operation == "!" && typedValue1.type == ExpressionType.Bool)
                res.boolValue = !typedValue1.boolValue;
            else if (typedValue1.type == typedValue2.type && typedValue1.type == ExpressionType.Bool)
            {
                if (operation == "&&") res.boolValue = typedValue1.boolValue && typedValue2.boolValue;
                else if (operation == "||") res.boolValue = typedValue1.boolValue || typedValue2.boolValue;
                else err = true;
            }
            else if (typedValue1.type == typedValue2.type && typedValue1.type == ExpressionType.Int)
            {
                if (operation == "==") res.boolValue = typedValue1.intValue == typedValue2.intValue;
                else if (operation == "!=") res.boolValue = typedValue1.intValue != typedValue2.intValue;
                else if (operation == "<=") res.boolValue = typedValue1.intValue <= typedValue2.intValue;
                else if (operation == ">=") res.boolValue = typedValue1.intValue >= typedValue2.intValue;
                else if (operation == "<") res.boolValue = typedValue1.intValue < typedValue2.intValue;
                else if (operation == ">") res.boolValue = typedValue1.intValue > typedValue2.intValue;
                else err = true;
            }
            else if (typedValue1.type == ExpressionType.Double || typedValue2.type == ExpressionType.Double)
            {
                if (operation == "==") res.boolValue = AlmostEquals(typedValue1.doubleValue, typedValue2.doubleValue, 1e-100);
                else if (operation == "!=") res.boolValue = typedValue1.doubleValue != typedValue2.doubleValue;
                else if (operation == "<=") res.boolValue = typedValue1.doubleValue <= typedValue2.doubleValue;
                else if (operation == ">=") res.boolValue = typedValue1.doubleValue >= typedValue2.doubleValue;
                else if (operation == "<") res.boolValue = typedValue1.doubleValue < typedValue2.doubleValue;
                else if (operation == ">") res.boolValue = typedValue1.doubleValue > typedValue2.doubleValue;
                else err = true;
            }
            else if (typedValue1.type == typedValue2.type && typedValue1.type == ExpressionType.Str)
            {
                if (operation == "==") res.boolValue = typedValue1.stringValue == typedValue2.stringValue;
                else if (operation == "!=") res.boolValue = typedValue1.stringValue != typedValue2.stringValue;
                else if (operation == "<=") res.boolValue = typedValue1.stringValue?.CompareTo(typedValue2.stringValue) <= 0;
                else if (operation == ">=") res.boolValue = typedValue1.stringValue?.CompareTo(typedValue2.stringValue) >= 0;
                else if (operation == "<") res.boolValue = typedValue1.stringValue?.CompareTo(typedValue2.stringValue) < 0;
                else if (operation == ">") res.boolValue = typedValue1.stringValue?.CompareTo(typedValue2.stringValue) > 0;
                else err = true;
            }
            else
            {
                err = true;
            }
        }
        else if (resultType == ExpressionType.Int)
        {
            if (typedValue1.type == ExpressionType.Int && operation == "Unary-")
            {
                res.intValue = -typedValue1.intValue;
            }
            else if (typedValue1.type == ExpressionType.Int && operation == "Unary+")
            {
                res.intValue = +typedValue1.intValue;
            }
            else if (typedValue1.type == typedValue2.type && typedValue1.type == ExpressionType.Int)
            {
                if (operation == "+") res.intValue = typedValue1.intValue + typedValue2.intValue;
                else if (operation == "-") res.intValue = typedValue1.intValue - typedValue2.intValue;
                else if (operation == "*") res.intValue = typedValue1.intValue * typedValue2.intValue;
                else if (operation == "/") res.intValue = typedValue1.intValue / typedValue2.intValue;
                else if (operation == "%") res.intValue = typedValue1.intValue % typedValue2.intValue;
                else err = true;
            }
            else err = true;
        }
        else if (resultType == ExpressionType.Double)
        {
            if (typedValue1.type == ExpressionType.Double && operation == "Unary-")
            {
                res.doubleValue = -typedValue1.doubleValue;
            }
            else if (typedValue1.type == ExpressionType.Double && operation == "Unary+")
            {
                res.doubleValue = +typedValue1.doubleValue;
            }
            else if (operation == "+") res.doubleValue = typedValue1.doubleValue + typedValue2.doubleValue;
            else if (operation == "-") res.doubleValue = typedValue1.doubleValue - typedValue2.doubleValue;
            else if (operation == "*") res.doubleValue = typedValue1.doubleValue * typedValue2.doubleValue;
            else if (operation == "/") res.doubleValue = typedValue1.doubleValue / typedValue2.doubleValue;
            else err = true;
        }
        else if (resultType == ExpressionType.Str)
        {
            if (typedValue1.type == typedValue2.type && typedValue1.type == ExpressionType.Str)
                if (operation == "+") res.stringValue = typedValue1.stringValue + typedValue2.stringValue;
                else err = true;
        }
        else
        {
            err = true;
        }

        if (err)
        {
            throw new InvalidOperationException($"Invalid operation: {operation} {typedValue1.type} {typedValue2.type} ");
        }

        res.type = resultType;
        return res;
    }

    public static bool AlmostEquals(double x, double y, double tolerance)
    {
        // https://roundwide.com/equality-comparison-of-floating-point-numbers-in-csharp/

        var diff = Math.Abs(x - y);
        return diff <= tolerance ||
               diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * tolerance;
    }

    private static int GetOperationPriority(string operation) =>
    operation switch
    {
       "||" => 70,
       "&&" => 80,
       "<" or ">" or "==" or "!=" or "<=" or ">=" => 100,
       "+" or "-" => 200,
       "*" or "/" or "%" => 300,
       "Unary-" or "Unary+" or "!" => 1000,
        //EndOfExpression => 0, // needed as marker with priority 0 to evaluate operations in stack (...expression... {  } )
        "PrepareCall" => -1,
           _ => 0 //EndOfExpression => 0, // needed as marker with priority 0 to evaluate operations in stack (...expression... {  } )
    };
    /***
    private static TokenConstantType TokenConstantType_Operate(Token left, Token? right, string operation)
    {
        //if (left.Type == TokenType.Constant && right.Type == TokenType.Constant)
        //{
        ExpressionType type1 = ExpressionType.Undefined, type2 = ExpressionType.Undefined;
        int i1 = 0, i2 = 0;
        double d1 = 0, d2 = 0;
        string? s1 = "", s2 = "";
        bool b1 = false, b2 = false;

        TypedValue typedValue1;
        TypedValue typedValue2;

        if (left is TokenVar tvar1)
        {
            typedValue1 = tvar1.def.typedValue;

            type1 = typedValue1.type;
            if (type1 == ExpressionType.Int) i1 = typedValue1.intValue;
            else if (type1 == ExpressionType.Double) d1 = typedValue1.doubleValue;
            else if (type1 == ExpressionType.Str) s1 = typedValue1.stringValue;
            else if (type1 == ExpressionType.Bool) b1 = typedValue1.boolValue;
        }
        else if (left is TokenConstant<int> ti)
        {
            type1 = ExpressionType.Int;
            i1 = ti.value;
        }
        else if (left is TokenConstant<double> td)
        {
            type1 = ExpressionType.Double;
            d1 = td.value;
        }
        else if (left is TokenConstant<string> ts)
        {
            type1 = ExpressionType.Str;
            s1 = ts.value;
        }
        else if (left is TokenConstant<bool> tb)
        {
            type1 = ExpressionType.Bool;
            b1 = tb.value;
        }

        if (right is TokenVar tvar2)
        {
            typedValue2 = tvar2.def.typedValue;

            type2 = typedValue2.type;
            if (type2 == ExpressionType.Int) i2 = typedValue2.intValue;
            else if (type2 == ExpressionType.Double) d2 = typedValue2.doubleValue;
            else if (type2 == ExpressionType.Str) s2 = typedValue2.stringValue;
            else if (type2 == ExpressionType.Bool) b2 = typedValue2.boolValue;
        }
        else if (right is TokenConstant<int> ti2)
        {
            type2 = ExpressionType.Int;
            i2 = ti2.value;
        }
        else if (right is TokenConstant<double> td2)
        {
            type2 = ExpressionType.Double;
            d2 = td2.value;
        }
        else if (right is TokenConstant<string> ts2)
        {
            type2 = ExpressionType.Str;
            s2 = ts2.value;
        }
        else if (right is TokenConstant<bool> tb2)
        {
            type2 = ExpressionType.Bool;
            b2 = tb2.value;
        }

        var resultType = TypeResolver.ResultingOperationType(operation, type1, type2);
        if (resultType == ExpressionType.Undefined)
            throw new InvalidOperationException($"Incompatible types: {operation} {type1} {type2} ");

        int res.intValue = 0;
        double res.doubleValue = 0;
        string res.stringValue = "";
        bool res.boolValue = false;
        bool err = false;

        if (type1 == ExpressionType.Double || type2 == ExpressionType.Double)
        {
            if (type1 == ExpressionType.Int)
            {
                d1 = i1;
            }
            else if (type2 == ExpressionType.Int)
            {
                d2 = i2;
            }
        }

        if (resultType == ExpressionType.Bool)
        {
            if (operation == "!" && type1 == ExpressionType.Bool)
                res.boolValue = !b1;
            else if (type1 == type2 && type1 == ExpressionType.Bool)
            {
                if (operation == "&&") res.boolValue = b1 && b2;
                else if (operation == "||") res.boolValue = b1 || b2;
                else err = true;
            }
            else if (type1 == type2 && type1 == ExpressionType.Int)
            {
                if (operation == "==") res.boolValue = i1 == i2;
                else if (operation == "!=") res.boolValue = i1 != i2;
                else if (operation == "<=") res.boolValue = i1 <= i2;
                else if (operation == ">=") res.boolValue = i1 >= i2;
                else if (operation == "<") res.boolValue = i1 < i2;
                else if (operation == ">") res.boolValue = i1 > i2;
                else err = true;
            }
            else if (type1 == ExpressionType.Double || type2 == ExpressionType.Double)
            {
                if (operation == "==") res.boolValue = d1 == d2;
                else if (operation == "!=") res.boolValue = d1 != d2;
                else if (operation == "<=") res.boolValue = d1 <= d2;
                else if (operation == ">=") res.boolValue = d1 >= d2;
                else if (operation == "<") res.boolValue = d1 < d2;
                else if (operation == ">") res.boolValue = d1 > d2;
                else err = true;
            }
            else if (type1 == type2 && type1 == ExpressionType.Str)
            {
                if (operation == "==") res.boolValue = s1 == s2;
                else if (operation == "!=") res.boolValue = s1 != s2;
                else if (operation == "<=") res.boolValue = s1.CompareTo(s2) <= 0;
                else if (operation == ">=") res.boolValue = s1.CompareTo(s2) >= 0;
                else if (operation == "<") res.boolValue = s1.CompareTo(s2) < 0;
                else if (operation == ">") res.boolValue = s1.CompareTo(s2) > 0;
                else err = true;
            }
            else
            {
                err = true;
            }
        }
        else if (resultType == ExpressionType.Int)
        {
            if (type1 == ExpressionType.Int && operation == "Unary-")
            {
                res.intValue = -i1;
            }
            else if (type1 == ExpressionType.Int && operation == "Unary+")
            {
                res.intValue = +i1;
            }
            else if (type1 == type2 && type1 == ExpressionType.Int)
            {
                if (operation == "+") res.intValue = i1 + i2;
                else if (operation == "-") res.intValue = i1 - i2;
                else if (operation == "*") res.intValue = i1 * i2;
                else if (operation == "/") res.intValue = i1 / i2;
                else if (operation == "%") res.intValue = i1 % i2;
                else err = true;
            }
            else err = true;
        }
        else if (resultType == ExpressionType.Double)
        {
            if (type1 == ExpressionType.Double && operation == "Unary-")
            {
                res.doubleValue = -d1;
            }
            else if (type1 == ExpressionType.Double && operation == "Unary+")
            {
                res.doubleValue = +d1;
            }
            else if (operation == "+") res.doubleValue = d1 + d2;
            else if (operation == "-") res.doubleValue = d1 - d2;
            else if (operation == "*") res.doubleValue = d1 * d2;
            else if (operation == "/") res.doubleValue = d1 / d2;
            else err = true;
        }
        else if (resultType == ExpressionType.Str)
        {
            if (type1 == type2 && type1 == ExpressionType.Str)
                if (operation == "+") res.stringValue = s1 + s2;
                else err = true;
        }
        else
        {
            err = true;
        }

        if (err)
        {
            throw new InvalidOperationException($"Invalid operation: {operation} {type1} {type2} ");
        }

        TokenConstantType resToken;

        if (resultType == ExpressionType.Bool)
            resToken = new TokenConstant<bool>(res.boolValue, ExpressionType.Bool);
        else if (resultType == ExpressionType.Int)
            resToken = new TokenConstant<int>(res.intValue, ExpressionType.Int);
        else if (resultType == ExpressionType.Double)
            resToken = new TokenConstant<double>(res.doubleValue, ExpressionType.Double);
        else if (resultType == ExpressionType.Str)
            resToken = new TokenConstant<string>(res.stringValue, ExpressionType.Str);
        else
        {
            throw new InvalidOperationException($"Invalid operation result type: {operation} {type1} {type2} {resultType} ");
        }

        return resToken;
    }
    ****/


    /******
    public double ComputeString(string source)
    {
        Stack<double> operands = new();
        Stack<char> operators = new();

        for (var i = 0; i < source.Length; i++)
        {
            char suspect = source[i];
            if (suspect == ' ') continue;

            if (suspect == '(')
            {
                operators.Push(suspect);
            }
            else if (IsDigit(suspect))
            {
                double value = 0;

                while (i < source.Length && IsDigit(source[i]))
                {
                    value = value * 10 + (source[i] - '0');
                    i++;
                }

                operands.Push(value);
                i--;
            }
            else if (suspect == ')')
            {
                while (operators.Count != 0 && operators.Peek() != '(')
                {
                    var val2 = operands.Pop();

                    var val1 = operands.Pop();

                    var op = operators.Pop();

                    operands.Push(Operate(val1, val2, op));
                }

                if (operators.Count != 0) operators.Pop();
            }
            else
            {
                var currentOperatorPriority = GetOperationPriority(suspect);

                while (operators.Count != 0 && GetOperationPriority(operators.Peek()) >= currentOperatorPriority)
                {
                    var val2 = operands.Pop();

                    var val1 = operands.Pop();

                    char op = operators.Pop();

                    operands.Push(Operate(val1, val2, op));
                }

                operators.Push(suspect);
            }
        }


        while (operators.Count != 0 && operands.Count > 1)
        {
            var val2 = operands.Pop();

            var val1 = operands.Pop();

            char op = operators.Pop();

            operands.Push(Operate(val1, val2, op));
        }

        if (operators.Count != 0)
        {
            char op = operators.Pop();
            throw new InvalidOperationException($"operators stack is not empty at the end: {op}");
            //return double.NaN;
        }

        if (operands.Count > 1)
        {
            double op = operands.Pop();
            throw new InvalidOperationException($"operands stack is not empty at the end: {op}");
            //return double.NaN;
        }

        return operands.Pop();
    }

     private static double Operate(double left, double right, string op) =>
        op switch
        {
            "+" => left + right,
            "-" => left - right,
            "*" => left * right,
            "/" => right switch
            {
                0 => throw new DivideByZeroException(),
                _ => left / right
            },
            _ => throw new InvalidOperationException($"Invalid operator: {op}")
        };
     ***********/


    /// <summary>
    /// Check if the given char is digit
    /// </summary>
    /// <param name="char">char to be determined</param>
    /// <returns>true if the given char is digit, otherwise false</returns>
    //private static bool IsDigit(char @char) => @char is >= '0' and <= '9';
}