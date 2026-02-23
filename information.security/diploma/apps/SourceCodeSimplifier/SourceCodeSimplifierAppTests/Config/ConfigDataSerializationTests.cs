using NUnit.Framework;
using SourceCodeSimplifierApp.Config;
using System.Xml.Serialization;

namespace SourceCodeSimplifierAppTests.Config
{
    [TestFixture]
    public class ConfigDataSerializationTests
    {
        [Test]
        public void DeserializeWithBaseConfigOnly()
        {
            const String source = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
                                  "<Config>\r\n" +
                                  "  <BaseConfig>\r\n" +
                                  "    <Target>..\\source\\someproj.csproj</Target>\r\n" +
                                  "    <OutputLevel>Warning</OutputLevel>\r\n" +
                                  "  </BaseConfig>\r\n" +
                                  "</Config>";
            ConfigData expected = new ConfigData
            {
                BaseConfig = new BaseConfig
                {
                    Target = "..\\source\\someproj.csproj",
                    OutputLevel = OutputLevel.Warning
                },
                Transformers = Array.Empty<TransformerEntry>()
            };
            CheckDeserialization(expected, source);
        }

        [Test]
        public void DeserializeWithBaseConfigOnlyWithDefaultOutputLevel()
        {
            const String source = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
                                  "<Config>\r\n" +
                                  "  <BaseConfig>\r\n" +
                                  "    <Target>..\\source\\someproj.csproj</Target>\r\n" +
                                  "  </BaseConfig>\r\n" +
                                  "</Config>";
            ConfigData expected = new ConfigData
            {
                BaseConfig = new BaseConfig
                {
                    Target = "..\\source\\someproj.csproj",
                    OutputLevel = OutputLevel.Error
                },
                Transformers = Array.Empty<TransformerEntry>()
            };
            CheckDeserialization(expected, source);
        }

        [Test]
        public void DeserializeWithBaseConfigWithAnalyzers()
        {
            const String source = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
                                  "<Config>\r\n" +
                                  "  <BaseConfig>\r\n" +
                                  "    <Target>..\\source\\someproj.csproj</Target>\r\n" +
                                  "    <OutputLevel>Warning</OutputLevel>\r\n" +
                                  "  </BaseConfig>\r\n" +
                                  "  <Transformers>\r\n" +
                                  "    <Transformer>\r\n" +
                                  "      <Name>SourceCodeSimplifierApp.Transformers.SomeTransformer</Name>\r\n" +
                                  "      <State>Off</State>\r\n" +
                                  "    </Transformer>\r\n" +
                                  "    <Transformer>\r\n" +
                                  "      <Name>SourceCodeSimplifierApp.Transformers.OtherTransformer</Name>\r\n" +
                                  "      <State>On</State>\r\n" +
                                  "    </Transformer>\r\n" +
                                  "  </Transformers>\r\n" +
                                  "</Config>";
            ConfigData expected = new ConfigData
            {
                BaseConfig = new BaseConfig
                {
                    Target = "..\\source\\someproj.csproj",
                    OutputLevel = OutputLevel.Warning
                },
                Transformers = new[]
                {
                    new TransformerEntry{Name = "SourceCodeSimplifierApp.Transformers.SomeTransformer", State = TransformerState.Off},
                    new TransformerEntry{Name = "SourceCodeSimplifierApp.Transformers.OtherTransformer", State = TransformerState.On},
                }
            };
            CheckDeserialization(expected, source);
        }

        [Test]
        public void DeserializeWithBaseConfigWithAnalyzersWithDefaultState()
        {
            const String source = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
                                  "<Config>\r\n" +
                                  "  <BaseConfig>\r\n" +
                                  "    <Target>..\\source\\someproj.csproj</Target>\r\n" +
                                  "    <OutputLevel>Warning</OutputLevel>\r\n" +
                                  "  </BaseConfig>\r\n" +
                                  "  <Transformers>\r\n" +
                                  "    <Transformer>\r\n" +
                                  "      <Name>SourceCodeSimplifierApp.Transformers.SomeTransformer</Name>\r\n" +
                                  "    </Transformer>\r\n" +
                                  "    <Transformer>\r\n" +
                                  "      <Name>SourceCodeSimplifierApp.Transformers.OtherTransformer</Name>\r\n" +
                                  "    </Transformer>\r\n" +
                                  "  </Transformers>\r\n" +
                                  "</Config>";
            ConfigData expected = new ConfigData
            {
                BaseConfig = new BaseConfig
                {
                    Target = "..\\source\\someproj.csproj",
                    OutputLevel = OutputLevel.Warning
                },
                Transformers = new[]
                {
                    new TransformerEntry{Name = "SourceCodeSimplifierApp.Transformers.SomeTransformer", State = TransformerState.Off},
                    new TransformerEntry{Name = "SourceCodeSimplifierApp.Transformers.OtherTransformer", State = TransformerState.Off}
                }
            };
            CheckDeserialization(expected, source);
        }

        private void CheckDeserialization(ConfigData expected, String actualSource)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigData));
            using (StringReader reader = new StringReader(actualSource))
            {
                ConfigData? actual = serializer.Deserialize(reader) as ConfigData;
                Assert.That(actual, Is.Not.Null);
                CheckConfigData(expected, actual!);
            }
        }

        private void CheckConfigData(ConfigData expected, ConfigData actual)
        {
            Assert.That(actual.BaseConfig, Is.Not.Null);
            CheckBaseConfig(expected.BaseConfig!, actual.BaseConfig!);
            CheckTransformers(expected.Transformers!, actual.Transformers);
        }

        private void CheckBaseConfig(BaseConfig expected, BaseConfig actual)
        {
            Assert.That(actual.Target, Is.EqualTo(expected.Target));
            Assert.That(actual.OutputLevel, Is.EqualTo(expected.OutputLevel));
        }

        private void CheckTransformers(TransformerEntry[] expected, TransformerEntry[]? actual)
        {
            if (expected.Length == 0)
                Assert.That(actual == null || actual.Length == 0);
            else
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual!.Length, Is.EqualTo(expected.Length));
                for (Int32 index = 0; index < expected.Length; ++index)
                    CheckTransformer(expected[index], actual[index]);
            }
        }

        private void CheckTransformer(TransformerEntry expected, TransformerEntry actual)
        {
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.State, Is.EqualTo(expected.State));
        }
    }
}
