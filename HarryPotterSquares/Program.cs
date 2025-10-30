
static void DrawSpelledSqure(int Size, Func<int, int, bool> Spell)
{
    for (int y = 0; y < Size; y++)
    {
        for (int x = 0; x < Size; x++)
        {
            if (Spell(x, y)) Console.Write('#');
            else Console.Write('.');
        }
        Console.WriteLine();
    }
}

const int N = 25;
Console.WriteLine("Let's spell some squres, Harry!");
Console.WriteLine("01");
DrawSpelledSqure(N, (x, y) => x > y);
Console.WriteLine("02");
DrawSpelledSqure(N, (x, y) => x == y);
Console.WriteLine("03");
DrawSpelledSqure(N, (x, y) => x == N - y - 1);
Console.WriteLine("04");
DrawSpelledSqure(N, (x, y) => x < N - y + 5);
Console.WriteLine("05");
DrawSpelledSqure(N, (x, y) => y == x / 2);
Console.WriteLine("06");
DrawSpelledSqure(N, (x, y) => (x < 10) || (y < 10));
Console.WriteLine("07");
DrawSpelledSqure(N, (x, y) => (x > 15) && (y > 15));
Console.WriteLine("08");
DrawSpelledSqure(N, (x, y) => (x == 0) || (y == 0));
Console.WriteLine("09");
DrawSpelledSqure(N, (x, y) => (x + 5) < (y - 5) || (x - 5) > (y + 5));
Console.WriteLine("10");
DrawSpelledSqure(N, (x, y) => x / (y + 1) == 1);
Console.WriteLine("11");
DrawSpelledSqure(N, (x, y) => ((x - 1) % (N - 3)) == 0 || ((y - 1) % (N - 3) == 0));
Console.WriteLine("12");
DrawSpelledSqure(N, (x, y) => x * x + y * y <= 20 * 20);
Console.WriteLine("13");
DrawSpelledSqure(N, (x, y) => (N - x - y + 12) / 9 == 1);
Console.WriteLine("14");
DrawSpelledSqure(N, (x, y) => x * y <= 100);
Console.WriteLine("15");
DrawSpelledSqure(N, (x, y) => (Math.Abs(x - y) - 1) / 10 == 1);
Console.WriteLine("16");
DrawSpelledSqure(N, (x, y) => (x + y > 14) && (x - y < 10) && (x + y < N + 9) && (x - y > 15 - N));
Console.WriteLine("17");
DrawSpelledSqure(N, (x, y) => y >= N - 9 + 8 * Math.Sin(x / Math.PI));
Console.WriteLine("18");
DrawSpelledSqure(N, (x, y) => (x > 0) && (y < 2) || (x < 2) && (y > 0));
Console.WriteLine("19");
DrawSpelledSqure(N, (x, y) => (x % (N - 1)) == 0 || (y % (N - 1) == 0));
Console.WriteLine("20");
DrawSpelledSqure(N, (x, y) => (x + y) % 2 == 0);
Console.WriteLine("21");
DrawSpelledSqure(N, (x, y) => x % (y + 1) == 0);
Console.WriteLine("22");
DrawSpelledSqure(N, (x, y) => (x + y * N) % 3 == 0);
Console.WriteLine("23");
DrawSpelledSqure(N, (x, y) => (x % 2) == 0 && (y % 3 == 0));
Console.WriteLine("24");
DrawSpelledSqure(N, (x, y) => (x == y) || (x == N - y - 1));
Console.WriteLine("25");
DrawSpelledSqure(N, (x, y) => (x % 6) == 0 || (y % 6 == 0));
