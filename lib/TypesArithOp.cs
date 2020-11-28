using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are Arithmetic operatorss
namespace Calculon.Types
{ 
    public abstract class ArithOpBase
    {
        public abstract Integer DoOp(Integer lhs, Integer rhs);
        public abstract Real DoOp(Real lhs, Real rhs);
        public abstract Rational DoOp(Rational lhs, Rational rhs);

        public EvalReturn Eval(ref ControllerState cs)
        {
            if (cs.stack.Count < 2)
            {
                return new EvalReturn(Response.Error, 
                    "ERROR: + Argument Count: Need two elements to operate on.", this.GetType());
            }
            
            ICalculonType rhs = cs.stack.Pop();
            ICalculonType lhs = cs.stack.Pop();

            string typecheck = this.TypeCheck(lhs, rhs);
            if (typecheck != string.Empty)
            {
                // restore the stack and return error
                cs.stack.Push(lhs);
                cs.stack.Push(rhs);
                return new EvalReturn(Response.Error, typecheck, this.GetType());
            }

            if (lhs.GetType() == typeof(Integer) && rhs.GetType() == typeof(Integer))
            {
                Integer retval = DoOp((Integer) lhs, (Integer) rhs );
                // Only checking if lhs is not base 10 so when the user does 
                // something like add 1 w/o explicit base, does what they expect
                if (((Integer) lhs).DisplayBase != Integer.Base.Dec)
                {
                    retval.DisplayBase = ((Integer) lhs).DisplayBase;
                }
                cs.stack.Push(retval);
                return new EvalReturn(Response.Ok, retval);
            }

            if (lhs.GetType() == typeof(Rational) || rhs.GetType() == typeof(Rational))
            {   
                Rational newLhs;
                if (lhs.GetType() == typeof(Integer))
                {
                    newLhs = new Rational((Integer) lhs);
                }
                else
                {
                    newLhs = (Rational) lhs;
                }
                Rational newRhs;
                if (rhs.GetType() == typeof(Integer))
                {
                    newRhs = new Rational((Integer) rhs);
                }
                else
                {
                    newRhs = (Rational) rhs;
                }
                Rational retVal = DoOp(newLhs, newRhs);
                cs.stack.Push(retVal);
                return new EvalReturn(Response.Ok, retVal);
            }
            
            if (lhs.GetType() == typeof(Real) || rhs.GetType() == typeof(Real))
            {
                // We're making copies so the Real ctor will handle any needed conversion
                Real newLhs = new Real(lhs);
                Real newRhs = new Real(rhs);
                Real retVal = DoOp(newLhs, newRhs);
                cs.stack.Push(retVal);
                return new EvalReturn(Response.Ok, retVal);
            }

            return new EvalReturn(Response.Error, "ERROR: Calculon bug. You shouldn't see this!", this.GetType());
        }

        // Return empty string on pass, error msg on fail
        private string TypeCheck(ICalculonType lhs, ICalculonType rhs)
        {
            System.Type lhsType = lhs.GetType();
            if (lhsType != typeof(Integer)
                && lhsType != typeof(Real)
                && lhsType != typeof(Rational)
                ) // add more checks as more types supported
            {
                return "ERROR: " + lhs.Display + " unsupported type " + lhsType.ToString();
            }

            System.Type rhsType = rhs.GetType();
            if (rhsType != typeof(Integer)
                && rhsType != typeof(Real)
                && rhsType != typeof(Rational)
                ) // add more checks as more types supported
            {
                return "ERROR: " + rhs.Display + " unsupported type " + rhsType.ToString();
            }

            // add any checks of the two types together here

            // Rational only supports other Rationals and Integer
            if ( ((lhsType == typeof(Rational)) && 
                    (rhsType != typeof(Rational) && rhsType != typeof(Integer)))
                 || ((rhsType == typeof(Rational)) &&
                    (lhsType != typeof(Rational) && lhsType != typeof(Integer))))
            {
                return "ERROR: Rational numbers only support Rational and Integers";
            }
            
            return string.Empty;
        }

    }
    public class AddOp: ArithOpBase, ICalculonType
    {
        public AddOp() {}

