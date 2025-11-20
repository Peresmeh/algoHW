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

        static object[] get(int i)
        {
            return [(int)1, (ulong)3];
        }


        static void Main(string[] args)
        {
            Test test = new Test();
            List<Chess> figs = new();
            figs.Add(new ChessKing());

            foreach (Chess chess in figs)
            {
                string path = GetTestPath(chess.Name);
                if (string.IsNullOrEmpty(path))
                    Console.WriteLine($"Couldn't find tests for chess {chess.Name}");
                else
                {
                    Console.WriteLine($"Tests for chess {chess.Name}");
                    test.Run(path, get);// chess.GetPositionMask);
                }
            }
        }
    }
}
