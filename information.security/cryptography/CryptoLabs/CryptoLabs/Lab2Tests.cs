using System.Numerics;
using System.Text;
using NUnit.Framework;

namespace CryptoLabs
{
    [TestFixture]
    public class Lab2Tests
    {
        [TestCase(193, 353)]
        [TestCase(211, 317)]
        [TestCase(157, 433)]
        [TestCase(179, 383)]
        [TestCase(223, 317)]
        [TestCase(199, 337)]
        [TestCase(181, 367)]
        [TestCase(163, 409)]
        [TestCase(227, 307)]
        [TestCase(233, 293)]
        [TestCase(241, 307)]
        [TestCase(167, 401)]
        [TestCase(271, 277)]
        [TestCase(137, 479)]
        public void ExecuteLab2(Int64 p, Int64 q)
        {
            (Int64 gcd, Int64, Int64) checkpq = LabUtils.CalcExtendedEuclideanAlgo(p, q);
            Assert.That(checkpq.gcd, Is.EqualTo(1));
            Int64 n = p * q;
            Int64 e = FindMinE(p, q);
            Int64 d = CalcD(p, q, e);
            Console.WriteLine($"private key: p = {p}, q = {q}, d = {d}");
            Console.WriteLine($"public key: n = {n}, e = {e}");
            Byte[] source = Encoding.UTF8.GetBytes(Source);
            Int64[] sourceBlocks = BytesToBlocks(source, n);
            Int64[] encodedBlocks = EncodeBlocks(sourceBlocks, e, n);
            Int64[] decodedBlocks = DecodeBlocks(encodedBlocks, d, n);
            Assert.That(decodedBlocks, Is.EquivalentTo(sourceBlocks));
            Byte[] result = BlocksToBytes(decodedBlocks, n);
            Assert.That(result, Is.EquivalentTo(source));
            Console.WriteLine(Encoding.UTF8.GetString(result));
        }

        private Int64 FindMinE(Int64 a, Int64 b)
        {
            // a and b - two prime numbers
            // so a -even, b - odd || a - odd, e-even || a - add, b - odd
            // so (a - 1) * (b - 1) - even => e - must be odd
            Int64 eulerFuncValue = (a - 1) * (b - 1);
            for (Int64 eValue = 3; ; eValue += 2)
            {
                if (LabUtils.CalcExtendedEuclideanAlgo(eulerFuncValue, eValue).gcd == 1)
                    return eValue;
            }
        }

        private Int64 CalcD(Int64 a, Int64 b, Int64 e)
        {
            Int64 eulerFuncValue = (a - 1) * (b - 1);
            (Int64 _, Int64 x, Int64 y) result = LabUtils.CalcExtendedEuclideanAlgo(e, eulerFuncValue);
            Int64 d = result.x;
            if (d < 0)
                d += eulerFuncValue;
            return d;
        }

        private Int64[] BytesToBlocks(Byte[] source, Int64 n)
        {
            BigInteger sourceRepresentation = new BigInteger(source);
            IList<Int64> parts = new List<Int64>();
            for (; sourceRepresentation > 0; sourceRepresentation /= n)
                parts.Add((Int64)(sourceRepresentation % n));
            return parts.ToArray();
        }

        private Byte[] BlocksToBytes(Int64[] source, Int64 n)
        {
            BigInteger sourceRepresentation = 0;
            foreach (Int64 number in source.Reverse())
            {
                sourceRepresentation *= n;
                sourceRepresentation += number;
            }

            return sourceRepresentation.ToByteArray();
        }

        private Int64[] EncodeBlocks(Int64[] source, Int64 e, Int64 n)
        {
            return source.Select(block => LabUtils.FastPower(block, e, n)).ToArray();
        }

        private Int64[] DecodeBlocks(Int64[] source, Int64 d, Int64 n)
        {
            return source.Select(block => LabUtils.FastPower(block, d, n)).ToArray();
        }

        private const String Source = "Every complex problem has a solution that is clear, simple, and wrong. - H. L. Mencken";
    }
}
