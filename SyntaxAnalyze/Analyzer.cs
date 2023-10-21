using System.Text;

namespace SyntaxAnalyze;

public static class Analyzer
{
    public static bool IsValidExpression(string expression)
    {
        int position = 0;
        
        bool hasOperand = ExtractOperand(expression, ref position);
        
        if (!hasOperand)
        {
            return true;
        }

        while (position < expression.Length)
        {
            SkipBlanks(expression, ref position);
            bool hasOperator = ExtractOperation(expression, ref position);
            
            if (!hasOperator)
            {
                return false;
            }
            
            SkipBlanks(expression, ref position);
            
            bool hasOperand2 = ExtractOperand(expression, ref position);
            
            if (!hasOperand2)
            {
                return false;
            }
        }

        return true;
    }

    private static bool ExtractOperand(string source, ref int position)
    {
        if (source[position] == '(')
        {
            int endPosition = position;
            if (!ExtractExpression(source, ref endPosition))
            {
                return false;
            }
            
            bool isValid = IsValidExpression(source[(position + 2)..endPosition]);
            if (isValid)
            {
                position = endPosition + 1;
                return true;
            }
        }
        
        if (ParseNumber(source, ref position))
        {
            return true;
        }

        return false;
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
        if (Validators.IsOperator(source[position]))
        {
            position++;
            return true;
        }

        return false;
    }
    
    private static bool ExtractExpression(string source, ref int position)
    {
        while (position < source.Length && source[position] != ')')
        {
            position++;
        }

        return position < source.Length;
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