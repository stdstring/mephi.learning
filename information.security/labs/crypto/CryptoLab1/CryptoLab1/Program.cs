namespace CryptoLab1
{
    internal class Program
    {
        // Подстановка: вариант 1
        // Перестановка: вариант 2
        // Многоалфавитные шифры: вариант 3
        static void Main(string[] args)
        {
            // Подстановка
            Console.WriteLine("Single substitution:");
            ExecuteSingleSubstitutionCase(EnglishText, EnglishAlphabet, EnglishSubstitution3);
            Console.WriteLine();
            // Перестановка
            Console.WriteLine("Permutation:");
            ExecutePermutationCase(AsciiText, new []{1, 2, 3, 4, 5, 6}, new []{3, 5, 2, 6, 1, 4});
            Console.WriteLine();
            // Многоалфавитные шифры
            Console.WriteLine("Mutli alphabet substitution:");
            ExecuteMutliAlphabetSubstitutionCase(RussianText, RussianAlphabet, RussianSubstitution1, RussianSubstitution2, RussianSubstitution5);
            Console.WriteLine();
        }

        // Обработка случая кодирования и декодирования подстановкой
        private static void ExecuteSingleSubstitutionCase(String message, String sourceAlphabet, String substitutionAlphabet)
        {
            Console.WriteLine($"Source text: {message}");
            // Кодируем сообщение
            String encodedMessage = EncodeBySubstitution(message, sourceAlphabet, substitutionAlphabet);
            Console.WriteLine($"Encoded text: {encodedMessage}");
            // Декодируем сообщение
            String decodedMessage = DecodeBySubstitution(encodedMessage, sourceAlphabet, substitutionAlphabet);
            Console.WriteLine($"Decoded text: {decodedMessage}");
            // Проверяем, что закодированное сообщение не равно исходному
            if (encodedMessage.Equals(message))
                throw new InvalidOperationException("Bad encoding");
            // Проверяем, что декодированное сообщение равно исходному
            if (!decodedMessage.Equals(message))
                throw new InvalidOperationException("Bad decoding");
        }

        // Кодирование подстановкой
        private static String EncodeBySubstitution(String message, String sourceAlphabet, String substitutionAlphabet)
        {
            // Создаем ассоциативный массив (Map) между символами двух алфавитов: исходного и подстановочного
            IDictionary<Char, Char> substitutionMap = CreateSubstitutionMap(sourceAlphabet, substitutionAlphabet);
            Char[] dest = new Char[message.Length];
            // Преобразуем каждый символ в исходном алфавите в соответствующий символ в алфавите подстановке
            for (Int32 index = 0; index < message.Length; ++index)
                dest[index] = substitutionMap[message[index]];
            return new String(dest);
        }

        // Декодирование подстановкой
        private static String DecodeBySubstitution(String message, String sourceAlphabet, String substitutionAlphabet)
        {
            // Создаем ассоциативный массив (Map) между символами двух алфавитов: подстановочного и исходного
            IDictionary<Char, Char> substitutionMap = CreateSubstitutionMap(substitutionAlphabet, sourceAlphabet);
            Char[] dest = new Char[message.Length];
            // Преобразуем каждый символ в алфавите подстановке в соответствующий символ в исходном алфавите (операция обратная кодированию)
            for (Int32 index = 0; index < message.Length; ++index)
                dest[index] = substitutionMap[message[index]];
            return new String(dest);
        }

        // Обработка случая кодирования и декодирования перестановкой
        private static void ExecutePermutationCase(String message, Int32[] sourceGroup, Int32[] permutationGroup)
        {
            // Обрабатываем случай, когда сообщение не кратно количеству символов в группе перестановки
            Int32 serviceRestSize = sourceGroup.Length - message.Length % sourceGroup.Length;
            if (serviceRestSize > 0)
                message += new String(' ', serviceRestSize);
            Console.WriteLine($"Source text: {message}");
            // Кодируем сообщение
            String encodedMessage = EncodeByPermutation(message, sourceGroup, permutationGroup);
            Console.WriteLine($"Encoded text: {encodedMessage}");
            // Декодируем сообщение
            String decodedMessage = DecodeByPermutation(encodedMessage, sourceGroup, permutationGroup);
            Console.WriteLine($"Decoded text: {decodedMessage}");
            // Проверяем, что закодированное сообщение не равно исходному
            if (encodedMessage.Equals(message))
                throw new InvalidOperationException("Bad encoding");
            // Проверяем, что декодированное сообщение равно исходному
            if (!decodedMessage.Equals(message))
                throw new InvalidOperationException("Bad decoding");
        }

        // Кодирование перестановкой
        private static String EncodeByPermutation(String message, Int32[] sourceGroup, Int32[] permutationGroup)
        {
            Char[] dest = new Char[message.Length];
            Int32 groupsCount = message.Length / sourceGroup.Length;
            // Обработка всех символов с помощью таблицы перестановки
            for (Int32 groupIndex = 0; groupIndex < groupsCount; ++groupIndex)
            {
                // Смещение данной обрабатываемой группы относительно начала
                Int32 shift = groupIndex * sourceGroup.Length;
                // Преобразуем каждый символ в с помощью таблицы перестановки для данной обрабатываемой группы
                for (Int32 index = 0; index < sourceGroup.Length; ++index)
                    dest[shift + sourceGroup[index] - 1] = message[shift + permutationGroup[index] - 1];
            }
            return new String(dest);
        }

        // Декодирование перестановкой
        private static String DecodeByPermutation(String message, Int32[] sourceGroup, Int32[] permutationGroup)
        {
            Char[] dest = new Char[message.Length];
            Int32 groupsCount = message.Length / sourceGroup.Length;
            // Обработка всех символов с помощью таблицы перестановки
            for (Int32 groupIndex = 0; groupIndex < groupsCount; ++groupIndex)
            {
                // Смещение данной обрабатываемой группы относительно начала
                Int32 shift = groupIndex * sourceGroup.Length;
                // Преобразуем каждый символ в с помощью таблицы перестановки для данной обрабатываемой группы в обратную сторону (операция обратная кодированию)
                for (Int32 index = 0; index < sourceGroup.Length; ++index)
                    dest[shift + permutationGroup[index] - 1] = message[shift + sourceGroup[index] - 1];
            }
            return new String(dest);
        }

        // Обработка случая кодирования и декодирования с помощью многоалфавитного шифра
        private static void ExecuteMutliAlphabetSubstitutionCase(String message, String sourceAlphabet, params String[] substitutionAlphabets)
        {
            Console.WriteLine($"Source text: {message}");
            // Кодируем сообщение
            String encodedMessage = EncodeByMutliAlphabetSubstitution(message, sourceAlphabet, substitutionAlphabets);
            Console.WriteLine($"Encoded text: {encodedMessage}");
            // Декодируем сообщение
            String decodedMessage = DecodeByMutliAlphabetSubstitution(encodedMessage, sourceAlphabet, substitutionAlphabets);
            Console.WriteLine($"Decoded text: {decodedMessage}");
            // Проверяем, что закодированное сообщение не равно исходному
            if (encodedMessage.Equals(message))
                throw new InvalidOperationException("Bad encoding");
            // Проверяем, что декодированное сообщение равно исходному
            if (!decodedMessage.Equals(message))
                throw new InvalidOperationException("Bad decoding");
        }

        // Кодирование с помощью многоалфавитного шифра
        private static String EncodeByMutliAlphabetSubstitution(String message, String sourceAlphabet, String[] substitutionAlphabets)
        {
            // Создаем массив ассоциативных массивов (Map) между символами алфавитов: исходного и всех подстановочных
            IDictionary<Char, Char>[] substitutionMaps = substitutionAlphabets
                .Select(substitutionAlphabet => CreateSubstitutionMap(sourceAlphabet, substitutionAlphabet))
                .ToArray();
            Char[] dest = new Char[message.Length];
            // Преобразуем каждый символ в исходном алфавите в соответствующий символ из соответствующего алфавита многоалфавитного шифра
            for (Int32 index = 0; index < message.Length; ++index)
                dest[index] = substitutionMaps[index % substitutionAlphabets.Length][message[index]];
            return new String(dest);
        }

        // Декодирование с помощью многоалфавитного шифра
        private static String DecodeByMutliAlphabetSubstitution(String message, String sourceAlphabet, String[] substitutionAlphabets)
        {
            // Создаем массив ассоциативных массивов (Map) между символами алфавитов: всех подстановочных и исходного
            IDictionary<Char, Char>[] substitutionMaps = substitutionAlphabets
                .Select(substitutionAlphabet => CreateSubstitutionMap(substitutionAlphabet, sourceAlphabet))
                .ToArray();
            Char[] dest = new Char[message.Length];
            // Преобразуем каждый символ в соответствующем алвавите многоалфавитного шифра в символ исходного алфавита (операция обратная кодированию)
            for (Int32 index = 0; index < message.Length; ++index)
                dest[index] = substitutionMaps[index % substitutionAlphabets.Length][message[index]];
            return new String(dest);
        }

        // Создаем ассоциативный массив (Map) между символами двух алфавитов
        private static IDictionary<Char, Char> CreateSubstitutionMap(String sourceAlphabet, String substitutionAlphabet)
        {
            return Enumerable.Range(0, sourceAlphabet.Length)
                .ToDictionary(index => sourceAlphabet[index], index => substitutionAlphabet[index]);
        }

        private const String EnglishAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ .,!:;?-";
        private const String RussianAlphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ ";

        private const String EnglishText = "EVERY COMPLEX PROBLEM HAS A SOLUTION THAT IS CLEAR, SIMPLE, AND WRONG.";
        private const String AsciiText = "<<(({{EVERY COMPLEX PROBLEM HAS A SOLUTION THAT IS CLEAR, SIMPLE, AND WRONG.}}))>>";
        private const String RussianText = "ШИФРОВАНИЕ ПРОСТОЙ ПОДСТАНОВКОЙ НА КОРОТКИХ АЛФАВИТАХ ОБЕСПЕЧИВАЕТ СЛАБУЮ ЗАЩИТУ ОТКРЫТОГО ТЕКСТА";

        private const String EnglishSubstitution3 = "Z .XY,!ST:;QR?-NOPLMUVWABCDEFGHIJK";
        private const String RussianSubstitution1 = "БЮГЫЕЬЗШЙЦЛФНТПРСОУМХКЧИЩЖЪДЭВЯ АЁ";
        private const String RussianSubstitution2 = "СОУМКХЧИЩЖЪДЭВЯАБЮГ ЕЬЗШЙЦЁФНТПРЫЛ";
        private const String RussianSubstitution5 = "МНОПРСТУФХЦЧШЩЪЬЫЭЮЯ АБВГДЕЁЖЗИЙКЛ";
    }
}
