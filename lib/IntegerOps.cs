using System;
using System.Numerics;
using System.Linq;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are functions that operate on integers
namespace Calculon.Types
{
    public static class IntOpExt
    {
        // Better algorithims exist but it becomes a rabbit hole fast. This'll do for now.
        public static Number Factorial(this Number num)
        {
            if (num.Numerator.Sign < 0 && num.Denominator != 1)
            {
                throw new ArgumentException("Factorial requires positive integers");
            }

            BigInteger result = BigInteger.One;
            for (BigInteger i=BigInteger.One; i <= num.Numerator; i++)
            {
                result = result * i;
            }

            return new Number(result);
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
        public string FunctionName { get { return "fact"; } }

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
            Number input = (Number) cs.stack.Pop();
            return input.Factorial();
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            Number test = ((Number)cs.stack.Peek());
            if (test.Numerator.Sign < BigInteger.Zero || test.Denominator != BigInteger.One)
            {
                return "Factorial requires positive integers";
            }
            return string.Empty;
        }
    }

    public class GreatestCommonFactor : IFunctionCog
    {
        public string FunctionName { get { return "gcf"; } }

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

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }
    }

    public class LeastCommonMultiple : IFunctionCog
    {
        public string FunctionName { get { return "lcm"; } }

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

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }
    }
}