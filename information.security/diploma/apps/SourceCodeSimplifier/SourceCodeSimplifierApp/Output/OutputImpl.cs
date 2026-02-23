using SourceCodeSimplifierApp.Config;

namespace SourceCodeSimplifierApp.Output
{
    // TODO (std_string) : think about using of logger - alternate approach
    // TODO (std_string) : think about name
    internal class OutputImpl : IOutput
    {
        public OutputImpl(TextWriter output, TextWriter error, OutputLevel outputLevel)
        {
            Output = output;
            Error = error;
            OutputLevel = outputLevel;
        }

        public void WriteInfoLine(String value)
        {
            if (OutputLevel >= OutputLevel.Info)
                WriteLine(Output, InfoPrefix, value);
        }

        public void WriteInfoLine(String filename, Int32 line, String value)
        {
            if (OutputLevel >= OutputLevel.Info)
                WriteLine(Output, InfoPrefix, filename, line, value);
        }

        public void WriteWarningLine(String value)
        {
            if (OutputLevel >= OutputLevel.Warning)
                WriteLine(Output, WarningPrefix, value);
        }

        public void WriteWarningLine(String filename, Int32 line, String value)
        {
            if (OutputLevel >= OutputLevel.Warning)
                WriteLine(Output, WarningPrefix, filename, line, value);
        }

        public void WriteErrorLine(String value)
        {
            if (OutputLevel >= OutputLevel.Error)
                WriteLine(Output, ErrorPrefix, value);
        }

        public void WriteErrorLine(String filename, Int32 line, String value)
        {
            if (OutputLevel >= OutputLevel.Error)
                WriteLine(Output, ErrorPrefix, filename, line, value);
        }

        public void WriteFailLine(String value)
        {
            WriteLine(Error, ErrorPrefix, value);
        }

        public void WriteFailLine(String filename, Int32 line, String value)
        {
            WriteLine(Error, ErrorPrefix, filename, line, value);
        }

        private TextWriter Output { get; }

        private TextWriter Error { get; }

        private OutputLevel OutputLevel { get; }

        private static void WriteLine(TextWriter writer, String prefix, String value)
        {
            writer.Write(prefix);
            writer.WriteLine(value);
        }

        private static void WriteLine(TextWriter writer, String prefix, String filename, Int32 line, String value)
        {
            // line is zero-based
            writer.WriteLine($"{filename}({line + 1}): {prefix}{value}");
        }

        private const String ErrorPrefix = "[ERROR]: ";
        private const String WarningPrefix = "[WARNING]: ";
        private const String InfoPrefix = "";
    }
}
