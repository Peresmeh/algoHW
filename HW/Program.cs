using System;
using System.Collections;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace AlgoHW
{

    internal static class AlgoMath
    {
        public static double PowIter(double x, ulong N)
        {
            double res = 1;
            for (ulong i = 0; i < N; i++)
                res *= x;
            return res;
        }
        public static double PowMul(double x, ulong N)
        {
            if (N == 0) return 1;
            double tmp = PowMul(x, N / 2);
            if (N % 2 == 0)
                return tmp * tmp;
            else
                return tmp * tmp * x;
        }
        public static double PowBinary(double x, ulong N)
        {
            uint mask = (uint)1 << (sizeof(uint) * 8 - 1);
            double res = (N & mask) != 0 ? 1 : 0;
            do
            {
                mask >>= 1;
                res *= 2;
                if ((N & mask) != 0)
                    res += 1;
            } while (mask != 1);
            return res;
        }

        public static ulong FibonacciReq(uint N)
        {
            if (N < 3) return 1;
            else return FibonacciReq(N - 1) + FibonacciReq(N - 2);
        }

        public static ulong FibonacciReqCached(uint N, ulong cache1 = 1, ulong cache2 = 1, uint target = 3)
        {
            if (N < 3) return 1;
            if (N > target) return FibonacciReqCached(N, cache2, cache1 + cache2, target + 1);
            else return cache1 + cache2;
        }

        public static ulong FibonacciIter(uint N)
        {
            ulong res = 1, cache = 1, tmp = cache;
            for (uint i = 2; i < N; i++)
            {
                tmp = res;
                res += cache;
                cache = tmp;
            }
            return res;
        }

        public static ulong FibonacciGold(int N)
        {
            double fi = (1.0 + Math.Sqrt(5.0)) / 2.0;
            return (ulong)Math.Floor(Math.Pow(fi, N) / Math.Sqrt(5.0) + 0.5);
        }

        public static ulong FibonacciMatr(int N)
        {
            if (N < 3) return 1;
            Matrix<ulong> init = new () { { 1, 1 }, { 1, 0 } };
            Matrix<ulong> res = MatrixPowBinary(init, (uint)(N - 1));
            return res[0, 0];
        }


        public static uint AmountOfPrimes(uint N, Func<uint, bool> method)
        {
            if (N < 4)
                return N;

            uint res = 3;
            for (uint i = 0; i <= N; i++)
                if (method(i)) res++;

            return res;
        }

        public static bool IsPrimeSimple(uint N)
        {
            for (int i = 2; i < N; i++)
            {
                if (N % i == 0)
                    return false;
            }
            return true;
        }
        public static bool IsPrimeOptimised(uint N)
        {
            if (N < 4) return true;

            EratosthenesSimple er = new();
            er.UpdateCache((uint) Math.Sqrt(N));

            for (uint i = 2; i <= Math.Sqrt(N); i++)
            {
                if(er.IsPrime(i) && (N % i == 0))
                    return false;
            }
            return true;
        }


        public static Matrix<ulong> MatrixPowBinary(Matrix<ulong> x, uint N) 
        {
            uint mask = (uint)1 << (sizeof(uint) * 8 - 1);
            Matrix<ulong> res = new() { {1, 1 }, { 1, 0 } }; 
            if ((N & mask) != 0)
                return res;

            Matrix<ulong> m1 = new() { { 1, 1 }, { 1, 1 } };
            Matrix<ulong> m2 = new() { { 2, 0 }, { 0, 2 } };

            do
            {
                mask >>= 1;
                res = res * m2;
                if ((N & mask) != 0)
                    res = res + m1;
            } while (mask != 1);
            return res;
        }//public static 
    }


    internal class Program
    {

        static void Main(string[] args)
        {
            Test test = new Test();

            Console.WriteLine("PowIter tests");
            test.Run("Tests\\Power", AlgoMath.PowIter);
            Console.WriteLine("PowMul tests");
            test.Run("Tests\\Power", AlgoMath.PowIter);
            Console.WriteLine("PowBinary tests");
            test.Run("Tests\\Power", AlgoMath.PowIter);
        }
    }
}
