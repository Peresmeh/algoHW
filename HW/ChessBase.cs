using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoHW
{
    public interface IChess
    {
        public string Name { get; }
        ulong GetPositionMask(int position);
    }

    public struct CheckStruct
    {
        private sbyte check;
        private ulong mask;
        public CheckStruct(sbyte check, ulong mask)
        {
            this.check = check;
            this.mask = mask;
        }
        public sbyte Check => check;
        public ulong Mask => mask;

        public override string ToString()
        {
            return $"{check}: {mask}";
        }
    }

    public abstract class Chess : IChess
    {
        public abstract string Name { get; }
        protected CheckStruct[] checks;
        protected const ulong Unmasked = 0xffffffffffffffff;
        protected Chess(CheckStruct[] checks)
        {
            this.checks = checks;
        }

        public ulong GetPositionMask(int position)
        {
            ulong positionMask = 1ul << position;
            ulong mask = 0;
            for (int i = 0; i < checks.Length; i++)
            {
                mask |= checks[i].Check > 0 ?
                    (positionMask & checks[i].Mask) << checks[i].Check :
                    (positionMask & checks[i].Mask) >> (-checks[i].Check);
            }
            return mask;
        }

        protected ulong MaskRows(byte rowMask)
        {
            ulong retMask = 0xffffffffffffffff;
            for (byte i = 0; i < 8; i++)
            {
                if ((rowMask & (1 << i)) != 0)
                    retMask &= ~(0xfful << (8 * i));
            }
            return retMask;
        }
        protected ulong MaskColumns(byte colMask)
        {
            ulong retMask = Unmasked;
            for (byte i = 0; i < 8; i++)
            {
                if ((colMask & (1 << i)) != 0)
                    retMask &= ~(0x0101010101010101ul << i);
            }
            return retMask;
        }
    }
}
