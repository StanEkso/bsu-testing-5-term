using System.Text;

namespace SyntaxAnalyze;

public class Analyzer
{
    private static readonly char[] BlankSymbols = { ' ', '\n', '\t', '\r' };
    private const string SingleLineComment = "//";
    static readonly ParseResult null_parseResult = new ParseResult();

    private ParseResult parseResult = new ParseResult();

    private readonly string expression;
    private int position;
    private readonly Dictionary<string, ParseResult> variables = new Dictionary<string, ParseResult>();

    public Analyzer(string expression)
    {
        this.expression = expression;
        this.position = 0;
    }

    public bool Parse()
    {
        position = 0;

        bool f;

        do
        {
            f = ParseAssigment();
        }
        while (f);

        f = EndCode();
        return f;
    }

    private bool EndCode()
    {
        SkipBlanks();

        if (position == expression.Length)
        {
            return true;
        }
        return false;
    }

    public bool IsValidExpression()
    {

        position = 0;

        ParseExpression();

        if (position == expression.Length)
        {
            return true;
        }

        throw new InvalidOperationException();
    }



    private bool ParseExpression(ParseResult parseResult)
    {
        var typeOp1 = TypesExpr.Undefined;
        if (!ParseOperand(parseResult))
        {
            throw new InvalidOperationException();
        }
        typeOp1 = parseResult.typeV;

        while (expression.Length >= position)
        {
            if (!ParseOperation())
            {
                return false;
            }

            if (!ParseOperand(parseResult))
            {
                throw new InvalidOperationException();
            }

            if (parseResult.typeV != typeOp1)
            {
                throw new InvalidOperationException();
            }
        }

        return true;
    }

    private bool ParseExpression()
    {
        /* if (!ExtractOperand())
         {
             throw new InvalidOperationException();
         }

         while (expression.Length >= position)
         {
             if (!ExtractOperation())
             {
                 return false;
             }

             if (!ExtractOperand())
             {
                 throw new InvalidOperationException();
             }
         }
        return true; */

        var parseResult = new ParseResult();

        return ParseExpression(parseResult);

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

    private bool ParseStr(string str)
    {
        SkipBlanks();

        if (expression.StartsWith(str))
        {
            position += str.Length;
            return true;
        }

        return false;
    }

    private bool ParseString()
    {
        SkipBlanks();

        if (!ParseChar('\''))
        {
            return false;
        }

        while (!EndCode())
        {
            //ParseStr(@"\'");
            if (ParseChar('\''))
            {
                return true;
            }

            position++;
        }

        throw new InvalidOperationException();
    }


    private bool ParseAssigment()
    {
        var parseResult = new ParseResult();

        var (variable, name) = ParseVariable(parseResult);
        
        if (!variable || name == null)
        {
            return false;
        }

        parseResult.nameV = name;

        if (!ParseChar('='))
        {
            return false;
        }

        ParseExpression(parseResult);

        if (!ParseChar(';'))
        {
            throw new InvalidOperationException();
        }

        AddVar(parseResult);

        return true;

    }

    private bool ParseOperation()
    {
        SkipBlanks();

        if (position < expression.Length && Validators.IsOperator(expression[position]))
        {
            position++;
            return true;
        }

        return false;
    }

    private bool ParseOperand(ParseResult parseResult)
    {
        if (ParseChar('('))
        {
            ParseExpression(parseResult);

            if (!ParseChar(')'))
            {
                throw new InvalidOperationException();
            }

            return true;
        }
        
        
        if (ParseString())
        {
            parseResult.typeV = TypesExpr.Str;
            return true;
        }

        if (ParseNumber())
        {
            parseResult.typeV = TypesExpr.Num;
            return true;
        }

        var (variable, str) = ParseVariable(parseResult);

        if (variable && str != null)
        {
            // parseResult.typeV = TypesExpr.STR;
            var parseResultRHS = GetVar(str); //, parseResult);
            parseResult.typeV = parseResultRHS.typeV;
            return true;
        }

        throw new InvalidOperationException();
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

    private (bool, string?) ParseVariable(ParseResult parseResult)
    {
        SkipBlanks();
        if (position >= expression.Length)
        {
            return (false, null);
        }

        if (!char.IsAscii(expression[position]) && expression[position] != '_')
        {
            return (false, null);
        }

        int p1 = position;
        position++;

        while (position < expression.Length && IsValidVariableSymbol())
        {
            position++;
        }

        return (true, expression.Substring(p1, position - p1));
    }

    private ParseResult GetVar(string name)
    {
        return variables[name];
        //return new ParseResult();
    }


    private void AddVar(ParseResult parseResult)
    {
        variables.TryAdd(parseResult.nameV, parseResult.Clone());
        // variables.Add(parseResult.nameV, parseResult.Clone());
    }

    private bool IsValidVariableSymbol()
    {
        if (position >= expression.Length)
        {
            return false;
        }

        char suspect = expression[position];

        return char.IsDigit(suspect) || char.IsAsciiLetter(suspect) || suspect == '_';
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