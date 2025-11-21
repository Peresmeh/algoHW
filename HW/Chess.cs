using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoHW
{

    public class ChessKing : Chess
    {
        public override string Name => "Король";
        public ChessKing() : base([])
        {
            checks = [new(8, Unmasked),
                new(7, MaskColumns(1)),
                new(-1, MaskColumns(1)),
                new(-9, MaskColumns(1)),
                new(-8, Unmasked),
                new(-7, MaskColumns(0x80)),
                new(1, MaskColumns(0x80)),
                new(9, MaskColumns(0x80)) ];
        }

    }
    public class ChessKnight : Chess
    {
        public override string Name => "Конь";
        public ChessKnight() : base([])
        {
            checks =
            [new(15, MaskColumns(1)),
                new(6, MaskColumns(3)),
                new(-10, MaskColumns(3)),
                new(-17, MaskColumns(1)),
                new(-15, MaskColumns(0x80)),
                new(-6, MaskColumns(0xc0)),
                new(10, MaskColumns(0xc0)),
                new(17, MaskColumns(0x80)) ];
        }
    }
    public class ChessRook : Chess
    {
        public override string Name => "Ладья";
        public ChessRook() : base(new CheckStruct[7 * 4])
        {
            for (sbyte i = 0; i < 7; i++)
            {
                //checktop
                checks[i] = new((sbyte)(8 * (i + 1)), Unmasked);
                //checkbottom
                checks[i + 7] = new((sbyte)(-8 * (i + 1)), Unmasked);
                //checkleft
                checks[i + 7 * 2] = new((sbyte)(i + 1), MaskColumns((byte)(0xff << (7 - i))));
                //checkright
                checks[i + 7 * 3] = new((sbyte)-(i + 1), MaskColumns((byte)(0xff >> (7 - i))));
            }
        }
    }
    public class ChessBishop : Chess
    {
        public override string Name => "Слон";
        public ChessBishop() : base(new CheckStruct[7 * 4])
        {
            for (sbyte i = 0; i < 7; i++)
            {
                //checktopleft
                checks[i] = new((sbyte)(8 * (i + 1) + i + 1), MaskColumns((byte)(0xff << (7 - i))));
                //checktopright
                checks[i + 7] = new((sbyte)(8 * (i + 1) - i - 1), MaskColumns((byte)(0xff >> (7 - i))));
                //checkbottomleft
                checks[i + 7 * 2] = new((sbyte)(-8 * (i + 1) + i + 1), MaskColumns((byte)(0xff << (7 - i))));
                //checkbottomright
                checks[i + 7 * 3] = new((sbyte)(-8 * (i + 1) - i - 1), MaskColumns((byte)(0xff >> (7 - i))));
            }
        }
    }
    public class ChessQueen : Chess
    {
        public override string Name => "Ферзь";
        public ChessQueen() : base(new CheckStruct[7 * 8])
        {
            for (sbyte i = 0; i < 7; i++)
            {
                //checktop
                checks[i] = new((sbyte)(8 * (i + 1)), Unmasked);
                //checkbottom
                checks[i + 7] = new((sbyte)(-8 * (i + 1)), Unmasked);
                //checkleft
                checks[i + 7 * 2] = new((sbyte)(i + 1), MaskColumns((byte)(0xff << (7 - i))));
                //checkright
                checks[i + 7 * 3] = new((sbyte)-(i + 1), MaskColumns((byte)(0xff >> (7 - i))));
                //checktopleft
                checks[i + 7 * 4] = new((sbyte)(8 * (i + 1) + i + 1), MaskColumns((byte)(0xff << (7 - i))));
                //checktopright
                checks[i + 7 * 5] = new((sbyte)(8 * (i + 1) - i - 1), MaskColumns((byte)(0xff >> (7 - i))));
                //checkbottomleft
                checks[i + 7 * 6] = new((sbyte)(-8 * (i + 1) + i + 1), MaskColumns((byte)(0xff << (7 - i))));
                //checkbottomright
                checks[i + 7 * 7] = new((sbyte)(-8 * (i + 1) - i - 1), MaskColumns((byte)(0xff >> (7 - i))));
            }
        }
    }
}
