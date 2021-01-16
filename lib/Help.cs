using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Calculon.Types
{
    public class HelpType : ICalculonType
    {
        public string Display { get { return string.Empty; } }

        public EvalReturn Eval(ref ControllerState cs)
        {
            return new EvalReturn(Response.Help, string.Empty, this.GetType());
        }

        public static EvalReturn ExtendedHelp(ref ControllerState cs, string[] args)
        {
            if (args.Length == 1 || string.IsNullOrEmpty(args[1]))
            {
                return new EvalReturn(Response.Help,
                    cs.Config.strings["HelpInitial"], typeof(HelpType));
            }
            Regex functions = regexize("HelpFun");
            if (functions.IsMatch(args[1]))
            {

                return new EvalReturn(Response.Help,
                    "todo", typeof(HelpType));
            }
            // need a generic help on help 

            throw new NotImplementedException();
        }

        #region parsing
        public static bool IsMatch(string s)
        {
            Regex HelpMatch = regexize("HelpCmd");
            return HelpMatch.IsMatch(s);
        }

        private static Regex regexize(string tableEntry)
        {
            StringBuilder match = new StringBuilder("^");
            match.Append(Config.handle.strings[tableEntry]);
            match.Append("$");
            return new Regex(match.ToString(), RegexOptions.IgnoreCase);
        }
        #endregion
    }
}
