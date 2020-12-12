using System;
using System.Linq;
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

        public static Real EPow(this Real num)
        {
            return new Real(Math.Exp(num.data));
        }

        public static Real Log(this Real num)
        {
            return new Real(Math.Log10(num.data));
        }

        public static Real Ln(this Real num)
        {
            return new Real(Math.Log(num.data));
        }
    }
    #endregion

    public class Pow : IFunctionCog
    {
        public string FunctionName { get { return "pow"; } }

        public int NumArgs { get { return 2; } }

        public Type[][] AllowedTypes
        {
            get
            {
                return FunctionFactory.TwoArgComboGenerator(typeof(Real), typeof(RealConstant));
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            ICalculonType rhs = cs.stack.Pop();
            ICalculonType lhs = cs.stack.Pop();
            Type[] argTypes = new Type[] { lhs.GetType(), rhs.GetType() };
            if (argTypes.SequenceEqual(new Type[] { typeof(Real), typeof(Real) })
                || argTypes.SequenceEqual(new Type[] { typeof(Real), typeof(RealConstant) })
                || argTypes.SequenceEqual(new Type[] { typeof(RealConstant), typeof(Real) })) 
            {
                Real left = new Real(lhs);
                Real right = new Real(rhs);
                return left.Pow(right);
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
                Type[][] retVal = new Type[2][];
                retVal[0] = new Type[] { typeof(Real) };
                retVal[1] = new Type[] { typeof(RealConstant) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            ICalculonType input = cs.stack.Pop();
            if (input.GetType() == typeof(Real) || input.GetType() == typeof(RealConstant))
            {
                return ((Real)input).EPow();
            }

            throw new NotImplementedException();
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
                Type[][] retVal = new Type[2][];
                retVal[0] = new Type[] { typeof(Real) };
                retVal[1] = new Type[] { typeof(RealConstant) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            ICalculonType input = cs.stack.Pop();
            if (input.GetType() == typeof(Real) || input.GetType() == typeof(RealConstant))
            {
                return ((Real)input).Log();
            }
            throw new NotImplementedException();
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
                Type[][] retVal = new Type[2][];
                retVal[0] = new Type[] { typeof(Real) };
                retVal[1] = new Type[] { typeof(RealConstant) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            ICalculonType input = cs.stack.Pop();
            if (input.GetType() == typeof(Real) || input.GetType() == typeof(RealConstant))
            {
                return ((Real)input).Ln();
            }
            throw new NotImplementedException();
        }
    }
}