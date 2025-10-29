using System.Numerics;
using System.Text;

namespace CryptoLab2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Выбранные значения p и q
            const Int64 p = 193;
            const Int64 q = 353;
            // pqEulerFuncValue = (p-1) * (q-1) – значение функции Эйлера
            Int64 pqEulerFuncValue = (p - 1) * (q - 1);
            // Проверяем, что p и q взаимно простые
            if (CalcExtendedEuclideanAlgo(p, q).gcd != 1)
                throw new InvalidOperationException("Bad p and q");
            Console.WriteLine($"p = {p} and q = {q} is mutually prime numbers");
            // Вычисляем n
            Int64 n = p * q;
            // Находим минимально возможное значение e
            Int64 e = FindMinE(p, q);
            // Проверяем, что e и (p-1) * (q-1) взаимно простые числа
            if (CalcExtendedEuclideanAlgo(e, pqEulerFuncValue).gcd != 1)
                throw new InvalidOperationException("Bad e");
            // Вычисляем значение d
            Int64 d = CalcD(p, q, e);
            // Проверяем, что (e * d) mod ((p-1) *(q-1)) == 1
            if ((e * d) % pqEulerFuncValue != 1)
                throw new InvalidOperationException("Bad d");
            // Выводим приватный и публичный ключи
            Console.WriteLine($"Private key: p = {p}, q = {q}, d = {d}");
            Console.WriteLine($"Public key: n = {n}, e = {e}");
            Console.WriteLine($"Source text: {Source}");
            // Преобразуем текст в набор байт в кодировке UTF8
            Byte[] sourceData = Encoding.UTF8.GetBytes(Source);
            // Разбиваем шифруемый текст на блоки, каждый из которых может быть представлен в виде числа Мi= 0,1,...,n-1
            Int64[] sourceBlocks = BytesToBlocks(sourceData, n);
            // Кодируем сообщение
            Int64[] encodedBlocks = EncodeBlocks(sourceBlocks, e, n);
            // Показываем закодированное сообщение
            ShowEncodedData(encodedBlocks);
            // Декодируем сообщение
            Int64[] decodedBlocks = DecodeBlocks(encodedBlocks, d, n);
            // Преобразуем блоки, на которые был разбит текст в набор байт
            Byte[] decodedData = BlocksToBytes(decodedBlocks, n);
            // Преобразуем набор байт (в кодировке UTF8) в текст
            String result = Encoding.UTF8.GetString(decodedData);
            Console.WriteLine($"Decoded text: {result}");
            // Проверяем, что закодированные данные не равны исходным
            if (CheckData(sourceBlocks, encodedBlocks))
                throw new InvalidOperationException("Bad encoding");
            // Проверяем, что декодированные данные равны исходным
            if (!CheckData(sourceBlocks, decodedBlocks))
                throw new InvalidOperationException("Bad decoding");
        }

        // Находим минимально возможное значение e
        private static Int64 FindMinE(Int64 a, Int64 b)
        {
            // a и b - два простых числа, значит произведение (a - 1) * (b - 1) - четно
            // (a - 1) * (b - 1) и e должны быть взаимно простыми - значит e - нечетно
            Int64 eulerFuncValue = (a - 1) * (b - 1);
            // Начинаем со значения 3 и перебираем все нечетные числа
            for (Int64 eValue = 3; ; eValue += 2)
            {
                // Если (a - 1) * (b - 1) и e взаимно простые, значит мы нашли число e - возвращаем его
                if (CalcExtendedEuclideanAlgo(eulerFuncValue, eValue).gcd == 1)
                    return eValue;
            }
        }

        // Вычисляем d по формуле (e * d) mod ((p-1) *(q-1)) == 1
        private static Int64 CalcD(Int64 a, Int64 b, Int64 e)
        {
            Int64 eulerFuncValue = (a - 1) * (b - 1);
            (Int64 _, Int64 x, Int64 y) result = CalcExtendedEuclideanAlgo(e, eulerFuncValue);
            Int64 d = result.x;
            if (d < 0)
                d += eulerFuncValue;
            return d;
        }

        // Разбиваем шифруемый текст (набор байт шифруемого текста) на блоки, каждый из которых может быть представлен в виде числа Мi= 0,1,...,n-1
        private static Int64[] BytesToBlocks(Byte[] source, Int64 n)
        {
            // Представляем набор байт в виде одного большого числа
            BigInteger sourceRepresentation = new BigInteger(source);
            // Разбиваем шифруемый текст (набор байт шифруемого текста) на блоки:
            // Переводим полученное большое число в систему счисления по основанию n
            IList<Int64> parts = new List<Int64>();
            for (; sourceRepresentation > 0; sourceRepresentation /= n)
                parts.Add((Int64)(sourceRepresentation % n));
            return parts.ToArray();
        }

        // Преобразуем блоки, на которые был разбит текст в набор байт
        private static Byte[] BlocksToBytes(Int64[] source, Int64 n)
        {
            // Преобразуем блоки, на которые был разбит текст в одно большое число
            // Для этого переводим блоки, которые являются большим числом по основанию n в большое число по основанию 10
            BigInteger sourceRepresentation = 0;
            foreach (Int64 number in source.Reverse())
            {
                sourceRepresentation *= n;
                sourceRepresentation += number;
            }
            // Преобразуем большое число в набор байт
            return sourceRepresentation.ToByteArray();
        }

        // Кодируем сообщение
        private static Int64[] EncodeBlocks(Int64[] source, Int64 e, Int64 n)
        {
            // Для каждого блока выполняем вычисление: Ci = (Mi ^ e) mod n
            return source.Select(block => FastPower(block, e, n)).ToArray();
        }

        // Декодируем сообщение
        private static Int64[] DecodeBlocks(Int64[] source, Int64 d, Int64 n)
        {
            // Для каждого блока выполняем вычисление: Mi = (Ci ^ d) mod n
            return source.Select(block => FastPower(block, d, n)).ToArray();
        }

        // Показываем закодированное сообщение
        private static void ShowEncodedData(Int64[] source)
        {
            Console.WriteLine($"EncodedData: {String.Join(", ", source)}");
        }

        // Проверяем на равенство 2 массива чисел
        private static Boolean CheckData(Int64[] expected, Int64[] actual)
        {
            if (expected.Length != actual.Length)
                return false;
            for (Int32 index = 0; index < expected.Length; ++index)
            {
                if (expected[index] != actual[index])
                    return false;
            }
            return true;
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

        // Реализация быстрого возведения числа a в степень b по основанию n
        private static Int64 FastPower(Int64 a, Int64 b, Int64 n)
        {
            if (a < 0)
                throw new ArgumentOutOfRangeException(nameof(a));
            if (b < 0)
                throw new ArgumentOutOfRangeException(nameof(b));
            if (n < 0)
                throw new ArgumentOutOfRangeException(nameof(n));
            // (a^b) mod n
            Int64 result = 1;
            Int64 current = a;
            // Используем двоичное представление степени b
            for (Int64 mask = 1; mask <= b; mask <<= 1)
            {
                if ((mask & b) != 0)
                    result = (result * current) % n;
                current = (current * current) % n;
            }
            return result;
        }

        private const String Source = "Every complex problem has a solution that is clear, simple, and wrong. - H. L. Mencken";
    }
}
