namespace TestProject2;

public class Tests
{
    [TestCase("2 + 3", true)]
    [TestCase("24 + 3", true)]
    [TestCase("24 + 3 + 9" ,true)]
    [TestCase("24 + 3 + 9" ,true)]
    [TestCase("24 + (3 + 5)", true)]
    [TestCase("24 + (3 * 6)", true)]
    [TestCase("24 + (3 + (5 * 4) * 4 - 4 * (2 + 3))", true)]
    public void ValidatesExpression(string expression, bool expected)
    {
        bool actual = SyntaxAnalyze.Analyzer.IsValidExpression(expression);
        
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase("((24 + 3) 4)")]
    [TestCase("24 + (3 & 6)")]
    public void ThrowsException(string expression)
    {
        Assert.That(() => SyntaxAnalyze.Analyzer.IsValidExpression(expression), Throws.InvalidOperationException);
    }
}