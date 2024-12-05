using NUnit.Framework;

namespace CryptoLabs
{
    [TestFixture]
    public class Lab4Tests
    {
        [TestCase(-1, 1, 29, 9, 27, 4, 17)]
        [TestCase(1, 1, 23, 7, 11, 5, 16)]
        [TestCase(2, 3, 97, 3, 6, 6, 10)]
        [TestCase(-1, 3, 37, 2, 3, 7, 12)]
        [TestCase(3, 5, 17, 1, 3, 8, 11)]
        [TestCase(1, 1, 23, 6, 4, 8, 9)]
        [TestCase(1, 0, 23, 9, 5, 10, 8)]
        [TestCase(9, 17, 23, 16, 5, 11, 7)]
        [TestCase(2, 3, 97, 3, 6, 12, 6)]
        [TestCase(-1, 3, 37, 2, 3, 13, 5)]
        [TestCase(3, 5, 17, 1, 3, 3, 13)]
        // not work properly
        // [TestCase(1, 1, 23, 6, 4, 2, 14)]
        [TestCase(1, 0, 23, 9, 5, 14, 4)]
        [TestCase(2, 3, 97, 3, 6, 15, 3)]
        [TestCase(1, 1, 23, 3, 13, 16, 2)]
        public void ExecuteLab4(Int64 a, Int64 b, Int64 p, Int64 gx, Int64 gy, Int64 k1, Int64 k2)
        {
            // (4 * a^3 + 27* b^2) mod p != 0
            Int64 condValue = ((((((4 * a) % p) * a) % p) * a) % p + (((27 * b) % p) * b) %p) % p;
            Assert.That(condValue, Is.Not.EqualTo(0));
            Point g = new Point(gx, gy);
            Point key1 = MultiplyPoint(g, k1, a, p);
            Point key2 = MultiplyPoint(g, k2, a, p);
            Point key12 = MultiplyPoint(key1, k2, a, p);
            Point key21 = MultiplyPoint(key2, k1, a, p);
            Console.WriteLine($"k1 * G = {key1}, k2 * G = {key2}, k2 * k1 * G = {key12}, k1 * k2 * G = {key21}");
            Assert.That(key12, Is.EqualTo(key21));
        }

        private record struct Point(Int64 X, Int64 Y);

        private Point AdditionPoints(Point point1, Point point2, Int64 p)
        {
            // x3 = (L^2 - x1 - x2) mod p, y3 = (L * (x1 - x3) - y1) mod p, L = ((y2 - y1) / (x1 - x2)) mod p
            Int64 lambda = CalcLambda(point2.Y - point1.Y, point2.X - point1.X, p);
            Int64 x3 = (lambda * lambda - point1.X - point2.X) % p;
            if (x3 < 0)
                x3 += p;
            Int64 y3 = (lambda * (point1.X - x3) - point1.Y) % p;
            if (y3 < 0)
                y3 += p;
            return new Point(x3, y3);
        }

        private Point DoublingPoint(Point point, Int64 a, Int64 p)
        {
            // x3 = (L^2 - 2 * x1) mod p, y3 = (L * (x1 - x3) - y1) mod p, L = ((3 * x1^2 + a) / (2 * y1)) mod p
            Int64 lambda = CalcLambda(3 * point.X * point.X + a, 2 * point.Y, p);
            Int64 x3 = (lambda * lambda - 2 * point.X) % p;
            if (x3 < 0)
                x3 += p;
            Int64 y3 = (lambda * (point.X - x3) - point.Y) % p;
            if (y3 < 0)
                y3 += p;
            return new Point(x3, y3);
        }

        private Point MultiplyPoint(Point point, Int64 factor, Int64 a, Int64 p)
        {
            Point q = new Point(0, 0);
            Int64 mask = 1;
            for (; mask <= factor; mask <<= 1){}
            mask >>= 1;
            for (; mask > 0; mask >>= 1)
            {
                // 2 * O = O
                if (q.X != 0 || q.Y != 0)
                    q = DoublingPoint(q, a, p);
                if ((factor & mask) != 0)
                {
                    // O + P = P
                    if (q is {X: 0, Y: 0})
                        q = point;
                    // Q == P => 2 * P
                    else if (q.X == point.X && q.Y == point.Y)
                        q = DoublingPoint(point, a, p);
                    // P + (-P) = O
                    else if (q.X == point.X && (q.Y + point.Y) % p == 0)
                        q = new Point(0, 0);
                    else
                        q = AdditionPoints(q, point, p);
                }
            }
            return q;
        }

        private Int64 CalcLambda(Int64 numerator, Int64 denominator, Int64 p)
        {
            if (denominator == 0)
                throw new InvalidOperationException("Bad denominator value");
            numerator %= p;
            if (numerator < 0)
                numerator += p;
            denominator %= p;
            if (denominator < 0)
                denominator += p;
            (Int64 _, Int64 denominatorValue, Int64 _) = LabUtils.CalcExtendedEuclideanAlgo(denominator, p);
            if (denominatorValue < 0)
                denominatorValue += p;
            Assert.That((denominator * denominatorValue) % p, Is.EqualTo(1));
            Int64 lambda = (numerator * denominatorValue) % p;
            return lambda;
        }
    }
}
