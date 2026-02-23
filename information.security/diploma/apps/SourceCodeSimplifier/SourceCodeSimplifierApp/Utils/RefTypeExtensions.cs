namespace SourceCodeSimplifierApp.Utils
{
    internal static class RefTypeExtensions
    {
        public static T Must<T>(this T? source) where T : class
        {
            if (source == null)
                throw new InvalidOperationException("Bad expectation: object must not be null");
            return source;
        }

        public static TDest MustCast<TSource, TDest>(this TSource? source)
            where TSource : class
            where TDest : class
        {
            TDest? dest = source.Must() as TDest;
            if (dest == null)
                throw new InvalidOperationException($"Bad expectation: object must be of {typeof(TDest).FullName} type");
            return dest;
        }
    }
}
