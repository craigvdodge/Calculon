using System;
using System.Numerics;
using System.Linq;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are functions that operate on integers
namespace Calculon.Types
{
    // BUGBUG: has an overflow wrap-around bug
    public class Factorial : IFunctionCog
    {
        public Factorial()
        {
            allowedTypes = new Type[1][];
            allowedTypes[0] = new Type[1];
            allowedTypes[0][0] = typeof(Integer);
        }

        public string FunctionName { get { return "fact"; } }

        public int NumArgs { get { return 1; } }

        private Type[][] allowedTypes;
        public Type[][] AllowedTypes { get { return allowedTypes; } }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Integer input = (Integer) cs.stack.Pop();
            Integer result = new Integer(factHelper(input.data));
            return result;         
        }

        // TODO: Reimpement more efficently
        private BigInteger factHelper(BigInteger input) 
        {
            if (input < 0)
            {
                throw new ArgumentException("ERROR: Factorial needs positive integers");
            }

            if (input == 0)
            {
                return 1;
            }
            else
            {
                return input * factHelper(input - 1);
            }
        }
    }

    public class GreatestCommonFactor : IFunctionCog
    {
        public GreatestCommonFactor()
        {
            allowedTypes = new Type[1][];
            allowedTypes[0] = new Type[] { typeof(Integer), typeof(Integer) };
        }
        public string FunctionName { get { return "gcf"; } }

        public int NumArgs { get { return 2; } }

        private Type[][] allowedTypes;
        public Type[][] AllowedTypes { get { return allowedTypes; } }

        public ICalculonType Execute(ref ControllerState cs)
        {
            BigInteger b = ((Integer)cs.stack.Pop()).data;
            BigInteger a = ((Integer) cs.stack.Pop()).data;
            Integer result = new Integer(GreatestCommonFactor.GCF(a, b));
            return result;
        }

        static internal BigInteger GCF(BigInteger a, BigInteger b)
        {
            while (b != 0)
            {
                BigInteger temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static internal Int64 GCF(Int64 a, Int64 b)
        {
            while (b != 0)
            {
                Int64 temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
    }

    public class LeastCommonMultiple : IFunctionCog
    {
        public LeastCommonMultiple()
        {
            allowedTypes = new Type[1][];
            allowedTypes[0] = new Type[] { typeof(Integer), typeof(Integer) };
        }
        public string FunctionName { get { return "lcm"; } }

        public int NumArgs { get { return 2; } }

        private Type[][] allowedTypes;
        public Type[][] AllowedTypes { get { return allowedTypes; } }

        public ICalculonType Execute(ref ControllerState cs)
        {
            BigInteger b = ((Integer)cs.stack.Pop()).data;
            BigInteger a = ((Integer)cs.stack.Pop()).data;
            Integer result = new Integer(LeastCommonMultiple.LCM(a, b));
            return result;
        }

        static internal BigInteger LCM(BigInteger a, BigInteger b)
        {
            return (a / GreatestCommonFactor.GCF(a, b)) * b;
        }

        static internal Int64 LCM(Int64 a, Int64 b)
        {
            return (a / GreatestCommonFactor.GCF(a, b)) * b;
        }
    }
}