using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;

// For readablity, breaking the ICalculonTypes files based on roles.
// These are misc. "data" types.
namespace Calculon.Types
{
    public class Literal: ICalculonType
    {
        public Literal(string s)
        {
            data = s;
        }

        public EvalReturn Eval(ref ControllerState cs)
        {
            cs.stack.Push(this);
            return new EvalReturn(Response.Ok, this);
        }

        private string data;
        public string Display 
        {
            get { return data; }
        }

        #region Parsing
        private static readonly Regex LiteralStringMatch = 
            new Regex("\\\"[A-Za-z0-9_]+\\\"", RegexOptions.Compiled);
        public static bool IsMatch(string s)
        {
            return LiteralStringMatch.IsMatch(s);
        }
            #endregion
        }
}