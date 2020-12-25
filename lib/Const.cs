using System;

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
        // TODO: This is just a stub. Need a real algorithim.
        public static Number Compute(int precision)
        {
            Number notRight = new Number(22, 7);
            notRight.View = Number.ViewType.Real;
            return notRight;
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
        // This is a stub. Replace with real algo
        public static Number Compute(int precision)
        {
            Number hack = new Number("2.7182818284590451");
            return hack;
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