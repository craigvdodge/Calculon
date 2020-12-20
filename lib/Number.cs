using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;


namespace Calculon.Types
{
    public class Number
    {
        #region ctors
        public Number(BigInteger Num, BigInteger Denom)
        {
            if (Denom == 0)
            {
                throw new DivideByZeroException();
            }
            Numerator = Num;
            Denominator = Denom;
            Reduce();
            View = ViewType.Rational;
            DisplayBase = Base.Dec;
        }   
        public Number(BigInteger WholeNum)
        {
            Numerator = WholeNum;
            Denominator = BigInteger.One;
            View = ViewType.Integer;
            DisplayBase = Base.Dec;
        }

        #endregion
        public static Number Parse(string s)
        {
            Regex rational = new Regex(@"^[\+-]?\d+\/\d+$");
            if (rational.IsMatch(s))
            {
                bool isNegative = false;
                if (Regex.IsMatch(s, @"^[\-]"))
                {
                    isNegative = true;
                }
                // strip leading sign from processing
                s = Regex.Replace(s, @"^[\+-]+", string.Empty);
                string[] parts = s.Split('/');
                BigInteger num = BigInteger.Parse(parts[0]);
                if (isNegative)
                {
                    num = BigInteger.Negate(num);
                }
                BigInteger denom = BigInteger.Parse(parts[1]);

                return new Number(num, denom);
            } //TODO: Integer, Reals

            throw new NotImplementedException();
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            switch (this.View)
            {
                case ViewType.Rational:
                    output.Append(Numerator.ToString());
                    output.Append('/');
                    output.Append(Denominator.ToString());
                    return output.ToString();
                //TODO: Integer and Real
                default: throw new NotImplementedException();
            }    
        }

        #region Helpers
        public void Reduce()
        {
            if (Numerator == BigInteger.Zero)
            { 
                Denominator = BigInteger.One;
            }

            BigInteger gcd = BigInteger.GreatestCommonDivisor(Numerator, Denominator);
            if (gcd > 1)
            {
                Numerator = Numerator / gcd;
                Denominator = Denominator / gcd;
            }
        }

        #endregion

        #region Data
        public enum ViewType { Integer, Rational, Real}
        public ViewType View { get; set; }
        public enum Base { Dec = 10, Hex = 16, Oct = 8, Bin = 2 };
        public Base DisplayBase { get; set; }
        public BigInteger Numerator { get; set; }
        public BigInteger Denominator { get; set; }
        #endregion
    }
}
