using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;

namespace Calculon.Types
{
    public static class TrigExt
    {
        public static Real ToRadians(this Real angle, Config.Mode mode)
        {
            switch (mode)
            {
                case Config.Mode.Degrees:
                    return new Real(angle.data * Math.PI / 180.0);
                case Config.Mode.Radians:
                    return angle;
                case Config.Mode.Grad:
                    return new Real(angle.data * Math.PI / 200.0);
                default:
                    string err = String.Format(Config.handle.strings["CorruptedConfig"], "AngleMode");
                    throw new Exception(err);
            }  
        }

        public static Real ToDegrees(this Real angle, Config.Mode mode)
        {
            switch (mode)
            {
                case Config.Mode.Degrees:
                    return angle;
                case Config.Mode.Radians:
                    return new Real(angle.data * 180.0 / Math.PI);
                case Config.Mode.Grad:
                    return new Real(angle.data * 0.9);
                default:
                    string err = String.Format(Config.handle.strings["CorruptedConfig"], "AngleMode");
                    throw new Exception(err);
            }
        }

        public static Real ToGrad(this Real angle, Config.Mode mode)
        {
            switch (mode)
            {
                case Config.Mode.Degrees:
                    return new Real(angle.data * 10.0 / 9.0);
                case Config.Mode.Radians:
                    return new Real(angle.data * 200.0 / Math.PI);
                case Config.Mode.Grad:
                    return angle;
                default:
                    string err = String.Format(Config.handle.strings["CorruptedConfig"], "AngleMode");
                    throw new Exception(err);
            }
        }
        // TODO: check for special angles and return specific answers?
        public static Real Sin(this Real angle)
        {
            Real angleRadians = angle.ToRadians(Config.handle.AngleMode);
            return new Real(Math.Sin(angleRadians.data));
        }

        public static Real ArcSin(this Real x)
        {
            Real output = new Real(Math.Asin(x.data));
            switch (Config.handle.AngleMode)
            {
                case Config.Mode.Radians: return output;
                case Config.Mode.Degrees: return output.ToDegrees(Config.Mode.Radians);
                case Config.Mode.Grad: return output.ToGrad(Config.Mode.Radians);
                default: throw new ArgumentException();
            }
        }

        public static Real Cos(this Real angle)
        {
            Real angleRadians = angle.ToRadians(Config.handle.AngleMode);
            return new Real(Math.Cos(angleRadians.data));
        }

        public static Real ArcCos(this Real x)
        {
            Real output = new Real(Math.Acos(x.data));
            switch (Config.handle.AngleMode)
            {
                case Config.Mode.Radians: return output;
                case Config.Mode.Degrees: return output.ToDegrees(Config.Mode.Radians);
                case Config.Mode.Grad: return output.ToGrad(Config.Mode.Radians);
                default: throw new ArgumentException();
            }
        }

        //BUGBUG 90 tan should be NaN
        public static Real Tan(this Real angle)
        {
            Real angleRadians = angle.ToRadians(Config.handle.AngleMode);
            return new Real(Math.Tan(angleRadians.data));
        }

        public static Real ArcTan(this Real x)
        {
            Real output = new Real(Math.Atan(x.data));
            switch (Config.handle.AngleMode)
            {
                case Config.Mode.Radians: return output;
                case Config.Mode.Degrees: return output.ToDegrees(Config.Mode.Radians);
                case Config.Mode.Grad: return output.ToGrad(Config.Mode.Radians);
                default: throw new ArgumentException();
            }
        }
    }

    public abstract class TrigBase : IFunctionCog
    {
        public abstract string[] FunctionName { get; }
        public int NumArgs { get { return 1; } }
        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] output = new Type[4][];
                output[0] = new Type[] { typeof(Real) };
                output[1] = new Type[] { typeof(Integer) };
                output[2] = new Type[] { typeof(Rational) };
                output[3] = new Type[] { typeof(RealConstant) };
                return output;
            }
        }
        public virtual ICalculonType Execute(ref ControllerState cs)
        {
            Real convertedAngle = new Real((ICalculonType)cs.stack.Pop());
            return Op(convertedAngle);
        }

        internal abstract Real Op(Real angle); 
    }

    public class Sin : TrigBase
    {
        public override string[] FunctionName { get { return new string[] { "sin"}; } }
        internal override Real Op(Real angle)
        {
            return angle.Sin();
        }
    }

    public class ArcSin : TrigBase
    {
        public override string[] FunctionName { get { return new string[] { "arcsin" }; } }
        internal override Real Op(Real angle)
        {
            return angle.ArcSin();
        }
    }

    public class Cos : TrigBase
    {
        public override string[] FunctionName { get { return new string[] { "cos" }; } }
        internal override Real Op(Real angle)
        {
            return angle.Cos();
        }
    }

    public class ArcCos : TrigBase
    {
        public override string[] FunctionName { get { return new string[] { "arccos" }; } }
        internal override Real Op(Real angle)
        {
            return angle.ArcCos();
        }
    }

    public class Tan : TrigBase
    {
        public override string[] FunctionName { get { return new string[] { "tan" }; } }
        internal override Real Op(Real angle)
        {
            return angle.Tan();
        }
    }

    public class ArcTan : TrigBase
    {
        public override string[] FunctionName { get { return new string[] { "arctan" }; } }
        internal override Real Op(Real angle)
        {
            return angle.ArcTan();
        }
    }

    public class GetMode : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "getmode" }; } }

        public int NumArgs { get { return 0; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[0][];
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            switch (cs.Config.AngleMode)
            {
                case Config.Mode.Degrees:
                    return new Literal("\"" + cs.Config.strings["Degrees"] + "\"");
                case Config.Mode.Radians:
                    return new Literal("\"" + cs.Config.strings["Radians"] + "\"");
                case Config.Mode.Grad:
                    return new Literal("\"" + cs.Config.strings["Grad"] + "\"");
                default:
                    string err = String.Format(cs.Config.strings["CorruptedConfig"], "AngleMode");
                    throw new Exception(err);
            }

        }
    }

    public class SetMode : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "setmode" }; } }

        public int NumArgs { get { return 1; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] output = new Type[1][];
                output[0] = new Type[] { typeof(Literal) };
                return output;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Literal literalArg = (Literal) cs.stack.Pop();
            string arg = literalArg.Display;
            arg = arg.Trim(new Char[] { ' ', '\"' });
            Regex argMatch = new Regex(arg, RegexOptions.IgnoreCase);
            if (argMatch.IsMatch(cs.Config.strings["Degrees"]))
            {
                cs.Config.AngleMode = Config.Mode.Degrees;
                return new EmptyType();
            }
            else if (argMatch.IsMatch(cs.Config.strings["Radians"]))
            {
                cs.Config.AngleMode = Config.Mode.Radians;
                return new EmptyType();
            }
            else if (argMatch.IsMatch(cs.Config.strings["Grad"]))
            {
                cs.Config.AngleMode = Config.Mode.Grad;
                return new EmptyType();
            }
            else
            {
                cs.stack.Push(literalArg);
                string errorMsg = String.Format(
                    cs.Config.strings["AngleModeError"], literalArg.Display);
                return new ErrorType(errorMsg);
            }        
        }
    }
}