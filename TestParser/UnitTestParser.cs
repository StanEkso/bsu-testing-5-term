using static System.Runtime.InteropServices.JavaScript.JSType;

using Execution.SyntaxAnalyze;
using NUnit.Framework;

namespace TestParser;

public class Tests
{
    [TestCase("var x=14+4;", true)]
    [TestCase(
        """
        var x;
        var x=14+4;
        x=x;
        """
      , true)]
    [TestCase(
        """
        var x=9;
        var x= 14 + 4;
        var y= x + 1;
        var z = 0;
        var z = 10;
        """
      , true)]
    [TestCase(
        """
        var  x = 2*6/2;
        var x= 14 + 4;
        var y= x + 1;
        var z = 0;
        z = 10;
        """
        , true)]
    [TestCase(
        """
        var x=-1;
        var x= 14 + 4;
        var y= x*(1+2/3) + 1;
        """
        , true)]
    [TestCase(
        """
        var x=0;
        var x, y;
        var x = 1, z;
        var x, z=2;
        """
        , true)]
    [TestCase(
        """
        var x=3;
        var x, y;

        function f(x,y)
        {
          x = y;
        }

        x = f(1,2);
        """
        , true)]
    [TestCase(
        """
        function f(x,y)
        {
          x = y;
        }
        """
        , true)]
    [TestCase(
        """
        var x=1;
        function f(x,y)
        {
          x = y + 1;
        }

        x = 1;
        """
        , true)]
    [TestCase(
        """
        var x=3;
        var x, y;

        function f(x,y)
        {
          x = y;
        }
        
        function f2(x)
        {
          x = y+1;
        }
        
        x = f(1,2) + f2 ( 1 ) ;
        """
        , true)]
    [TestCase(
        """
        function f2(x)
        {
          return x;
          return (x);
          return(x);
          return;
          return ;
          return 1;
          return -1;
        }
        """
        , true)]
    [TestCase(
        """
          return-1;
        """
        , true)]
    [TestCase(
        """
        var varx=11;
        varx=1;
        """
        , true
        )]
    [TestCase(
        """
        var x=11;
        if ( 1==1)
        {
        x=1;
        }
        """, true)]
    [TestCase(
        """
        var x=-1;
        if 1==1
        {
        x=1;
        }
        """, true)]
    [TestCase(
        """
        var x=-1;
        if 1==-1
        {
        x=11;
        }
        """, true)]
    [TestCase(
        """
        var 
            x = -(1)
            ;
        if 1==1
        {
         x=1;
        }
        else if (2==2)
        {
        x=2;
        }
        """, true)]
    [TestCase(
        """
        var x=+1;
        if 1==1
        {
          x=1;
        }
        
        else if 2==2
        {
           x=2;
        }
        
        else if 1==1
        {
         x=3; 
        }
        """, true)]
    [TestCase(
        """
        var x=+1;
        if 1==1
        {
        x=1;
        } else if 2==2{
        x=2;
        } else {
        x=3; 
        }
        """, true)]
    [TestCase(
        """
        var x=(+1);
        while (1==1){
        x=2;
        }
        """, true)]
    [TestCase(
        """
        var x=+1;
        while (1==1){
        if(1==1){
        x=1;
        }
        x=2;
        }
        """, true)]
    [TestCase(
        """
        var x=+(1);
        while (1==1){
         if(1==1){
        x=1;
        } else{
        x=2;
        }
        }
        """, true)]
    public void ValidatesParse(string expression, bool expected)

