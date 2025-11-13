using System;
using System.Collections;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace AlgoHW
{
    public class Matrix<T> : IEnumerable where T : unmanaged
    {
        private static readonly Func<T, T, T> addMethod;
        private static readonly Func<T, T, T> mulMethod;

        private int _height;
        public int Height => _height;

        private int _width;
        public int Width => _width;

        private T[,] buf;

        static Matrix()
        {
            try
            {
                ParameterExpression left = Expression.Parameter(typeof(T), "left");
                ParameterExpression right = Expression.Parameter(typeof(T), "right");
                addMethod = Expression.Lambda<Func<T, T, T>>(Expression.Add(left, right), left, right).Compile();
                mulMethod = Expression.Lambda<Func<T, T, T>>(Expression.Multiply(left, right), left, right).Compile();
            }
            catch { throw new InvalidCastException("Generic type  doesn't suppor opertors"); }
        }
        public Matrix(int width, int height)
        {
            _height = height;
            _width = width;
            buf = new T[height, width];
        }
        public Matrix()
        {
            _height = 0;
            _width = 0;
            buf = new T[0, 0];
        }

        public void Add(params T[] row)
        {
            _width = Math.Max(row.Length, buf.GetLength(0));
            _height++;
            var tmp = buf;
            buf = new T[_height, _width];
            for (int y = 0; y < _height - 1; y++)
                for (int x = 0; x < _width; x++)
                    buf[y, x] = tmp[y, x];
            for (int x = 0; x < _width; x++)
                buf[_height-1, x] = row[x];
        }

        public T this[int y, int x]
        {
            get => buf[y, x];
            set => buf[y, x] = value;
        }

        public static Matrix<T> operator +(Matrix<T> m1, Matrix<T> m2)
        {
            if ((m1 == null) || (m2 == null))
                throw new NullReferenceException("Argument must be intialised");
            if ((m1.Height != m2.Height) || (m1.Width != m2?.Width))
                throw new ArgumentException("Uncompatible matrix size");

            Matrix<T> ret = new(m1.Width, m1.Height);
            for (int y = 0; y < ret.Height; y++)
                for (int x = 0; x < ret.Width; x++)
                    ret.buf[x, y] = addMethod(m1[x, y], m2[x, y]);

            return ret;
        }
        public static Matrix<T> operator *(Matrix<T> m1, Matrix<T> m2)
        {
            if ((m1 == null) || (m2 == null))
                throw new NullReferenceException("Argument must be intialised");
            if (m1.Width != m2.Height)
                throw new ArgumentException("Uncompatible matrix size");

            Matrix<T> ret = new(m1.Width, m2.Height);
            for (int y = 0; y < ret.Height; y++)
                for (int x = 0; x < ret.Width; x++)
                {
                    for (int c = 0; c < m1.Width; c++)
                    {
                        var add = mulMethod(m1[c, y], m2[x, c]);
                        ret.buf[x, y] = addMethod(ret.buf[x, y], add);
                    }
                }
            return ret;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return buf.GetEnumerator();
        }
    }

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
        public static double PowBinary(double x, uint N)
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
            for (int i = 2; i < N; i++)
            {
                if (N % i == 0)
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

    internal class Solver
    {
        public string Run(string[] args)
        {
            Matrix<int> t1;
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
            Matrix<int> m=new(){ {1,2 },{3,4 } };
            Solver solver = new Solver();
            Test test = new Test(solver.Run);
            test.Run();
        }
    }
}
