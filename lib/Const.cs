using System;
using System.Numerics;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are constants
namespace Calculon.Types
{
    public class Constant : ICalculonType
    {
        public Constant(string Name, Func<int, Number> fun) => (name, function) = (Name, fun);
        public string Display { get { return name; } }
        private string name;
        private Func<int, Number> function;

        public Number GetNumber(int precision)
        {
            if (precision <= 0)
            {
                precision = Number.GlobalPrecision;
            }
            return function(precision);
        }

        public EvalReturn Eval(ref ControllerState cs)
        {
            cs.stack.Push(this);
            return new EvalReturn(Response.Ok, this);
        }
    }

    public class Pi : IFunctionCog
    {
        // Chudnovsky algorithm
        // verfied to 10k places
        public static Number Compute(int precision)
        {
            Number C = new Number(10005);
            C.View = Number.ViewType.Real;
            C.Precision = precision;
            C = (C.Sqrt()).Multiply(new Number(426880));
            int maxQ = precision / 14;
            Number M, Mnum, Mdenom = new Number(BigInteger.Zero);
            Number L = new Number(BigInteger.Zero);
            Number L1 = new Number(new BigInteger(545140134));
            Number L2 = new Number(new BigInteger(13591409));
            Number X = new Number(BigInteger.Zero);
            Number qFact = new Number(BigInteger.Zero);
            Number sum = new Number(BigInteger.Zero); 
            Number term = new Number(BigInteger.Zero);
            BigInteger Xbase = new BigInteger(-262537412640768000);

            for (int q=0; q<=maxQ; q++)
            {
                Number Q = new Number(new BigInteger(q));
                qFact = Q.Factorial();
                Mnum = (Q.Multiply(new Number(6)).Factorial());
                Mdenom = Q.Multiply(new Number(3)).Factorial();
                Mdenom = Mdenom.Multiply(qFact).Multiply(qFact).Multiply(qFact);
                M = Mnum.Divide(Mdenom);
                L = L1.Multiply(Q).Add(L2);
                X = new Number(BigInteger.Pow(Xbase, q));
                term = (M.Multiply(L)).Divide(X);
                sum = sum.Add(term);
            }
            return C.Multiply(sum.Inverse());
        }
        public string FunctionName { get { return "pi"; } }

        public int NumArgs { get { return 0; } }

        public Type[][] AllowedTypes { get { return null; } }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Constant pi = new Constant("pi", Pi.Compute);
            return pi;
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }
    }

    public class E : IFunctionCog
    {
        // Algorithim taken from Harlen Brothers
        // "Improving the Convergance of Newton's Series Approximation for e"
        // The College Mathematics Journal vol 35, no. 1.
        // Confirmed to 10000 decimal places
        public static Number Compute(int precision)
        {
            BigInteger k = BigInteger.Zero;
            int dp = 0;
            Number result = new Number(BigInteger.Zero);
            result.View = Number.ViewType.Real;
            do
            {
                Number next = new Number((2 * k + 2), (2 * k + 1).Factorial());
                result = result.Add(next);
                dp = E.DecimalPlace(k);
                k++;
             } while (dp < precision);
            
            return result;
        }

        private static int DecimalPlace(BigInteger k)
        {
            BigInteger numerator = (2 * k + 1).Factorial();
            BigInteger denominator = 2 * k + 2;
            BigInteger temp = numerator / denominator;
            double foo = BigInteger.Log(temp, 10);
            return (int) Math.Floor(foo);
        }
        public string FunctionName { get { return "e"; } }

        public int NumArgs { get { return 0; } }

        public Type[][] AllowedTypes { get { return null; } }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Constant e = new Constant("e", E.Compute);
            return e;
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }
    }

    // This are for constants that are typically Reals (decimals)
    public class RealConstant: ICalculonType
    {
        public RealConstant(Constant c) => (Const) = (c);

        public Real ToReal()
        {
            switch (Const)
            {
                case Constant.pi : return new Real(Math.PI);
                case Constant.e : return new Real(Math.E);
                case Constant.tau : return new Real(Math.Tau);
                default : throw new Exception("Unknown constant");
            }
        } 
        public enum Constant {pi, e, tau}
        public Constant Const { get; }

        public EvalReturn Eval(ref ControllerState cs)
        {
            cs.stack.Push(this);
            return new EvalReturn(Response.Ok, this);
        }

        public string Display
        {
            get
            {
                switch (Const)
                {
                    case Constant.pi : return "pi"; 
                    case Constant.e : return "e";
                    case Constant.tau : return "tau";
                    default: return "If you're seeing this, this is a bug.";
                }
            }
        }

    }
}