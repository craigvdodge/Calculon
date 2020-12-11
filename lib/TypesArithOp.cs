using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are Arithmetic operators
namespace Calculon.Types
{
    public abstract class ArithBase : IFunctionCog
    {
        public abstract string FunctionName { get; }
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

            //TODO: some of these clauses can be grouped together

            if (argTypes.SequenceEqual(new Type[] { typeof(Integer), typeof(Integer) }))
            {
                return Op((Integer)lhs, (Integer)rhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Integer), typeof(Real) }))
            {
                return Op(new Real(lhs), (Real)rhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Integer), typeof(Rational) }))
            {
                return Op(new Rational((Integer)lhs), (Rational)rhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Real), typeof(Integer) }))
            {
                return Op((Real)lhs, new Real(rhs));
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Real), typeof(Real) }))
            {
                return Op((Real)lhs, (Real)rhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Real), typeof(Rational) }))
            {
                return Op((Real)lhs, new Real(rhs));
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Rational), typeof(Integer) }))
            {
                return Op((Rational)lhs, new Rational((Integer)rhs));
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Rational), typeof(Real) }))
            {
                return Op(new Real(lhs), (Real)rhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Rational), typeof(Rational) }))
            {
                return Op((Rational)lhs, (Rational)rhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(RealConstant), typeof(Integer)}))
            {
                return Op(new Real(lhs), new Real(rhs));
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(RealConstant), typeof(Real)}))
            {
                return Op(new Real(lhs), (Real)rhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(RealConstant), typeof(Rational)}))
            {
                return Op(new Real(lhs), new Real(rhs));
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(RealConstant), typeof(RealConstant)}))
            {
                return Op(new Real(lhs), new Real(rhs));
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Integer), typeof(RealConstant)}))
            {
                return Op(new Real(lhs), new Real(rhs));
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Real), typeof(RealConstant)}))
            {
                return Op((Real) lhs, new Real(rhs));
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Rational), typeof(RealConstant)}))
            {
                return Op(new Real(lhs), new Real(rhs));
            }
            throw new ArgumentException("Unhandled argument types " + argTypes.ToString());
        }

        internal Integer.Base BaseRules(Integer lhs, Integer rhs)
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
        public override string FunctionName { get { return "add"; } }
        internal override Integer Op(Integer lhs, Integer rhs)
        {
            Integer.Base b = BaseRules(lhs, rhs);
            return new Integer(lhs.data + rhs.data, b);
        }

        internal override Real Op(Real lhs, Real rhs)
        {
            return new Real(lhs.data + rhs.data);
        }

        internal override Rational Op(Rational lhs, Rational rhs)
        {
            Int64 newDenom = LeastCommonMultiple.LCM(lhs.denominator, rhs.denominator);
            Int64 newLhsNum = lhs.numerator * (newDenom / lhs.denominator);
            Int64 newRhsNum = rhs.numerator * (newDenom / rhs.denominator);

            return new Rational((newLhsNum + newRhsNum), newDenom);
        }
    }

    public class Sub : ArithBase
    {
        public override string FunctionName { get { return "sub"; } }

        internal override Integer Op(Integer lhs, Integer rhs)
        {
            Integer.Base b = BaseRules(lhs, rhs);
            return new Integer(lhs.data - rhs.data, b);
        }

        internal override Real Op(Real lhs, Real rhs)
        {
            return new Real(lhs.data - rhs.data);
        }

        internal override Rational Op(Rational lhs, Rational rhs)
        {
            Int64 newDenom = LeastCommonMultiple.LCM(lhs.denominator, rhs.denominator);
            Int64 newLhsNum = lhs.numerator * (newDenom / lhs.denominator);
            Int64 newRhsNum = rhs.numerator * (newDenom / rhs.denominator);

            return new Rational((newLhsNum - newRhsNum), newDenom);
        }
    }

    public class Mult : ArithBase
    {
        public override string FunctionName { get { return "mult"; } }

        internal override Integer Op(Integer lhs, Integer rhs)
        {
            Integer.Base b = BaseRules(lhs, rhs);
            return new Integer(lhs.data * rhs.data, b);
        }

        internal override Real Op(Real lhs, Real rhs)
        {
            return new Real(lhs.data * rhs.data);
        }

        internal override Rational Op(Rational lhs, Rational rhs)
        {
            return new Rational((lhs.numerator * rhs.numerator), (lhs.denominator * rhs.denominator));
        }
    }

    public class Div : ArithBase
    {
        public override string FunctionName { get { return "div"; } }

        internal override Integer Op(Integer lhs, Integer rhs)
        {
            Integer.Base b = BaseRules(lhs, rhs);
            return new Integer(lhs.data / rhs.data, b);
        }

        internal override Real Op(Real lhs, Real rhs)
        {
            return new Real(lhs.data / rhs.data);
        }

        internal override Rational Op(Rational lhs, Rational rhs)
        {
            return new Rational((lhs.numerator * rhs.denominator), (lhs.denominator * rhs.numerator));
        }
    }

    public class Mod : ArithBase
    {
        public override string FunctionName { get { return "mod"; } }

        internal override Integer Op(Integer lhs, Integer rhs)
        {
            Integer.Base b = BaseRules(lhs, rhs);
            return new Integer(lhs.data % rhs.data, b);
        }

        internal override Real Op(Real lhs, Real rhs)
        {
            return new Real(lhs.data % rhs.data);
        }

        // a mod b = a - b*(floor(a/b))
        internal override Rational Op(Rational lhs, Rational rhs)
        {
            Div div = new Div();
            Rational AonB = div.Op(lhs, rhs);
            double intermediateDiv = (double)AonB;
            Integer floorOfDiv = new Integer((Int64)Math.Floor(intermediateDiv));
            Mult mult = new Mult();
            Rational newRhs = mult.Op(rhs, new Rational(floorOfDiv));
            Sub sub = new Sub();

            return sub.Op(lhs, newRhs);
        }
    }
}