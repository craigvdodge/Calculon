using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Calculon.Types
{
    // Registering every new function as a "keyword" 
    // in the peg file is unsustainable. This is a factory
    // which supervises the creation of all functions
    public class FunctionFactory
    {
        // made singleton b/c of expensive intial startup.
        public static FunctionFactory Instance { get { return _instance;  } }

        private FunctionFactory()
        {
            functions = new Dictionary<string, IFunctionCog>();
            
            var cogType = typeof(IFunctionCog);
            var cogs = AppDomain.CurrentDomain.GetAssemblies()
                .Where(p => !p.IsDynamic)
                .SelectMany(s => s.GetExportedTypes())
                .Where(p => cogType.IsAssignableFrom(p) && !p.IsInterface);

            foreach (Type t in cogs)
            {
                IFunctionCog cog = (IFunctionCog)Activator.CreateInstance(t);
                functions.Add(cog.FunctionName.ToLower(), cog);
            }
        }

        private static readonly FunctionFactory _instance = new FunctionFactory();
        private Dictionary<string, IFunctionCog> functions;

        public FunctionInstance this[string fname]
        {
            get
            {
                if (functions.ContainsKey(fname.ToLower()))
                {
                    return new FunctionInstance(fname.ToLower());
                }
                throw new ArgumentOutOfRangeException("fname", "function not found");
            }
        }

        public EvalReturn Execute(string function, ref ControllerState cs)
        {
            // verify cog exists
            if (!functions.ContainsKey(function))
            {
                throw new ArgumentOutOfRangeException(function, "function not found");
            }

            IFunctionCog cog = functions[function];

            if (cog.NumArgs > 0) //Don't bother checking count & type of args if you take none.
            {
                // check number of arguments
                if (cs.stack.Count < cog.NumArgs)
                {
                    return new EvalReturn(Response.Error, 
                        cog.FunctionName + " requires " + cog.NumArgs.ToString() + " argument(s)",
                        typeof(FunctionInstance));
                }
                // Check the types of arguments
                Type[] argTypes = new Type[cog.NumArgs];
                int topStack = cs.stack.Count;
                
                // Get all the types
                for (int i=0; i<cog.NumArgs; i++)
                {
                    argTypes[i] = cs.stack.ElementAt(topStack - (topStack - i)).GetType();
                }

                bool allowed = false;
                foreach(Type[] t in cog.AllowedTypes)
                {
                    if (t.SequenceEqual(argTypes))
                    {
                        allowed = true;
                        break;
                    }
                }

                if (!allowed)
                {
                    string argListString = "(";
                    foreach (Type t in argTypes)
                    {
                        argListString += t.ToString() + " ";
                    }
                    argListString += ")";

                    return new EvalReturn(Response.Error,
                        "Unsupported types " + argListString,
                        typeof(FunctionInstance));
                }
            }
            // call cog Execute
            ICalculonType retVal = cog.Execute(ref cs);
            cs.stack.Push(retVal);
            return new EvalReturn(Response.Ok, retVal.Display, retVal.GetType());
        } 
    }

    // Externalizes the "state" of the FUnction Factory, i.e. the selected function
    public class FunctionInstance : ICalculonType
    {
        public FunctionInstance(string fname) => (Display) = (fname.ToLower());

        public string Display { get; }

        public EvalReturn Eval(ref ControllerState cs)
        {
            return FunctionFactory.Instance.Execute(Display, ref cs); 
        }
    }

    // Functions will implement this interface. It's
    // job is to allow the function to become part of 
    // the Factory. Hence it's a "cog" in the "machine"
    public interface IFunctionCog
    {
        string FunctionName { get; }
        int NumArgs {get;}
        Type[][] AllowedTypes {get;}
        ICalculonType Execute(ref ControllerState cs);
    }

}