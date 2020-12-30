using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace Calculon.Types
{
    #region Extension methods
    public static class PowerExtensions
    {
        // Uses Bakhshali algorithim
        public static Number Sqrt(this Number num)
        {
            int precision = Number.GlobalPrecision;
            Number a = new Number(BigInteger.Zero);
            Number b = new Number(BigInteger.Zero);
            if (num.Precision > 0)
            {
                precision = num.Precision;
            }
            // inital seed is double, so we should start w/15 dp precision
            Number S = InitSqrtSeed(num);
            int curPre = 15;
            while (curPre < precision)
            {
                a = num.Subtract((S.Multiply(S))).Divide(S.Multiply(new Number(2)));
                b = a.Multiply(a).Divide((S.Add(a)).Multiply(new Number(2)));
                S = S.Add(a).Subtract(b);

                curPre = curPre * 4; // converges quartically
            }

            return S;
        }

        // Initial seed for square root
        // Next math tutorial I read that tells me to "just" start
        // with an "approximate value" gets a punch in the mouth.
        private static Number InitSqrtSeed(Number num)
        {
            double approx = Math.Pow(10, BigInteger.Log10(num.Numerator) / 2)
                 / Math.Pow(10, BigInteger.Log10(num.Denominator) / 2);
            Number output = new Number(approx.ToString());
            return output;
        }

        public static Number Inverse(this Number num)
        {
            Number output = new Number(num.Denominator, num.Numerator);
            output.View = num.View;
            return output;
        }

        public static Number Abs(this Number num)
        {
            Number output = new Number(BigInteger.Abs(num.Numerator), num.Denominator);
            output.View = num.View;
            output.DisplayBase = num.DisplayBase;
            return output;
        }

        public static Number Pow(this Number num, Number exp)
        {
            if (exp.Equals(0))
            {
                return new Number(BigInteger.One);
            }
            if (exp.Equals(1))
            {
                return num;
            }

            if (exp.LessThan(0))
            {
                Number output = num.Pow(exp.Abs()).Inverse();
                if (num.View == Number.ViewType.Integer
                    || num.View == Number.ViewType.Rational)
                {
                    output.View = Number.ViewType.Rational;
                }
                else
                {
                    output.View = Number.ViewType.Real;
                }
                return output;
            }

            if (!exp.IsWholeNumber)
            {
                // TODO: find nth root to pass along
                throw new NotImplementedException();
            }

            // for whole number only
            return new Number(num.Numerator.Pow(exp.Numerator));
        }

        // Exponentiation by squaring
        public static BigInteger Pow(this BigInteger value, BigInteger exponent)
        {
            BigInteger two = new BigInteger(2);
            BigInteger y = BigInteger.One;
            BigInteger remainder;

            while (exponent > BigInteger.One)
            {
                BigInteger.DivRem(exponent, two, out remainder);
                if (remainder == 0)
                {
                    value = value * value;
                    exponent = exponent / two;
                }
                else
                {
                    y = value * y;
                    value = value * value;
                    exponent = (exponent - BigInteger.One) / two;
                }
            }
            return value * y;
        }

        public static Real Pow(this Real lhs, Real rhs)
        {
            return new Real(Math.Pow(lhs.data, rhs.data));
        }

        public static Integer Pow(this Integer lhs, Integer rhs)
        {
            // We can only handle powers withing the range of 32 bit integers.
            return new Integer(BigInteger.Pow(lhs.data, (int) rhs.data));
        }

        public static Real EPow(this Real num)
        {
            return new Real(Math.Exp(num.data));
        }

        public static Real EPow(this Integer num)
        {
            return new Real(Math.Exp((double)num.data));
        }

        public static Real Log(this Real num)
        {
            return new Real(Math.Log10(num.data));
        }

        public static Real Log(this Integer num)
        {
            return new Real(Math.Log10((double) num.data));
        }

        public static Real Ln(this Real num)
        {
            return new Real(Math.Log(num.data));
        }

        public static Real Ln(this Integer num)
        {
            return new Real(Math.Log((double) num.data));
        }
    }
    #endregion

    public class Sqrt : IFunctionCog
    {
        public string FunctionName { get { return "sqrt"; } }

        public int NumArgs { get { return 1; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[1][];
                retVal[0] = new Type[] { typeof(Number) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Number num = (Number) cs.stack.Pop();
            return num.Sqrt();
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            Number num = (Number) cs.stack.Peek();
            if (num.IsNegative)
            {
                return "Square roots of negative numbers not supported.";
            }
            return string.Empty;
        }
    }

    public class Inverse : IFunctionCog
    {
        public string FunctionName { get { return "inverse"; } }

        public int NumArgs { get { return 1; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[1][];
                retVal[0] = new Type[] { typeof(Number) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Number n = (Number) cs.stack.Pop();
            return n.Inverse();
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }
    }

    public class Pow : IFunctionCog
    {
        public string FunctionName { get { return "pow"; } }

        public int NumArgs { get { return 2; } }

        public Type[][] AllowedTypes
        {
            get
            {
                return FunctionFactory.TwoArgComboGenerator(typeof(Number), typeof(Constant));
            }
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            ICalculonType rhs = cs.stack.Pop();
            ICalculonType lhs = cs.stack.Pop();
            Type[] argTypes = new Type[] { lhs.GetType(), rhs.GetType() };
            if (argTypes.SequenceEqual(new Type[] { typeof(Number), typeof(Number)}))
            {
                return ((Number) lhs).Pow((Number) rhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Constant), typeof(Number) }))
            {
                Number newRhs = (Number)rhs;
                Number newLhs = ((Constant)lhs).GetNumber(newRhs.Precision);
                return newLhs.Pow(newRhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Number), typeof(Constant) }))
            {
                Number newLhs = (Number)lhs;
                Number newRhs = ((Constant)rhs).GetNumber(newLhs.Precision);
                return newLhs.Pow(newRhs);
            }
            if (argTypes.SequenceEqual(new Type[] { typeof(Constant), typeof(Constant) }))
            {
                Number newLhs = ((Constant)lhs).GetNumber(Number.GlobalPrecision);
                Number newRhs = ((Constant)rhs).GetNumber(Number.GlobalPrecision);
                return newLhs.Pow(newRhs);
            }
            throw new ArgumentException("Unhandled argument types " + argTypes.ToString());
        }
    }

    public class EPow : IFunctionCog
    {
        public string FunctionName { get { return "epow"; } }

        public int NumArgs { get { return 1; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[3][];
                retVal[0] = new Type[] { typeof(Real) };
                retVal[1] = new Type[] { typeof(RealConstant) };
                retVal[2] = new Type[] { typeof(Integer) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            ICalculonType input = cs.stack.Pop();
            if (input.GetType() == typeof(Real))
            {
                return ((Real)input).EPow();
            }
            if (input.GetType() == typeof(RealConstant))
            {
                return ((RealConstant)input).ToReal().EPow();
            }
            if (input.GetType() == typeof(Integer))
            {
                return ((Integer)input).EPow();
            }

            throw new NotImplementedException();
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Base 10 logarithim
    /// </summary>
    public class Log : IFunctionCog
    {
        public string FunctionName { get { return "log"; } }

        public int NumArgs { get { return 1; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[3][];
                retVal[0] = new Type[] { typeof(Real) };
                retVal[1] = new Type[] { typeof(RealConstant) };
                retVal[2] = new Type[] { typeof(Integer) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            ICalculonType input = cs.stack.Pop();
            if (input.GetType() == typeof(Real))
            {
                return ((Real)input).Log();
            }
            if (input.GetType() == typeof(RealConstant))
            {
                return ((RealConstant)input).ToReal().Log();
            }
            if (input.GetType() == typeof(Integer))
            {
                return ((Integer)input).Log();
            }
            throw new NotImplementedException();
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }
    }

    public class Ln : IFunctionCog
    {
        public string FunctionName { get { return "ln"; } }
        public int NumArgs { get { return 1; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[3][];
                retVal[0] = new Type[] { typeof(Real) };
                retVal[1] = new Type[] { typeof(RealConstant) };
                retVal[2] = new Type[] { typeof(Integer) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            ICalculonType input = cs.stack.Pop();
            if (input.GetType() == typeof(Real))
            {
                return ((Real)input).Ln();
            }
            if (input.GetType() == typeof(RealConstant))
            {
                return ((RealConstant)input).ToReal().Ln();
            }
            if (input.GetType() == typeof(Integer))
            {
                return ((Integer)input).Ln();
            }
            throw new NotImplementedException();
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }
    }
}