using System.Text;

namespace CryptoLab3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Выбранные параметры:
            // p - простое число
            const Int64 p = 2027;
            // q - простое число, простой делитель числа p - 1
            const Int64 q = 1013;
            // a – целое число, 1 < a < p - 1, при этом (a^q) mod p == 1
            const Int64 a = 2025;
            // x - секретный ключ для формирования подписи 1 < х < q
            const Int64 x = 983;
            // y - открытый ключ для проверки подписи, y = (a^x) mod p
            Int64 y = FastPower(a, x, p);
            // Выбранная хеш функция: количество 1 в битовом представлении символов исходного текста
            Func<Byte[], Int64> hashFunc = data =>
            {
                Int64 hashValue = 0;
                foreach (Byte symbol in data)
                    hashValue += Count1(symbol);
                return hashValue;
            };
            // Создаем ЭЦП для исходного текста: значения r1,s
            (Int64 r1, Int64 s) = GenerateDigitalSignature(p, q, a, x, hashFunc, Message);
            Console.WriteLine($"Digital signature: r1 = {r1}, s = {s}");
            // Проверяем, что созданная ЭЦП (значения r1,s) соответствует тексту
            if (!CheckDigitalSignature(p, q, a, y, hashFunc, Message, r1, s))
                throw new InvalidOperationException("Bad digital signature");
            Console.WriteLine("Digital signature is valid");
            // Как-то изменяем созданную ЭЦП (значения r1,s)
            r1 = r1 > 3 ? r1 - 3 : r1 + 3;
            s = s > 5 ? s - 5 : s + 5;
            // Проверяем, что измененная ЭЦП (значения r1,s) не соответствует тексту
            if (CheckDigitalSignature(p, q, a, y, hashFunc, Message, r1, s))
                throw new InvalidOperationException("Bad digital signature");
            Console.WriteLine("Changed digital signature is invalid");
        }

        // Создаем ЭЦП для исходного текста: значения r1,s
        private static (Int64 r1, Int64 s) GenerateDigitalSignature(Int64 p, Int64 q, Int64 a, Int64 x, Func<Byte[], Int64> hashFunc, String message)
        {
            // Вычисляем хэш-код сообщения m: h=H(m)
            Int64 hash = hashFunc(Encoding.ASCII.GetBytes(message));
            // Если h(m) mod q = 0,то h(m) присваивается значение 1.
            if (hash % q == 0)
                hash = 1;
            // Инициализируем генератор псевдослучайных чисел
            Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            // Цикл, пока не сгенерируем ЭЦП
            while (true)
            {
                // Из диапазона [1,q] случайным образом выбираем значение k
                Int64 k = rand.NextInt64(1, q + 1);
                // Вычисляем r = a^k mod p
                Int64 r = FastPower(a, k, p);
                // Вычисляем r1 = r mod q
                Int64 r1 = r % q;
                // Если r1 = 0, то возвращаемся к предыдущему этапу и выбираем другое значение k.
                if (r1 == 0)
                    continue;
                // Вычисляем s= (x * r1 + k * h) mod q
                Int64 s = (x * r1 + k * hash) % q;
                // Если s = 0, то возвращаемся в начало и выбираем другое значение k.
                if (s == 0)
                    continue;
                // Удалось сформировать ЭЦП
                return (r1, s);
            }
        }

        // Проверяем созданную ЭЦП (значения r1,s) на соответствует тексту
        private static Boolean CheckDigitalSignature(Int64 p, Int64 q, Int64 a, Int64 y, Func<Byte[], Int64> hashFunc, String message, Int64 r1, Int64 s)
        {
            // Проверяем выполнение условий 0 < r1 < q, 0 < s < q, и если хотя бы одно из них нарушено, то отвергаем подпись
            if (r1 <= 0 || r1 >= q)
                return false;
            if (s <= 0 || s >= q)
                return false;
            // Вычисляем хэш-код сообщения m: h=H(m)
            Int64 hash = hashFunc(Encoding.ASCII.GetBytes(message));
            // Если h(m) mod q = 0,то h(m) присваивается значение 1.
            if (hash % q == 0)
                hash = 1;
            // Вычисляем v = h(m1)^(q-2) mod q
            Int64 v = FastPower(hash, q - 2, q);
            // Вычисляем z1 = s * v mod q
            Int64 z1 = (s * v) % q;
            // Вычисляем z2 = (q-r1) * v mod q
            Int64 z2 = ((q - r1) * v) % q;
            // Вычисляем u=((a^z1 * y^z2) mod p) mod q
            Int64 u = ((FastPower(a, z1, p) * FastPower(y, z2, p)) % p) % q;
            // Проверяем равенство u = r1.Если равенство выполняется, то подпись принимаем. В противном подпись считается недействительной.
            return u == r1;
        }

        // Считаем количество 1 в битовом представлении байта
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

        private const String Message = "The divide-and-conquer algorithmic paradigm involves subdividing a large problem instance into smaller instances of the same problem.";
    }
}
