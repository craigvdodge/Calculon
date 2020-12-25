using System;
using Xunit;
using Calculon;
using Calculon.Types;
using System.Numerics;

namespace ut
{
    public class LibTest
    {
        #region Number Class Tests
        [Fact]
        public void NumberCtors()
        {
            Number oneHalf = new Number(16, 32);
            Assert.Equal(1, oneHalf.Numerator);
            Assert.Equal(2, oneHalf.Denominator);
            Assert.Equal(Number.ViewType.Rational, oneHalf.View);
            Number answer = new Number((BigInteger) 42);
            Assert.Equal(42, answer.Numerator);
            Assert.Equal(1, answer.Denominator);
            Assert.Equal(Number.ViewType.Integer, answer.View);
            Assert.Throws<ArgumentException>(() => Number.Parse("blarg"));
        }

        [Fact]
        public void NumberToStringTest()
        {
            Number answer = new Number(42);
            Assert.Equal("42", answer.ToString());
            answer.DisplayBase = Number.Base.Bin;
            Assert.Equal("0101010b", answer.ToString());
            answer.DisplayBase = Number.Base.Hex;
            Assert.Equal("2Ah", answer.ToString());
            answer.DisplayBase = Number.Base.Oct;
            Assert.Equal("052o", answer.ToString());
            answer.DisplayBase = Number.Base.Dec;
            answer.View = Number.ViewType.Rational;
            Assert.Equal("42/1", answer.ToString());
            Number piish = new Number(22,7);
            piish.View = Number.ViewType.Real;
            piish.Precision = 14;
            Assert.Equal("3.14285714285714", piish.ToString());
        }

        [Theory]
        [InlineData("-3/4", -3, 4, Number.ViewType.Rational, Number.Base.Dec)]
        [InlineData("+FH", 15, 1, Number.ViewType.Integer, Number.Base.Hex)]
        [InlineData("42", 42, 1, Number.ViewType.Integer, Number.Base.Dec)]
        [InlineData("77o", 63, 1, Number.ViewType.Integer, Number.Base.Oct)]
        [InlineData("-1000b", -8, 1, Number.ViewType.Integer, Number.Base.Bin)]
        [InlineData(".75", 3, 4, Number.ViewType.Real, Number.Base.Dec)]
        public void NumberStringParsing(
            string input, int num, int denom, Number.ViewType view, Number.Base b)
        {
            Number n = new Number(input);
            Assert.Equal(num, (int) n.Numerator);
            Assert.Equal(denom, (int)n.Denominator);
            Assert.Equal(view, n.View);
            Assert.Equal(b, n.DisplayBase);
        }

        #endregion

        [Theory]
        [InlineData("8675309", typeof(Number))]
        [InlineData("-45", typeof(Number))]
        [InlineData("-2.8", typeof(Number))]
        [InlineData("3.14159", typeof(Number))]
        [InlineData("3/4", typeof(Number))]
        [InlineData("-5/8", typeof(Number))]
        [InlineData(@"""Plan_9_From_Outer_Space""", typeof(Literal))]
        [InlineData("pi", typeof(Constant))]
        [InlineData("e", typeof(Constant))]
        [InlineData("@@@", typeof(ErrorType), Response.Error, "PARSE ERROR: @@@")]
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
        [InlineData("ah 1 +", "0Bh")]
        [InlineData("7o 2o *", "016o")]
        [InlineData("101b 10b + todec", "7")]
        [InlineData("8 7 mod", "1")]
        [InlineData("2.1 2 mod", "0.1")]
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
            BasicTest(test, expectedOut, maxChar);
        }

        [Theory]
        [InlineData("10 toHex", "0Ah")]
        [InlineData("9 ToOct", "011o")]
        [InlineData("3 tobin", "011b")]
        [InlineData("todec", "todec requires 1 argument(s)")]
        [InlineData("1.5 toHex", "Base Conversions only supported for Whole Integers.")]
        public void BaseConversionTest(string test, string expectedOut)
        {
            BasicTest(test, expectedOut);
        }

        [Theory]
        [InlineData("3.14159 round", "3")]
        [InlineData("3.14159 3 roundto", "3.142")]
        [InlineData("3.6 floor", "3")]
        [InlineData("3.2 ceiling", "4")]
        [InlineData("2 floor", "2")]
        [InlineData("ceiling", "ceiling requires 1 argument(s)")]
        [InlineData("roundTo", "roundto requires 2 argument(s)")]
        [InlineData("1.1 1.2 roundto", "RoundTo requires a nonnegative whole number to roundto.")]
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
        public void PowerTest(string test, string expected)
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
