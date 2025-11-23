using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoHW
{
    public abstract class SorterBase
    {
        [Metric("Equals")]
        public int Equalities { get; }
        [Metric("Actions")]
        public int Actions { get; }
    }


}
