﻿using System;
using System.Linq;
using System.Collections.Generic;
using Calculon.Types;

namespace Calculon
{
    public class Controller
    {
        public Controller()
        {
            state = new ControllerState();
        }

        public string[] StackView
        {
            get
            {
                string[] view = new string[state.stack.Count];
                for (int i=0; i<state.stack.Count; i++)
                {
                    view[i] = state.stack.ElementAt(i).Display;
                }

                return view;
            }

        }

        public EvalReturn Eval(string input)
        {
            try
            {
                EvalReturn retVal = new EvalReturn(Response.Ok, string.Empty, typeof(Types.EmptyType));
                string[] parts = input.Split(' ');
                foreach (string i in parts)
                {
                    Types.ICalculonType val = Parse(i);
                    retVal = val.Eval(ref state);
                    if (retVal.Response == Response.Help)
                    {
                        return HelpType.ExtendedHelp(ref state, parts);
                    }
                    if (retVal.Response != Response.Ok)
                    {
                        return retVal; // Something went wrong, bail here
                    }
                }

                return retVal;
            }
            catch (System.FormatException)  //Parser didn't understand input
            {
                Types.ErrorType err = new Types.ErrorType(
                    String.Format(GetString("ParseErr"), input));
                return err.Eval(ref state);
            }
        }

        public string GetString(string key)
        {
            return state.Config.strings[key];
        }

        private ControllerState state;
        public bool Running { get{ return state.Running; } }

        public Types.ICalculonType Parse(string input)
        {
            if (Literal.IsMatch(input))
            {
                Literal l = new Literal(input);
                return l;
            }
            else if (Real.IsMatch(input))
            {
                Real r = new Real(input);
                return r;
            }
            else if (Rational.IsMatch(input))
            {
                Rational rat = new Rational(input);
                return rat;
            }
            else if (Integer.IsMatch(input))
            {
                Integer i = new Integer(input);
                return i;
            }
            else if (RealConstant.IsMatch(input))
            {
                RealConstant rc = new RealConstant(input);
                return rc;
            }
            else if (HelpType.IsMatch(input))
            {
                HelpType ht = new HelpType();
                return ht;
            }
            else
            {
                FunctionInstance fi = FunctionFactory.Instance[input];
                if (fi.IsError)
                {
                    return new ErrorType(String.Format(GetString("NotNumNotString"), input));
                }
                return fi;
            }
        }
    }

    // Hoist this into it's own class so it can be passed aroumd
    public class ControllerState
    {
        public ControllerState()
        {
            stack = new Stack<Types.ICalculonType>();
            running = true;
            Config = Calculon.Types.Config.handle;
        }

        internal Stack<Types.ICalculonType> stack;

        internal bool running;
        public bool Running { get{ return running; } }

        public Calculon.Types.Config Config;
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
        Exit = 1,
        Help = 2
    }
}
