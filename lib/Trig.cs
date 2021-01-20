using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;

namespace Calculon.Types
{
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