using System;
using System.Collections.Generic;

namespace Calculon
{
    public class Controller
    {
        public Controller() 
        {
            state = new ControllerState();
        }
        
        public EvalReturn Eval(string input)
        {
            Parser parser = new Parser();

            try
            {
                EvalReturn retVal = new EvalReturn(Response.Ok, string.Empty, typeof(Types.EmptyType));
                string[] parts = input.Split(' ');
                foreach (string i in parts)
                {
                    Types.ICalculonType val = parser.Parse(i);
                    retVal = val.Eval(ref state);
                    if (retVal.Response != Response.Ok)
                    {
                        return retVal; // Something went wrong, bail here
                    }
                }

                return retVal;
            }
            catch (System.FormatException)  //Parser didn't understand input
            {
                Types.ErrorType err = new Types.ErrorType("PARSE ERROR: " + input);
                return err.Eval(ref state);
            }
        }
        
        private ControllerState state;
        public bool Running { get{ return state.Running; } }

    }

    // Hoist this into it's own class so it can be passed aroumd
    public class ControllerState
    {
        public ControllerState()
        {
            stack = new Stack<Types.ICalculonType>();
            running = true;
        }

        internal Stack<Types.ICalculonType> stack;

        internal bool running;
        public bool Running { get{ return running; } }

    }

    public sealed record EvalReturn
        {
            public Response Response { get; }
            public string Msg { get; }
            public Type Type { get; }

            public EvalReturn(Response code, string msg, Type type) => (Response, Msg, Type) = (code, msg, type);
            public EvalReturn(Response code, Types.ICalculonType calculonType) => (Response, Msg, Type) = (code, calculonType.Display, calculonType.GetType());
        }

        public enum Response
        {
            NotImpl = -2,
            Error = -1,
            Ok = 0,
            Exit = 1
        }
}
