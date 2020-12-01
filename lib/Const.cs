using System;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are constants
namespace Calculon.Types
{
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