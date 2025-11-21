using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

                string[] returnedStrings = null;
                string[] expectedStrings = null;

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
                    expectedValues = new object[output.Length];
                    if (retInfo.IsArray)
                    {
                        returnedValues = new object[(returnedValue as Array).Length];
                        for (int i = 0; i < returnedValues.Length; i++)
                            returnedValues[i] = (returnedValue as Array).GetValue(i);
                        for (int i = 0; i < expectedValues.Length; i++)
                        {
                            if (i < returnedValues.Length)
                                expectedValues[i] = returnedValues[i].GetType().GetRuntimeMethod("Parse", [typeof(string)]).Invoke(null, [output[i]]);
                            else
                                expectedValues[i] = null;
                        }
                    }
                    else
                    {
                        returnedValues = [returnedValue];
                        Array.Fill(expectedValues, null);
                        expectedValues[0] = retInfo.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, [typeof(string)]).Invoke(null, [output[0]]);
                    }
                    MethodInfo[] stringer;

                    returnedStrings = new string[returnedValues.Length];
                    stringer = new MethodInfo[returnedValues.Length];
                    expectedStrings = new string[output.Length];

                    for (int i = 0; i < returnedStrings.Length; i++)
                    {
                        stringer[i] = returnedValues[i].GetType().GetRuntimeMethod("ToString", [typeof(string)]);
                        returnedStrings[i] = (string)stringer[i].Invoke(returnedValues[i], [formater[i]]);
                    }
                    for (int i = 0; i < expectedStrings.Length; i++)
                        if ((expectedValues[i] != null) && (i < stringer.Length)) expectedStrings[i] = (string)stringer[i].Invoke(expectedValues[i], [formater[i]]);
                        else expectedStrings[i] = output[i];
                }
                catch (Exception pe)
                {
                    ResultOutput(iter, pe);
                    iter++;
                    continue;
                }

                ResultOutput(iter, returnedStrings, expectedStrings, sw.ElapsedTicks);
                iter++;
            }
        }

        private static bool ShortCheck(string[] actual, string[] expected)
        {
            if (actual.Length == expected.Length)
            {
                for (int i = 0; i < expected.Length; i++)
                    if (!actual[i].Equals(expected[i]))
                        return false;
            }
            else
                return false;

            return true;

        }

        private static void WriteShortForm(string output, int length, ConsoleColor normalColor, ConsoleColor shortColor)
        {
            var color = Console.ForegroundColor;
            if (output.Length < length)
            {
                Console.Write($"{output}");
                Console.ForegroundColor = normalColor;
            }
            else
            {
                Console.ForegroundColor = shortColor;
                Console.Write($"{output.Substring(0, output.Length / 2 - 1)}...{output.Substring(output.Length - (output.Length / 2 - 1))} ");
            }
            Console.ForegroundColor = color;
        }

        private static void ResultOutput(int iter, string[] actual, string[] expected, long ticks)
        {
            if (ShortCheck(actual, expected))
            {
                Console.Write($"Тест {iter} OK: ");

                for (int i = 0; i < expected.Length; i++)
                {
                    WriteShortForm($"{expected[i]} ", 40, Console.ForegroundColor, ConsoleColor.Yellow);
                }
            }
            else
            {
                Console.WriteLine($"Тест {iter} ошибка: ");
                for (int i = 0; i < Math.Max(actual.Length, expected.Length); i++)
                {
                    var color = Console.ForegroundColor;

                    if ((actual.Length > i) && (expected.Length > i) && (actual.Equals(expected)))
                    {
                        WriteShortForm($"{expected[i]} ", 40, ConsoleColor.Green, ConsoleColor.Green);
                    }
                    else
                    {
                        Console.Write("Результат: ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        WriteShortForm($"{((i >= actual.Length) || (actual[i] == null) ? "Null" : actual[i])}", 40, ConsoleColor.Red, ConsoleColor.Red);
                        Console.Write(" ожидалось ");
                        WriteShortForm($"{((i >= expected.Length) || (expected[i] == null) ? "Null" : expected[i])}", 40, Console.ForegroundColor, Console.ForegroundColor);
                        Console.WriteLine();
                    }
                    Console.ForegroundColor = color;

                }
            }
            Console.WriteLine($" (завершено за {ticks:###,###,###,###,###,###,###,###,###} тиков)");
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
