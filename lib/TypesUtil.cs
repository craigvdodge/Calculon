using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are "utility" types
namespace Calculon.Types
{
    // All Calculon types implement this interface.
    public interface ICalculonType
    {
        string Display { get; }
        EvalReturn Eval(ref ControllerState cs);
    }

    public class EmptyType: ICalculonType
    {
        public EmptyType(){}

        public EvalReturn Eval(ref ControllerState cs)
        {
             return new EvalReturn(Response.Ok, this);
        }
        
        public string Display { get {return string.Empty;} }
    }

    public class ErrorType: ICalculonType
    {
        public ErrorType(string s) => (Display) = (s);
        
        public EvalReturn Eval(ref ControllerState cs)
        {
             return new EvalReturn(Response.Error, this);
        }

        public string Display {get;}
    }

    public class Exit : IFunctionCog
    {
        public virtual string FunctionName { get { return "exit"; } }

        public int NumArgs { get { return 0; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[0][];
                return retVal;
            }
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            cs.running = false;
            return new EmptyType();
        }
    }

    public class Quit : Exit 
    {
        public override string FunctionName { get { return "quit"; } }
    }

    /// <summary>
    /// Use to bypass type-checking on utilty functions that 
    /// can take literally anything.
    /// </summary>
    public class AnyType : ICalculonType
    {
        public string Display { get { return string.Empty; } }

        public EvalReturn Eval(ref ControllerState cs)
        {
            return new EvalReturn(Response.Ok, string.Empty, this.GetType()) ;
        }
    }

    public class Drop : IFunctionCog
    {
        public string FunctionName { get { return "drop"; } }

        public int NumArgs { get { return 1; } }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[1][];
                retVal[0] = new Type[] { typeof(AnyType) };
                return retVal;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            cs.stack.Pop();
            return new EmptyType();
        }
    }

    public class Swap : IFunctionCog
    {
        public string FunctionName { get { return "swap"; } }

        public int NumArgs { get { return 2; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[1][];
                retVal[0] = new Type[] { typeof(AnyType), typeof(AnyType) };
                return retVal;
            }
        }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            ICalculonType zero = cs.stack.Pop();
            ICalculonType one = cs.stack.Pop();
            cs.stack.Push(zero);
            cs.stack.Push(one);
            return new EmptyType();
        }
    }

    public class Clear : IFunctionCog
    {
        public string FunctionName { get { return "clear";  } }

        public int NumArgs { get { return 0; } }

        public Type[][] AllowedTypes { get { return null; } }

        public string PreExecCheck(ref ControllerState cs)
        {
            return string.Empty;
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            cs.stack.Clear();
            return new EmptyType();
        }
    }

    public class util
    {
        // There's some way to get Pegasus to return the value as a nice
        // single string, but putting a rabid wolverine into my trousers
        // is preferable to debugging that, so do this for right now.
        public static string Concat(IList<string> list)
        {
            StringBuilder sb = new StringBuilder(list.Count);
            foreach (string s in list)
            {
                sb.Append(s);
            }
            return sb.ToString();
        } 
    }
}