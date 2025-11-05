using System;

namespace AlgoHW
{
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
