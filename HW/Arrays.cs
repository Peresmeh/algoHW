using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoHW
{
    public interface IDynamicArray<T> //One-dimension dynamic array interface
    {
        int Count { get; }
        int AllocSize { get; }
        T this[int index] { get; set; }
        int ReallocCount { get; }
        string Name { get; }

        void Add(T item, int index);
        T Remove(int index);
    }

    public class SingleDynamicArray<T> : IDynamicArray<T>
    {
        private T[] container;
        public T this[int index] { get => container[index]; set => container[index] = value; }

        private int realocCount;
        public int ReallocCount => realocCount;

        public string Name => "Динамический массив + 1";

        public int Count => container.Length;
        public int AllocSize => container.Length;

        public SingleDynamicArray(int initialSize = 0)
        {
            container = new T[initialSize];
        }

        public void Add(T item, int index)
        {
            if (index < 0 || index > Count) throw new ArgumentOutOfRangeException("Index must be between 0 and Count");
            var tmp = new T[Count + 1];
            realocCount++;
            Array.Copy(container, 0, tmp, 0, index);
            tmp[index] = item;
            Array.Copy(container, index, tmp, index + 1, Count - index);
            container = tmp;
        }

        public T Remove(int index)
        {
            if (index < 0 || index > Count) throw new ArgumentOutOfRangeException("Index must be between 0 and Count - 1");
            T ret = container[index];
            var tmp = new T[Count - 1];
            realocCount++;
            Array.Copy(container, 0, tmp, 0, index);
            Array.Copy(container, index + 1, tmp, index, Count - index - 1);
            container = tmp;
            return ret;
        }
    }
    public class VectorDynamicArray<T> : IDynamicArray<T>
    {
        private T[] container;
        public T this[int index] { get => container[index]; set => container[index] = value; }

        private int realocCount;
        public int ReallocCount => realocCount;

        public string Name => $"Динамический массив + {vectorStep}";

        private int capacity;
        public int Count => capacity;
        public int AllocSize => container.Length;

        private int vectorStep;
        public VectorDynamicArray(int step = 100, int initialSize = 0)
        {
            int targetSize;
            if (initialSize == 0) targetSize = 0;
            else targetSize = initialSize - (initialSize % step) + step;

            container = new T[initialSize];
            vectorStep = step;
            capacity = initialSize;
        }

        public void Add(T item, int index)
        {
            if (index < 0 || index > Count) throw new ArgumentOutOfRangeException("Index must be between 0 and Count");
            T[] tmp;
            if (capacity == container.Length)
            {
                tmp = new T[Count + vectorStep];
                realocCount++;
            }
            else tmp = container;
            Array.Copy(container, 0, tmp, 0, index);
            tmp[index] = item;
            Array.Copy(container, index, tmp, index + 1, Count - index);
            container = tmp;
            capacity++;
        }

        public T Remove(int index)
        {
            if (index < 0 || index > Count) throw new ArgumentOutOfRangeException("Index must be between 0 and Count - 1");
            T ret = container[index];
            T[] tmp;
            if (container.Length - capacity - 1 == vectorStep)
            {
                tmp = new T[Count - 1];
                realocCount++;
            }
            else tmp = container;
            Array.Copy(container, 0, tmp, 0, index);
            Array.Copy(container, index + 1, tmp, index, Count - index - 1);
            container = tmp;
            capacity--;
            return ret;
        }
    }
    public class FactorDynamicArray<T> : IDynamicArray<T>
    {
        private T[] container;
        public T this[int index] { get => container[index]; set => container[index] = value; }

        private int realocCount;
        public int ReallocCount => realocCount;

        public string Name => $"Динамический массив * {factorStep}";

        private int capacity;
        public int Count => capacity;
        public int AllocSize => container.Length;

        private int factorStep;
        public FactorDynamicArray(int step = 2, int initialSize = 0)
        {
            int targetSize;
            if (initialSize == 0) targetSize = 1;
            else targetSize = initialSize;
                container = new T[targetSize];
            factorStep = step;
            capacity = initialSize;
        }

        public void Add(T item, int index)
        {
            if (index < 0 || index > Count) throw new ArgumentOutOfRangeException("Index must be between 0 and Count");
            T[] tmp;
            if (capacity == container.Length)
            {
                tmp = new T[Count * factorStep];
                realocCount++;
            }
            else tmp = container;
            Array.Copy(container, 0, tmp, 0, index);
            tmp[index] = item;
            Array.Copy(container, index, tmp, index + 1, Count - index);
            container = tmp;
            capacity++;
        }

        public T Remove(int index)
        {
            if (index < 0 || index > Count) throw new ArgumentOutOfRangeException("Index must be between 0 and Count - 1");
            T ret = container[index];
            T[] tmp;
            if ((container.Length - capacity - 1) % factorStep == 0)
            {
                tmp = new T[Count / factorStep];
                realocCount++;
            }
            else tmp = container;
            Array.Copy(container, 0, tmp, 0, index);
            Array.Copy(container, index + 1, tmp, index, Count - index - 1);
            container = tmp;
            capacity--;
            return ret;
        }
    }

    public class MatrixDynamicArray<T> : IDynamicArray<T>
    {
        private VectorDynamicArray<VectorDynamicArray<T>> container;
        public T this[int index] { get => container[index / colCapacity][index % colCapacity]; set => container[index / colCapacity][index % colCapacity] = value; }

        public int ReallocCount
        {
            get
            {
                int res = container.ReallocCount;
                for (int i = 0; i < container.Count; i++) res += container[i].ReallocCount;
                return res;
            }
        }

        public string Name => $"Динамический массив  {rowStep}*{colCapacity}";

        public int Count
        {
            get
            {
                int res = 0;
                for (int i = 0; i < container.Count; i++) res += container[i].Count;
                return res;
            }
        }
        public int AllocSize => container.Count * colCapacity;

        private int rowStep, colCapacity;
        public MatrixDynamicArray(int rowStep = 10, int colCapacity = 100)
        {
            this.rowStep = rowStep;
            this.colCapacity = colCapacity;
            container = new(rowStep);
        }

        private void ShiftRight(VectorDynamicArray<T> dest, int startIndex, int count)
        {
            if (dest.Count < startIndex + count)
                dest.Add(dest[0], dest.Count);

            for (int i = 0; i < count; i++)
                dest[startIndex + count - i] = dest[startIndex + count - i - 1];
        }
        private void ShiftLeft(VectorDynamicArray<T> dest, int startIndex, int count)
        {
            for (int i = 0; i < count; i++)
                dest[startIndex + count + i - 1] = dest[startIndex + count + i];
        }

        public void Add(T item, int index)
        {
            if (index < 0 || index > Count) throw new ArgumentOutOfRangeException("Index must be between 0 and Count");
            int row = index / colCapacity;
            int col = index % colCapacity;

            if (Count%colCapacity == 0)
                container.Add(new(colCapacity), container.Count);

            for (int tmpRow = container.Count - 1; tmpRow > row; tmpRow--)
            {
                ShiftRight(container[tmpRow], 0, Math.Min(container[tmpRow].Count, colCapacity - 1));
                container[tmpRow][0] = container[tmpRow - 1][container[tmpRow - 1].Count - 1];
            }
            if (container[row].Count == colCapacity)
            {
                ShiftRight(container[row], col, container[row].Count - col - 1);
                container[row][col] = item;
            }
            else container[row].Add(item, col);
        }

        public T Remove(int index)
        {
            int row = index / colCapacity;
            int col = index % colCapacity;
            T ret = container[row][col];

            if (index < 0 || index > Count) throw new ArgumentOutOfRangeException("Index must be between 0 and Count - 1");

            ShiftLeft(container[row], col + 1, container[row].Count - col - 1);

            for (int tmpRow = row + 1; tmpRow < container.Count; tmpRow--)
            {
                container[tmpRow - 1][colCapacity - 1] = container[tmpRow + 1][0];
                if (container[tmpRow].Count == 1)
                    container[tmpRow].Remove(0);
                else
                    ShiftLeft(container[tmpRow], 1, container[tmpRow].Count - 1);
            }
            if (container[container.Count - 1].Count == 0)
                container.Remove(container.Count - 1);
            else
                container[container.Count - 1].Remove(container[container.Count - 1].Count - 1);


            return ret;
        }
    }
}
