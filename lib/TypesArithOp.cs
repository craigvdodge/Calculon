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
        public abstract Int64 DoOp(Int64 lhs, Int64 rhs);
        public abstract double DoOp(double lhs, double rhs);
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
                Integer retval = new Integer(DoOp(((Integer) lhs).data, ((Integer) rhs).data) );
                // Only checking if lhs is not base 10 so when the user does 
                // something like add 1 w/o explicit base, does what they expect
                if (((Integer) lhs).DisplayBase != Integer.Base.Dec)
                {
                    retval.DisplayBase = ((Integer) lhs).DisplayBase;
                }
                cs.stack.Push(retval);
                return new EvalReturn(Response.Ok, retval.Display, this.GetType());
            }
            
            if (lhs.GetType() == typeof(Real) || rhs.GetType() == typeof(Real))
            {
                // We're making copies so the Real ctor will handle any needed conversion
                Real newLhs = new Real(lhs);
                Real newRhs = new Real(rhs);
                Real retVal = new Real(DoOp(newLhs.data, newRhs.data));
                cs.stack.Push(retVal);
                return new EvalReturn(Response.Ok, retVal.Display, retVal.GetType());
            }

            return new EvalReturn(Response.Error, "ERROR: Calculon bug. You shouldn't see this!", this.GetType());
        }

        // Return empty string on pass, error msg on fail
        private string TypeCheck(ICalculonType lhs, ICalculonType rhs)
        {
            System.Type lhsType = lhs.GetType();
            if (lhsType != typeof(Integer)
                && lhsType != typeof(Real)
                ) // add more checks as more types supported
            {
                return "ERROR: " + lhs.Display + " unsupported type " + lhsType.ToString();
            }

            System.Type rhsType = rhs.GetType();
            if (rhsType != typeof(Integer)
                && rhsType != typeof(Real)
                ) // add more checks as more types supported
            {
                return "ERROR: " + rhs.Display + " unsupported type " + rhsType.ToString();
            }

            // add any checks of the two types together here

            return string.Empty;
        }

    }
    public class AddOp: ArithOpBase, ICalculonType
    {
        public AddOp() {}

        public override Int64 DoOp(Int64 lhs, Int64 rhs)
        {
            return lhs + rhs;
        }

        public override double DoOp(double lhs, double rhs)
        {
            return lhs + rhs;
        }

        public string Display { get{ return "+"; } }

    }

    public class SubOp: ArithOpBase, ICalculonType
    {
        public SubOp(){}

        public override Int64 DoOp(Int64 lhs, Int64 rhs)
        {
            return lhs - rhs;
        }

        public override double DoOp(double lhs, double rhs)
        {
            return lhs - rhs;
        }

        public string Display { get{ return "-"; } }
    }

    public class MultOp: ArithOpBase, ICalculonType
    {
        public MultOp(){}

        public override Int64 DoOp(Int64 lhs, Int64 rhs)
        {
            return lhs * rhs;
        }

        public override double DoOp(double lhs, double rhs)
        {
            return lhs * rhs;
        }

        public string Display { get{ return "*"; } }
    }

    public class DivOp: ArithOpBase, ICalculonType
    {
        public DivOp(){}

        public override Int64 DoOp(Int64 lhs, Int64 rhs)
        {
            return lhs / rhs;
        }

        public override double DoOp(double lhs, double rhs)
        {
            return lhs / rhs;
        }

        public string Display { get{ return "/"; } }
    }

    public class ModOp: ArithOpBase, ICalculonType
    {
        public ModOp(){}

        public override Int64 DoOp(Int64 lhs, Int64 rhs)
        {
            return lhs % rhs;
        }

        public override double DoOp(double lhs, double rhs)
        {
            return lhs % rhs;
        }

        public string Display { get{ return "%"; } }
    }
}