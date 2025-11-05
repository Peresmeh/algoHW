using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoHW
{
    internal class Test
    {
        private Func<string[], string> run;
        public Test(Func<string[], string> run) => this.run = run;
        public void Run()
        {
            int iter = 0;
            while (true)
            {
                string fileIn = $"Tests\\test.{iter}.in";
                string fileOut = $"Tests\\test.{iter}.out";
                if (!File.Exists(fileIn) || !File.Exists(fileOut))
                    return;
                string[] input = File.ReadAllLines(fileIn);
                string[] output = File.ReadAllLines(fileOut);
                string x = run(input);
                if (x == output[0])
                {
                    Console.WriteLine("Тест " + iter + " OK: " + x);
                }
                else
                {
                    Console.WriteLine("Тест " + iter + " ошибка: " + x +
                                      " ожидалось: " + output[0]);
                }
                iter++;
            }
        }
    }
}
