namespace SourceCodeSimplifierApp.Output
{
    internal interface IOutput
    {
        void WriteInfoLine(String value);
        void WriteInfoLine(String filename, Int32 line, String value);
        void WriteWarningLine(String value);
        void WriteWarningLine(String filename, Int32 line, String value);
        void WriteErrorLine(String value);
        void WriteErrorLine(String filename, Int32 line, String value);
        void WriteFailLine(String value);
        void WriteFailLine(String filename, Int32 line, String value);
    }
}