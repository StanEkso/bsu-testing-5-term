using System.Text;

namespace SyntaxAnalyze;

public static class Analyzer
{
    public static bool IsValidExpression(string expression)
    {
        int position = 0;
        ExtractExpression(expression, ref position);

        if (position == expression.Length)
        {
            return true;
        }

        return false;
    }
    
    private static bool ExtractExpression(string source, ref int position)
    {
        if (!ExtractOperand(source, ref position))
        {
            throw new InvalidOperationException();
        }

        while (source.Length >= position)
        {
            if (!ExtractOperation(source, ref position))
            {
                return false;
            }

            if (!ExtractOperand(source, ref position))
            {
                throw new InvalidOperationException();
            }
        }

        return true;
    }
    
    private static void SkipBlanks(string source, ref int position)
    {
        while (position < source.Length && source[position] == ' ')
        {
            position++;
        }
    }

    private static bool ExtractOperation(string source, ref int position)
    {
        SkipBlanks(source, ref position);
        if (position < source.Length && Validators.IsOperator(source[position]))
        {
            position++;
            return true;
        }

        return false;
    }

    private static bool ExtractOperand(string source, ref int position)
    {
        if (Parse(source, ref position, '('))
        {
            ExtractExpression(source, ref position);

            if (!Parse(source, ref position, ')'))
            {
                throw new InvalidOperationException();
            }

            return true;
        }

        if (ParseNumber(source, ref position))
        {
            return true;
        }

        throw new InvalidOperationException();
    }

    private static bool Parse(string source, ref int position, char symbol)
    {
        SkipBlanks(source, ref position);

        if (source[position] == symbol && position < source.Length)
        {
            position++;
            return true;
        }

        return false;
    }
    
    
    
    private static bool ParseNumber(string expression, ref int position)
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