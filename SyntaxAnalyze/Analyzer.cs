using System.Text;
using DomainLibrary;

namespace SyntaxAnalyze;

public class Analyzer
{
    private static readonly char[] BlankSymbols = { ' ', '\n', '\t', '\r' };
    private static readonly char[] EscapedSymbols = { 'n', 't', 'r', '\\', '\'' };
    private const string SingleLineComment = "//";

    private readonly string expression;
    private int position;
    private readonly Dictionary<string, VariableDef> variables = new();
    private readonly Dictionary<string, FuncDef> functions = new();
    private string? _funcName;
    private List<Token> _tokens = new ();

    private string? error;
    public string? Error { get => error; }

    internal class ParserException : ApplicationException
    {
        public ParserException() : base() { }
        public ParserException(string message) : base(message) { }
        public ParserException(string message, Exception inner) : base(message, inner) { }
    }

    public Analyzer(string expression)
    {
        this.expression = expression;
        this.position = 0;
        this.error = null;
    }

    public bool StopOnError(string msg)
    {
        error = msg;
        throw new ParserException();
    }

    public void LogError(string Error)
    {
        Console.WriteLine("!!! Parsing error:");
        Console.WriteLine(Error);
        Console.WriteLine($"at position {position} .");
        if (EndCode())
        {
            Console.WriteLine($"at the end of code (last 20 chars): ");
            Console.WriteLine(expression[Math.Max(expression.Length - 20, 0)..]);
        }
        else
        {
            Console.WriteLine($"at text (first 20 chars): ");
            Console.WriteLine(expression[position..Math.Min(expression.Length, position + 20)]);
        }
    }


    public bool Parse()
    {
        try
        {
            return _Parse();
        }
        catch (ParserException)
        {
            LogError(Error ?? "");
            return false;
        }
    }


    public bool _Parse()
    {
        position = 0;
        error = null;
        _funcName = null;

        bool f;

        // declare global vars
        do
        {
            f = ParseVar();
        }
        while (f);

        do
        {
            f = ParseFunction();
        }
        while (f);

        // top level statements
        ParseOperators();

        f = EndCode();
        
        if (!f)
        {
            StopOnError("expected End Of Code."); return false;
        }
        
        return f;
    }

    public bool ParseVar()
    {
        if (!ParseWordAndBlank("var"))
        {
            return false;
        }

        do
        {
            if (!ParseAssigment(true))
            {
                StopOnError("qqqError"); return false;
            }
        } while (ParseChar(','));

        if (!ParseChar(';'))
        {
            StopOnError("qqqError"); return false;
        }
        
        return true;
    }

    public bool ParseReturn()
    {
        if (!ParseKeyWord("return"))
        {
            return false;
        }

        if (!ParseChar(';'))
        {
            ParseExpression();
            
            if (!ParseChar(';'))
            {
                StopOnError("qqqError");
                return false;
            }
        }
        
        this.AddToken(TokenType.Return, null);
        
        return true;
    }

    private void AddToken(TokenType type, int position)
    {
        this._tokens.Add(new Token(type, position));
    }

    private void AddToken(TokenType type, string name)
    {
        this._tokens.Add(new Token(type, name));
    }

    private bool ParseIf()
    {
        if (!ParseKeyWord("if"))
        {
            return false;
        }

        ParseExpression();

        if (!ParseChar('{'))
        {
            StopOnError("Expected '{' after if"); return false;
        }
        
        ParseOperators();
        
        if (!ParseChar('}'))
        {
            StopOnError("Expected '{' after if"); return false;
        }
     
        if (ParseKeyWord("else"))
        {
            if (ParseChar('{'))
            {
                ParseOperators();
                
                if (!ParseChar('}'))
                {
                    StopOnError("Expected '}' after else"); return false;
                }
            }
            else
            {
                ParseIf();
            }
        }
        
        return true;
    }

    public bool ParseWhile()
    {
        if (!ParseKeyWord("while"))
        {
            return false;
        }

        ParseExpression();

        if (!ParseChar('{'))
        {
            StopOnError("Expacted '{' arter if"); return false;
        }
        ParseOperators();
        if (!ParseChar('}'))
        {
            StopOnError("Expacted '{' arter if"); return false;
        }

        return true;

    }



    public bool ParseOperators()
    {
        bool f;

        do

        {
            f = ParseReturn();
            if (!f)
            {
                f = ParseIf();
            }
            if (!f)
            {
                f = ParseWhile();
            }
            if (!f)
            {
                f = ParseAssigment();
            }

        }
        while (f);

        f = EndCode();
        return f;
    }


    private bool ParseFunction()
    {
        SkipBlanks();
        if (!ParseWordAndBlank("function"))
        {
            return false;
        }

        ParseFunctionHeader();
        if (!ParseChar('{'))
        {
            StopOnError("qqqError"); return false;
        }

        ParseOperators();

        if (!ParseChar('}'))
        {
            StopOnError("qqqError"); return false;
        }
        _funcName = null;

        return true;
    }

