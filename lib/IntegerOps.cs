using System;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are functions that operate on integers
namespace Calculon.Types
{
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
        private Int64 factHelper(Int64 input) 
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

    public class IntegerOp: ICalculonType
    {
        public IntegerOp(Function f) => (Fun) = (f);

        public enum Function {GCF, LCM, Fact};
        public Function Fun { get; }

        public EvalReturn Eval(ref ControllerState cs)
        {
            string typecheck = TypeCheck(ref cs);
            if (typecheck != string.Empty)
            {
                return new EvalReturn(Response.Error, typecheck, this.GetType());
            }
            if (Fun == Function.GCF || Fun == Function.LCM)
            {
                Integer rhs = (Integer) cs.stack.Pop();
                Integer lhs = (Integer) cs.stack.Pop();
                Int64 result;
                if (Fun == Function.GCF)
                {
                    result = GreatestCommonFactor(lhs.data, rhs.data);
                }
                else 
                {
                    result = LeastCommonMultiple(lhs.data, rhs.data);
                }
                cs.stack.Push(new Integer(result));
            }
            else
            {
                Integer i = (Integer) cs.stack.Pop();
                cs.stack.Push(new Integer(Factorial(i.data)));
            }
            return new EvalReturn(Response.Ok, cs.stack.Peek().Display, this.GetType());
        }

        public string TypeCheck(ref ControllerState cs)
        {
            if (Fun == Function.Fact)
            {
                if ((cs.stack.Count < 1) ||  (cs.stack.Peek().GetType() != typeof(Integer)))
                {
                    return "ERROR: Requires an integer to operate on";
                }
            }
            else
            {
                if (cs.stack.Count < 2)
                {
                    return "ERROR: Requries two integers to operate on";
                }
                if (cs.stack.Peek().GetType() != typeof(Integer))
                {
                    return "ERROR: Only works on integers";
                }
                ICalculonType temp = cs.stack.Pop();
                Type t = cs.stack.Peek().GetType();
                cs.stack.Push(temp);
                if (t != typeof(Integer))
                {
                    return "ERROR: Only works on integers";
                }
            }
            return string.Empty;
        }

        static internal Int64 GreatestCommonFactor(Int64 a, Int64 b)
        {
            while (b != 0)
            {
                Int64 temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static internal Int64 LeastCommonMultiple(Int64 a, Int64 b)
        {
            return (a / GreatestCommonFactor(a, b)) * b;
        }

        // TODO: Reimpement more efficently
        static internal Int64 Factorial(Int64 input)
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
                return input * Factorial(input-1);
            }
        }

        public string Display { get{ return Fun.ToString(); } }
    }
}