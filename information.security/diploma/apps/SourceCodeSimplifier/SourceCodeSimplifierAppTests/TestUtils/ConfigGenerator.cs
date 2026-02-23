using System.Xml.Serialization;
using SourceCodeSimplifierApp.Config;

namespace SourceCodeSimplifierAppTests.TestUtils
{
    internal static class ConfigGenerator
    {
        public static String Generate(String configPrefix, String target)
        {
            return Generate(configPrefix, target, OutputLevel.Error, new Dictionary<string, TransformerState>());
        }

        public static String Generate(String configPrefix, String target, OutputLevel outputLevel)
        {
            return Generate(configPrefix, target, outputLevel, new Dictionary<String, TransformerState>());
        }

        public static String Generate(String configPrefix, String target, OutputLevel outputLevel, IDictionary<String, TransformerState> transformers)
        {
            ConfigData configData = new ConfigData
            {
                BaseConfig = new BaseConfig {Target = target, OutputLevel = outputLevel},
                Transformers = transformers.Select(kvPair => new TransformerEntry {Name = kvPair.Key, State = kvPair.Value}).ToArray()
            };
            DateTime now = DateTime.Now;
            String timePart = $"{now.Year}{now.Month:00}{now.Day:00}.{now.Hour:00}{now.Minute:00}{now.Second:00}{now.Millisecond:000}";
            String filePath = Path.GetFullPath($"./{configPrefix}.{timePart}{ConfigSuffix}");
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigData));
                serializer.Serialize(writer, configData);
            }
            return filePath;
        }

        public const String ConfigSuffix = ".testconfig.xml";
    }
}