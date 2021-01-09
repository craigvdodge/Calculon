using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

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

        #region strings
        public override string ToString()
        {
            return Display;
        }
        // Needs to understand sig. digits   
        // 3.14 1 + returns 4.140000000000001 
        public string Display
        {
            get { return data.ToString();}
        }
        #endregion

        #region parsing
        public static bool IsMatch(string s)
        {
            return RealMatch.IsMatch(s);
        }
        private static readonly Regex RealMatch =
            new Regex(@"^[\+-]?\d*\.\d+$", RegexOptions.Compiled);
        #endregion
    }

    public class Integer: ICalculonType
    {
        public Integer(string s) : this(s, Base.Dec) { }
        public Integer(string s, Integer.Base b)
        {
            if (HexIntegerMatch.IsMatch(s))
            {
                // chomp the trailing "H"
                s = s.TrimEnd(new char[] { 'h', 'H' });
                data = ParseBigInteger(s, (int)Base.Hex);
                displayBase = Base.Hex;
            }
            else if (s.EndsWith('d') || s.EndsWith('D'))
            {
                // chomp the trailing "d"
                s = s.TrimEnd(new char[] { 'd', 'D' });
                data = ParseBigInteger(s, (int)Base.Dec);
                displayBase = Base.Dec;
            }
            else if (OctIntegerMatch.IsMatch(s))
            {
                // chomp the trailing "o"
                s = s.TrimEnd(new char[] { 'o', 'O' });
                data = ParseBigInteger(s, (int)Base.Oct);
                displayBase = Base.Oct;
            }
            else if (BinIntegerMatch.IsMatch(s))
            {
                // chomp the trailing "b"
                s = s.TrimEnd(new char[] { 'b', 'B' });
                data = ParseBigInteger(s, (int)Base.Bin);
                displayBase = Base.Bin;
            }
            else
            {
                data = ParseBigInteger(s, (int)b);
                displayBase = b;
            }
        }

        public Integer(BigInteger i, Base b = Base.Dec) => (data, displayBase) = (i, b);

        public EvalReturn Eval(ref ControllerState cs)
        {
            cs.stack.Push(this);
            return new EvalReturn(Response.Ok, this);
        }

        public enum Base {Dec=10, Hex=16, Oct=8, Bin=2};

        private Integer.Base displayBase;

        #region bigInteger base parsing and formatting
        // see https://stackoverflow.com/questions/14040483/biginteger-parse-octal-string/14040916
        // and https://stackoverflow.com/questions/14048476/biginteger-to-hex-decimal-octal-binary-strings
        private const string digits = "0123456789ABCDEF";
        private readonly Dictionary<char, int> values
           = digits.ToDictionary(c => c, c => digits.IndexOf(c));
        private BigInteger ParseBigInteger(string value, int baseOfValue)
        {
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
        public Base DisplayBase
        {
            get { return displayBase; }
            set { displayBase = value; }
        }

        internal BigInteger data;

        #region Parsing 
        public static bool IsMatch(string s)
        {
            return HexIntegerMatch.IsMatch(s) || DecIntegerMatch.IsMatch(s)
                || OctIntegerMatch.IsMatch(s) || BinIntegerMatch.IsMatch(s);
        }

        private static readonly Regex HexIntegerMatch =
            new Regex(@"^[\+-]?[0-9A-Fa-f]+[hH]$", RegexOptions.Compiled);
        private static readonly Regex DecIntegerMatch =
            new Regex(@"^[\+-]?\d+[dD]?$", RegexOptions.Compiled);
        private static readonly Regex OctIntegerMatch =
            new Regex(@"^[\+-]?[0-7]+[oO]$", RegexOptions.Compiled);
        private static readonly Regex BinIntegerMatch =
            new Regex(@"^[\+-]?[0-1]+[bB]$", RegexOptions.Compiled);
        #endregion

        #region strings
        public override string ToString()
        {
            return Display;
        }
        public string Display
        {
            get
            {
                string val = string.Empty;
                switch (this.displayBase)
                {
                    case Base.Dec : 
                        val = data.ToString();
                        break;
                    case Base.Hex :
                        val = data.ToString("X");
                        val += "h";
                        break;
                    case Base.Bin :
                        val = ToBinaryString(data);
                        val += "b";
                        break;
                    case Base.Oct :
                        val = ToOctalString(data);
                        val += "o";
                        break;
                }
                return val; 
            }
        }
        #endregion
    }

    public class Rational : ICalculonType
    {
        public Rational(string s)
        {
            string[] parts = s.Split('/');
            numerator = BigInteger.Parse(parts[0]);
            denominator = BigInteger.Parse(parts[1]);
            this.Reduce();
        }

        public Rational(BigInteger num, BigInteger denom)
        {
            numerator = num;
            denominator = denom;
            this.Reduce();
        }

        public Rational(Integer i)
        {
            numerator = (BigInteger) i.data;
            denominator = 1;
        }

        internal BigInteger numerator;
        internal BigInteger denominator;

        public static explicit operator double(Rational r) => (double) (r.numerator / r.denominator);

        public void Reduce()
        {
            BigInteger gcf = numerator.GCF(denominator);
            if (gcf > 1)
            {
                numerator = numerator / gcf;
                denominator = denominator / gcf;
            }
        }

        #region strings
        public override string ToString()
        {
            return Display;
        }
        public string Display
        {
            get { return numerator.ToString() + "/" + denominator.ToString();}
        }
        #endregion
        #region Parsing
        public static bool IsMatch(string s)
        {
            return RationalMatch.IsMatch(s);
        }

        private static readonly Regex RationalMatch =
            new Regex(@"^[\+-]?\d+\/\d+$", RegexOptions.Compiled);
        #endregion

        public EvalReturn Eval(ref ControllerState cs)
        {
            cs.stack.Push(this);
            return new EvalReturn(Response.Ok, this);
        }
    }

}