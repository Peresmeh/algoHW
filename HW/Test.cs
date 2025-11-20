using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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

                try
                {
                    object[] tparams = new object[parameters.Length];
                    for (int i = 0; i < input.Length; i++)
                    {
                        tparams[i] = parameters[i].ParameterType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, [typeof(string)])
                            .Invoke(null, [input[i].Replace(System.Globalization.NumberFormatInfo.InvariantInfo.NumberDecimalSeparator, System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)]);
                    }

                    string[] formater = new string[output.Length];
                    Array.Fill(formater, "0");

                    for (int j = 0; j < output.Length; j++)
                    {
                        var tmp = output[j].Replace(System.Globalization.NumberFormatInfo.InvariantInfo.NumberDecimalSeparator, System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                        if (tmp.Contains(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                        {
                            int len = tmp.Length - tmp.IndexOf(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) - 1;
                            StringBuilder lbldr = new(".");
                            for (int i = 0; i < len; i++) lbldr.Append('0');
                            formater[j] = lbldr.ToString();
                        }
                    }
                    sw.Restart();
                    object returnedValue = test.DynamicInvoke(tparams);
                    sw.Stop();
                    object[] expectedValues = null;
                    var retInfo = test.GetMethodInfo().ReturnParameter.ParameterType;
                    object[] returnedValues;
                    if (retInfo.IsArray)
                    {
                    }
                    else
                    {
                        returnedValues = [returnedValue];
                        expectedValues = [retInfo
                        .GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, [typeof(string)])
                            .Invoke(null, [formater[0]])];
                    }
                    var stringer = test.GetMethodInfo().ReturnType.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, [typeof(string)]);

                    string[] returnedStrings = new string[expectedValues.Length];
                    string[] expectedStrings = new string[output.Length];

                    for (int i = 0; i < returnedStrings.Length; i++)
                    {
                        returnedStrings[i] = (string)stringer.Invoke(returnedValue, [formater]);
                        expectedStrings[i] = (string)stringer.Invoke(expectedValues[i], [formater]);
                    }
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
                    Console.WriteLine($" (завершено за {ticks:###,###,###,###,###,###,###,###,###} тиков)");

                }
            }
            else
            {
                var color = Console.ForegroundColor;
                string common = string.Concat(actual.TakeWhile((c, i) => i < Math.Min(actual.Length, expected.Length) && (c == expected[i])));
                Console.Write($"Тест {iter} ошибка: {actual} ожидалось: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(common);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(expected.Substring(common.Length));
                Console.ForegroundColor = color;
                Console.WriteLine($" (завершено за {ticks:###,###,###,###,###,###,###,###,###} тиков)");
            }
        }
        private static void ResultOutput(int iter, Exception ex)
        {
            Console.WriteLine($"Тест {iter} ошибка: {ex.Message}");
        }

        private const int repeaterMS = 2000;


        private static DateTime checkTime = DateTime.Now;
        private static object locker = new();

        public static void SetChecker(object _check, object _limit)
        {

            if (_check == null || _limit == null) return;

            if ((DateTime.Now - checkTime).TotalMilliseconds > repeaterMS)
            {
                lock (locker)
                {
                    checkTime = DateTime.Now;
                    Console.Write($"{_check} of {_limit}");
                    Console.SetCursorPosition(0, Console.CursorTop);
                }
            }
        }
    }
}
