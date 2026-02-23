namespace SourceCodeSimplifierAppTests
{
    internal static class SourceCodeSimplifierAppOutputDef
    {
        public const String BadArgsMessage = "[ERROR]: Bad args";
        public const String UnknownConfigMessage = "[ERROR]: Unknown config";
        public const String BadConfigMessage = "[ERROR]: Bad config path";
        public const String UnknownTargetMessage = "[ERROR]: Unknown Config.BaseConfig.Target";
        public const String UnsupportedTargetMessage = "[ERROR]: Unsupported Config.BaseConfig.Target";

        public const String AppDescription = "Application usage:\r\n" +
                                             "1. <app> --config=<path to config file>\r\n" +
                                             "2. <app> --help\r\n" +
                                             "3. <app> --version\r\n";

        public const String CompilationCheckSuccessOutput = "Checking compilation for errors and warnings:\r\n" +
                                                            "Found 0 errors in the compilation\r\n" +
                                                            "Found 0 warnings in the compilation\r\n";
    }
}