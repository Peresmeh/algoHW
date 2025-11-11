using System;

namespace AlgoHW
{
    internal static class AlgoMath
    {
        public static double Pow(double x, uint N)
        {
            double res = 1;
            for (int i = 0; i < N; i++)
                res *= x;
            return res;
        }
        public static double PowMul(double x, uint N)
        {
            if (N == 0) return 1;
            double tmp = PowMul(x, N / 2);
            if (N % 2 == 0)
                return tmp * tmp;
            else
                return tmp * tmp * x;
        }

        public static ulong FibonachiReq(uint N)
        {
            if (N < 3) return 1;
            else return FibonachiReq(N - 1) + FibonachiReq(N - 2);
        }

        public static ulong FibonachiReqCached(uint N, ulong cache1 = 1, ulong cache2 = 1, uint target = 3)
        {
            if (N < 3) return 1;
            if (N > target) return FibonachiReqCached(N, cache2, cache1 + cache2, target + 1);
            else return cache1 + cache2;
        }

        public static ulong FibonachiIter(uint N)
        {
            ulong res = 1, cache = 1, tmp = cache;
            for(uint i=2; i < N;i++)
            {
                tmp = res;
                res += cache;
                cache = tmp;
            }
            return res;
        }

        public static uint AmountOfPrime(uint N, Func<uint, bool> method)
        {


        }

        public static bool IsPrimeSimple()
    }

    internal class Solver
    {
        public string Run(string[] args)
        {
            int n = int.Parse(args[0]);
            return Tickets(n).ToString();
        }

        private void DPReset(long[][] dp)
        {
            for (int i = 0; i < dp.Length; i++)
                Array.Fill<long>(dp[i], 0);
        }

        private long Tickets(int N)
        {
            //Let's make some dynamic staff
            long[][] dp = new long[10][]; //dp array
            long[] dpSum = new long[9 * N + 1]; //array of sum

            //Some init staff
            int sumLen = 1;
            Array.Fill<long>(dpSum, 0);
            dpSum[0] = 1;
            for (int i = 0; i < dp.Length; i++)
                dp[i] = new long[9 * N + 1];

            for (int res = 0; res < N; res++) //Iterating N
            {
                DPReset(dp);
                for (int i = 0; i < dp.Length; i++)  //Build main diagonal in dp
                {
                    Array.Copy(dpSum, 0, dp[i], i, sumLen);
                }

                Array.Fill<long>(dpSum, 0);
                sumLen = 9 * (res + 1) + 1; //extending sum length
                for (int i = 0; i < sumLen; i++) //iterating dp columns
                    for (int j = 0; j < dp.Length; j++) //iterating dp rows
                        dpSum[i] += dp[j][i];
            }

            //Now we've got sum of N. Let's calculate sum of squares 
            long result = 0;
            for (int i = 0; i < sumLen; i++)
                result += dpSum[i] * dpSum[i];

            return result;
        }
    }

    internal class Program
    {

        static void Main(string[] args)
        {
            Solver solver = new Solver();
            Test test = new Test(solver.Run);
            test.Run();
        }
    }
}
