using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AlgoHW
{
    internal class Test
    {
        public void Run(string path, Func<string[], string> run)
        {
            int iter = 0;
            while (true)
            {
                string fileIn = $"{path}\\test.{iter}.in";
                string fileOut = $"{path}\\test.{iter}.out";
                if (!File.Exists(fileIn) || !File.Exists(fileOut))
                    return;
                string[] input = File.ReadAllLines(fileIn);
                string[] output = File.ReadAllLines(fileOut);
                string x = run(input);
                if (x == output[0])
                {
                    Console.WriteLine("Тест " + iter + " OK: " + x);
                }
                else
                {
                    Console.WriteLine("Тест " + iter + " ошибка: " + x +
                                      " ожидалось: " + output[0]);
                }
                iter++;
            }
        }

        public void Run(string path, Delegate test)
        {
            Stopwatch sw = new();
            var parameters = test.Method.GetParameters();
            int iter = 0;
            while (true)
            {
                string fileIn = $"{path}\\test.{iter}.in";
                string fileOut = $"{path}\\test.{iter}.out";
                if (!File.Exists(fileIn) || !File.Exists(fileOut))
                    return;
                string[] input = File.ReadAllLines(fileIn);
                string[] output = File.ReadAllLines(fileOut);

                if (input.Length > parameters.Length)
                    Console.WriteLine($"Тест {iter} ошибка: передано {input.Length} аргументов, а ожидалось {parameters.Length}");

                string returnedString = "";
                string expectedString = "";
                try
                {
                    object[] tparams = new object[parameters.Length];
                    for (int i = 0; i < input.Length; i++)
                    {
                        tparams[i] = parameters[i].ParameterType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, [typeof(string)])
                            .Invoke(null, [input[i].Replace(System.Globalization.NumberFormatInfo.InvariantInfo.NumberDecimalSeparator, System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)]);
                    }

                    string formater = "0";
                    var tmp = output[0].Replace(System.Globalization.NumberFormatInfo.InvariantInfo.NumberDecimalSeparator, System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                    if(tmp.Contains(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                    {
                        int len = tmp.Length - tmp.IndexOf(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) - 1;
                        StringBuilder lbldr = new (".");
                        for (int i = 0; i < len; i++) lbldr.Append('0');
                        formater = lbldr.ToString();
                    }

                    sw.Restart();
                    object returnedValue = test.DynamicInvoke(tparams);
                    sw.Stop();
                    object expectedValue = test.GetMethodInfo().ReturnParameter.ParameterType
                        .GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, [typeof(string)])
                            .Invoke(null, [tmp]);

                    var stringer = test.GetMethodInfo().ReturnType.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, [typeof(string)]);
                    returnedString = (string)stringer.Invoke(returnedValue,[formater]);
                    expectedString = (string)stringer.Invoke(expectedValue, [formater]);  
                }
                catch (Exception pe)
                {
                    ResultOutput(iter, pe);
                    iter++;
                    continue;
                }

                ResultOutput(iter, returnedString, expectedString, sw.ElapsedTicks);
                iter++;
            }
        }

        private static void ResultOutput(int iter, string actual, string expected, long ticks)
        {
            if (actual.Equals(expected))
            {
                if (actual.Length < 20)
                    Console.WriteLine($"Тест {iter} OK: {actual} (завершено за {ticks:###,###,###,###,###,###,###,###,###} тиков)");
                else
                {
                    var color = Console.ForegroundColor;
                    Console.Write($"Тест {iter} OK: ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"{actual.Substring(0, 9)}...{actual.Substring(actual.Length - 9)}");
                    Console.ForegroundColor = color;
                       Console.WriteLine ($" (завершено за {ticks:###,###,###,###,###,###,###,###,###} тиков)");

                }
            }
            else
            {
                var color = Console.ForegroundColor;
                string common = string.Concat(actual.TakeWhile((c, i) =>  c == expected[i]));
                Console.Write($"Тест {iter} ошибка: {actual} ожидалось: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(common);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(expected.Substring(common.Length));
                Console.ForegroundColor = color;
                Console.WriteLine($" (завершено за {ticks::###,###,###,###,###,###,###,###,###} тиков)");
            }
        }
        private static void ResultOutput(int iter, Exception ex)
        {
            Console.WriteLine($"Тест {iter} ошибка: {ex.Message}");
        }
    }
}
