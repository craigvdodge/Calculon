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
        public string[] FunctionName { get { return new string[] { "round" }; } }

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
            Real arg = (Real) cs.stack.Pop();
            //Note underlying .net function is double->double
            //But I'm writing my on calculator (with blackjack and hookers)
            //and returning Integer makes more sense to me
            return new Integer((BigInteger) Math.Round(arg.data));
        }
    }

    public class Floor : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "floor" }; } }

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
    }

    public class Ceiling : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "ceiling" }; } }

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
    }

    public class RoundTo : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "roundto" }; } }

        public int NumArgs { get { return 2; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[1][];
                retVal[0] = new Type[] { typeof(Integer), typeof(Real) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Integer places = (Integer) cs.stack.Pop();
            Real arg = (Real)cs.stack.Pop();
            // We're rounding to decimal palces so still should be Real
            double newData = Math.Round(arg.data, (Int32)places.data);
            return new Real(newData);
        }
    }
}