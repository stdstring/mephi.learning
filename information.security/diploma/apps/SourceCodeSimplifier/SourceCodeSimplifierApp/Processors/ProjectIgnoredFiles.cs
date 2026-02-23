namespace SourceCodeSimplifierApp.Processors
{
    internal static class ProjectIgnoredFiles
    {
        public static bool IgnoreFile(string filename)
        {
            return KnownIgnoredFilesSuffix.Any(filename.EndsWith);
        }

        // TODO (std_string) : think about another approach (via config)
        private static readonly string[] KnownIgnoredFilesSuffix =
        {
            ".AssemblyInfo.cs",
            ".AssemblyAttributes.cs",
            ".GlobalUsings.g.cs"
        };
    }
}
