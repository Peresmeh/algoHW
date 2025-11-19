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
            Console.WriteLine("Queue tests");

            SimpleDynamicQueue<int> queue = new(5);
            for (int i = 0; i < 7; i++)
                queue.Enqueue(i);
            Console.WriteLine($"{queue.Count} used from {queue.AllocSize}");

            for (int i = 7; i < 1107; i++)
            {
                queue.Enqueue(i);
                var deq = queue.Dequeue();
                if (i - deq != 7)
                    Console.WriteLine("Error");
            }
            Console.WriteLine($"{queue.Count} used from {queue.AllocSize}");

            for (int i = 0; i < 7; i++)
                queue.Enqueue(i);

            Console.WriteLine($"{queue.Count} used from {queue.AllocSize}");

            for (int i = 0; i < 7; i++)
                queue.Dequeue();

            Console.WriteLine($"{queue.Count} used from {queue.AllocSize}");
            Console.Write("Dequeueing from 0 to 6:");
            for (int i = 0; i < 7; i++)
                Console.Write($" {queue.Dequeue()}");

            Console.WriteLine();
            Console.WriteLine($"{queue.Count} used from {queue.AllocSize}");
            Console.WriteLine($"Queue maust be empty at this point. Trying to get exception");
            try { queue.Dequeue(); }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"Exception caught!");
            }


            Console.WriteLine();
            Console.WriteLine("Test of priority");

            PriorityQueue<int> prQue = new(5, 10);
            for(int i = 0;i < 7;i++)
            {
                prQue.Enqueue(i, (i * 2) % 7);
            }
            Console.WriteLine($"{queue.Count} used from {queue.AllocSize}");
            Console.Write("Lets dequeue:");
            for (int i = 0; i < 7; i++)
                Console.Write($" {prQue.Dequeue()}");

            Console.WriteLine();
            Console.WriteLine("Test of arrays");

            Stopwatch sw = new Stopwatch();
            IDynamicArray<int>[] arrays = new IDynamicArray<int>[4];

            for (int N = 100; N <= 1000000; N *= 10)
            {
                Console.WriteLine("N = " + N.ToString());
                arrays[0] = new SingleDynamicArray<int>();
                arrays[1] = new VectorDynamicArray<int>();
                arrays[2] = new FactorDynamicArray<int>();
                arrays[3] = new MatrixDynamicArray<int>();
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
            if ((dt - timePoint).TotalMilliseconds > DelayMS)
            {
                timePoint = dt;
                Console.Write($"{position} of {count}");
                Console.CursorLeft = 0;
            }
        }
    }
}