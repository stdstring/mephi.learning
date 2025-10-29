using NUnit.Framework;

namespace CryptoLabs
{
    public abstract record Lab1TestCaseItem(String Message)
    {
        public record SingleSubstitutionTestCase(String Message, String SourceAlphabet, String SubstitutionAlphabet) : Lab1TestCaseItem(Message);

        public record PermutationTestCase(String Message, Int32[] SourceGroup, Int32[] PermutationGroup) : Lab1TestCaseItem(Message);

        public record MutliAlphabetSubstitutionTestCase(String Message, String SourceAlphabet, params String[] SubstitutionAlphabets) : Lab1TestCaseItem(Message);
    }

    [TestFixture]
    public class Lab1Tests
    {
        [TestCaseSource(nameof(LabTestCases))]
        public void ExecuteLab1(Lab1TestCaseItem item)
        {
            switch (item)
            {
                case Lab1TestCaseItem.SingleSubstitutionTestCase(Message: var message,
                    SourceAlphabet: var sourceAlphabet,
                    SubstitutionAlphabet: var substitutionAlphabet):
                    ExecuteSingleSubstitutionCase(message, sourceAlphabet, substitutionAlphabet);
                    break;
                case Lab1TestCaseItem.PermutationTestCase(Message: var message,
                    SourceGroup: var sourceGroup,
                    PermutationGroup: var permutationGroup):
                    ExecutePermutationCase(message, sourceGroup, permutationGroup);
                    break;
                case Lab1TestCaseItem.MutliAlphabetSubstitutionTestCase(Message: var message,
                    SourceAlphabet: var sourceAlphabet,
                    SubstitutionAlphabets: var substitutionAlphabets):
                    ExecuteMutliAlphabetSubstitutionCase(message, sourceAlphabet, substitutionAlphabets);
                    break;
                default:
                    Assert.Fail("Unknown type of test case");
                    break;
            }
        }

        private void ExecuteSingleSubstitutionCase(String message, String sourceAlphabet, String substitutionAlphabet)
        {
            Console.WriteLine($"Source text: {message}");
            String encodedMessage = EncodeBySubstitution(message, sourceAlphabet, substitutionAlphabet);
            Console.WriteLine($"Encoded text: {encodedMessage}");
            String decodedMessage = DecodeBySubstitution(encodedMessage, sourceAlphabet, substitutionAlphabet);
            Console.WriteLine($"Decoded text: {decodedMessage}");
            Assert.That(encodedMessage, Is.Not.EqualTo(message));
            Assert.That(decodedMessage, Is.EqualTo(message));
        }

        private String EncodeBySubstitution(String message, String sourceAlphabet, String substitutionAlphabet)
        {
            IDictionary<Char, Char> substitutionMap = CreateSubstitutionMap(sourceAlphabet, substitutionAlphabet);
            Char[] dest = new Char[message.Length];
            for (Int32 index = 0; index < message.Length; ++index)
                dest[index] = substitutionMap[message[index]];
            return new String(dest);
        }

        private String DecodeBySubstitution(String message, String sourceAlphabet, String substitutionAlphabet)
        {
            IDictionary<Char, Char> substitutionMap = CreateSubstitutionMap(substitutionAlphabet, sourceAlphabet);
            Char[] dest = new Char[message.Length];
            for (Int32 index = 0; index < message.Length; ++index)
                dest[index] = substitutionMap[message[index]];
            return new String(dest);
        }

        private void ExecutePermutationCase(String message, Int32[] sourceGroup, Int32[] permutationGroup)
        {
            if (message.Length % sourceGroup.Length != 0)
                message += new String(' ', sourceGroup.Length - message.Length % sourceGroup.Length);
            Console.WriteLine($"Source text: {message}");
            String encodedMessage = EncodeByPermutation(message, sourceGroup, permutationGroup);
            Console.WriteLine($"Encoded text: {encodedMessage}");
            String decodedMessage = DecodeByPermutation(encodedMessage, sourceGroup, permutationGroup);
            Console.WriteLine($"Decoded text: {decodedMessage}");
            Assert.That(encodedMessage, Is.Not.EqualTo(message));
            Assert.That(decodedMessage, Is.EqualTo(message));
        }

