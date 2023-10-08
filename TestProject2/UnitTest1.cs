namespace TestProject2;

public class Tests
{
    [Test]
    public void Test1()
    {
        Assert.That(SyntaxAnalyze.Analyzer.IsValidExpression("24 + 3"), Is.EqualTo(true));
    }
    
    [Test]
    public void Test2()
    {
        Assert.That(SyntaxAnalyze.Analyzer.IsValidExpression("24 +/ 3"), Is.EqualTo(false));
    }
    
    [Test]
    public void Test3()
    {
        Assert.That(SyntaxAnalyze.Analyzer.IsValidExpression("24 + (3 + 5)"), Is.EqualTo(true));
    }

    [TestCase("24 + 3", true)]
    [TestCase("24 + 3 + 9" ,true)]
    [TestCase("24 + 3 + 9" ,true)]
    [TestCase("((24 + 3) 4)", false)]
    [TestCase("24 + (3 + 5)", true)]
    [TestCase("24 + (3 * 6)", true)]
    [TestCase("24 + (3 & 6)", false)]
    [TestCase("24 + (3 + (5 * 4) * 4 - 4 * (2 + 3))", true)]
    public void ValidatesExpression(string expression, bool expected)
    {
        bool actual = SyntaxAnalyze.Analyzer.IsValidExpression(expression);
        
        Assert.That(actual, Is.EqualTo(expected));
    }
}