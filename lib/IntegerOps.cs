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
            if (num.IsNegative || ! num.IsWholeNumber)
            {
                throw new ArgumentException("Factorial requires positive integers");
            }

            return new Number(num.Numerator.Factorial());
        }

        public static BigInteger Factorial(this BigInteger num)
        {
            BigInteger result = BigInteger.One;
            for (BigInteger i = BigInteger.One; i <= num; i++)
            {
                result = result * i;
            }
            return result;
        }

        public static Number GCF(this Number lhs, Number rhs)
        {
            if ((lhs.IsNegative || !lhs.IsWholeNumber) ||
                (rhs.IsNegative || !rhs.IsWholeNumber))
            {
                throw new ArgumentException("GCF requires positive integers");
            }

            Number output = new Number(lhs.Numerator.GCF(rhs.Numerator));
            output.DisplayBase = ArithOpExtensions.BaseRules(lhs.DisplayBase, rhs.DisplayBase);

            return output;
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

        public static Number LCM(this Number lhs, Number rhs)
        {
            if ((lhs.IsNegative || !lhs.IsWholeNumber) ||
                (rhs.IsNegative || !rhs.IsWholeNumber))
            {
                throw new ArgumentException("LCM requires positive integers");
            }
            Number output = new Number(lhs.Numerator.LCM(rhs.Numerator));
            output.DisplayBase = ArithOpExtensions.BaseRules(lhs.DisplayBase, rhs.DisplayBase);

            return output;
        }

        public static BigInteger LCM(this BigInteger a, BigInteger b)
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
            if (test.IsNegative || ! test.IsWholeNumber)
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
                retVal[0] = new Type[] { typeof(Number), typeof(Number) };
                return retVal;
            } 
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Number b = ((Number)cs.stack.Pop());
            Number a = ((Number) cs.stack.Pop());
            return a.GCF(b);
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            Number zero = (Number) cs.stack.ElementAt(0);
            Number one = (Number)cs.stack.ElementAt(1);
            if (zero.IsNegative || ! zero.IsWholeNumber || 
                one.IsNegative || ! one.IsWholeNumber)
            {
                return "GCF requires positive integers";
            }

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
                retVal[0] = new Type[] { typeof(Number), typeof(Number) };
                return retVal; 
            } 
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Number b = (Number)cs.stack.Pop();
            Number a = (Number)cs.stack.Pop();
            return a.LCM(b);
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            Number zero = (Number)cs.stack.ElementAt(0);
            Number one = (Number)cs.stack.ElementAt(1);
            if (zero.IsNegative || !zero.IsWholeNumber ||
                one.IsNegative || !one.IsWholeNumber)
            {
                return "LCM requires positive integers";
            }

            return string.Empty;
        }
    }
}