        private String EncodeByPermutation(String message, Int32[] sourceGroup, Int32[] permutationGroup)
        {
            Char[] dest = new Char[message.Length];
            Int32 groupsCount = message.Length / sourceGroup.Length;
            for (Int32 groupIndex = 0; groupIndex < groupsCount; ++groupIndex)
            {
                Int32 shift = groupIndex * sourceGroup.Length;
                for (Int32 index = 0; index < sourceGroup.Length; ++index)
                    dest[shift + sourceGroup[index] - 1] = message[shift + permutationGroup[index] - 1];
            }
            return new String(dest);
        }

        private String DecodeByPermutation(String message, Int32[] sourceGroup, Int32[] permutationGroup)
        {
            Char[] dest = new Char[message.Length];
            Int32 groupsCount = message.Length / sourceGroup.Length;
            for (Int32 groupIndex = 0; groupIndex < groupsCount; ++groupIndex)
            {
                Int32 shift = groupIndex * sourceGroup.Length;
                for (Int32 index = 0; index < sourceGroup.Length; ++index)
                    dest[shift + permutationGroup[index] - 1] = message[shift + sourceGroup[index] - 1];
            }
            return new String(dest);
        }

        private void ExecuteMutliAlphabetSubstitutionCase(String message, String sourceAlphabet, String[] substitutionAlphabets)
        {
            Console.WriteLine($"Source text: {message}");
            String encodedMessage = EncodeByMutliAlphabetSubstitution(message, sourceAlphabet, substitutionAlphabets);
            Console.WriteLine($"Encoded text: {encodedMessage}");
            String decodedMessage = DecodeByMutliAlphabetSubstitution(encodedMessage, sourceAlphabet, substitutionAlphabets);
            Console.WriteLine($"Decoded text: {decodedMessage}");
            Assert.That(encodedMessage, Is.Not.EqualTo(message));
            Assert.That(decodedMessage, Is.EqualTo(message));
        }

        private String EncodeByMutliAlphabetSubstitution(String message, String sourceAlphabet, String[] substitutionAlphabets)
        {
            IDictionary<Char, Char>[] substitutionMaps = substitutionAlphabets
                .Select(substitutionAlphabet => CreateSubstitutionMap(sourceAlphabet, substitutionAlphabet))
                .ToArray();
            Char[] dest = new Char[message.Length];
            for (Int32 index = 0; index < message.Length; ++index)
                dest[index] = substitutionMaps[index % substitutionAlphabets.Length][message[index]];
            return new String(dest);
        }

        private String DecodeByMutliAlphabetSubstitution(String message, String sourceAlphabet, String[] substitutionAlphabets)
        {
            IDictionary<Char, Char>[] substitutionMaps = substitutionAlphabets
                .Select(substitutionAlphabet => CreateSubstitutionMap(substitutionAlphabet, sourceAlphabet))
                .ToArray();
            Char[] dest = new Char[message.Length];
            for (Int32 index = 0; index < message.Length; ++index)
                dest[index] = substitutionMaps[index % substitutionAlphabets.Length][message[index]];
            return new String(dest);
        }

        private IDictionary<Char, Char> CreateSubstitutionMap(String sourceAlphabet, String substitutionAlphabet)
        {
            return Enumerable.Range(0, sourceAlphabet.Length)
                .ToDictionary(index => sourceAlphabet[index], index => substitutionAlphabet[index]);
        }

