using System;
using System.Numerics;
using System.Linq;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are functions that operate on integers
namespace Calculon.Types
{
    public static class IntOpExt
    {
        // TODO: Reimpement more efficently
        public static Integer Factorial(this Integer num)
        {
            if (num.data < 0)
            {
                throw new ArgumentException("ERROR: Factorial needs positive integers");
            }
            if (num.data == 0)
            {
                return new Integer(1, num.DisplayBase);
            }
            else
            {
                Integer last = num.Subtract(new Integer(1, num.DisplayBase)).Factorial();
                return num.Multiply(last);
            }
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

        public static Int64 GCF(this Int64 a, Int64 b)
        {
            while (b != 0)
            {
                Int64 temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static Integer LCM(this Integer a, Integer b)
        {
            return a.Divide(a.GCF(b)).Multiply(b);
        }

        static internal Int64 LCM(this Int64 a, Int64 b)
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
                retVal[0] = new Type[] { typeof(Integer) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Integer input = (Integer) cs.stack.Pop();
            return input.Factorial();
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
    }
}