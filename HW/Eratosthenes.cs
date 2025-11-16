using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoHW
{
    public interface IEratosthenesCache
    {
        public void UpdateCache(uint N);
        public bool IsPrime(uint N);
    }

    public class EratosthenesSimple : IEratosthenesCache
    {
        private bool[] cache = Array.Empty<bool>();

        public void UpdateCache(uint N)
        {
            if (N > cache.Length)
            {
                cache = new bool[N];
                Array.Fill(cache, true);
                for (uint i = 2; i < N; i++)
                    for (uint j = i; j < N; j += i)
                        cache[j] = false;
            }
        }

        public bool IsPrime(uint N) => cache[N];
    }
    public class EratosthenesON : IEratosthenesCache
    {
        private bool[] cache = Array.Empty<bool>();

        public void UpdateCache(uint N)
        {
            if (N > cache.Length)
            {
                cache = new bool[N];
                Array.Fill(cache, false);
                for (uint i = 2; i < N; i++)
                {
                    if (!cache[i])
                    {
                        cache[i] = true;
                        for (uint j = i; j * i < N; j += i)
                            cache[j] = true;
                    }
                }
            }
        }

        public bool IsPrime(uint N) => (N == 1) || cache[N];
    }
    public class EratosthenesMemoryOptimised : IEratosthenesCache
    {
        private uint[] cache = Array.Empty<uint>();

        public void UpdateCache(uint N)
        {
            uint limit = N / (sizeof(uint) * 8) + 1;
            if (limit > cache.Length)
            {
                cache = new uint[limit];
                Array.Fill(cache, 0xffffffff);
                for (uint i = 3; i < limit*sizeof(uint)*8*2; i+=2)
                    for (uint j = i; j < N; j += i)
                    {
                        uint offset = i / (sizeof(uint) * 8) / 2;  //Devide by tow because of skipping even values
                        uint mask = 1u << (int)(i % (sizeof(uint) * 8 * 2));
                        cache[offset] &= ~mask;
                    }
            }
        }

        public bool IsPrime(uint N)
        {
            if (N % 2 == 0) return N == 2;
            else
            {
                uint offset = N / (sizeof(uint) * 8) / 2;  //Devide by tow because of skipping even values
                uint mask = 1u << (int)(N % (sizeof(uint) * 8 * 2));
                return (cache[offset] & mask) != 0;
            }
        }
    }
}
