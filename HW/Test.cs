using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.IO.Pipes;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AlgoHW
{

    [AttributeUsage(AttributeTargets.Property)]
    public class MetricAttribute : Attribute
    {
        public string MetricName { get; }
        public string OutputFormat { get; }

        public MetricAttribute(string metricName, string outputFormat = "0")
        {
            MetricName = metricName;
            OutputFormat = outputFormat;
        }

        public static Dictionary<string, string> GetMetrics(object source)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach (var property in source.GetType().GetRuntimeProperties())
            {
                var attr = property.GetCustomAttribute<MetricAttribute>();
                if (attr != null)
                {
                    var format = $"{{0:{attr.OutputFormat}}}";
                    ret[attr.MetricName] = string.Format(format, property.GetValue(source));
                }
            }
            return ret;
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class MetricTestAttribute : Attribute
    {
        public string TestPathMask { get; }
        public bool ResultIsParameter { get; }
        public int ResultParameter { get; }

        public MetricTestAttribute(string testPathMask, bool resultIsParameter = false, int resultParameter = 0)
        {
            TestPathMask = testPathMask;
            ResultIsParameter = resultIsParameter;
            ResultParameter = resultParameter;
        }
        private static Delegate CreateDelegate(MethodInfo methodInfo, object target)
        {
            Func<Type[], Type> getType;
            var isAction = methodInfo.ReturnType.Equals((typeof(void)));
            var types = methodInfo.GetParameters().Select(p => p.ParameterType);
            if (isAction)
            {
                getType = Expression.GetActionType;
            }
            else
            {
                getType = Expression.GetFuncType;
                types = types.Concat(new[] { methodInfo.ReturnType });
            }
            if (methodInfo.IsStatic)
            {
                return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
            }
            return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
        }
        public static Delegate GetTestDelegate(object source)
        {
            Delegate ret = null;
            var methods = source.GetType().GetRuntimeMethods();
            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<MetricTestAttribute>() != null)
                {
                    ret = CreateDelegate(method, source); break;
                }
            }
            return ret;
        }

        public static MetricTestAttribute GetTestAttribute(object source)
        {
            MetricTestAttribute ret = null;
            var methods = source.GetType().GetRuntimeMethods();
            foreach (var method in methods)
            {
                ret = method.GetCustomAttribute<MetricTestAttribute>();
                if (ret != null) break;
            }
            return ret;
        }

    }



    internal class Test
    {
        private string[] input, output;
        private List<object> parameters = new();
        private List<Type> parameterTypes = new();

        private List<string> outputVals = new();
        private List<string> expectedVals = new();

        private Delegate runner = null;

        private bool LoadTest(string path, int iteration)
        {
            string fileIn = $"{path}\\test.{iteration}.in";
            string fileOut = $"{path}\\test.{iteration}.out";
            if (!File.Exists(fileIn) || !File.Exists(fileOut))
                return false;

            input = File.ReadAllLines(fileIn);
            output = File.ReadAllLines(fileOut);

            ParseParameters();

            return true;
        }

        private void ParseParameters()
        {
            parameters.Clear();
            int off = 0;
            for (int i = 0; i < parameterTypes.Count; i++)
            {
                if (parameterTypes[i].IsArray)
                {
                    int len = int.Parse(input[off]);
                    off++;
                    var elType = parameterTypes[i].GetElementType();
                    parameters.Add(Array.CreateInstance(elType, len));
                    var tegs = input[off].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (tegs.Length != len) throw new InvalidDataException("Количество элементов не соответсвует длине массива");
                    for (int j = 0; j < len; j++)
                    {
                        object box = elType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, [typeof(string)])
                            .Invoke(null, [tegs[j].Replace(System.Globalization.NumberFormatInfo.InvariantInfo.NumberDecimalSeparator, System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)]);
                        (parameters[i] as Array).SetValue(box, j);
                    }
                }
                else
                {
                    parameters.Add(parameterTypes[i].GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, [typeof(string)])
                        .Invoke(null, [input[i].Replace(System.Globalization.NumberFormatInfo.InvariantInfo.NumberDecimalSeparator, System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)]));
                }
                off++;
            }
        }

        private void ParseResult(object result)
        {
            outputVals.Clear();
            expectedVals.Clear();

            string[] source;
            if (result.GetType().IsArray)
            {
                if (output.Length == 1)
                {
                    var tegs = output[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (tegs.Length != (result as Array).Length)
                        throw new InvalidDataException("Количество элементов не соответсвует длине массива");
                    source = tegs;
                }
                else
                {
                    if (output.Length != (result as Array).Length)
                        throw new InvalidDataException("Количество элементов не соответсвует длине массива");

                    source = output;
                }

                for (int i = 0; i < source.Length; i++)
                {
                    expectedVals.Add((result as Array).GetValue(i).ToString());
                    outputVals.Add(source[i]);
                }

            }
            else
            {
                outputVals.Add(result.ToString());
                expectedVals.Add(output[0]);
            }
        }

        private void InitTest(object source)
        {
            runner = MetricTestAttribute.GetTestDelegate(source);
            if (runner == null) return;

            parameterTypes.Clear();
            parameters.Clear();
            foreach (var param in runner.Method.GetParameters())
                parameterTypes.Add(param.ParameterType);
        }

        private static CancellationTokenSource cts = new();
        private static Task testTask = Task.CompletedTask;

        private Dictionary<object, Dictionary<string, (long, List<(string, string)>)>> resultTable = new();

        public void ResetMetricsResult()
        {
            resultTable.Clear();
        }

        private static string FitCellSimple(string s, int width = 20)
        {
            if (s == null) s = "";
            if (s.Length > width) s = s.Substring(0, width - 3) + "...";
            return s.PadRight(width);
        }

        public void BuildMetricsResult()
        {
            List<string> metrics = new();
            foreach (var test in resultTable.Values)
                foreach (var iter in test.Values)
                {
                    foreach (var metric in iter.Item2)
                        if (!metrics.Contains(metric.Item1))
                            metrics.Add(metric.Item1);
                }


            StringBuilder bldr = new();
            foreach (var metric in metrics)
            {
                Console.WriteLine($"\nMetric summary for {metric}");
                Console.WriteLine("------------------------------------------");
                bldr.Clear();
                bldr.Append(FitCellSimple("Тест")).Append('|');
                foreach (var iter in resultTable.First().Value.Keys)
                {
                    bldr.Append(FitCellSimple(iter, 15)).Append('|');
                }

                Console.WriteLine(bldr.ToString());
                foreach (var test in resultTable)
                {
                    bldr.Clear();
                    bldr.Append(FitCellSimple(test.Key.ToString())).Append('|');
                    foreach (var iter in test.Value.Values)
                    {
                        foreach (var metricItem in iter.Item2)
                            if (metricItem.Item1 == metric)
                                bldr.Append(FitCellSimple(metricItem.Item2, 15)).Append('|');
                    }
                    Console.WriteLine(bldr.ToString());
                }
            }
            Console.WriteLine($"\nPerformance summaryin tics");
            Console.WriteLine("------------------------------------------");
            bldr.Clear();
            bldr.Append(FitCellSimple("Тест")).Append('|');
            foreach (var iter in resultTable.First().Value.Keys)
            {
                bldr.Append(FitCellSimple(iter, 15)).Append('|');
            }

            Console.WriteLine(bldr.ToString());
            foreach (var test in resultTable)
            {
                bldr.Clear();
                bldr.Append(FitCellSimple(test.Key.ToString())).Append('|');
                foreach (var iter in test.Value.Values)
                {
                    bldr.Append(FitCellSimple(iter.Item1.ToString(), 15)).Append('|');
                }
                Console.WriteLine(bldr.ToString());
            }


            Console.WriteLine("------------------------------------------");
        }

        private void AddMetricTime(object source, string test, long time)
        {
            if (!resultTable.ContainsKey(source))
                resultTable[source] = new();

            if (!resultTable[source].ContainsKey(test))
                resultTable[source][test] = new();

            var rez = resultTable[source][test];
            rez.Item1 = time;
            resultTable[source][test] = rez;
        }

        private void AddMetricResult(object source, string test, string metric, string value)
        {
            if (!resultTable.ContainsKey(source))
                resultTable[source] = new();

            if (!resultTable[source].ContainsKey(test))
                resultTable[source][test] = new();

            var rez = resultTable[source][test];
            if (rez.Item2 == null) rez.Item2 = new();
            rez.Item2.Add((metric, value));
            resultTable[source][test] = rez;
        }

        public void Run(string path, object source, TimeSpan? timeout = null)
        {
            int iter = 0;
            Stopwatch sw = new();

            InitTest(source);
            if (runner == null) return;

            while (LoadTest(path, iter))
            {
                ParseParameters();

                cts.Cancel();

                if (!timeout.HasValue)
                    cts = new CancellationTokenSource();
                else
                    cts = new CancellationTokenSource(timeout.Value);
                object? result = null;
                testTask = Task.Run(() =>
                {
                    sw.Start();
                    result = runner.DynamicInvoke(parameters.ToArray());
                    sw.Stop();
                }, cts.Token);

                string column = $"Iter {iter}";
                try
                {
                    testTask.Wait();
                    var attr = MetricTestAttribute.GetTestAttribute(source);
                    if (attr.ResultIsParameter)
                        ParseResult(parameters[attr.ResultParameter]);
                    else
                        ParseResult(result);
                }
                catch (Exception ex)
                {
                    var exmetrics = MetricAttribute.GetMetrics(source);
                    ResultOutput(iter, ex);
                    AddMetricTime(source, column, sw.ElapsedTicks);
                    foreach (var metric in exmetrics)
                    {
                        AddMetricResult(source, column, metric.Key, "N/A");
                    }
                    iter++;
                    continue;
                }

                var metrics = MetricAttribute.GetMetrics(source);
                ResultOutput(iter, outputVals.ToArray(), expectedVals.ToArray(), sw.ElapsedTicks, false, metrics);
                AddMetricTime(source, column, sw.ElapsedTicks);
                foreach (var metric in metrics)
                {
                    AddMetricResult(source, column, metric.Key, metric.Value);
                }

                iter++;
            }
        }


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

        private static void ResultOutput(int iter, string[] actual, string[] expected, long ticks, bool showDetailedResult = true, Dictionary<string, string> metrics = null)
        {
            if (ShortCheck(actual, expected))
            {
                Console.Write($"Тест {iter} OK: ");
                if (showDetailedResult)
                {
                    for (int i = 0; i < expected.Length; i++)
                    {
                        WriteShortForm($"{expected[i]} ", 40, Console.ForegroundColor, ConsoleColor.Yellow);
                    }
                }

                if (metrics != null)
                {
                    foreach (var metric in metrics)
                    {
                        Console.Write($"{metric.Key}: {metric.Value} ");
                    }
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
            while (ex.InnerException != null) ex = ex.InnerException;
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
            cts.Token.ThrowIfCancellationRequested();
        }
    }
}