    private int ParseFunctionHeader()
    {
        string? funcName = ParseVariable();
        if (funcName == null)
        {
            StopOnError("qqqError"); return -1;
        }
        _funcName = funcName;

        if (!ParseChar('('))
        {
            StopOnError("qqqError"); return -1;
        }

        AddFunc(_funcName);

        int argcount = 0;
        string? name;

        if (!ParseChar(')'))
        {
            do
            {
                name = ParseVariable();
                
                if (name == null)
                {
                    StopOnError("qqqError");
                    return -1;
                }

                AddFuncVar(name, funcName);

                argcount++;

            } while (ParseChar(','));
        }

        if (!ParseChar(')'))
        {
            StopOnError("qqqError"); return -1;
        }
        
        return argcount;
    }

    private void AddFunc(string name)
    {
        functions.TryAdd(name, new FuncDef(name));
    }

    private FuncDef GetFunc(string name)
    {
        //return functions[name];
        if (functions.TryGetValue(name, out FuncDef? v))
            return v;
        else
            return null;
    }

    private void AddVar(string name, string? funcName = null)
    {
        if (funcName == null)
            variables.TryAdd(name, new VariableDef(name));
        else
            AddFuncVar(name, funcName);
    }

    private void AddFuncVar(string name, string funcName)
    {
        var localVars = functions[funcName].localVariables;  // functions.TryGetValue(funcName, out _);
        localVars.TryAdd(name, new VariableDef(name));
    }

    private VariableDef? GetVar(string name, string? funcName)
    {
        VariableDef? v;
        if (funcName != null)
        {
            var localVars = functions[funcName].localVariables;  // functions.TryGetValue(funcName, out _);
            if (localVars.TryGetValue(name, out v))
                return v;
        }
        if (variables.TryGetValue(name, out v))
            return v;
        else
            return null;
    }


    //-------------------------------------------------------
    private bool EndCode()
    {
        SkipBlanks();

        if (position == expression.Length)
        {
            return true;
        }
        return false;
    }


    // for testing only
    public bool IsValidExpression()
    {
        try
        {
            position = 0;
            error = "";
            ParseExpression();
            if (!EndCode())
            {
                error = "expected End Of Code.";
                LogError(Error);
                return false;
            }
            return true;
        }
        catch (ParserException _)
        {
            LogError(Error);
            return false;
        }
    }


    private bool ParseExpression()
    {
        if (ParseUnaryOperation())
        {
            if (!ParseOperand())
            {
                StopOnError("qqqError"); return false;
            }
        }
        else if (!ParseOperand())
        {
            return false;
        }

        while (expression.Length >= position)
        {
            if (!ParseOperation())
            {
                return false;
            }

            if (!ParseOperand())
            {
                StopOnError("qqqError"); return false;
            }
        }

        return true;
    }

    private void SkipBlanks()
    {
        while (position < expression.Length && BlankSymbols.Contains(expression[position]))
        {
            position++;
        }

        while (expression.Substring(position).StartsWith(SingleLineComment))
        {
            while (position < expression.Length && expression[position] != '\n')
            {
                position++;
            }

            if (position < expression.Length)
            {
                position++;
            }

            SkipBlanks();
        }
    }

    private char CurrentChar()
    {
        if (position < expression.Length)
            return expression[position];
        else
            return '\0';
    }

    private bool ParseString()
    {
        SkipBlanks();

        bool Escape = true;

        if (ParseChar('@'))
        {
            Escape = false;

            if (!ParseChar('\''))
            {
                StopOnError("qqqError"); return false;
            }
        }
        else
        {
            if (!ParseChar('\''))
            {
                return false;
            }
        }

        int pos1 = position;

        while (!EndCode()) // don't use ParseChar() or SkipBlanks() here, they skip comments!
        {
            if (Escape && CurrentChar() == '\\')
            {
                position++;

                if (position >= expression.Length)
                {
                    StopOnError("qqqError"); return false;
                }

                if (!EscapedSymbols.Contains(CurrentChar()))
                {
                    StopOnError("qqqError"); return false;
                }
            }
            else if (CurrentChar() == '\'')
            {
                position++;
                return true;
            }

            position++;
        }

        StopOnError("qqqError"); return false;
    }


    // is used also in var declaration
    private bool ParseAssigment(bool varOnly = false)
    {
        string? name = ParseVariable();

        if (name == null)
        {
            return false;
        }

        if (!ParseChar('='))
        {
            if (varOnly)
            {
                AddVar(name, _funcName);
                return true;
            }
            return false;
        }

        ParseExpression();

        if (!varOnly && !ParseChar(';'))
        {
            StopOnError("qqqError"); return false;
        }

        AddVar(name, _funcName);

        return true;
    }



