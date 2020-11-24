using System;
using Calculon;
using Calculon.Types;

namespace cl
{
    class Program
    {
        static void Main(string[] args)
        {
            Controller calc = new Controller();
            if (Console.IsInputRedirected)
            {
                Console.Out.WriteLine(calc.Eval(Console.In.ReadLine()).Msg);
            }
            else{
                while (calc.Running)
                {
                    Console.Write("calculon>");
                    EvalReturn eval = calc.Eval(Console.ReadLine());
                    Console.WriteLine(eval.Msg);
                }
            }
            
        }
    }
}
