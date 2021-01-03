using System;
using System.Collections.Generic;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are integer base conversion operations
namespace Calculon.Types
{
    public abstract class BaseConvBase : IFunctionCog
    {
        public virtual string[] FunctionName { get; }
        public virtual Integer.Base NewBase { get; }

        public int NumArgs { get { return 1; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[1][];
                retVal[0] = new Type[] { typeof(Integer) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Integer i = (Integer)cs.stack.Pop();
            i.DisplayBase = NewBase;
            return i;
        }
    }

    public class ToDec : BaseConvBase
    {
        public override string[] FunctionName { get { return new string[] { "todec" }; } }
        public override Integer.Base NewBase { get { return Integer.Base.Dec; } }
    }

    public class ToBin : BaseConvBase
    {
        public override string[] FunctionName { get { return new string[] { "tobin" }; } }
        public override Integer.Base NewBase { get { return Integer.Base.Bin; } }
    }

    public class ToHex : BaseConvBase
    {
        public override string[] FunctionName { get { return new string[] { "tohex" }; } }
        public override Integer.Base NewBase { get { return Integer.Base.Hex; } }
    }

    public class ToOct : BaseConvBase
    {
        public override string[] FunctionName { get { return new string[] { "tooct" }; } }
        public override Integer.Base NewBase { get { return Integer.Base.Oct; } }
    }

}