using System;
using Xunit;
using Calculon;
using Calculon.Types;
using System.Text;
using System.Numerics;

namespace ut
{
    public class LibTest
    {
        [Fact]
        public void ConfigTest()
        {
            Config c = new Config();
            Assert.True(c.AllowFilesystemWrites);
        }
        [Theory]
        [InlineData("8675309", typeof(Integer))]
        [InlineData("-45", typeof(Integer))]
        [InlineData("-2.8", typeof(Real))]
        [InlineData("3.14159", typeof(Real))]
        [InlineData("3/4", typeof(Rational))]
        [InlineData("-5/8", typeof(Rational))]
        [InlineData(@"""Plan_9_From_Outer_Space""", typeof(Literal))]
        [InlineData("pi", typeof(RealConstant))]
        [InlineData("e", typeof(RealConstant))]
        [InlineData("tau", typeof(RealConstant))]
        [InlineData("@@@", typeof(ErrorType), Response.Error, "Error: @@@ is not a number or function")]
        public void BasicPushParsingTest(
            string testStr,
            Type ExpectedType,
            Response ExpectedResponse = Response.Ok,
            string ExpectedMsg = null)
        {
            string msg;
            if (ExpectedMsg is null)
            {
                msg = testStr;
            }
            else
            {
                msg = ExpectedMsg;
            }
            Controller calc = new Controller();
            EvalReturn ret = calc.Eval(testStr);
            Assert.Equal(ExpectedResponse, ret.Response);
            Assert.Equal(msg, ret.Msg);
            Assert.True(ExpectedType == ret.Type);
        }

        [Theory]
        [InlineData("0 sin", "0")]
        [InlineData("30 sin", "0.5")]  
        [InlineData("90 sin", "1")]
        [InlineData("0 cos", "1")]
        [InlineData("60 cos", "0.5")]
        [InlineData("90 cos", "0")]
        [InlineData("0 tan", "0")]
        [InlineData("45 tan", "1")]
        [InlineData("0.5 arcsin", "30")]
        [InlineData("0.5 arccos", "60")]
        [InlineData("1 arctan", "45")]
        [InlineData("10 sinh", "11013.2329")]
        [InlineData("1000 arcsinh", "7.6009")]
        [InlineData("8 cosh", "1490.4792")]
        [InlineData("9001 arccosh", "9.7982")]
        [InlineData("1 tanh", "0.7616")]
        [InlineData("0.25 arctanh", "0.2554")]
        public void TrigTest(string input, string expected)
        {
            Controller calc = new Controller();
            StringBuilder fullTest = new StringBuilder("\"degrees\" setmode ");
            fullTest.Append(input);
            fullTest.Append(" 4 roundto");
            EvalReturn ret = calc.Eval(fullTest.ToString());
            Assert.True(typeof(Calculon.Types.Real) == ret.Type);
            Assert.Equal(expected, ret.Msg);
        }

        [Theory]
        [InlineData("2 2 +", "4")]
        [InlineData("1.1 1 +", "2.1", 3)]
        [InlineData("1.1 1.1 +", "2.2", 3)]
        [InlineData("2 3 -", "-1")]
        [InlineData("2.1 1 -","1.1",3)]
        [InlineData("2.1 1.1 -","1")]
        [InlineData("3 -3 *", "-9")]
        [InlineData("1.1 2 *", "2.2", 3)]
        [InlineData("pi -2.0 *", "-6.28", 5)]
        [InlineData("8 4 /", "2")]
        [InlineData("ah 1 +", "0Bh")]
        [InlineData("7o 2o *", "016o")]
        [InlineData("101b 10b + todec", "7")]
        [InlineData("8 8 mod", "0")]
        [InlineData("2.1 2 mod", "0.1", 3)]
        [InlineData("1/2 1/4 +", "3/4")]
        [InlineData("1/4 1/2 -", "-1/4")]
        [InlineData("1/4 -1/4 *", "-1/16")]
        [InlineData("1/2 1/6 /", "3/1")]
        [InlineData("28/11 10 mod", "28/11")]
        [InlineData("1/2 3 mod", "1/2")]
        [InlineData("5 !", "120")]
        [InlineData("2 4 lcm", "4")]
        [InlineData("2 4 gcf", "2")]
        [InlineData("-1/3 abs", "1/3")]
        [InlineData("-3.1415 abs", "3.1415")]
        [InlineData("-12 abs", "12")]
        [InlineData("2 inv", "0.5")]
        [InlineData("0.5 inv", "2")]
        [InlineData("5/8 inv", "8/5")]
        [InlineData("9 3 logx", "2")]
        public void ArithOpTest(string test, string expectedOut, int maxChar = -1)
        {
            BasicTest(test, expectedOut, maxChar);
        }

