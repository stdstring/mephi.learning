using System.Xml.Serialization;
using SourceCodeSimplifierApp.Processors;

namespace SourceCodeSimplifierApp.Config
{
    internal record AppConfig(ConfigData Config);

    internal static class AppConfigFactory
    {
        public static AppConfig Create(AppArgsResult.MainConfig config)
        {
            if (!File.Exists(config.ConfigPath))
                throw new InvalidOperationException("Unknown config");
            using (StreamReader reader = new StreamReader(config.ConfigPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigData));
                ConfigData? configData = serializer.Deserialize(reader) as ConfigData;
                if (configData == null)
                    throw new InvalidOperationException("Bad config data");
                (Boolean Success, String Reason) checkResult = ConfigDataChecker.Check(configData);
                if (!checkResult.Success)
                    throw new InvalidOperationException(checkResult.Reason);
                return new AppConfig(configData);
            }
        }
    }

    internal static class ConfigDataChecker
    {
        public static (Boolean Success, String Reason) Check(ConfigData config)
        {
            if (config.BaseConfig == null)
                return (false, "Bad Config.BaseConfig");
            if (config.BaseConfig.Target == null)
                return (false, "Bad Config.BaseConfig.Target");
            if (!File.Exists(config.BaseConfig.Target))
                return (false, "Unknown Config.BaseConfig.Target");
            if (!SourceProcessorFactory.IsSupportedSource(config.BaseConfig.Target))
                return (false, "Unsupported Config.BaseConfig.Target");
            if ((config.Transformers != null) && config.Transformers.Any(entry => String.IsNullOrEmpty(entry.Name)))
                return (false, "Bad Config.Transformers entries");
            return (true, "");
        }
    }
}
