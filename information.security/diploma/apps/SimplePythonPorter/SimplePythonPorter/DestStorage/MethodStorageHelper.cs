namespace SimplePythonPorter.DestStorage
{
    internal static class MethodStorageHelper
    {
        public static void AddException(this MethodStorage methodStorage, String exceptionFullName, String message, Boolean needIndentation = false)
        {
            const Char delimiter = '.';
            Int32 index = exceptionFullName.LastIndexOf(delimiter);
            if (index != -1)
                methodStorage.ImportStorage.AddImport(exceptionFullName.Substring(0, index));
            if (needIndentation)
                methodStorage.IncreaseLocalIndentation();
            methodStorage.AddBodyLine($"raise {exceptionFullName}(\"{message}\")");
            if (needIndentation)
                methodStorage.DecreaseLocalIndentation();
        }

        public static void AddBodyLineWithIndentation(this MethodStorage methodStorage, String bodyLine)
        {
            methodStorage.IncreaseLocalIndentation();
            methodStorage.AddBodyLine(bodyLine);
            methodStorage.DecreaseLocalIndentation();
        }
    }
}
