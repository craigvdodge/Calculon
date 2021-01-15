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
            return new EvalReturn(Response.Help, 
                cs.Config.strings["HelpInitial"], typeof(HelpType));
        }

        #region parsing
        public static bool IsMatch(string s)
        {
            return HelpMatch.IsMatch(s);
        }

        private static readonly Regex HelpMatch =
            new Regex("^help$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        #endregion
    }
}
