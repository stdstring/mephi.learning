using System.Text;
using NUnit.Framework;

namespace CryptoLabs
{
    [TestFixture]
    public class Lab3Tests
    {
        // p = 2027, q = 1013, a = 2025
        [TestCaseSource(nameof(LabTestCases))]
        public void ExecuteLab3(Func<Byte[], Int64> hashFunc)
        {
            // selected parameters
            const Int64 p = 2027;
            const Int64 q = 1013;
            const Int64 a = 2025;
            const Int64 x = 983;
            const String message = "The divide-and-conquer algorithmic paradigm involves subdividing a large problem instance into smaller instances of the same problem.";
            // y=a^x mod p
            Int64 y = LabUtils.FastPower(a, x, p);
            (Int64 r1, Int64 s) = GenerateDigitalSignature(p, q, a, x, hashFunc, message);
            Assert.That(CheckDigitalSignature(p, q, a, y, hashFunc, message, r1, s), Is.True);
            r1 = r1 > 3 ? r1 - 3 : r1 + 3;
            s = s > 5 ? s - 5 : s + 5;
            Assert.That(CheckDigitalSignature(p, q, a, y, hashFunc, message, r1, s), Is.False);
        }

        private (Int64 r1, Int64 s) GenerateDigitalSignature(Int64 p, Int64 q, Int64 a, Int64 x, Func<Byte[], Int64> hashFunc, String message)
        {
            Int64 hash = hashFunc(Encoding.UTF8.GetBytes(message));
            if (hash % q == 0)
                hash = 1;
            Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            while (true)
            {
                // 1 <= k <= q
                Int64 k = rand.NextInt64(1, q + 1);
                // r = a^k mod p
                Int64 r = LabUtils.FastPower(a, k, p);
                // r1 = r mod q
                Int64 r1 = r % q;
                if (r1 == 0)
                    continue;
                // s= (x * r1 + k * h) mod q
                Int64 s = (x * r1 + k * hash) % q;
                if (s == 0)
                    continue;
                return (r1, s);
            }
        }

        private Boolean CheckDigitalSignature(Int64 p, Int64 q, Int64 a, Int64 y, Func<Byte[], Int64> hashFunc, String message, Int64 r1, Int64 s)
        {
            if (r1 <= 0 || r1 >= q)
                return false;
            if (s <= 0 || s >= q)
                return false;
            Int64 hash = hashFunc(Encoding.UTF8.GetBytes(message));
            if (hash % q == 0)
                hash = 1;
            // v = h(m1)^(q-2) mod q
            Int64 v = LabUtils.FastPower(hash, q - 2, q);
            // z1 = s * v mod q
            Int64 z1 = (s * v) % q;
            // z2 = (q-r1) * v mod q
            Int64 z2 = ((q - r1) * v) % q;
            // u=((a^z1 * y^z2) mod p) mod q
            Int64 u = ((LabUtils.FastPower(a, z1, p) * LabUtils.FastPower(y, z2, p)) % p) % q;
            return u == r1;
        }

        private static IEnumerable<Func<Byte[], Int64>> LabTestCases()
        {
            // variant 1, 6, 9, 12
            yield return data =>
            {
                Int64 hashValue = 0;
                foreach (Byte symbol in data)
                    hashValue += LabUtils.Count1(symbol);
                return hashValue;
            };
            // variant 2
            yield return data =>
            {
                const Int64 modValue = 1 << 16;
                Int64 hashValue = 0;
                foreach (Byte symbol in data)
                    hashValue = (hashValue + symbol) % modValue;
                return hashValue;
            };
            // variant 3
            yield return data =>
            {
                Int64 hashValue = 0;
                for (Int32 index = 0; index < data.Length; index += 4)
                {
                    Int64 current = (data[index] +
                                     (index + 1 < data.Length ? data[index + 1] : 0) << 8 +
                                     (index + 2 < data.Length ? data[index + 2] : 0) << 16 +
                                     (index + 3 < data.Length ? data[index + 3] : 0) << 24);
                    hashValue ^= current;
                }
                return hashValue;
            };
            // variant 4, 11
            yield return data =>
            {
                const Int64 modValue = 1 << 16;
                Int64 hashValue = 1;
                foreach (Byte symbol in data)
                    hashValue = (hashValue * symbol) % modValue;
                return hashValue;
            };
            // variant 5, 8
            yield return data =>
            {
                Int64 hashValue = 0;
                for (Int32 index = 0; index < data.Length; index += 2)
                {
                    Int64 current = (data[index] + (index + 1 < data.Length ? data[index + 1] : 0) << 8);
                    hashValue ^= (current >> 1);
                }
                return hashValue;
            };
            // variant 7, 10
            yield return data =>
            {
                UInt32 hashValue = 0;
                for (Int32 index = 0; index < data.Length; index += 2)
                {
                    UInt32 current = (UInt32)(data[index] + (index + 1 < data.Length ? data[index + 1] : 0) << 8);
                    hashValue ^= (current << 1);
                }
                return hashValue;
            };
        }
    }

    [TestFixture]
    public class Lab3HelperTests
    {
        [Test]
        public void FindDigitalSignatureParameters()
        {
            // find p - q pair
            Int64 p;
            Int64 q = 1001;
            for (;; q += 2)
            {
                if (!IsPrime(q))
                    continue;
                p = 2 * q + 1;
                if (IsPrime(p))
                    break;
                p = 4 * q + 1;
                if (IsPrime(p))
                    break;
            }
            // 1 < a < p-1
            Int64 a = p - 2;
            for (;; --a)
            {
                Int64 powerValue = LabUtils.FastPower(a, q, p);
                if (powerValue == 1)
                    break;
            }
            Console.WriteLine($"p = {p}, q = {q}, a = {a}");
        }

        private Boolean IsPrime(Int64 number)
        {
            if (number < 2)
                throw new ArgumentOutOfRangeException(nameof(number));
            if (number == 2)
                return true;
            if (number % 2 == 0)
                return false;
            for (Int64 divider = 3; divider * divider <= number; divider += 2)
            {
                if (number % divider == 0)
                    return false;
            }
            return true;
        }
    }
}
