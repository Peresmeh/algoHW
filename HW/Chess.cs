using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    }

    public abstract class Chess : IChess
    {
        public abstract string Name { get; }
        protected CheckStruct[] checks;
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
    }

    public class ChessKing : Chess
    {
        public override string Name => "Король";
        public ChessKing() : base(
            [new(8, 0xffffffffffffffff),
                new(7, 0xfefefefefefefefe),
                new(-1, 0xfefefefefefefefe),
                new(-9, 0xfefefefefefefefe),
                new(-8, 0xffffffffffffffff),
                new(-7, 0x7f7f7f7f7f7f7f7f),
                new(1, 0x7f7f7f7f7f7f7f7f),
                new(9, 0x7f7f7f7f7f7f7f7f) ])
        { }

    }
}
