using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AlgoHW
{
    public class Matrix<T> : IEnumerable 
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
                buf[_height - 1, x] = row[x];
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
}
