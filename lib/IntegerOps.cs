using System;
using System.Numerics;
using System.Linq;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are functions that operate on integers
namespace Calculon.Types
{
    public static class IntOpExt
    {
        public static Integer Factorial(this Integer num)
        {
            if (num.data < 0)
            {
                throw new ArgumentException(Config.handle.strings["FactorialNeg"]);
            }
            BigInteger result = BigInteger.One;
            for (BigInteger i = BigInteger.One; i <= num.data; i++)
            {
                result = result * i;
            }
            return new Integer(result);
        }

        public static Integer GCF(this Integer lhs, Integer rhs)
        {
            BigInteger a = lhs.data;
            BigInteger b = rhs.data;
            while (b != 0)
            {
                BigInteger temp = b;
                b = a % b;
                a = temp;
            }

            return new Integer(a, lhs.DisplayBase);
        }

        public static BigInteger GCF(this BigInteger a, BigInteger b)
        {
            while (b != 0)
            {
                BigInteger temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static Integer LCM(this Integer a, Integer b)
        {
            return a.Divide(a.GCF(b)).Multiply(b);
        }

        static internal BigInteger LCM(this BigInteger a, BigInteger b)
        {
            return (a / a.GCF(b)) * b;
        }

    }

    public class Factorial : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "fact", "!" }; } }

        public int NumArgs { get { return 1; } }

        public Type[][] AllowedTypes 
        { 
            get 
            {
                Type[][] retVal = new Type[1][];
                retVal[0] = new Type[] { typeof(Integer) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Integer input = (Integer) cs.stack.Pop();
            if (input.data < 0)
            {
                cs.stack.Push(input);
                return new ErrorType(cs.Config.strings["FactorialNeg"]);
            }
            return input.Factorial();
        }
    }

    public class GreatestCommonFactor : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "gcf" }; } }

        public int NumArgs { get { return 2; } }

        public Type[][] AllowedTypes 
        { 
            get 
            {
                Type[][] retVal = new Type[1][];
                retVal[0] = new Type[] { typeof(Integer), typeof(Integer) };
                return retVal;
            } 
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Integer b = ((Integer)cs.stack.Pop());
            Integer a = ((Integer) cs.stack.Pop());
            return a.GCF(b);
        }
    }

    public class LeastCommonMultiple : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "lcm" }; } }

        public int NumArgs { get { return 2; } }

        public Type[][] AllowedTypes 
        { 
            get 
            {
                Type[][] retVal = new Type[1][];
                retVal[0] = new Type[] { typeof(Integer), typeof(Integer) };
                return retVal; 
            } 
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Integer b = (Integer)cs.stack.Pop();
            Integer a = (Integer)cs.stack.Pop();
            return a.LCM(b);
        }
    }
}