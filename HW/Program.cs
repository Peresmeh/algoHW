using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

namespace AlgoHW
{
    internal class Program
    {
        private static void putValues(IDynamicArray<int> array, int n)
        {
            for (int i = 0; i < n; i++)
            {
                array.Add(i, array.Count);
                Progress(i, n);
            }
        }

        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            IDynamicArray<int>[] arrays = new IDynamicArray<int>[3];

            for (int N = 100; N <= 1000000; N *= 10)
            {
                Console.WriteLine("N = " + N.ToString());
                arrays[0] = new SingleDynamicArray<int>();
                arrays[1] = new VectorDynamicArray<int>();
                arrays[2] = new FactorDynamicArray<int>();
                for (int i = 0; i < arrays.Length; i++)
                {
                    if (arrays[i] == null)
                        break;
                    timePoint = DateTime.Now;
                    sw.Restart();
                    putValues(arrays[i], N);
                    sw.Stop();
                    Console.WriteLine($"{arrays[i].Name}: {arrays[i].ReallocCount} {sw.ElapsedMilliseconds}  мс");
                }

            }
        }

        private static DateTime timePoint;
        private const int DelayMS = 2000;
        private static void Progress(int position, int count)
        {
            var dt = DateTime.Now;
            if((dt - timePoint).TotalMilliseconds > DelayMS)
            {
                timePoint = dt;
                Console.Write($"{position} of {count}");
                Console.CursorLeft = 0;
            }
        }
    }
}