        [Theory]
        [InlineData("10 toHex", "0Ah")]
        [InlineData("9 ToOct", "011o")]
        [InlineData("3 tobin", "011b")]
        [InlineData("todec", "todec requires 1 argument(s)")]
        [InlineData("1.5 toHex", "Unsupported types (Calculon.Types.Real )")]
        public void BaseConversionTest(string test, string expectedOut)
        {
            BasicTest(test, expectedOut);
        }

        [Theory]
        [InlineData("3.14159 round", "3")]
        [InlineData("3.14159 2 roundto", "3.14")]
        [InlineData("3.6 floor", "3")]
        [InlineData("3.2 ceiling", "4")]
        [InlineData("2 floor", "Unsupported types (Calculon.Types.Integer )")]
        [InlineData("ceiling", "ceiling requires 1 argument(s)")]
        [InlineData("roundTo", "roundto requires 2 argument(s)")]
        [InlineData("1.1 1.2 roundto", "Unsupported types (Calculon.Types.Real Calculon.Types.Real )")]
        [InlineData("round", "round requires 1 argument(s)")]
        public void RoundingOpsTests(string test, string expected)
        {
            BasicTest(test, expected);
        }

        [Theory]
        [InlineData("3.0 epow ln", "3")]
        [InlineData("10.0 5.0 pow log", "5")]
        [InlineData("e 3.0 pow ln", "3")]
        [InlineData("5 epow ln", "5")]
        [InlineData("2 4 pow", "16")]
        [InlineData("4 pi pow floor", "77")]
        [InlineData("e ln", "1")]
        [InlineData("10 log", "1")]
        [InlineData("25 sqrt", "5")]
        [InlineData("27 3 root", "3")]
        [InlineData("256 log2", "8")]
        public void PowerTest(string test, string expected)
        {
            BasicTest(test, expected);
        }

        [Theory]
        [InlineData("2 \"foo\" sto \"foo\" rcl", "2")]
        [InlineData("3.14 \"bar\" sto \"bar\" rcl", "3.14")]
        [InlineData("1/2 \"fred\" sto \"fred\" rcl", "1/2")]
        [InlineData("\"HelloWorld\" \"barney\" sto \"barney\" rcl", "\"HelloWorld\"")]
        public void MemoryTest(string test, string expected)
        {
            BasicTest(test, expected);
        }

        [Theory]
        [InlineData("0.625 torat", "5/8")]
        [InlineData("1/8 toreal", "0.125")]
        public void RealRatoinalConversions(string test, string expected)
        {
            BasicTest(test, expected);
        }

        // This code gets written over and over but it just
        // needs different names
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public void BasicTest(string test, string expectedOut, int maxChar = -1)
        {
            // What Calculon returns is a string, without data type
            // So regular xunit precision won't work
            // instead, truncate string representations of floating point
            // to provide bounds for "equals"
            Controller calc = new Controller();
            EvalReturn ret = calc.Eval(test);
            string Actual;
            if (maxChar < 0)
            {
                Actual = ret.Msg;
            }
            else
            {
                Actual = ret.Msg.Substring(0, maxChar);
            }
            Assert.Equal(expectedOut, Actual);
        }
    }
}
