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
             return new EvalReturn(Response.Ok, Display, this.GetType());
        }
        
        public string Display { get {return string.Empty;} }
    }

    public class ErrorType: ICalculonType
    {
        public ErrorType(string s) => (Display) = (s);
        
        public EvalReturn Eval(ref ControllerState cs)
        {
             return new EvalReturn(Response.Error, Display, this.GetType());
        }

        public string Display {get;}
    }

    public class ExitType: ICalculonType
    {
        public ExitType(){}

        public EvalReturn Eval(ref ControllerState cs)
        {
            cs.running = false;
            return new EvalReturn(Response.Exit, string.Empty, this.GetType());
        }

        public string Display { get{ return string.Empty; } }
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