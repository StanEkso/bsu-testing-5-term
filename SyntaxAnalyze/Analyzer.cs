using System.Text;

namespace SyntaxAnalyze;

public static class Analyzer
{
    public static bool IsValidExpression(string expression)
    {
        CharType? prevCharType = null;
        for (var i = 0; i < expression.Length; i++)
        {
            char suspect = expression[i];
            if (IsDigit(suspect))
            {
                prevCharType = CharType.Digit;
                continue;
            }

            if (IsOperator(suspect))
            {
                if (HasPrecedingOperator(prevCharType))
                {
                    return false;
                }

                prevCharType = CharType.Operator;
                continue;
            }

            if (suspect == ' ')
            {
                prevCharType |= CharType.Space;
                continue;
            }

            if (suspect == '(')
            {
                StringBuilder sb = new();
                int openerCount = 1;

                i++;

                while (i < expression.Length)
                {
                    suspect = expression[i];
                    if (suspect == '(')
                    {
                        openerCount++;
                    }
                    
                    if (suspect == ')')
                    {
                        openerCount--;
                        if (openerCount == 0)
                        {
                            break;
                        }
                    }

                    i++;
                    sb.Append(suspect);
                }

                bool isNestedValid = IsValidExpression(sb.ToString());

                if (!isNestedValid)
                {
                    return false;
                }

                prevCharType = CharType.Digit;
                
                continue;
            }

            return false;
        }

        return true;
    }
    
    private static bool IsDigit(char @char) => @char is >= '0' and <= '9';

    private static bool IsOperator(char @char)
    {
        char[] operators = { '+', '-', '*', '/' };

        return operators.Contains(@char);
    }

    private static bool HasPrecedingOperator(CharType? charType) => charType switch
    {
        CharType.Operator => true,
        CharType.Operator | CharType.Space => true,
        _ => false
    };
}