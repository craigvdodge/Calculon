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

        public Number(string s) : this(Number.Parse(s)) { }

        public Number(Number number)
        {
            this.Numerator = number.Numerator;
            this.Denominator = number.Denominator;
            this.DisplayBase = number.DisplayBase;
            this.View = number.View;
        }

        #endregion
        public static Number Parse(string s)
        {
            if (Number.RationalMatch.IsMatch(s))
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
                if (denom == BigInteger.Zero) { throw new DivideByZeroException(); }

                return new Number(num, denom);
            } 
            if (Number.HexIntegerMatch.IsMatch(s))
            {
                //strip trailing h
                s = Regex.Replace(s, @"[hH]$", string.Empty);
                //strip any begining + (redundant but allowed)
                s = Regex.Replace(s, @"^\+", string.Empty);
                BigInteger num = Number.ParseBigInteger(s, 16);
                Number output = new Number(num);
                output.DisplayBase = Base.Hex;
                return output;
            }
            if (Number.DecIntegerMatch.IsMatch(s))
            {
                //strip any trailing d
                s = Regex.Replace(s, @"[dD]$", string.Empty);
                //strip any begining +
                s = Regex.Replace(s, @"^\+", string.Empty);
                BigInteger num = BigInteger.Parse(s);
                return new Number(num);
            }
            if (Number.OctIntegerMatch.IsMatch(s))
            {
                //strip trailing o
                s = Regex.Replace(s, @"[oO]$", string.Empty);
                //strip any begining + (redundant but allowed)
                s = Regex.Replace(s, @"^\+", string.Empty);
                BigInteger num = Number.ParseBigInteger(s, 8);
                Number output = new Number(num);
                output.DisplayBase = Base.Oct;
                return output;
            }
            if (Number.BinIntegerMatch.IsMatch(s))
            {
                //strip trailing b
                s = Regex.Replace(s, @"[bB]$", string.Empty);
                //strip any begining + (redundant but allowed)
                s = Regex.Replace(s, @"^\+", string.Empty);
                BigInteger num = Number.ParseBigInteger(s, 2);
                Number output = new Number(num);
                output.DisplayBase = Base.Bin;
                return output;
            }
            //TODO: Reals

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

        // see https://stackoverflow.com/questions/14040483/biginteger-parse-octal-string/14040916
        // and https://stackoverflow.com/questions/14048476/biginteger-to-hex-decimal-octal-binary-strings
        private static BigInteger ParseBigInteger(string value, int baseOfValue)
        {
            const string digits = "0123456789ABCDEF";
            Dictionary<char, int> values = digits.ToDictionary(c => c, c => digits.IndexOf(c));
            //Calculon is case-insensitive, but this method isn't. Make case match digits.
            string casefix = value.ToUpper();
            bool negative = false;
            if (casefix[0] == '-')
            {
                negative = true;
                casefix = casefix.Substring(1);
            }
            BigInteger retVal = casefix.Aggregate(
                new BigInteger(),
                (current, digit) => current * baseOfValue + values[digit]);

            if (negative)
            {
                retVal = BigInteger.Negate(retVal);
            }

            return retVal;
        }

        #endregion

        #region parsing regexes
        private static readonly Regex RationalMatch = 
            new Regex(@"^[\+-]?\d+\/\d+$", RegexOptions.Compiled);
        private static readonly Regex HexIntegerMatch =
            new Regex(@"^[\+-]?[0-9A-Fa-f]+[hH]$", RegexOptions.Compiled);
        private static readonly Regex DecIntegerMatch =
            new Regex(@"^[\+-]?\d+[dD]?$", RegexOptions.Compiled);
        private static readonly Regex OctIntegerMatch =
            new Regex(@"^[\+-]?[0-7]+[oO]$", RegexOptions.Compiled);
        private static readonly Regex BinIntegerMatch =
            new Regex(@"^[\+-]?[0-1]+[bB]$", RegexOptions.Compiled);

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
