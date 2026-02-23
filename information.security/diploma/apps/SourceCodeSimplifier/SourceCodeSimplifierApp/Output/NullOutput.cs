namespace SourceCodeSimplifierApp.Output
{
    internal class NullOutput : IOutput
    {
        public void WriteInfoLine(String value)
        {
        }

        public void WriteInfoLine(String filename, Int32 line, String value)
        {
        }

        public void WriteWarningLine(String value)
        {
        }

        public void WriteWarningLine(String filename, Int32 line, String value)
        {
        }

        public void WriteErrorLine(String value)
        {
        }

        public void WriteErrorLine(String filename, Int32 line, String value)
        {
        }

        public void WriteFailLine(String value)
        {
        }

        public void WriteFailLine(String filename, Int32 line, String value)
        {
        }
    }
}
