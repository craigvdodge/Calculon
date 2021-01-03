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
        public static Integer Add(this Integer lhs, Integer rhs)
        {
            Integer.Base b = ArithBase.BaseRules(lhs, rhs);
            return new Integer(lhs.data + rhs.data, b);
        }

        public static Real Add(this Real lhs, Real rhs)
        {
            return new Real(lhs.data + rhs.data);
        }

        public static Rational Add(this Rational lhs, Rational rhs)
        {
            BigInteger newDenom = lhs.denominator.LCM(rhs.denominator);
            BigInteger newLhsNum = lhs.numerator * (newDenom / lhs.denominator);
            BigInteger newRhsNum = rhs.numerator * (newDenom / rhs.denominator);

            return new Rational((newLhsNum + newRhsNum), newDenom);
        }

        public static Integer Subtract(this Integer lhs, Integer rhs)
        {
            Integer.Base b = ArithBase.BaseRules(lhs, rhs);
            return new Integer(lhs.data - rhs.data, b);
        }

        public static Real Subtract(this Real lhs, Real rhs)
        {
            return new Real(lhs.data - rhs.data);
        }

        public static Rational Subtract(this Rational lhs, Rational rhs)
        {
            BigInteger newDenom = lhs.denominator.LCM(rhs.denominator);
            BigInteger newLhsNum = lhs.numerator * (newDenom / lhs.denominator);
            BigInteger newRhsNum = rhs.numerator * (newDenom / rhs.denominator);

            return new Rational((newLhsNum - newRhsNum), newDenom);
        }

        public static Integer Multiply(this Integer lhs, Integer rhs)
        {
            Integer.Base b = ArithBase.BaseRules(lhs, rhs);
            return new Integer(lhs.data * rhs.data, b);
        }

        public static Real Multiply(this Real lhs, Real rhs)
        {
            return new Real(lhs.data * rhs.data);
        }

        public static Rational Mutliply(this Rational lhs, Rational rhs)
        {
            return new Rational((lhs.numerator * rhs.numerator), (lhs.denominator * rhs.denominator));
        }

        public static Integer Divide(this Integer lhs, Integer rhs)
        {
            Integer.Base b = ArithBase.BaseRules(lhs, rhs);
            return new Integer(lhs.data / rhs.data, b);
        }

        public static Real Divide(this Real lhs, Real rhs)
        {
            return new Real(lhs.data / rhs.data);
        }

        public static Rational Divide(this Rational lhs, Rational rhs)
        {
            return new Rational((lhs.numerator * rhs.denominator), (lhs.denominator * rhs.numerator));
        }

        public static Integer Mod(this Integer lhs, Integer rhs)
        {
            Integer.Base b = ArithBase.BaseRules(lhs, rhs);
            return new Integer(lhs.data % rhs.data, b);
        }

        public static Real Mod(this Real lhs, Real rhs)
        {
            return new Real(lhs.data % rhs.data);
        }

        public static Rational Mod(this Rational lhs, Rational rhs)
        {
            // a mod b = a - b*(floor(a/b))
            Div div = new Div();
            Rational AonB = div.Op(lhs, rhs);
            double intermediateDiv = (double)AonB;
            Integer floorOfDiv = new Integer((BigInteger)Math.Floor(intermediateDiv));
            Mult mult = new Mult();
            Rational newRhs = mult.Op(rhs, new Rational(floorOfDiv));
            Sub sub = new Sub();

            return sub.Op(lhs, newRhs);
        }
    }

    public abstract class ArithBase : IFunctionCog
    {
        public abstract string[] FunctionName { get; }
        public int NumArgs { get { return 2; } }
        public Type[][] AllowedTypes
        {
            get
            {
                return FunctionFactory.TwoArgComboGenerator(typeof(Integer), typeof(Real), typeof(Rational), typeof(RealConstant));
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            ICalculonType rhs = cs.stack.Pop();
            ICalculonType lhs = cs.stack.Pop();
            Type[] argTypes = new Type[] { lhs.GetType(), rhs.GetType() };

            if (argTypes.SequenceEqual(new Type[] { typeof(Integer), typeof(Integer) }))
            {
                return Op((Integer)lhs, (Integer)rhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Integer), typeof(Real) })
                || argTypes.SequenceEqual(new Type[] { typeof(Rational), typeof(Real) })
                || argTypes.SequenceEqual(new Type[] { typeof(RealConstant), typeof(Real) }))
            {
                return Op(new Real(lhs), (Real)rhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Integer), typeof(Rational) }))
            {
                return Op(new Rational((Integer)lhs), (Rational)rhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Real), typeof(Integer) })
                || argTypes.SequenceEqual(new Type[] { typeof(Real), typeof(Rational) }))
            {
                return Op((Real)lhs, new Real(rhs));
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Real), typeof(Real) }))
            {
                return Op((Real)lhs, (Real)rhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Rational), typeof(Integer) }))
            {
                return Op((Rational)lhs, new Rational((Integer)rhs));
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Rational), typeof(Rational) }))
            {
                return Op((Rational)lhs, (Rational)rhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(RealConstant), typeof(Integer)})
                || argTypes.SequenceEqual(new Type[] { typeof(RealConstant), typeof(Rational) })
                || argTypes.SequenceEqual(new Type[] { typeof(RealConstant), typeof(RealConstant) })
                || argTypes.SequenceEqual(new Type[] { typeof(Integer), typeof(RealConstant) })
                || argTypes.SequenceEqual(new Type[] { typeof(Rational), typeof(RealConstant) }))
            {
                return Op(new Real(lhs), new Real(rhs));
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Real), typeof(RealConstant)}))
            {
                return Op((Real) lhs, new Real(rhs));
            }
            throw new ArgumentException("Unhandled argument types " + argTypes.ToString());
        }

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
        internal abstract Integer Op(Integer lhs, Integer rhs);
        internal abstract Real Op(Real lhs, Real rhs);
        internal abstract Rational Op(Rational lhs, Rational rhs);
    }

    public class Add : ArithBase
    {
        public override string[] FunctionName { get { return new string[] { "add", "+" }; } }
        internal override Integer Op(Integer lhs, Integer rhs)
        {
            return lhs.Add(rhs);
        }

        internal override Real Op(Real lhs, Real rhs)
        {
            return lhs.Add(rhs);
        }

        internal override Rational Op(Rational lhs, Rational rhs)
        {
            return lhs.Add(rhs);
        }
    }

    public class Sub : ArithBase
    {
        public override string[] FunctionName { get { return new string[] { "sub", "-" }; } }

        internal override Integer Op(Integer lhs, Integer rhs)
        {
            return lhs.Subtract(rhs);
        }

        internal override Real Op(Real lhs, Real rhs)
        {
            return lhs.Subtract(rhs);
        }

        internal override Rational Op(Rational lhs, Rational rhs)
        {
            return lhs.Subtract(rhs);
        }
    }

    public class Mult : ArithBase
    {
        public override string[] FunctionName { get { return new string[] { "mult", "*" }; } }

        internal override Integer Op(Integer lhs, Integer rhs)
        {
            return lhs.Multiply(rhs);
        }

        internal override Real Op(Real lhs, Real rhs)
        {
            return lhs.Multiply(rhs);
        }

        internal override Rational Op(Rational lhs, Rational rhs)
        {
            return lhs.Mutliply(rhs);
        }
    }

    public class Div : ArithBase
    {
        public override string[] FunctionName { get { return new string[] { "div", "/" }; } }

        internal override Integer Op(Integer lhs, Integer rhs)
        {
            return lhs.Divide(rhs);
        }

        internal override Real Op(Real lhs, Real rhs)
        {
            return lhs.Divide(rhs);
        }

        internal override Rational Op(Rational lhs, Rational rhs)
        {
            return lhs.Divide(rhs);
        }
    }

    public class Mod : ArithBase
    {
        public override string[] FunctionName { get { return new string[] { "mod" }; } }

        internal override Integer Op(Integer lhs, Integer rhs)
        {
            return lhs.Mod(rhs);
        }

        internal override Real Op(Real lhs, Real rhs)
        {
            return lhs.Mod(rhs);
        }

        internal override Rational Op(Rational lhs, Rational rhs)
        {
            return lhs.Mod(rhs);
        }
    }
}