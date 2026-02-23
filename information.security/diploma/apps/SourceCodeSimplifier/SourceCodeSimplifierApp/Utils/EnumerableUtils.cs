namespace SourceCodeSimplifierApp.Utils
{
    internal static class EnumerableUtils
    {
        public static Boolean IsEmpty<TElement>(this IEnumerable<TElement> source)
        {
            return !source.Any();
        }

        public static Boolean IsEmpty<TElement>(this ICollection<TElement> source)
        {
            return source.Count == 0;
        }

        public static void ForEach<TElement>(this IEnumerable<TElement> source, Action<TElement> action)
        {
            foreach (TElement element in source)
                action(element);
        }

        public static void AddRange<TElement>(this ICollection<TElement> dest, IEnumerable<TElement> source)
        {
            foreach (TElement element in source)
                dest.Add(element);
        }
    }
}
