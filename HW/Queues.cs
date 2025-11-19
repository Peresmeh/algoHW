using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoHW
{
    public interface IDynamicQueue<T> //One-dimension dynamic array interface
    {
        int Count { get; }
        int ReallocCount { get; }
        string Name { get; }

        void Enqueue(T item);
        T Dequeue();
    }

    public class SimpleDynamicQueue<T> : IDynamicQueue<T>
    {
        private VectorDynamicArray<T> array;
        private int head, tail, step;
        private const float fillPercent = 0.75f;

        public SimpleDynamicQueue(int capacityStep)
        {
            step = capacityStep;
            array = new VectorDynamicArray<T>(capacityStep);
            head = tail = 0;
        }

        public int Count => (tail - head + array.Count) % array.Count;

        public int ReallocCount => array.ReallocCount;

        public string Name => $"Очередь + {step}";

        public T Dequeue()
        {
            T ret = array[tail];
            tail = (tail + 1) % array.Count;
            if ((array.AllocSize - Count > step * (1.0f + (1.0f - fillPercent))) && (tail < head)) //Time ro free some memory
            {
                for (int i = 0; i < Count; i++)
                    array[i] = array[i + tail];
                head = Count;
                tail = 0;
                while (array.AllocSize - Count > step) array.Remove(head + 1);
            }

            return ret;
        }

        public void Enqueue(T item)
        {
            if (head == array.AllocSize)
            {
                if (Count < array.AllocSize * fillPercent)
                    head = 0;
            }

            if(head == array.Count)
                array.Add(item, head);
            else {
            }
            array[head] = item;
        }
    }
}
