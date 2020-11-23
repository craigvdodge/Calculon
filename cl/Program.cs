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
            while (calc.Running)
            {
                Console.Write("calculon>");
                EvalReturn eval = calc.Eval(Console.ReadLine());
                Console.WriteLine(eval.Msg);
            }
            
        }
    }
}
