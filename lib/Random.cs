using System;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Calculon.Types
{
    public class Rand : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "rand" }; } }

        public int NumArgs { get { return 0; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] retVal = new Type[0][];
                return retVal;
            }
        }
        public ICalculonType Execute(ref ControllerState cs)
        {
            Random rng = new Random();
            return new Real(rng.NextDouble());
        }
    }
}
