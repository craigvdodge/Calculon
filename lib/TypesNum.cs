using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are number types.
namespace Calculon.Types
{
    // "Decimal" would create type confusion with builtin types
    public class Real: ICalculonType
    {
        public Real(string s)
        {
            data = double.Parse(s, CultureInfo.InvariantCulture);
        }

        public Real(double dbl) => (data) = (dbl);

        public Real(ICalculonType toCopy)
        {
            if (toCopy.GetType() == typeof(Integer))
            {
                data = (double) ((Integer) toCopy).data;
            }
            else if (toCopy.GetType() == typeof(Real))
            {
                data = ((Real) toCopy).data;
            }
            else
            {
                throw new ArgumentException("Unhandled type " + toCopy.GetType().ToString());
            }
        }

        public EvalReturn Eval(ref ControllerState cs)
        {
            cs.stack.Push(this);
            return new EvalReturn(Response.Ok, data.ToString(), this.GetType());
        }

        internal double data;

        // Needs to understand sig. digits   
        // 3.14 1 + returns 4.140000000000001 
        public string Display
        {
            get { return data.ToString();}
        }
    }

    public class Integer: ICalculonType
    {
        public Integer(string s)
        {
            data = Convert.ToInt64(s, (int) Base.Dec);
            displayBase = Base.Dec;
        }
        public Integer(string s, Integer.Base b)
        {
            data = Convert.ToInt64(s, (int) b);
            displayBase = b;
        }

        public Integer(Int64 i) => (data, displayBase) = (i, Base.Dec);

        public EvalReturn Eval(ref ControllerState cs)
        {
            cs.stack.Push(this);
            return new EvalReturn(Response.Ok, this.Display, this.GetType());
        }

        public enum Base {Dec=10, Hex=16, Oct=8, Bin=2};

        private Integer.Base displayBase;

        public Base DisplayBase
        {
            get { return displayBase; }
            set { displayBase = value; }
        }

        internal Int64 data;
        public string Display
        {
            get { return Convert.ToString(data, (int) displayBase); }
        }
    }

    public class BaseConvOp: ICalculonType
    {
        public BaseConvOp(Integer.Base baseOp) => (data) = (baseOp);

        private Integer.Base data;

        public EvalReturn Eval(ref ControllerState cs)
        {
            if (cs.stack.Count < 1)
            {
                return new EvalReturn(Response.Error, "ARG ERROR: Need Integer to convert", this.GetType());
            }
            if (cs.stack.Peek().GetType() != typeof(Integer))
            {
                return new EvalReturn(Response.Error, "TYPE ERROR: Base Op Requires Integer", this.GetType());
            }

            ((Integer) cs.stack.Peek()).DisplayBase = data;
            
            return new EvalReturn(Response.Ok, cs.stack.Peek().Display, cs.stack.Peek().GetType());
        }

        public string Display
        {
            get
            {
                switch (data)
                {
                    case Integer.Base.Dec : return "ToDec";
                    case Integer.Base.Bin : return "ToBin";
                    case Integer.Base.Hex : return "ToHex";
                    case Integer.Base.Oct : return "ToOct";
                    default: return "BASE TYPE ERROR";
                }
            }
        }
    }

}