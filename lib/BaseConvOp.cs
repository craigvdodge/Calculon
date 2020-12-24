using System;
using System.Collections.Generic;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are base conversion operations
namespace Calculon.Types
{
    public abstract class BaseConvBase : IFunctionCog
    {
        public virtual string FunctionName { get; }
        public virtual Number.Base NewBase { get; }

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
            Number i = (Number)cs.stack.Pop();
            i.DisplayBase = NewBase;
            return i;
        }
    }

    public class ToDec : BaseConvBase
    {
        public override string FunctionName { get { return "todec"; } }
        public override Number.Base NewBase { get { return Number.Base.Dec; } }
    }

    public class ToBin : BaseConvBase
    {
        public override string FunctionName { get { return "tobin"; } }
        public override Number.Base NewBase { get { return Number.Base.Bin; } }
    }

    public class ToHex : BaseConvBase
    {
        public override string FunctionName { get { return "tohex"; } }
        public override Number.Base NewBase { get { return Number.Base.Hex; } }
    }

    public class ToOct : BaseConvBase
    {
        public override string FunctionName { get { return "tooct"; } }
        public override Number.Base NewBase { get { return Number.Base.Oct; } }
    }

}