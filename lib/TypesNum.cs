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
            else if (toCopy.GetType() == typeof(RealConstant))
            {
                Real temp = ((RealConstant) toCopy).ToReal();
                data = temp.data;
            } else if (toCopy.GetType() == typeof(Rational))
            {
                double numerator = (double) ((Rational)toCopy).numerator;
                double denominator = (double)((Rational)toCopy).denominator;
                data = numerator / denominator;
            }
            else
            {
                throw new ArgumentException("Unhandled type " + toCopy.GetType().ToString());
            }
        }

        public EvalReturn Eval(ref ControllerState cs)
        {
            cs.stack.Push(this);
            return new EvalReturn(Response.Ok, this);
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
            return new EvalReturn(Response.Ok, this);
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
            get 
            { 
                string val = Convert.ToString(data, (int) displayBase).ToUpper();
                switch (this.displayBase)
                {
                    case Base.Hex : val += "h"; break;
                    case Base.Bin : val += "b"; break;
                    case Base.Oct : val += "o"; break;
                }
                return val; 
            }
        }
    }

    public class Rational : ICalculonType
    {
        public Rational(string s)
        {
            string[] parts = s.Split('/');
            numerator = Int64.Parse(parts[0]);
            denominator = Int64.Parse(parts[1]);
            this.Reduce();
        }

        public Rational(Int64 num, Int64 denom)
        {
            numerator = num;
            denominator = denom;
            this.Reduce();
        }

        public Rational(Integer i)
        {
            numerator = i.data;
            denominator = 1;
        }

        internal Int64 numerator;
        internal Int64 denominator;

        public static explicit operator double(Rational r) => (double) (r.numerator / r.denominator);

        public void Reduce()
        {
            Int64 gcf = GreatestCommonFactor.GCF(numerator, denominator);
            if (gcf > 1)
            {
                numerator = numerator / gcf;
                denominator = denominator / gcf;
            }
        }

        public string Display
        {
            get { return numerator.ToString() + "/" + denominator.ToString();}
        }

        public EvalReturn Eval(ref ControllerState cs)
        {
            cs.stack.Push(this);
            return new EvalReturn(Response.Ok, this);
        }
    }

}