        public override Integer DoOp(Integer lhs, Integer rhs)
        {
            return new Integer(lhs.data + rhs.data);
        }

        public override Real DoOp(Real lhs, Real rhs)
        {
            return new Real(lhs.data + rhs.data);
        }

        public override Rational DoOp(Rational lhs, Rational rhs)
        {
            Int64 newDenom = Rational.LeastCommonMultiple(lhs.denominator, rhs.denominator);
            Int64 newLhsNum = lhs.numerator * (newDenom / lhs.denominator);
            Int64 newRhsNum = rhs.numerator * (newDenom / rhs.denominator);

            return new Rational((newLhsNum + newRhsNum), newDenom);
        }

        public string Display { get{ return "+"; } }

    }

    public class SubOp: ArithOpBase, ICalculonType
    {
        public SubOp(){}

        public override Integer DoOp(Integer lhs, Integer rhs)
        {
            return new Integer(lhs.data - rhs.data);
        }

        public override Real DoOp(Real lhs, Real rhs)
        {
            return new Real(lhs.data - rhs.data);
        }

        public override Rational DoOp(Rational lhs, Rational rhs)
        {
            Int64 newDenom = Rational.LeastCommonMultiple(lhs.denominator, rhs.denominator);
            Int64 newLhsNum = lhs.numerator * (newDenom / lhs.denominator);
            Int64 newRhsNum = rhs.numerator * (newDenom / rhs.denominator);

            return new Rational((newLhsNum - newRhsNum), newDenom);
        }

        public string Display { get{ return "-"; } }
    }

    public class MultOp: ArithOpBase, ICalculonType
    {
        public MultOp(){}

        public override Integer DoOp(Integer lhs, Integer rhs)
        {
            return new Integer(lhs.data * rhs.data);
        }

        public override Real DoOp(Real lhs, Real rhs)
        {
            return new Real(lhs.data * rhs.data);
        }

        public override Rational DoOp(Rational lhs, Rational rhs)
        {
            return new Rational((lhs.numerator * rhs.numerator), (lhs.denominator * rhs.denominator));
        }

        public string Display { get{ return "*"; } }
    }

    public class DivOp: ArithOpBase, ICalculonType
    {
        public DivOp(){}

        public override Integer DoOp(Integer lhs, Integer rhs)
        {
            return new Integer(lhs.data / rhs.data);
        }

        public override Real DoOp(Real lhs, Real rhs)
        {
            return new Real(lhs.data / rhs.data);
        }

        public override Rational DoOp(Rational lhs, Rational rhs)
        {
           return new Rational((lhs.numerator * rhs.denominator), (lhs.denominator * rhs.numerator));
        }

        public string Display { get{ return "/"; } }
    }

    public class ModOp: ArithOpBase, ICalculonType
    {
        public ModOp(){}

        public override Integer DoOp(Integer lhs, Integer rhs)
        {
            return new Integer(lhs.data % rhs.data);
        }

        public override Real DoOp(Real lhs, Real rhs)
        {
            return new Real(lhs.data % rhs.data);
        }

        // a mod b = a - b*(floor(a/b))
        public override Rational DoOp(Rational lhs, Rational rhs)
        {
            DivOp div = new DivOp();
            Rational AonB = div.DoOp(lhs, rhs);
            double intermediateDiv = (double) AonB;
            Integer floorOfDiv = new Integer((Int64) Math.Floor(intermediateDiv));
            MultOp mult = new MultOp();
            Rational newRhs = mult.DoOp(rhs, new Rational(floorOfDiv));
            SubOp sub = new SubOp();
            return sub.DoOp(lhs, newRhs);
        }

        public string Display { get{ return "%"; } }
    }
}