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
        private static bool[] cache = Array.Empty<bool>();

        public void UpdateCache(uint N)
        {
            if (N > cache.Length)
            {
                cache = new bool[N];
                Array.Fill(cache, true);
                for (uint i = 2; i <= N; i++)
                //if (cache[i - 1])
                {
                    for (uint j = 2 * i; j <= N; j += i)
                        cache[j - 1] = false;
                }
            }
        }

        public bool IsPrime(uint N) => cache[N - 1];
    }
    public class EratosthenesON : IEratosthenesCache
    {
        private static bool[] cache = Array.Empty<bool>();

        public void UpdateCache(uint N)
        {
            if (N > cache.Length)
            {
                cache = new bool[N];
                Array.Fill(cache, true);
                for (uint i = 2; i <= N; i++)
                {
                    if (cache[i - 1])
                    {
                        for (ulong j = i * 2; j <= N; j += i)
                            cache[j - 1] = false;
                    }
                }
            }
        }

        public bool IsPrime(uint N) => (N == 1) || cache[N - 1];
    }
    public class EratosthenesMemoryOptimised : IEratosthenesCache
    {
        private static uint[] cache = Array.Empty<uint>();

        public void UpdateCache(uint N)
        {
            uint limit = N / (sizeof(uint) * 8 * 2) + 1;
            if (limit > cache.Length)
            {
                cache = new uint[limit];
                Array.Fill(cache, 0xffffffff);
                for (uint i = 3; i <= limit * sizeof(uint) * 8 * 2; i += 2)
                    for (uint j = 3 * i; j <= limit * sizeof(uint) * 8 * 2; j += 2 * i)
                    {
                        uint offset = j / (sizeof(uint) * 8) / 2;  //Devide by tow because of skipping even values
                        int maskOffset = (int)((j / 2) % (sizeof(uint) * 8));
                        uint mask = 1u << maskOffset;
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
                uint mask = 1u << (int)((N / 2) % (sizeof(uint) * 8));
                return (cache[offset] & mask) != 0;
            }
        }
    }
    public class EratosthenesMemoryOptimisedON : IEratosthenesCache
    {
        private static uint[] cache = Array.Empty<uint>();

        public void UpdateCache(uint N)
        {
            uint limit = N / (sizeof(uint) * 8 * 2) + 1;
            if (limit > cache.Length)
            {
                cache = new uint[limit];
                Array.Fill(cache, 0xffffffff);
                for (uint i = 3; i <= limit * sizeof(uint) * 8 * 2; i += 2)
                {
                    if(IsPrime(i))
                    for (uint j = 3 * i; j <= limit * sizeof(uint) * 8 * 2; j += 2 * i)
                    {
                        uint offset = j / (sizeof(uint) * 8) / 2;  //Devide by tow because of skipping even values
                        int maskOffset = (int)((j / 2) % (sizeof(uint) * 8));
                        uint mask = 1u << maskOffset;
                        cache[offset] &= ~mask;
                    }
                }
            }
        }

        public bool IsPrime(uint N)
        {
            if (N % 2 == 0) return N == 2;
            else
            {
                uint offset = N / (sizeof(uint) * 8) / 2;  //Devide by tow because of skipping even values
                uint mask = 1u << (int)((N / 2) % (sizeof(uint) * 8));
                return (cache[offset] & mask) != 0;
            }
        }
    }
}
