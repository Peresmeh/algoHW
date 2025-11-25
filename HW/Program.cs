using System;
using System.Collections;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AlgoHW
{
    internal class Program
    {
        static string GetTestPath(string name)
        {
            var path = Directory.GetDirectories("Tests", $"*{name}");
            if (path.Length != 1)
                return "";
            else return path[0];
        }

        internal static int GetOnes(ulong value)
        {
            int res = 0;
            while (value > 0)
            {
                res++;
                value &= (value - 1);
            }
            return res;
        }
        internal static int GetOnesSimple(ulong value)
        {
            int res = 0;
            for (int i = 0; i < 64; i++)
            {
                if ((value & (1ul << i)) != 0) res++;
            }
            return res;
        }

        private static byte[] oneCached;
        internal static int GetOnesCached(ulong value)
        {
            int res = 0;
            for (int i = 0; i < 8; i++)
                res += oneCached[(value >> (8 * i)) & 0xfful];
            return res;
        }

        static Program()
        {
            oneCached = new byte[256];
            for (uint i = 0; i < 256; i++)
                oneCached[i] = (byte)GetOnes(i);
        }

        static int Shell(int index, int length)
        {
            int ret = length / (int)Math.Pow(2, index + 1);

            return ret;
        }
        static int Lazarus(int index, int length)
        {
            int ret = 2 * length / (int)Math.Pow(2, index + 2) + 1;

            return ret;
        }
        static int Hibbard(int index, int length)
        {
            int i = 0;
            int ret;
            do
            {
                i++;
                ret = (int)Math.Pow(2, index + i) - 1;
            }
            while (ret < length);
            ret = (int)Math.Pow(2, i - 1) - 1;

            return Math.Max(ret, 1);
        }
        static int Knuth(int index, int length)
        {
            int i = 0;
            int ret;
            do
            {
                i++;
                ret = ((int)Math.Pow(3, index + i) - 1) / 2;
            }
            while (ret < length / 3);
            ret = ((int)Math.Pow(3, i - 1) - 1) / 2;

            return Math.Max(ret, 1);
        }


        static void Main(string[] args)
        {
            Test test = new Test();
            TimeSpan timeout = TimeSpan.FromMinutes(2);
            List<SorterBase<int>> sorters = new List<SorterBase<int>>()
            {
                new BubbleSort<int>(), new BubbleSortOptimised<int>(),
                new InsertSort<int>(), new InsertSortShift<int>(), new InsertSortBisection<int>(),
                new ShellSort<int>(Shell) {PredicateName = "Shell"}, new ShellSort<int>(Lazarus) {PredicateName = "Lazarus"},
                new ShellSort<int>(Hibbard) {PredicateName = "Hibbard"}, new ShellSort<int>(Knuth) {PredicateName = "Knuth"}
            };
            List<string> listPath = new List<string>() { "Tests\\0.random", "Tests\\1.digits", "Tests\\2.sorted", "Tests\\3.revers" };

            foreach (string path in listPath)
            {
                Console.WriteLine($"\n*****************************PERFORMING TESTS {path}*****************************");
                foreach (var sorter in sorters)
                {
                    Console.WriteLine($"\nTesting {sorter.Name} sorter");
                    test.Run(path, sorter, timeout);
                }
            }

        }
    }
}
