using AlgoHW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoHW
{
    public abstract class SorterBase<T> where T : IComparable
    {
        protected T[] array;
        protected ulong changes, compares;

        public abstract string Name { get; }

        [Metric("Compares")]
        public ulong Compares => compares;

        [Metric("Changes")]
        public ulong Changes => changes;

        protected abstract void PerformSorting();
        [MetricTest("*", true)]
        public void Sort(T[] array)
        {
            changes = compares = 0;
            this.array = array;
            PerformSorting();
            this.array = null;
        }

        protected void Swap(int a, int b)
        {
            T c = array[a];
            array[a] = array[b];
            array[b] = c;
            changes += 3;
        }

        protected bool MoreOff(int a, int b)
        {
            compares++;
            return array[a].CompareTo(array[b]) > 0;
        }
        protected bool More(T a, T b)
        {
            compares++;
            return a.CompareTo(b) > 0;
        }
        protected bool MoreEqualOff(int a, int b)
        {
            compares++;
            return array[a].CompareTo(array[b]) >= 0;
        }
        protected bool MoreEqual(T a, T b)
        {
            compares++;
            return a.CompareTo(b) >= 0;
        }
    }

    public class BubbleSort<T> : SorterBase<T> where T : IComparable
    {
        public override string Name => "Bubble";
        protected override void PerformSorting()
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                for (int j = 0; j < i; j++)
                    if (MoreOff(j, j + 1))
                        Swap(j, j + 1);

                Test.SetChecker(array.Length - i, array.Length);
            }
        }
    }
    public class BubbleSortOptimised<T> : SorterBase<T> where T : IComparable
    {
        public override string Name => "Optimised Bubble";
        protected override void PerformSorting()
        {
            int lastSwap;
            for (int i = array.Length - 1; i > 0; i--)
            {
                lastSwap = 0;
                for (int j = 0; j < i; j++)
                    if (MoreOff(j, j + 1))
                    {
                        Swap(j, j + 1);
                        lastSwap = j;
                    }

                i = lastSwap + 1;
                Test.SetChecker(array.Length - i, array.Length);
            }
        }
    }
    public class InsertSort<T> : SorterBase<T> where T : IComparable
    {
        public override string Name => "Insert";

        protected override void PerformSorting()
        {
            for (int i = 1; i < array.Length; i++)
            {
                for (int j = i; j > 0; j--)
                    if (MoreOff(j - 1, j))
                        Swap(j - 1, j);

                Test.SetChecker(array.Length - i, array.Length);
            }
        }
    }

    public class InsertSortShift<T> : SorterBase<T> where T : IComparable
    {
        public override string Name => "Insert with shift";
        protected override void PerformSorting()
        {
            T tmp;
            int j;
            for (int i = 1; i < array.Length; i++)
            {
                tmp = array[i];
                for (j = i - 1; j >= 0 && More(array[j], tmp); j--)
                {
                    array[j + 1] = array[j];
                    changes++;
                }
                array[j + 1] = tmp;
                changes++;

                Test.SetChecker(array.Length - i, array.Length);
            }
        }
    }
    public class InsertSortBisection<T> : SorterBase<T> where T : IComparable
    {
        public override string Name => "Insert with bisection";
        private int Bisection(T key, int l, int r)
        {
            if (r <= l)
            {
                if (MoreEqual(key, array[l]))
                    return l + 1;
                else
                    return l;
            }

            int m = (l + r) / 2;
            if (More(key, array[m]))
                return Bisection(key, m + 1, r);
            else
                return Bisection(key, l, m - 1);
        }

        protected override void PerformSorting()
        {
            T tmp;
            int j;
            for (int i = 1; i < array.Length; i++)
            {
                tmp = array[i];
                int bs = Bisection(tmp, 0, i - 1);
                for (j = i - 1; j >= bs; j--)
                {
                    array[j + 1] = array[j];
                    changes++;
                }
                array[j + 1] = tmp;
                changes++;

                Test.SetChecker(array.Length - i, array.Length);
            }
        }
    }

    public class ShellSort<T> : SorterBase<T> where T : IComparable
    {
        public override string Name => string.IsNullOrEmpty(PredicateName) ? "Shell" : $"Shell with {PredicateName} predicate";

        public string PredicateName { get; set; } = "";

        Func<int, int, int> gapPredicate;
        public ShellSort(Func<int, int, int> predicate)
        {
            gapPredicate = predicate;
        }

        protected override void PerformSorting()
        {
            int gap = gapPredicate(0, array.Length);
            for (int i = 0; gap > 0; i++)
            {
                gap = gapPredicate(i, array.Length);

                for (int j = gap; j < array.Length; j++)
                {
                    for (int k = j; k >= gap && MoreOff(k - gap, k); k -= gap)
                        Swap(k - gap, k);
                    Test.SetChecker(array.Length - i, array.Length);
                }

                Test.SetChecker(array.Length - i, array.Length);
                if (gap == 1) break;
            }
        }
    }
}
