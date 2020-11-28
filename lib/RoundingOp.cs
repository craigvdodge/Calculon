using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are Rounding operatorss
namespace Calculon.Types
{
    public abstract class OneArgRoundingOp
    {
        public abstract double IntermediateOp(double data);

        public EvalReturn Eval(ref ControllerState cs)
        {
            if (cs.stack.Count < 1)
            {
                return new EvalReturn(Response.Error, "ARG ERROR: Requires Real", this.GetType());
            }
            if (cs.stack.Peek().GetType() != typeof(Real))
            {
                return new EvalReturn(Response.Error, "TYPE ERROR: Argument is not Real", this.GetType());
            }

            Real input = (Real) cs.stack.Pop();
            Int64 newData = Convert.ToInt64(IntermediateOp(input.data));
            Integer retVal = new Integer(newData);
            cs.stack.Push(retVal);
            
            return new EvalReturn(Response.Ok, retVal);
        }

    }
    public class RoundOp: OneArgRoundingOp, ICalculonType
    {
        public override double IntermediateOp(double data)
        {
            return Math.Round(data);
        }

        public string Display { get{ return "Round"; } }
    }

    public class RoundToOp: ICalculonType
    {
        public EvalReturn Eval(ref ControllerState cs)
        {
            if (cs.stack.Count < 2)
            {
                return new EvalReturn(Response.Error, "ARG ERROR: Requires Real and int decimal places", this.GetType());
            }
            if (cs.stack.Peek().GetType() != typeof(Integer))
            {
                return new EvalReturn(Response.Error, "TYPE ERROR: Places Argument not Integerl", this.GetType());
            }
            Integer places = (Integer) cs.stack.Pop();
            if (cs.stack.Peek().GetType() != typeof(Real))
            {
                //Push 1st arg back onto stack
                cs.stack.Push(places);
                return new EvalReturn(Response.Error, "TYPE ERROR: Attempting to RoundTo on non-Real", this.GetType());
            }
            Real number = (Real) cs.stack.Pop();
            Int32 digits = (Int32) places.data;
            
            Real retval = new Real(Math.Round(number.data, digits));
            cs.stack.Push(retval);
            
            return new EvalReturn(Response.Ok, retval);
        }

        public string Display { get{ return "Round"; } }
    }

    public class FloorOp: OneArgRoundingOp, ICalculonType
    {
        public override double IntermediateOp(double data)
        {
            return Math.Floor(data);
        }

        public string Display { get{ return "Floor"; } }
    }

    public class CeilingOp: OneArgRoundingOp, ICalculonType
    {
        public override double IntermediateOp(double data)
        {
            return Math.Ceiling(data);
        }

        public string Display { get{ return "Ceiling"; } }
    }
}