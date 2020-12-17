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
                    try
                    {
                        AnsiConsole.Render(DrawStack(calc.StackView));
                        string input = AnsiConsole.Ask<string>("[green]calculon[/]");
                        EvalReturn eval = calc.Eval(input);
                        if (eval.Response == Response.Error)
                        {
                            AnsiConsole.MarkupLine("[red]" + eval.Msg + "[/]");
                        }
                    }
                    catch (Exception e)
                    {
                        AnsiConsole.WriteException(e);
                    }
                }
            }
        }

        private static readonly int MaxStackView = 9;
        public static Table DrawStack(string[] stackView)
        {
            Table sview = new Table();

            sview.AddColumn(new TableColumn("id"));
            sview.AddColumn(new TableColumn("val"));

            int maxView = Math.Min(stackView.Length, MaxStackView);
            if (stackView.Length > MaxStackView)
            {
                sview.AddRow("-", "(more)");
            }

            for (int i= maxView; i>0; i--)
            {
                sview.AddRow((i-1).ToString(), stackView[i-1]);
            }

            sview.Border = TableBorder.Square;
            sview.Columns[0].Width(2);
            sview.Columns[0].Alignment = Justify.Right;
            sview.Columns[1].Width(78);
            sview.Columns[1].Alignment = Justify.Left;
            sview.HideHeaders();

            return sview;
        }
    }
}
