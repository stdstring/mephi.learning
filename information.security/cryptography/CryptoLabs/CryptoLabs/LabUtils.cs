namespace CryptoLabs
{
    internal static class LabUtils
    {
        public static (Int64 gcd, Int64 x, Int64 y) CalcExtendedEuclideanAlgo(Int64 a, Int64 b)
        {
            // ax + by = GCD(a, b)
            if (a < b)
            {
                (Int64 gcd, Int64 x, Int64 y) result = CalcExtendedEuclideanAlgo(b, a);
                return (result.gcd, x: result.y, y: result.x);
            }
            Int64 rPrev = a;
            Int64 sPrev = 1;
            Int64 tPrev = 0;
            Int64 rCurrent = b;
            Int64 sCurrent = 0;
            Int64 tCurrent = 1;
            do
            {
                Int64 qCurrent = rPrev / rCurrent;
                Int64 rNext = rPrev - qCurrent * rCurrent;
                Int64 sNext = sPrev - qCurrent * sCurrent;
                Int64 tNext = tPrev - qCurrent * tCurrent;
                rPrev = rCurrent;
                sPrev = sCurrent;
                tPrev = tCurrent;
                rCurrent = rNext;
                sCurrent = sNext;
                tCurrent = tNext;
            } while (rCurrent != 0);
            return (gcd: rPrev, x: sPrev, y: tPrev);
        }

        public static Int64 FastPower(Int64 a, Int64 q, Int64 p)
        {
            if (a < 0)
                throw new ArgumentOutOfRangeException(nameof(a));
            if (q < 0)
                throw new ArgumentOutOfRangeException(nameof(q));
            if (p < 0)
                throw new ArgumentOutOfRangeException(nameof(p));
            // (a^q) mod p
            Int64 result = 1;
            Int64 current = a;
            for (Int64 mask = 1; mask <= q; mask <<= 1)
            {
                if ((mask & q) != 0)
                    result = (result * current) % p;
                current = (current * current) % p;
            }
            return result;
        }

        public static UInt32 Count1(Byte number)
        {
            UInt32 result = 0;
            for (Int32 mask = 1; mask <= number; mask <<= 1)
            {
                if ((mask & number) != 0)
                    ++result;
            }
            return result;
        }
    }
}
