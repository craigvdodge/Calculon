﻿using System;
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
            Precision = BigInteger.MinusOne;
        }   
        public Number(BigInteger WholeNum)
        {
            Numerator = WholeNum;
            Denominator = BigInteger.One;
            View = ViewType.Integer;
            DisplayBase = Base.Dec;
            Precision = BigInteger.MinusOne;
        }

        public Number(string s) : this(Number.Parse(s)) { }

        public Number(Number number)
        {
            this.Numerator = number.Numerator;
            this.Denominator = number.Denominator;
            this.DisplayBase = number.DisplayBase;
            this.View = number.View;
            this.Precision = number.Precision;
        }

        #endregion
        #region To and from strings
        public static Number Parse(string s)
        {
            s = s.Trim();
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
            if (Number.RealMatch.IsMatch(s))
            {
                int decimalPoint = s.IndexOf('.');
                s = s.Replace(".", string.Empty);
                BigInteger num = BigInteger.Parse(s);
                BigInteger denom = BigInteger.Pow(10, s.Length - decimalPoint);
                Number output = new Number(num, denom);
                //num and denom are reduced as part of rational ctor
                output.View = ViewType.Real;
                return output;
            }
            throw new ArgumentException("Unrecognized input: " + s, s);
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
                case ViewType.Integer:
                    switch (this.DisplayBase)
                    {
                        case Base.Dec:
                            output.Append(this.Numerator.ToString());
                            break;
                        case Base.Hex:
                            output.Append(this.Numerator.ToString("X"));
                            output.Append("h");
                            break;
                        case Base.Bin:
                            output.Append(this.ToBinaryString(Numerator));
                            output.Append("b");
                            break;
                        case Base.Oct:
                            output.Append(this.ToOctalString(Numerator));
                            output.Append("o");
                            break;
                    }
                    //TODO: Real
                    return output.ToString();  
                default: throw new NotImplementedException();
            }    
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

        public static bool IsNumber(string s)
        {
            return RationalMatch.IsMatch(s) || HexIntegerMatch.IsMatch(s)
                || DecIntegerMatch.IsMatch(s) || OctIntegerMatch.IsMatch(s)
                || BinIntegerMatch.IsMatch(s) || RealMatch.IsMatch(s);
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

        private string ToBinaryString(BigInteger bigint)
        {
            byte[] bytes = bigint.ToByteArray();
            int idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            StringBuilder base2 = new StringBuilder(bytes.Length * 8);

            // Convert first byte to binary.
            string binary = Convert.ToString(bytes[idx], 2);

            // Ensure leading zero exists if value is positive.
            if (binary[0] != '0' && bigint.Sign == 1)
            {
                base2.Append('0');
            }

            // Append binary string to StringBuilder.
            base2.Append(binary);

            // Convert remaining bytes adding leading zeros.
            for (idx--; idx >= 0; idx--)
            {
                base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
            }

            return base2.ToString();
        }

        private string ToOctalString(BigInteger bigint)
        {
            byte[] bytes = bigint.ToByteArray();
            int idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            StringBuilder base8 = new StringBuilder(((bytes.Length / 3) + 1) * 8);

            // Calculate how many bytes are extra when byte array is split
            // into three-byte (24-bit) chunks.
            int extra = bytes.Length % 3;

            // If no bytes are extra, use three bytes for first chunk.
            if (extra == 0)
            {
                extra = 3;
            }

            // Convert first chunk (24-bits) to integer value.
            int int24 = 0;
            for (; extra != 0; extra--)
            {
                int24 <<= 8;
                int24 += bytes[idx--];
            }

            // Convert 24-bit integer to octal without adding leading zeros.
            string octal = Convert.ToString(int24, 8);

            // Ensure leading zero exists if value is positive.
            if (octal[0] != '0' && bigint.Sign == 1)
            {
                base8.Append('0');
            }

            // Append first converted chunk to StringBuilder.
            base8.Append(octal);

            // Convert remaining 24-bit chunks, adding leading zeros.
            for (; idx >= 0; idx -= 3)
            {
                int24 = (bytes[idx] << 16) + (bytes[idx - 1] << 8) + bytes[idx - 2];
                base8.Append(Convert.ToString(int24, 8).PadLeft(8, '0'));
            }

            return base8.ToString();
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
        private static readonly Regex RealMatch =
            new Regex(@"^[\+-]?\d*\.\d+$", RegexOptions.Compiled);
        #endregion
        #region Data
        // This will eventually be set some ofther way
        // as a global config of the whole Calculon interpreter.
        public static BigInteger GlobalPrecision = 70;
        // Local precision override. -1 is use global
        public BigInteger Precision { get; set; }
        //TODO: sanity checking on setters
        // e.g. don't set to Integer if denominator is not 1.
        public enum ViewType { Integer, Rational, Real}
        public ViewType View { get; set; }
        public enum Base { Dec = 10, Hex = 16, Oct = 8, Bin = 2 };
        public Base DisplayBase { get; set; }
        // Wait, it's all just fractions?
        // Always has been *click*
        public BigInteger Numerator { get; set; }
        public BigInteger Denominator { get; set; }
        #endregion
    }
}
