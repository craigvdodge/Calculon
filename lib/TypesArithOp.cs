using System;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Globalization;
using System.Collections.Generic;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are Arithmetic operators
namespace Calculon.Types
{
    public static class ArithOpExtensions
    {
        internal static Number.Base BaseRules(Number.Base lhs, Number.Base rhs)
        {
            Number.Base output = Number.Base.Dec;
            if (lhs != Number.Base.Dec)
            {
                output = lhs;
            }
            if (rhs != Number.Base.Dec)
            {
                output = rhs;
            }
            return output;
        }
        #region DEPRICATION STATION
        // These are around just long enough to get this to compile
        // Once references go to zero, delete.
        public static Integer Subtract(this Integer lhs, Integer rhs)
        {
            Integer.Base b = ArithBase.BaseRules(lhs, rhs);
            return new Integer(lhs.data - rhs.data, b);
        }
        public static Integer Multiply(this Integer lhs, Integer rhs)
        {
            Integer.Base b = ArithBase.BaseRules(lhs, rhs);
            return new Integer(lhs.data * rhs.data, b);
        }
        public static Integer Divide(this Integer lhs, Integer rhs)
        {
            Integer.Base b = ArithBase.BaseRules(lhs, rhs);
            return new Integer(lhs.data / rhs.data, b);
        }
        #endregion

        private static Number.ViewType ViewDecider(Number.ViewType lhs, Number.ViewType rhs)
        {
            Number.ViewType output = Number.ViewType.Rational;

            if (lhs == Number.ViewType.Integer && rhs == Number.ViewType.Integer)
            {
                output = Number.ViewType.Integer;
            }
            if (lhs == Number.ViewType.Rational || rhs == Number.ViewType.Rational)
            {
                output = Number.ViewType.Rational;
            }
            if (lhs == Number.ViewType.Real || rhs == Number.ViewType.Real)
            {
                output = Number.ViewType.Real;
            }
            return output;
        }
        public static Number Add(this Number lhs, Number rhs)
        {
            BigInteger newDenom = lhs.Denominator.LCM(rhs.Denominator);
            BigInteger newLhsNum = lhs.Numerator * (newDenom / lhs.Denominator);
            BigInteger newRhsNum = rhs.Numerator * (newDenom / rhs.Denominator);

            Number output = new Number(newLhsNum + newRhsNum, newDenom);
            output.View = ViewDecider(lhs.View, rhs.View);
            if (output.View == Number.ViewType.Integer)
            {
                output.DisplayBase = BaseRules(lhs.DisplayBase, rhs.DisplayBase);
            }
            return output;
        }

        public static Number Subtract(this Number lhs, Number rhs)
        {
            BigInteger newDenom = lhs.Denominator.LCM(rhs.Denominator);
            BigInteger newLhsNum = lhs.Numerator * (newDenom / lhs.Denominator);
            BigInteger newRhsNum = rhs.Numerator * (newDenom / rhs.Denominator);

            Number output = new Number(newLhsNum - newRhsNum, newDenom);
            output.View = ViewDecider(lhs.View, rhs.View);
            if (output.View == Number.ViewType.Integer)
            {
                output.DisplayBase = BaseRules(lhs.DisplayBase, rhs.DisplayBase);
            }
            return output;
        }

        public static Number Multiply(this Number lhs, Number rhs)
        {
            Number output = 
                new Number(lhs.Numerator * rhs.Numerator, lhs.Denominator * rhs.Denominator);
            output.View = ViewDecider(lhs.View, rhs.View);
            if (output.View == Number.ViewType.Integer)
            {
                output.DisplayBase = BaseRules(lhs.DisplayBase, rhs.DisplayBase);
            }
            return output;
        }

        public static Number Divide(this Number lhs, Number rhs)
        {
            Number output = 
                new Number(lhs.Numerator * rhs.Denominator, lhs.Denominator * rhs.Numerator);
            output.View = ViewDecider(lhs.View, rhs.View);
            if (output.View == Number.ViewType.Integer)
            {
                output.DisplayBase = BaseRules(lhs.DisplayBase, rhs.DisplayBase);
            }
            return output;
        }
        public static Number Mod(this Number lhs, Number rhs)
        {
            // a mod b = a - b*(floor(a/b))
            Number AonB = lhs.Divide(rhs);
            AonB.View = Number.ViewType.Real;
            //TODO: replace use of double when Number.Floor() is written
            double intermediateDiv = double.Parse(AonB.ToString());
            Number floorOfDiv = new Number((BigInteger)Math.Floor(intermediateDiv));
            Number newRhs = rhs.Multiply(floorOfDiv);
            Number output = lhs.Subtract(newRhs);
            
            output.View = ViewDecider(lhs.View, rhs.View);
            if (output.View == Number.ViewType.Integer)
            {
                output.DisplayBase = BaseRules(lhs.DisplayBase, rhs.DisplayBase);
            }

            return output;
        }
        
    }

    public abstract class ArithBase : IFunctionCog
    {
        public abstract string FunctionName { get; }
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
            ICalculonType rhs = cs.stack.Pop();
            ICalculonType lhs = cs.stack.Pop();
            Type[] argTypes = new Type[] { lhs.GetType(), rhs.GetType() };

            if (argTypes.SequenceEqual(new Type[] { typeof(Number), typeof(Number) }))
            {
                return Op((Number)lhs, (Number)rhs);
            }
            throw new ArgumentException("Unhandled argument types " + argTypes.ToString());
        }

        internal abstract Number Op(Number lhs, Number rhs);

        //DEPRECATED
        internal static Integer.Base BaseRules(Integer lhs, Integer rhs)
        {
            Integer.Base b = Integer.Base.Dec;
            if (lhs.DisplayBase != Integer.Base.Dec)
            {
                b = lhs.DisplayBase;
            }
            if (rhs.DisplayBase != Integer.Base.Dec)
            {
                b = rhs.DisplayBase;
            }
            return b;
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }
    }

    public class Add : ArithBase
    {
        public override string FunctionName { get { return "add"; } }
        internal override Number Op(Number lhs, Number rhs)
        {
            return lhs.Add(rhs);
        }
    }

    public class Sub : ArithBase
    {
        public override string FunctionName { get { return "sub"; } }
        internal override Number Op(Number lhs, Number rhs)
        {
            return lhs.Subtract(rhs);
        }
    }

    public class Mult : ArithBase
    {
        public override string FunctionName { get { return "mult"; } }
        internal override Number Op(Number lhs, Number rhs)
        {
            return lhs.Multiply(rhs);
        }
    }

    public class Div : ArithBase
    {
        public override string FunctionName { get { return "div"; } }
        internal override Number Op(Number lhs, Number rhs)
        {
            return lhs.Divide(rhs);
        }
    }

    public class Mod : ArithBase
    {
        public override string FunctionName { get { return "mod"; } }

        internal override Number Op(Number lhs, Number rhs)
        {
            return lhs.Mod(rhs);
        }
    }
}