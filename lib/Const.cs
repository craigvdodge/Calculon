using System;
using System.Text.RegularExpressions;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are constants
namespace Calculon.Types
{
    // This are for constants that are typically Reals (decimals)
    public class RealConstant: ICalculonType
    {
        public RealConstant(Constant c) => (Const) = (c);
        public RealConstant(string s)
        {
            if (PiMatch.IsMatch(s))
            {
                Const = Constant.pi;
            }
            else if (eMatch.IsMatch(s))
            {
                Const = Constant.e;
            }
            else if (TauMatch.IsMatch(s))
            {
                Const = Constant.tau;
            }
            else
            {
                throw new ArgumentException("Unknown constant: " + s);
            }
        }

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

        public override string ToString()
        {
            return Display;
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

        #region Parsing
        public static bool IsMatch(string s)
        {
            return PiMatch.IsMatch(s) || eMatch.IsMatch(s) || TauMatch.IsMatch(s);
        }
        private static readonly Regex PiMatch =
            new Regex("^pi$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex eMatch =
            new Regex("^e$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex TauMatch =
            new Regex("^tau$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        #endregion

    }
}