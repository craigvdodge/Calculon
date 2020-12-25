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
    public class Round : IFunctionCog
    {
        public string FunctionName { get { return "round"; } }

        public int NumArgs { get { return 1; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[1][];
                retVal[0] = new Type[] { typeof(Real) };
                return retVal;
            }
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Real arg = (Real) cs.stack.Pop();
            //Note underlying .net function is double->double
            //But I'm writing my on calculator (with blackjack and hookers)
            //and returning Integer makes more sense to me
            return new Integer((BigInteger) Math.Round(arg.data));
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
                retVal[0] = new Type[] { typeof(Real) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Real arg = (Real)cs.stack.Pop();
            //see comment on Round
            return new Integer((BigInteger)Math.Floor(arg.data));
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
                retVal[0] = new Type[] { typeof(Real) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Real arg = (Real)cs.stack.Pop();
            //see comment on Round
            return new Integer((BigInteger)Math.Ceiling(arg.data));
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