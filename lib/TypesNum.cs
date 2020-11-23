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
            data = Int64.Parse(s, CultureInfo.InvariantCulture);
        }

        public Integer(Int64 i) => (data) = (i);

        public EvalReturn Eval(ref ControllerState cs)
        {
            cs.stack.Push(this);
            return new EvalReturn(Response.Ok, data.ToString(), this.GetType());
        }

        internal Int64 data;
        public string Display
        {
            get {return data.ToString();}
        }
    }

}