using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoHW
{

    public interface IDynamicQueue<T> : IDynamicMemoryStructure //One-dimension dynamic array interface
    {
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

        public int Count => head >= tail ? head - tail : (head - tail + array.AllocSize) % array.AllocSize;

        public int ReallocCount => array.ReallocCount;

        public string Name => $"Очередь + {step}";

        public int AllocSize => array.AllocSize;

        public T Dequeue()
        {
            T ret = array[tail];
            if (tail == head) throw new InvalidOperationException("Queue is empty");
            tail = (tail + 1) % array.AllocSize;
            if ((array.AllocSize - Count > step * (1.0f + (1.0f - fillPercent))) && (tail < head)) //Time ro free some memory
            {
                for (int i = 0; i < Count; i++)
                    array[i] = array[i + tail];
                head = Count;
                tail = 0;
                while (array.AllocSize - Count > step) array.Remove(head);
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

            if (head == array.Count)
                array.Add(item, head);
            else
                array[head] = item;
            head++;
        }
    }


    public struct KVPair<keyT, keyV>
    {
        private keyT key;
        private keyV value;

        public keyT Key => key;
        public keyV Value => value;

        public KVPair(keyT key, keyV value)
        {
            this.key = key;
            this.value = value;
        }
    }

    public class PriorityQueue<T> : IDynamicQueue<T>
    {
        private int coontainerStep, queueAllocStep;
        private VectorDynamicArray<KVPair<int, SimpleDynamicQueue<T>>> array;
        public string Name => $"Очередь с приритетом (+{coontainerStep})*(+{queueAllocStep})";

        public int Count
        {
            get
            {
                int res = 0;
                for (int i = 0; i < array.Count; i++)
                    res += array[i].Value.Count;
                return res;
            }
        }

        public int ReallocCount
        {
            get
            {
                int res = array.ReallocCount;
                for (int i = 0; i < array.Count; i++)
                    res += array[i].Value.ReallocCount;
                return res;
            }
        }

        public int AllocSize
        {
            get
            {
                int res = array.AllocSize;
                for (int i = 0; i < array.Count; i++)
                    res += array[i].Value.AllocSize;
                return res;
            }
        }

        public PriorityQueue(int coontainerStep, int queueAllocStep)
        {
            array = new(coontainerStep);
            this.coontainerStep = coontainerStep;
            this.queueAllocStep = queueAllocStep;
        }

        public T Dequeue()
        {
            if (Count == 0) throw new InvalidOperationException("Queue is empty");

            T ret = array[0].Value.Dequeue();
            if (array[0].Value.Count == 0)
                array.Remove(0);
            return ret;
        }

        private void Sort()
        {
            KVPair<int, SimpleDynamicQueue<T>> tmp;
            bool opt;

            for(int i = 0;i < array.Count;i++)
            {
                opt = false;
                for (int j = 0; j < array.Count - 1 - i; j++)
                {
                    if (array[j + 1].Key < array[j].Key)
                    {
                        tmp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = tmp;
                        opt = true;
                    }
                }
                if(!opt) break;
            }
        }


        private int FindPriorityIndex(int priority)        
        { 
            for(int i = 0; i < array.Count;i++)
                if (array[i].Key == priority) return i;

            return -1;
        }

        public void Enqueue(T item)
        {
            if (array.Count == 0)
                array.Add(new KVPair<int, SimpleDynamicQueue<T>>(0, new(queueAllocStep)), 0);
            array[0].Value.Enqueue(item);
        }
        public void Enqueue(T item, int priority)
        {
            KVPair<int, SimpleDynamicQueue<T>> tmp;
            int index = FindPriorityIndex(priority);
            if (index == -1)
            {
                tmp = new KVPair<int, SimpleDynamicQueue<T>>(priority, new(queueAllocStep));
                array.Add(tmp, 0);
                Sort();
            }
            else tmp = array[index];

            tmp.Value.Enqueue(item);
        }
    }
}
