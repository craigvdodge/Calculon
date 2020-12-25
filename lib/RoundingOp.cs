using System;
using System.Text;
using System.Linq;
using System.Numerics;
using System.Globalization;
using System.Collections.Generic;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are Rounding operators
namespace Calculon.Types
{
    public static class RoundingOpsExt
    {
        public static Number Floor(this Number num)
        {
            if (num.IsWholeNumber) { return num; } // you're already a whole num
            if (num.IsNegative)
            {
                num.Numerator += num.Denominator;
            }
            num.Numerator -= BigInteger.Remainder(num.Numerator, num.Denominator);
            num.Reduce();
            return num;
        }

        public static Number Ceiling(this Number num)
        {
            if (num.IsWholeNumber) { return num; } // you're already a whole num
            if (!num.IsNegative)
            {
                num.Numerator += num.Denominator;
            }
            num.Numerator -= BigInteger.Remainder(num.Numerator, num.Denominator);
            num.Reduce();
            return num;
        }

        public static Number Round(this Number num)
        {
            int originalPrecision = num.Precision;
            num.Precision = 1;
            string NumAsString = num.ToString();
            num.Precision = originalPrecision;
            // Now look only at the first digit to the right
            int test = int.Parse(NumAsString.Split('.')[1]);
            if (test < 5)
            {
                return num.Floor();
            }
            else
            {
                return num.Ceiling();
            }
        }
    }
    public class Round : IFunctionCog
    {
        public string FunctionName { get { return "round"; } }

        public int NumArgs { get { return 1; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[1][];
                retVal[0] = new Type[] { typeof(Number) };
                return retVal;
            }
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Number arg = (Number) cs.stack.Pop();
            arg = arg.Round();
            return arg;
        }
    }

    public class Floor : IFunctionCog
    {
        public string FunctionName { get { return "floor"; } }

        public int NumArgs { get { return 1; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[1][];
                retVal[0] = new Type[] { typeof(Number) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Number arg = (Number) cs.stack.Pop();
            arg = arg.Floor();
            return arg;
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }
    }

    public class Ceiling : IFunctionCog
    {
        public string FunctionName { get { return "ceiling"; } }

        public int NumArgs { get { return 1; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[1][];
                retVal[0] = new Type[] { typeof(Number) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Number arg = (Number) cs.stack.Pop();
            arg = arg.Ceiling();
            return arg;
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }
    }

    public class RoundTo : IFunctionCog
    {
        public string FunctionName { get { return "roundto"; } }

        public int NumArgs { get { return 2; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[1][];
                retVal[0] = new Type[] { typeof(Number), typeof(Number) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Number rawPlaces = (Number)cs.stack.Pop();
            int places = int.Parse(rawPlaces.ToString());
            Number arg = (Number) cs.stack.Pop();
            arg.Precision = places;

            return arg;
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            Number test = (Number) cs.stack.Peek();
            if (test.IsNegative || !test.IsWholeNumber)
            {
                return "RoundTo requires a nonnegative whole number to roundto.";
            }
            // Decimal places current limited to max int - 1
            int maximum = int.MaxValue - 1;
            if (test.Numerator > (BigInteger) maximum)
            {
                return "Calculon limited to " + maximum.ToString() + " digits to the right of the decimal point.";
            }
            return string.Empty;
        }
    }
}