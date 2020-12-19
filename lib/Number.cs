using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        }   
        public Number(BigInteger WholeNum)
        {
            Numerator = WholeNum;
            Denominator = BigInteger.One;
            View = ViewType.Integer;
        }

        #endregion
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
        public BigInteger Numerator { get; set; }
        public BigInteger Denominator { get; set; }
        #endregion
    }
}
