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
        [InlineData(@"""Plan_9_From_Outer_Space""", typeof(Literal))]
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
        [InlineData("3.14 -2.0 *", "-6.28", 4)]
        [InlineData("8 4 /", "2")]
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
            Assert.Equal(expectedOut, ret.Msg);
        }
    }
}
