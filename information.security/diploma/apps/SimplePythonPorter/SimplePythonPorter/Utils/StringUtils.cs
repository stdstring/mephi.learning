using System.Text;

namespace SimplePythonPorter.Utils
{
    internal static class StringUtils
    {
        public static String Escape(String source)
        {
            StringBuilder dest = new StringBuilder();
            foreach (Char ch in source)
            {
                switch (ch)
                {
                    case '"':
                        dest.Append("\\\"");
                        break;
                    case '\r':
                        dest.Append("\\\\r");
                        break;
                    case '\n':
                        dest.Append("\\\\n");
                        break;
                    case '\t':
                        dest.Append("\\\\t");
                        break;
                    case '\\':
                        dest.Append("\\\\");
                        break;
                    default:
                        dest.Append(ch);
                        break;
                }
            }
            return dest.ToString();
        }

        public static String ConvertEscapeSequences(String source)
        {
            const Int32 maxHexSize = 4;
            String GetHexPart(Int32 start)
            {
                StringBuilder dest = new StringBuilder();
                for (Int32 shift = 0; (shift < maxHexSize) && (start + shift < source.Length); ++shift)
                {
                    if (!Char.IsAsciiHexDigit(source[start + shift]))
                        break;
                    dest.Append(Char.ToLower(source[start + shift]));
                }
                return dest.ToString();
            }
            Int32 maxIndex = source.Length - 1;
            StringBuilder dest = new StringBuilder();
            for (Int32 index = 0; index < source.Length;)
            {
                if ((index < maxIndex) && (source[index] == '\\') && (source[index + 1] == 'x'))
                {
                    String hexPart = GetHexPart(index + 2);
                    String zeroPrefix = new String('0', maxHexSize - hexPart.Length);
                    dest.Append($"\\u{zeroPrefix}{hexPart}");
                    index += 2 + hexPart.Length;
                }
                else
                    dest.Append(source[index++]);
            }
            return dest.ToString();
        }

        public static String PrepareVerbatimString(String source)
        {
            return source.Replace("\\", "\\\\");
        }

        public static String UnquoteString(String source)
        {
            if (source.Length == 0)
                return source;
            Int32 startPosition = 0;
            Int32 endPosition = source.Length - 1;
            if (source[startPosition] == '@')
                ++startPosition;
            if ((startPosition <= endPosition) && (source[startPosition] == '"'))
                ++startPosition;
            if (source[endPosition] == '"')
                --endPosition;
            return startPosition <= endPosition ?
                   source.Substring(startPosition, endPosition - startPosition + 1) :
                   "";
        }
    }
}
