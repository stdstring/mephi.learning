namespace SourceCodeSimplifierApp.Config
{
    internal abstract record AppArgsResult
    {
        internal record VersionConfig(String Version) : AppArgsResult;

        internal record HelpConfig(String Help) : AppArgsResult;

        internal record MainConfig(String ConfigPath) : AppArgsResult;

        internal record WrongConfig(String Reason, String Help) : AppArgsResult;
    }

    internal static class AppArgsParser
    {
        public static AppArgsResult Parse(String[] args)
        {
            return args switch
            {
                [] => new AppArgsResult.HelpConfig(Help),
                [HelpKey] => new AppArgsResult.HelpConfig(Help),
                [VersionKey] => new AppArgsResult.VersionConfig(Version),
                [var arg] when arg.StartsWith(ConfigKey) => ProcessMainConfig(arg),
                _ => new AppArgsResult.WrongConfig("Bad args", Help)
            };
        }

        private static AppArgsResult.MainConfig ProcessMainConfig(String arg)
        {
            String configPath = arg.Substring(ConfigKey.Length);
            if (String.IsNullOrEmpty(configPath))
                throw new InvalidOperationException("Bad config path");
            return new AppArgsResult.MainConfig(configPath);
        }

        private const String ConfigKey = "--config=";

        private const String HelpKey = "--help";

        private const String VersionKey = "--version";

        private const String Version = "0.0.1";

        private const String Help = "Application usage:\r\n" +
                                    "1. <app> --config=<path to config file>\r\n" +
                                    "2. <app> --help\r\n" +
                                    "3. <app> --version\r\n";
    }
}
