using System;
using Xunit;
using Calculon;
using Calculon.Types;

namespace ut
{
    public class LibTest
    {
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
        [InlineData("@@@", typeof(ErrorType), Response.Error, "PARSE ERROR: @@@")]
        [InlineData("Exit", typeof(ExitType), Response.Exit, "")]
        [InlineData("quIt", typeof(ExitType), Response.Exit, "")]
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
        [InlineData("ah 1 +", "Bh")]
        [InlineData("7o 2o *", "16o")]
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
        public void ArithOpTest(string test, string expectedOut, int maxChar = -1)
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

        [Theory]
        [InlineData("10 toHex", "Ah")]
        [InlineData("9 ToOct", "11o")]
        [InlineData("3 tobin", "11b")]
        [InlineData("todec", "ARG ERROR: Need Integer to convert")]
        [InlineData("1.5 toHex", "TYPE ERROR: Base Op Requires Integer")]
        public void BaseConversionTest(string test, string expectedOut)
        {
            Controller calc = new Controller();
            EvalReturn ret = calc.Eval(test);
            Assert.Equal(expectedOut, ret.Msg);
        }

        [Theory]
        [InlineData("3.14159 round", "3")]
        [InlineData("3.14159 2 roundto", "3.14")]
        [InlineData("3.6 floor", "3")]
        [InlineData("3.2 ceiling", "4")]
        [InlineData("2 floor", "TYPE ERROR: Argument is not Real")]
        [InlineData("ceiling", "ARG ERROR: Requires Real")]
        [InlineData("roundTo", "ARG ERROR: Requires Real and int decimal places")]
        [InlineData("1.1 1.2 roundto", "TYPE ERROR: Places Argument not Integerl")]
        [InlineData("33 1 roundto", "TYPE ERROR: Attempting to RoundTo on non-Real")]
        public void RoundingOpsTests(string test, string expected)
        {
            Controller calc = new Controller();
            EvalReturn ret = calc.Eval(test);
            Assert.Equal(expected, ret.Msg);
        }
    }
}
