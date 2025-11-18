using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoHW
{
    public interface IDynamicArray<T> //One-dimension dynamic array interface
    {
        int Count { get; }
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

        public SingleDynamicArray()
        {
            container = [];
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

        private int vectorStep;
        public VectorDynamicArray(int step = 100)
        {
            container = [];
            vectorStep = step;
            capacity = 0;
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

        private int factorStep;
        public FactorDynamicArray(int step = 2)
        {
            container = new T[1];
            factorStep = step;
            capacity = 0;
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
}