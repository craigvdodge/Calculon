using System;
using Calculon;
using Spectre.Console;

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
                    string input = AnsiConsole.Ask<string>("[green]calculon[/]");
                    EvalReturn eval = calc.Eval(input);
                    if (eval.Response == Response.Error)
                    {
                        AnsiConsole.Markup("[red]" + eval.Msg + "\n[/]");
                    }
                    else
                    {
                        AnsiConsole.Markup(eval.Msg + "\n");
                    }
                }
            }
            
        }
    }
}