    //--------------------------------------------------
    private bool ParseUnaryOperation()
    {
        SkipBlanks();
        int p1 = position;
        if (
              ParseChars("!")
           || ParseChars("-")
           || ParseChars("+")

           )
        {
            ;
        }
        else
        {
            return false;
        }
        var operation = expression[p1..position];
        return true;
    }

    private bool ParseOperation() // Binary
    {
        SkipBlanks();
        int p1 = position;
        if (
              ParseChars("==")
           || ParseChars("!=")
           || ParseChars("<=")
           || ParseChars(">=")
           || ParseChars("&&")
           || ParseChars("||")
           || ParseChars("<")
           || ParseChars(">")
           )
        {
            ;
        }
        //else if (
        //      ParseChars("++")
        //   || ParseChars("--")
        //   )
        //{
        //    ;
        //}
        else if (
              ParseChars("+")
           || ParseChars("-")
           || ParseChars("*")
           || ParseChars("/")
           || ParseChars("%")
           )
        {
            ;
        }
        else
        {
            return false;
        }
        var operation = expression[p1..position];
        return true;

        //SkipBlanks();
        //if (position < expression.Length && Validators.IsOperation(expression[position]))
        //{
        //    position++;
        //    return true;
        //}
        //return false;
    }

    private static ExpressionType ResultingOperationType(string operation, ExpressionType type1, ExpressionType type2)
    {
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

        if (type1 == ExpressionType.Num)
        {
            if (operation == "++" || operation == "--")
            {
                return type1;
            }

            if (type2 == ExpressionType.Num)
            {
                if (operation == "+"
                    || operation == "-"
                    || operation == "*"
                    || operation == "/"
                    || operation == "%"
                    )
                {
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


    private bool ParseOperand()
    {
        if (ParseChar('('))
        {
            ParseExpression();

            if (!ParseChar(')'))
            {
                StopOnError("qqqError"); return false;
            }

            return true;
        }

        if (ParseString())
        {
            //Type = ExpressionType.Str;
            return true;
        }

        if (ParseNumber())
        {
            //Type = ExpressionType.Num;
            return true;
        }

        string? name = ParseVariable();

        if (name == null)
        {
            StopOnError("qqqError"); return false;
        }

        if (ParseChar('(')) // function call, not var
        {
            if (GetFunc(name) == null)
            {
                StopOnError("qqqError"); return false;
            }

            ParseArguments(name);

            if (!ParseChar(')'))
            {
                StopOnError("qqqError"); return false;
            }

            return true;
        }

        if (GetVar(name, _funcName) == null)
        {
            StopOnError("qqqError"); return false;
        }

        return true;
    }

    private bool ParseArguments(string funcName)
    {
        /*
        var func = GetVar(funcName);
        if (func.Operation != "func")
        {
            return false;
        }
        */
        int argcount = 0;

        while (!EndCode())
        {
            ParseExpression();

            argcount++;

            // TO DO check argcount
            //if (argcount !=)
            //{
            //    StopOnError("qqqError"); return false;
            //}

            if (!ParseChar(','))
            {
                return false;
            }
        }
        return false;
    }

    private bool ParseChar(char symbol)
    {
        SkipBlanks();

        if (position < expression.Length && expression[position] == symbol)
        {
            position++;
            return true;
        }

        return false;
    }

    private bool ParseChars(string str)
    {
        SkipBlanks();

        if (position < expression.Length && expression[position..].StartsWith(str))
        {
            position += str.Length;
            return true;
        }

        return false;
    }

    private bool ParseWordAndBlank(string str)
    {
        // ParseWordAndBlank('var')
        //  var x;  // true
        //  var1 = x;  // false
        //  var;  // false
        //  var(  // false

        SkipBlanks();
        int p1 = position;
        bool f = ParseChars(str);
        int p2 = position;
        SkipBlanks();
        if (f && position > p2)
        {
            return true;
        }
        position = p1;
        return false;
    }

    private bool ParseKeyWord(string str)
    {
        if (ParseWordAndBlank(str))
        {
            return true;
        }

        int p1 = position;
        bool f = ParseChars(str);

        if (f && !char.IsLetterOrDigit(expression[position]) && expression[position] != '_')
        {
            return true;
        }

        position = p1;
        return false;
    }

    private string? ParseVariable()
    {
        SkipBlanks();
        if (position >= expression.Length)
        {
            return null;
        }

        if (!char.IsLetterOrDigit(expression[position]) && expression[position] != '_')
        {
            return null;
        }

        var p1 = position;
        position++;

        while (position < expression.Length && (char.IsLetterOrDigit(expression[position]) || expression[position] == '_')) // IsValidVariableSymbol())
        {
            position++;
        }

        string v = expression[p1..position];
        return v;
    }

    private bool ParseNumber()
    {
        StringBuilder number = new();

        while (position < expression.Length && Validators.IsDigit(expression[position]))
        {
            number.Append(expression[position]);
            position++;
        }

        return number.Length > 0;
    }
}