using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are Arithmetic operators
namespace Calculon.Types
{ 
    public interface IArithOpBase
    {
        Integer DoOp(Integer lhs, Integer rhs);
        Real DoOp(Real lhs, Real rhs);
        Rational DoOp(Rational lhs, Rational rhs);
    }
    // Moved all Arithmetic ops to a factory-type model
    // This lets us simplify the peg files
    public class ArithOp: ICalculonType, IArithOpBase
    {
        public ArithOp(Op Operation)
        {
            switch(Operation)
            {
                case Op.Add : op = (IArithOpBase) new AddOp(); break;
                case Op.Sub : op = (IArithOpBase) new SubOp(); break;
                case Op.Mult: op = (IArithOpBase) new MultOp(); break;
                case Op.Div: op = (IArithOpBase) new DivOp(); break;
                case Op.Mod: op = (IArithOpBase) new ModOp(); break;
            }
        }
        public enum Op {Add, Sub, Mult, Div, Mod}

        #region IArithOpBase
        public Integer DoOp(Integer lhs, Integer rhs) {return op.DoOp(lhs, rhs); }
        public Real DoOp(Real lhs, Real rhs) {return op.DoOp(lhs, rhs); }
        public Rational DoOp(Rational lhs, Rational rhs) {return op.DoOp(lhs, rhs); }
        #endregion
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
                Integer retval = op.DoOp((Integer) lhs, (Integer) rhs );
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
                Rational retVal = op.DoOp(newLhs, newRhs);
                cs.stack.Push(retVal);
                return new EvalReturn(Response.Ok, retVal);
            }
            
            if (lhs.GetType() == typeof(Real) || rhs.GetType() == typeof(Real)
                || lhs.GetType() == typeof(RealConstant) || rhs.GetType() == typeof(RealConstant))
            {
                // We're making copies so the Real ctor will handle any needed conversion
                Real newLhs = new Real(lhs);
                Real newRhs = new Real(rhs);
                Real retVal = op.DoOp(newLhs, newRhs);
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
                && lhsType != typeof(RealConstant)
                && lhsType != typeof(Rational)
                ) // add more checks as more types supported
            {
                return "ERROR: " + lhs.Display + " unsupported type " + lhsType.ToString();
            }

            System.Type rhsType = rhs.GetType();
            if (rhsType != typeof(Integer)
                && rhsType != typeof(Real)
                && rhsType != typeof(RealConstant)
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

        public string Display 
        { 
            get
            { 
                Type OpType = op.GetType();
                // not a switch b/c C# no likey case typeof()
                if (OpType == typeof(AddOp)) { return "+"; }
                else if (OpType == typeof(SubOp)) { return "-"; }
                else if (OpType == typeof(MultOp)) { return "*"; }
                else if (OpType == typeof(DivOp)) { return "/"; }
                else if (OpType == typeof(ModOp)) { return "mod"; }
                else { throw new Exception("Unknown Operator"); }
            }
        }
        private IArithOpBase op;
    }
    
    internal class AddOp: IArithOpBase
    {
        public Integer DoOp(Integer lhs, Integer rhs)
        {
            return new Integer(lhs.data + rhs.data);
        }

        public Real DoOp(Real lhs, Real rhs)
        {
            return new Real(lhs.data + rhs.data);
        }

        public Rational DoOp(Rational lhs, Rational rhs)
        {
            Int64 newDenom = IntegerOp.LeastCommonMultiple(lhs.denominator, rhs.denominator);
            Int64 newLhsNum = lhs.numerator * (newDenom / lhs.denominator);
            Int64 newRhsNum = rhs.numerator * (newDenom / rhs.denominator);

            return new Rational((newLhsNum + newRhsNum), newDenom);
        }
    }

    internal class SubOp: IArithOpBase
    {
        public Integer DoOp(Integer lhs, Integer rhs)
        {
            return new Integer(lhs.data - rhs.data);
        }

        public Real DoOp(Real lhs, Real rhs)
        {
            return new Real(lhs.data - rhs.data);
        }

        public Rational DoOp(Rational lhs, Rational rhs)
        {
            Int64 newDenom = IntegerOp.LeastCommonMultiple(lhs.denominator, rhs.denominator);
            Int64 newLhsNum = lhs.numerator * (newDenom / lhs.denominator);
            Int64 newRhsNum = rhs.numerator * (newDenom / rhs.denominator);

            return new Rational((newLhsNum - newRhsNum), newDenom);
        }
    }

    internal class MultOp: IArithOpBase
    {
        public Integer DoOp(Integer lhs, Integer rhs)
        {
            return new Integer(lhs.data * rhs.data);
        }

        public Real DoOp(Real lhs, Real rhs)
        {
            return new Real(lhs.data * rhs.data);
        }

        public Rational DoOp(Rational lhs, Rational rhs)
        {
            return new Rational((lhs.numerator * rhs.numerator), (lhs.denominator * rhs.denominator));
        }
    }

    internal class DivOp: IArithOpBase
    {
        public Integer DoOp(Integer lhs, Integer rhs)
        {
            return new Integer(lhs.data / rhs.data);
        }

        public Real DoOp(Real lhs, Real rhs)
        {
            return new Real(lhs.data / rhs.data);
        }

        public Rational DoOp(Rational lhs, Rational rhs)
        {
           return new Rational((lhs.numerator * rhs.denominator), (lhs.denominator * rhs.numerator));
        }
    }

    internal class ModOp: IArithOpBase
    {
        public Integer DoOp(Integer lhs, Integer rhs)
        {
            return new Integer(lhs.data % rhs.data);
        }

        public Real DoOp(Real lhs, Real rhs)
        {
            return new Real(lhs.data % rhs.data);
        }

        // a mod b = a - b*(floor(a/b))
        public Rational DoOp(Rational lhs, Rational rhs)
        {
            ArithOp div = new ArithOp(ArithOp.Op.Div);
            Rational AonB = div.DoOp(lhs, rhs);
            double intermediateDiv = (double) AonB;
            Integer floorOfDiv = new Integer((Int64) Math.Floor(intermediateDiv));
            ArithOp mult = new ArithOp(ArithOp.Op.Mult);
            Rational newRhs = mult.DoOp(rhs, new Rational(floorOfDiv));
            ArithOp sub = new ArithOp(ArithOp.Op.Sub);
            return sub.DoOp(lhs, newRhs);
        }
    }
}