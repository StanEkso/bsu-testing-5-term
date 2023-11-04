namespace TestProject2;

public class Tests
{
    [TestCase("2 + 3", true)]
    [TestCase("2 +\n 3", true)]
    [TestCase("2 + x3", true)]
    [TestCase("2 +\n x3", true)]
    [TestCase("24 + _x3", true)]
    [TestCase("24 + x_123", true)]
    [TestCase("24 + x_123\n", true)]
    [TestCase("24 + y12", true)]
    [TestCase("24 + 3 + 9" ,true)]
    [TestCase("24 + 3\n + 9" ,true)]
    [TestCase("24 + 3 + 9\t" ,true)]
    [TestCase("24 + (3 + 5)", true)]
    [TestCase("24 + (3 * 6)", true)]
    [TestCase("24 + (3\n\r * 6)", true)]
    [TestCase("24 + (x * y)", true)]
    [TestCase("24 + (x\r\n * y)", true)]
    [TestCase("24 + (x * 12)", true)]
    [TestCase("24 + (x* y)", true)]
    [TestCase("24 + (x*y)", true)]
    [TestCase("24 + (x\r*y)", true)]
    [TestCase("24 + (x_*_y)", true)]
    [TestCase("24 \n + \n (x_*_y)", true)]
    [TestCase("24 \t + \r\n (3 + (5 * 4) * 4 - 4 * (2 + 3))", true)]
    [TestCase("24 + (3 + (5 * 4) * 4 - 4 * (2 + 3))", true)]
    [TestCase("24 + (3 + (5 * x) * 4 - 4 * (y + 3))", true)]
    public void ValidatesExpression(string expression, bool expected)
    {
        bool actual = SyntaxAnalyze.Analyzer.IsValidExpression(expression);
        
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase("((24 + 3) 4)")]
    [TestCase("24 + (3 & 6)")]
    [TestCase("24 + 3y,3")]
    [TestCase("24 + 1_2x + (x_*_y)")]
    [TestCase("24 + 12x + (x_*_y)")]
    public void ThrowsException(string expression)
    {
        Assert.That(() => SyntaxAnalyze.Analyzer.IsValidExpression(expression), Throws.InvalidOperationException);
    }
}