    {
        var parser = new Analyzer(expression);
        bool actual = parser.Parse();

        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [TestCase(
        "var x= 'abc';"
        , true)] 
    [TestCase(
        """
        var x=1;
        x= 'bcd';
        x= 'abc';
        """
        , true)]
    [TestCase(
        """
        var x=1;
        x= '';
        """
        , true)]
    [TestCase(
        """
        var x= 'bcd';
        var y= x + 'abc';
        """, true)]
    public void ValidatesParseSTR(string expression, bool expected)
    {
        var parser = new Analyzer(expression);
        bool actual = parser.Parse();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase("x=y4;")]
    [TestCase("x=x;")]    
    [TestCase(
        """
        x= a';
        """)]
    [TestCase(
        """
        x= a'';
        """)]
    [TestCase(
        """
        var x, y;

        function f(x,y)
        {
          x = z;
        }
        """
        )]
    [TestCase(
        """
        function f(x,y)
        {
          x = y + 1;
        }

        x = f2();
        """
        )]
    [TestCase(
        """
        functionf(x,y)
        {
          x = y + 1;
        }
        """
        )]
    [TestCase(
        """
          return124;
        """
        )]
    [TestCase(
        """
          returnx,y;
        """
        )]
    [TestCase(
        """
          returnxy;
        """
        )]
    [TestCase(
        """
          return x,y;
        """
        )]
    [TestCase(
        """ 
          return *;
        """
        )]
    [TestCase(
        """
          return 124
        """
        )]
    [TestCase(
        """
        var *;
        """
        )]
    [TestCase(
        """
        var1 x;
        """
        )]
    [TestCase(
        """
        var x=1
        """
        )]
    [TestCase(
        """
        if if  (1==1){
        x=31;
        }
        """
        )]
    [TestCase(
        """
        if  (1==1){ 
        {
        x=32;
        }
        """
        )]
    [TestCase(
        """
        if  ((1==1){
        x=33;
        }
        """
        )]
    [TestCase(
        """
        if  (1==1){
        x=34;
        """
        )]
   
    [TestCase(
        """
        else{
        x=0;
        } if(x==1){
        x=25;
        }

        """
        )]
    [TestCase(
        """
        if  (1==1){
        x=26;
        } 
        if else{
        x=17;
        }
        """
        )]
    [TestCase(
        """
        if  (1==1){
        } 
        else else{
        x=28;
        }
        """
      )]
    [TestCase(
        """
        while (){
        }
        """
      )]
    [TestCase(
        """
        while ()
        }
        """
      )]
    [TestCase(
        """
        while (){
        

        """
      )]
    [TestCase(
        """
        while {
        }
        """
      )]
    [TestCase(
        """
        while (x){
        }
        """
      )]
    [TestCase(
        """
        while (*){
        }
        """
      )]
    [TestCase(
        """
        while (1*2=2){
        x=3
        }
        """
      )]
    public void ValidatesParseThrowException(string expression)
    {
        var parser = new Analyzer(expression);
        var actual = parser.Parse();
        Assert.True(!actual && parser.Error != "");

        ////Assert.Throws(typeof(KeyNotFoundException), () => parser.Parse());
        //Assert.Catch(() => parser.Parse());
    }


    [TestCase("2 + 3", true)]
    [TestCase("2 +\n 3", true)]
    [TestCase("24 + 3 + 9", true)]
    [TestCase("24 + 3\n + 9", true)]
    [TestCase("24 + 3 + 9\t", true)]
    [TestCase("24 + (3 + 5)", true)]
    [TestCase("24 + (3 * 6)", true)]
    [TestCase("24 + (3\n\r * 6)", true)]
    [TestCase("24 \t + \r\n (3 + (5 * 4) * 4 - 4 * (2 + 3))", true)]
    [TestCase("24 + (3 + (5 * 4) * 4 - 4 * (2 + 3))", true)]
    [TestCase("2 > 1", true)]
    [TestCase("2 == 2", true)]
    [TestCase("(1+1) == (1+1)", true)]
    [TestCase("(1+11) >= (1+1)", true)]
    [TestCase("(1+1) <(1+0)", true)]
    [TestCase("(1+1) <(1+0) && 2==2", true)]
    [TestCase("(1==1) || (2==2)", true)]
    [TestCase("(1==1) || 2==2", true)]
    [TestCase("1==1 || 2==2", true)]
    [TestCase("1==-1 || 2==2", true)]
    [TestCase("1==-1 || -2==2", true)]
    [TestCase("1==-1 || -2==-2", true)]
    [TestCase("!(1==1)", true)]
    [TestCase("1!=1", true)]
    [TestCase("-1", true)]
    [TestCase("-(1+1)", true)]
    [TestCase("-(-1)", true)]
    [TestCase("-(-(1+1))", true)]
    [TestCase("1-(-(1+1))", true)]
    [TestCase("1-(-(1+1))-1", true)]
    [TestCase("+1", true)]
    [TestCase("+(1+1)", true)]
    [TestCase("+(+1)", true)]
    [TestCase("+(+(1+1))", true)]
    [TestCase("1+(+(1+1))-1", true)]
    [TestCase("1-(+(1+1))-1", true)]
    public void ValidatesExpression(string expression, bool expected)
    {
        var parser = new Analyzer(expression);
        bool actual = parser.IsValidExpression();

        Assert.That(actual, Is.EqualTo(expected));
    }


    [TestCase(
        """
        2 + 3 // Hello world
        // It's test
        + 4
        """)]
    [TestCase(
        """
        2 + 3 // Hello world
        + 4
        """)]
    [TestCase(
        """
        2 + 3 // Hello world 
                     // It's test
        + 4
        """)]
    public void AcceptsComments(string expression)
    {
        var parser = new Analyzer(expression);
        Assert.That(parser.IsValidExpression(), Is.True);
        // Assert.That(SyntaxAnalyze.Analyzer.IsValidExpression(expression), Is.True);
    }

    [TestCase("((24 + 3) 4)")]
    [TestCase("(24 + 3")]
    [TestCase("24 + 3)")]
    [TestCase("24 + (3 & 6)")]
    [TestCase("24 + 3y,3")]
    [TestCase("24 + 1_2x + (x_*_y)")]
    [TestCase("24 + 12x + (x_*_y)")]
    [TestCase("! ")]
    [TestCase(" 1! ")]
    [TestCase(" !1! ")]
    [TestCase(" (1+1)! ")]
    [TestCase(" !(1+1)! ")]
    [TestCase(" !! ")]
    [TestCase(" !+ ")]
    [TestCase(" !-1 ")]
    [TestCase(" -! ")]
    [TestCase(" -- ")]
    [TestCase(" -1- ")]
    [TestCase(" --- ")]
    [TestCase(" --! ")]
    [TestCase(" !-- ")]
    // no static type analyzing
    //[TestCase("2 > '3' ")]
    //[TestCase("1 < '1' ")]
    //[TestCase("2 + '3' ")]
    //[TestCase("2 + (1==1) ")]
    //[TestCase("(1+1) + (1==1) ")]
    //[TestCase("1 > (1==1) ")]
    //[TestCase("!'1'")]
    public void ThrowsException(string expression)
    {
        var parser = new Analyzer(expression);
        var actual = parser.IsValidExpression();
        Assert.True(!actual && parser.Error != "");

        //Assert.That(() => parser.IsValidExpression(), Throws.InvalidOperationException);
        //Assert.That(() => SyntaxAnalyze.Analyzer.IsValidExpression(expression), Throws.InvalidOperationException);
    }

    [TestCase(
        """
        x= 'abc;
        """)]
    [TestCase(
        """
        x= ';
        """)]
    [TestCase(
        """
        x= '''';
        """)]
    [TestCase(
        """
        x= ''';
        """)]
    [TestCase(
        """
        a=1;
        x= ''a;
        """)]
    [TestCase(
        """
        x= 'a;
        """)]
    // no checking dynamic types
    //[TestCase(
    //    """
    //    x= 'bcd';
    //    y= x + 1;
    //    """)]
    public void ParseThrowsException(string expression)
    {
        var parser = new Analyzer(expression);
        var actual = parser.Parse();
        Assert.True(!actual && parser.Error != "");

        //Assert.That(() => parser.Parse(), Throws.InvalidOperationException);
        
        //Assert.That(() => SyntaxAnalyze.Analyzer.IsValidExpression(expression), Throws.InvalidOperationException);
    }
}