        private static IEnumerable<Lab1TestCaseItem> LabTestCases()
        {
            yield return new Lab1TestCaseItem.SingleSubstitutionTestCase(EnglishText, EnglishAlphabet, EnglishSubstitution3);
            yield return new Lab1TestCaseItem.PermutationTestCase(AsciiText, new []{1, 2, 3, 4, 5, 6}, new []{3, 5, 2, 6, 1, 4});
            yield return new Lab1TestCaseItem.MutliAlphabetSubstitutionTestCase(RussianText, RussianAlphabet, RussianSubstitution1, RussianSubstitution2, RussianSubstitution5);
            yield return new Lab1TestCaseItem.PermutationTestCase(RussianText, new []{1, 2, 3, 4, 5}, new []{5, 4, 1, 2, 3});
            yield return new Lab1TestCaseItem.SingleSubstitutionTestCase(EnglishText, EnglishAlphabet, EnglishSubstitution4);
            yield return new Lab1TestCaseItem.MutliAlphabetSubstitutionTestCase(RussianText, RussianAlphabet, RussianSubstitution1, RussianSubstitution3);
            yield return new Lab1TestCaseItem.SingleSubstitutionTestCase(EnglishText, EnglishAlphabet, EnglishSubstitution1);
            yield return new Lab1TestCaseItem.MutliAlphabetSubstitutionTestCase(EnglishText, EnglishAlphabet, EnglishSubstitution2, EnglishSubstitution5);
            yield return new Lab1TestCaseItem.PermutationTestCase(AsciiText, new []{1, 2, 3, 4, 5, 6}, new []{2, 5, 3, 4, 1, 6});
            yield return new Lab1TestCaseItem.SingleSubstitutionTestCase(RussianText, RussianAlphabet, RussianSubstitution2);
            yield return new Lab1TestCaseItem.PermutationTestCase(AsciiText, new []{1, 2, 3, 4, 5, 6}, new []{2, 6, 3, 5, 1, 4});
            yield return new Lab1TestCaseItem.MutliAlphabetSubstitutionTestCase(RussianText, RussianAlphabet, RussianSubstitution1, RussianSubstitution3, RussianSubstitution4);
        }

        private const String EnglishAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ .,!:;?-";
        private const String RussianAlphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ ";
        private const String EnglishSubstitution1 = "VWXYZ .,!:;?-KLMNOPQRSTUABCDEFGHIJ";
        private const String RussianSubstitution1 = "БЮГЫЕЬЗШЙЦЛФНТПРСОУМХКЧИЩЖЪДЭВЯ АЁ";
        private const String EnglishSubstitution2 = "CDABHIJEFGOPQRKLMNUVW:STZ XY;?-.,!";
        private const String RussianSubstitution2 = "СОУМКХЧИЩЖЪДЭВЯАБЮГ ЕЬЗШЙЦЁФНТПРЫЛ";
        private const String EnglishSubstitution3 = "Z .XY,!ST:;QR?-NOPLMUVWABCDEFGHIJK";
        private const String RussianSubstitution3 = "ОПМНХЛИЙЖЗДЕВГАБЮЯЫЭЬ ШЩЦЧФКТУРСЪЁ";
        private const String EnglishSubstitution4 = "CDABHIJEFGOPQRKLMNUVW:STZ XY;?-.,!";
        private const String RussianSubstitution4 = "ЮЯЫЭЬЪШЩЦЧФХТУРСОПМНКЛ ЙЖЗДЕВГАБЁИ";
        private const String EnglishSubstitution5 = "VWXYZ .,!:;?-KLMNOPQRSTUABCDEFGHIJ";
        private const String RussianSubstitution5 = "МНОПРСТУФХЦЧШЩЪЬЫЭЮЯ АБВГДЕЁЖЗИЙКЛ";

        private const String EnglishText = "EVERY COMPLEX PROBLEM HAS A SOLUTION THAT IS CLEAR, SIMPLE, AND WRONG.";
        private const String AsciiText = "<<(({{EVERY COMPLEX PROBLEM HAS A SOLUTION THAT IS CLEAR, SIMPLE, AND WRONG.}}))>>";
        private const String RussianText = "ШИФРОВАНИЕ ПРОСТОЙ ПОДСТАНОВКОЙ НА КОРОТКИХ АЛФАВИТАХ ОБЕСПЕЧИВАЕТ СЛАБУЮ ЗАЩИТУ ОТКРЫТОГО ТЕКСТА";
    }
}
