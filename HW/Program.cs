using System;
using System.Collections;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AlgoHW
{

    internal static class AlgoPow
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
            ulong mask = 1ul << (sizeof(ulong) * 8 - 1);
            double res = (N & mask) != 0 ? x : 1;
            do
            {
                mask >>= 1;
                res *= res;
                if ((N & mask) != 0)
                    res *= x;
            } while (mask != 1);
            return res;
        }

        public static Matrix<ulong> MatrixPowBinary(Matrix<ulong> x, uint N)
        {
            uint mask = (uint)1 << (sizeof(uint) * 8 - 1);
            Matrix<ulong> res = new() { { 1, 1 }, { 1, 0 } };
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
    internal static class AlgoFibonacci
    {
        static int depth = 1;
        const int depthLimit = 4100;

        public static BigInteger FibonacciRec(uint N)
        {
            BigInteger res = BigInteger.Zero;
            depth++;

            if(depth > depthLimit)
                throw new InvalidOperationException("Recursion is too depth");

            if (N == 0) res = 0;
            else if (N < 3) res = 1;
            else res = FibonacciRec(N - 1) + FibonacciRec(N - 2);

            depth--;

            return res;
        }
        public static BigInteger FibonacciRecCachedWrapper(uint N, ulong cache1 = 1, ulong cache2 = 1, uint target = 3) => FibonacciRecCached(N, BigInteger.One, BigInteger.One, 3);

        public static BigInteger FibonacciRecCached(uint N, BigInteger cache1, BigInteger cache2, uint target)
        {
            BigInteger res = BigInteger.Zero;
            depth++;

            if (depth > depthLimit)
                throw new InvalidOperationException("Recursion is too depth");

            if (N == 0) res = 0;
            else if (N < 3) res = 1;
            if (N > target) res= FibonacciRecCached(N, cache2, cache1 + cache2, target + 1);
            else res =  cache1 + cache2;

            depth--;

            return res;
        }

        public static BigInteger FibonacciIter(uint N)
        {
            var arTmp = new byte[1024*1024*1024];
            arTmp[0] = 1;
            BigInteger res = new(arTmp), cache = 1, tmp = cache;
            for (uint i = 2; i < N; i++)
            {
                tmp = res;
                res += cache;
                cache = tmp;
            }
            return res;
        }

        public static BigInteger FibonacciGold(int N)
        {
            if (N == 0) return 0;
            else if (N < 3) return 1;

            double fi = (1.0 + Math.Sqrt(5.0)) / 2.0;
            return (BigInteger)Math.Floor(Math.Pow(fi, N) / Math.Sqrt(5.0) + 0.5);
        }

        public static ulong FibonacciMatr(int N)
        {
            if (N < 3) return 1;
            Matrix<ulong> init = new() { { 1, 1 }, { 1, 0 } };
            Matrix<ulong> res = AlgoPow.MatrixPowBinary(init, (uint)(N - 1));
            return res[0, 0];
        }
    }
    internal static class AlgoPrime
    {

        public static uint AmountOfPrimesSimple(uint N) => AmountOfPrimes(N, IsPrimeSimple);
        public static uint AmountOfPrimesOptimised1(uint N) => AmountOfPrimes(N, IsPrimeOptimised1);
        public static uint AmountOfPrimesOptimised2(uint N) => AmountOfPrimes(N, IsPrimeOptimised2);
        public static uint AmountOfPrimesOptimisedEr1(uint N)
        {
            EratosthenesSimple er = new();
            er.UpdateCache(N);
            return AmountOfPrimes(N, IsPrimeOptimisedEr1);
        }
        public static uint AmountOfPrimesOptimisedEr2(uint N)
        {
            EratosthenesMemoryOptimised er = new();
            er.UpdateCache(N);
            return AmountOfPrimes(N, IsPrimeOptimisedEr2);
        }
        public static uint AmountOfPrimesOptimisedEr3(uint N)
        {
            EratosthenesON er = new();
            er.UpdateCache(N);
            return AmountOfPrimes(N, IsPrimeOptimisedEr3);
        }
        public static uint AmountOfPrimesOptimisedEr4(uint N)
        {
            EratosthenesMemoryOptimisedON er = new();
            er.UpdateCache(N);
            return AmountOfPrimes(N, IsPrimeOptimisedEr3);
        }


        public static uint AmountOfPrimes(uint N, Func<uint, bool> method)
        {
            if (N < 4)
                return N - 1;

            uint res = 2;
            for (uint i = 5; i <= N; i++)
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
        public static bool IsPrimeOptimised1(uint N)
        {
            if (N < 4) return true;
            if (N % 2 == 0) return false;

            for (uint i = 3; i < N; i += 2)
            {
                if (N % i == 0)
                    return false;
            }
            return true;
        }

        public static bool IsPrimeOptimised2(uint N)
        {
            if (N < 4) return true;
            if (N % 2 == 0) return false;

            for (uint i = 3; i <= Math.Sqrt(N); i += 2)
            {
                if (N % i == 0)
                    return false;
            }
            return true;
        }

        public static bool IsPrimeOptimisedEr1(uint N)
        {
            if (N < 4) return true;
            if (N % 2 == 0) return false;

            EratosthenesSimple er = new();
            er.UpdateCache(N);

            return er.IsPrime(N);
            /*for (uint i = 3; i <= Math.Sqrt(N); i++)
            {
                if (er.IsPrime(i) && (N % i == 0))
                    return false;
            }
            return true;*/
        }
        public static bool IsPrimeOptimisedEr2(uint N)
        {
            if (N < 4) return true;

            EratosthenesMemoryOptimised er = new();
            er.UpdateCache(N);
            return er.IsPrime(N);

        }
        public static bool IsPrimeOptimisedEr3(uint N)
        {
            if (N < 4) return true;

            EratosthenesON er = new();
            er.UpdateCache(N);

            return er.IsPrime(N);
        }
        public static bool IsPrimeOptimisedEr4(uint N)
        {
            if (N < 4) return true;

            EratosthenesMemoryOptimisedON er = new();
            er.UpdateCache(N);

            return er.IsPrime(N);
        }

    }


    internal class Program
    {

        static void Main(string[] args)
        {
            Test test = new Test();

            Console.WriteLine("<---PowIter tests--->");
            //test.Run("Tests\\Power", AlgoMath.PowIter);
            Console.WriteLine("<---PowMul tests--->");
            test.Run("Tests\\Power", AlgoPow.PowMul);
            Console.WriteLine("<---PowBinary tests--->");
            test.Run("Tests\\Power", AlgoPow.PowBinary);
            Console.WriteLine("***********************");
            Console.WriteLine("<---IsPrimeSimple tests--->");
            //test.Run("Tests\\Primes", AlgoPrime.AmountOfPrimesSimple);
            Console.WriteLine("<---AmountOfPrimesOptimised1 tests--->");
            //test.Run("Tests\\Primes", AlgoPrime.AmountOfPrimesOptimised1);
            Console.WriteLine("<---AmountOfPrimesOptimised2 tests--->");
            //test.Run("Tests\\Primes", AlgoPrime.AmountOfPrimesOptimised2);
            Console.WriteLine("<---AmountOfPrimesOptimisedEr1 tests--->");
            //test.Run("Tests\\Primes", AlgoPrime.AmountOfPrimesOptimisedEr1);
            //Console.WriteLine("<---AmountOfPrimesOptimisedEr2 tests--->");
            //test.Run("Tests\\Primes", AlgoPrime.AmountOfPrimesOptimisedEr2);
            //Console.WriteLine("<---AmountOfPrimesOptimisedEr3 tests--->");
            //test.Run("Tests\\Primes", AlgoPrime.AmountOfPrimesOptimisedEr3);
            //Console.WriteLine("<---AmountOfPrimesOptimisedEr4 tests--->");
            //test.Run("Tests\\Primes", AlgoPrime.AmountOfPrimesOptimisedEr4);
            Console.WriteLine("***********************");
            Console.WriteLine("<---FibonacciIter tests--->");
            //test.Run("Tests\\Fibo", AlgoFibonacci.FibonacciIter);
            Console.WriteLine("<---FibonacciReq tests--->");
            //test.Run("Tests\\Fibo", AlgoFibonacci.FibonacciRec);
            Console.WriteLine("<---FibonacciReqCached tests--->");
            //test.Run("Tests\\Fibo", AlgoFibonacci.FibonacciRecCachedWrapper);
            Console.WriteLine("<---FibonacciGold tests--->");
            test.Run("Tests\\Fibo", AlgoFibonacci.FibonacciGold);
            Console.WriteLine("<---FibonacciMatr tests--->");
            test.Run("Tests\\Fibo", AlgoFibonacci.FibonacciMatr);
        }
    }
}
