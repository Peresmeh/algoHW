using System;
using System.Collections.Generic;
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

                object returnedString = null;
                object expectedString = null;
                try
                {
                    object[] tparams = new object[parameters.Length];
                    for (int i = 0; i < input.Length; i++)
                    {
                        tparams[i] = parameters[i].ParameterType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, [typeof(string)])
                            .Invoke(null, [input[i].Replace(System.Globalization.NumberFormatInfo.InvariantInfo.NumberDecimalSeparator, System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)]);
                    }

                    string formater = "#";
                    var tmp = output[0].Replace(System.Globalization.NumberFormatInfo.InvariantInfo.NumberDecimalSeparator, System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                    if(tmp.Contains(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                    {
                        int len = tmp.Length - tmp.IndexOf(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) - 1;
                        StringBuilder lbldr = new (".");
                        for (int i = 0; i < len; i++) lbldr.Append('0');
                        formater = lbldr.ToString();
                    }

                    
                    object returnedValue = test.DynamicInvoke(tparams);
                    object expectedValue = test.GetMethodInfo().ReturnParameter.ParameterType
                        .GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, [typeof(string)])
                            .Invoke(null, [tmp]);

                    var stringer = test.GetMethodInfo().ReturnType.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, [typeof(string)]);
                    returnedString = stringer.Invoke(returnedValue,[formater]);
                    expectedString = stringer.Invoke(expectedValue, [formater]);  
                }
                catch (Exception pe)
                {
                    Console.WriteLine($"Тест {iter} ошибка: {pe.Message}");
                    iter++;
                    continue;
                }

                if (returnedString.Equals(expectedString))
                {
                    Console.WriteLine($"Тест {iter} OK: {returnedString}");
                }
                else
                {
                    Console.WriteLine($"Тест {iter} ошибка: { returnedString} ожидалось: {  expectedString}");
                }
                iter++;
            }
        }
    }
}
