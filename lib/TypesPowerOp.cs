using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace Calculon.Types
{
    #region Extension methods
    public static class PowerExtensions
    {
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

        public static Real Log2(this Real num)
        {
            return new Real(Math.Log2(num.data));
        }

        public static Real Sqrt(this Real num)
        {
            return new Real(Math.Sqrt(num.data));
        }

        public static Real Root(this Real num, Real n)
        {
            return new Real(Math.Pow(num.data, 1.0 / n.data));
        }
    }
    #endregion

    public class Pow : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "pow", "^" }; } }

        public int NumArgs { get { return 2; } }

        public Type[][] AllowedTypes
        {
            get
            {
                return FunctionFactory.TwoArgComboGenerator(typeof(Real), typeof(RealConstant), typeof(Integer));
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            ICalculonType rhs = cs.stack.Pop();
            ICalculonType lhs = cs.stack.Pop();
            Type[] argTypes = new Type[] { lhs.GetType(), rhs.GetType() };
            if (argTypes.SequenceEqual(new Type[] { typeof(Real), typeof(Real) })
                || argTypes.SequenceEqual(new Type[] { typeof(Real), typeof(RealConstant) })
                || argTypes.SequenceEqual(new Type[] { typeof(RealConstant), typeof(Real) })
                || argTypes.SequenceEqual(new Type[] { typeof(Real), typeof(Integer)})
                || argTypes.SequenceEqual(new Type[] { typeof(RealConstant), typeof(Integer) })
                || argTypes.SequenceEqual(new Type[] { typeof(Integer), typeof(Real) })
                || argTypes.SequenceEqual(new Type[] { typeof(Integer), typeof(RealConstant) })
                ) 
            {
                Real left = new Real(lhs);
                Real right = new Real(rhs);
                return left.Pow(right);
            }
            if(argTypes.SequenceEqual(new Type[] { typeof(Integer), typeof(Integer) }))
            {
                return ((Integer)lhs).Pow((Integer)rhs);
            }
            throw new ArgumentException(
                String.Format(Config.handle.strings["UnsupportedTypes"], argTypes));
        }
    }

    public class EPow : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "epow" }; } }

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
    }

    public class Sqrt : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "sqrt" }; } }

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
            Real ConvertedInput = new Real(input);
            return ConvertedInput.Sqrt();
        }
    }

    public class Root : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "root" }; } }

        public int NumArgs { get { return 2; } }

        public Type[][] AllowedTypes 
        {
            get
            {
                return FunctionFactory.TwoArgComboGenerator(typeof(Real), typeof(RealConstant), typeof(Integer));
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Real y = new Real(cs.stack.Pop());
            Real x = new Real(cs.stack.Pop());
            return x.Root(y);
        }
    }

    /// <summary>
    /// Base 10 logarithim
    /// </summary>
    public class Log : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "log" }; } }

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
    }

    /// <summary>
    /// Log base e
    /// </summary>
    public class Ln : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "ln" }; } }
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
    }

    /// <summary>
    /// log base 2
    /// </summary>
    public class Log2 : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "log2" }; } }

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
            Real ConvertedInput = new Real(input);
            return ConvertedInput.Log2();
        }
    }
}