using SourceCodeSimplifierApp.Output;

namespace SourceCodeSimplifierApp.Processors
{
    internal static class SourceProcessorFactory
    {
        public static ISourceProcessor Create(String source, IOutput output)
        {
            String sourceExtension = Path.GetExtension(source);
            if (String.IsNullOrEmpty(sourceExtension) || !ProcessorsMap.ContainsKey(sourceExtension))
                throw new ArgumentException(nameof(source));
            return ProcessorsMap[sourceExtension](source, output);
        }

        public static Boolean IsSupportedSource(String source)
        {
            String sourceExtension = Path.GetExtension(source);
            return ProcessorsMap.ContainsKey(sourceExtension);
        }

        private static readonly IDictionary<String, Func<String, IOutput, ISourceProcessor>> ProcessorsMap = new Dictionary<String, Func<String, IOutput, ISourceProcessor>>
        {
            //{".sln", (source, output) => new SolutionProcessor(source, output)},
            {".csproj", (source, output) => new ProjectProcessor(source, output)},
            //{".cs", (source, output) => new FileProcessor(source, output)}
        };
    }
}
