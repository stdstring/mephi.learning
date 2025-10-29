namespace CryptoLab4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Выбранные значения a, b, p, G(x,y), k1, k2
            Int64 a = -1;
            Int64 b = 1;
            Int64 p = 29;
            Int64 gx = 9;
            Int64 gy = 27;
            Int64 k1 = 4;
            Int64 k2 = 17;
            // Проверяем, что (4 * a^3 + 27* b^2) mod p != 0
            Int64 condValue = (4 * a * a * a + 27 * b * b) % p;
            if (condValue == 0)
                throw new InvalidOperationException("Bad a, b, p parameters");
            // Начальная точка G
            Point g = new Point(gx, gy);
            // Вычисляем произведение k1 * G для пользователя A (и отправляем его пользователю B)
            Point key1 = MultiplyPoint(g, k1, a, p);
            // Вычисляем произведение k2 * G для пользователя B (и отправляем его пользователю A)
            Point key2 = MultiplyPoint(g, k2, a, p);
            // Вычисляем для пользователя A на основе имеющегося у него числа и полученного по сети значения, ключ K = k1 * (k2 * G)
            Point key12 = MultiplyPoint(key1, k2, a, p);
            // Вычисляем для пользователя B на основе имеющегося у него числа и полученного по сети значения, ключ K = k2 * (k1 * G)
            Point key21 = MultiplyPoint(key2, k1, a, p);
            Console.WriteLine($"k1 * G = {key1}");
            Console.WriteLine($"k2 * G = {key2}");
            Console.WriteLine($"k2 * k1 * G = {key21}");
            Console.WriteLine($"k1 * k2 * G = {key12}");
            // Сравниваем значение ключа K у пользователей A и B
            if (key12 != key21)
                throw new InvalidOperationException("Bad value of k2 * k1 * G and k1 * k2 * G");
            Console.WriteLine("k1 * k2 * G and k2 * k1 * G are equal");
        }

        // Структура для хранения координат точек
        private record struct Point(Int64 X, Int64 Y);

        // Сложение точек на эллиптической кривой
        private static Point AdditionPoints(Point point1, Point point2, Int64 p)
        {
            // Вычисляем L = ((y2 - y1) / (x2 - x1)) mod p
            Int64 lambda = CalcLambda(point2.Y - point1.Y, point2.X - point1.X, p);
            // Вычисляем x3 = (L^2 - x1 - x2) mod p
            Int64 x3 = (lambda * lambda - point1.X - point2.X) % p;
            if (x3 < 0)
                x3 += p;
            // Вычисляем y3 = (L * (x1 - x3) - y1) mod p
            Int64 y3 = (lambda * (point1.X - x3) - point1.Y) % p;
            if (y3 < 0)
                y3 += p;
            return new Point(x3, y3);
        }

        // Удвоение точки на эллиптической кривой
        private static Point DoublingPoint(Point point, Int64 a, Int64 p)
        {
            // Вычисляем L = ((3 * x1^2 + a) / (2 * y1)) mod p
            Int64 lambda = CalcLambda(3 * point.X * point.X + a, 2 * point.Y, p);
            // Вычисляем x3 = (L^2 - 2 * x1) mod p
            Int64 x3 = (lambda * lambda - 2 * point.X) % p;
            if (x3 < 0)
                x3 += p;
            // Вычисляем y3 = (L * (x1 - x3) - y1) mod p
            Int64 y3 = (lambda * (point.X - x3) - point.Y) % p;
            if (y3 < 0)
                y3 += p;
            return new Point(x3, y3);
        }

        // Умножение точки на скаляр
        private static Point MultiplyPoint(Point point, Int64 factor, Int64 a, Int64 p)
        {
            Point q = new Point(0, 0);
            // Находим наиболее значимый бит двоичного представления скаляра
            Int64 mask = 1;
            for (; mask <= factor; mask <<= 1) {}
            mask >>= 1;
            // Умножение точки на число реализуем последовательностью сложений и удвоений точки эллиптической кривой
            // Для этого используем двоичное представление скаляра
            for (; mask > 0; mask >>= 1)
            {
                // Удвоение точки на эллиптической кривой (для точки O: 2 * O = O)
                if (q.X != 0 || q.Y != 0)
                    q = DoublingPoint(q, a, p);
                // Сложение точек на эллиптической кривой
                if ((factor & mask) != 0)
                {
                    // Обрабатываем случай O + P = P
                    if (q is {X: 0, Y: 0})
                        q = point;
                    // Обрабатываем случай Q == P => 2 * P
                    else if (q.X == point.X && q.Y == point.Y)
                        q = DoublingPoint(point, a, p);
                    // Обрабатываем случай P + (-P) = O
                    else if (q.X == point.X && (q.Y + point.Y) % p == 0)
                        q = new Point(0, 0);
                    else
                        q = AdditionPoints(q, point, p);
                }
            }
            return q;
        }

        // Вычисляем значение L (lambda), используемое при вычислении сложения точек и удвоения точки на эллиптической кривой
        private static Int64 CalcLambda(Int64 numerator, Int64 denominator, Int64 p)
        {
            if (denominator == 0)
                throw new InvalidOperationException("Bad denominator value");
            numerator %= p;
            if (numerator < 0)
                numerator += p;
            denominator %= p;
            if (denominator < 0)
                denominator += p;
            (Int64 _, Int64 denominatorValue, Int64 _) = CalcExtendedEuclideanAlgo(denominator, p);
            if (denominatorValue < 0)
                denominatorValue += p;
            Int64 lambda = (numerator * denominatorValue) % p;
            return lambda;
        }

        // Реализация расширенного алгоритма Евклида: https://en.wikipedia.org/wiki/Extended_Euclidean_algorithm
        private static (Int64 gcd, Int64 x, Int64 y) CalcExtendedEuclideanAlgo(Int64 a, Int64 b)
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
    }
}
