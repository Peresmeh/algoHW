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

        static void Main(string[] args)
        {
            Test test = new Test();
            SorterBase sorter = new SorterBase();
            var tt = MetricAttribute.GetMetrics(sorter);
/*                string path = GetTestPath(chess.Name);
                if (string.IsNullOrEmpty(path))
                    Console.WriteLine($"Couldn't find tests for chess {chess.Name}");
                else
                {
                    Console.WriteLine($"Tests for chess {chess.Name}");
                    test.Run(path, testMerger(chess.GetPositionMask));
                }*/
        }
    }
}
