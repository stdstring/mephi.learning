namespace SimplePythonPorter.Utils
{
    internal static class RefTypeExtensions
    {
        public static T Must<T>(this T? source, String description = "") where T : class
        {
            if (source == null)
                throw new InvalidOperationException(String.IsNullOrEmpty(description)
                    ? "Bad expectation: object must not be null"
                    : description);
            return source;
        }

        public static TDest MustCast<TSource, TDest>(this TSource? source, String description = "")
            where TSource : class
            where TDest : class
        {
            TDest? dest = source.Must() as TDest;
            if (dest == null)
                throw new InvalidOperationException(String.IsNullOrEmpty(description)
                    ? $"Bad expectation: object must be of {typeof(TDest).FullName} type"
                    : description);
            return dest;
        }
    }
}
