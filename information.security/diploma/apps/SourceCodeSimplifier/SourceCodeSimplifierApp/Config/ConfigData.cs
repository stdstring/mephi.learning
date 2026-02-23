using System.Xml.Serialization;

namespace SourceCodeSimplifierApp.Config
{
    public enum OutputLevel
    {
        [XmlEnum(Name = "Error")]
        Error = 0,
        [XmlEnum(Name = "Warning")]
        Warning = 1,
        [XmlEnum(Name = "Info")]
        Info = 2
    }

    [XmlRoot("BaseConfig")]
    public class BaseConfig
    {
        [XmlElement("Target")]
        public String? Target { get; set; }

        [XmlElement("OutputLevel")]
        public OutputLevel OutputLevel { get; set; } = OutputLevel.Error;
    }

    public enum TransformerState
    {
        [XmlEnum(Name = "Off")]
        Off = 0,
        [XmlEnum(Name = "On")]
        On = 1
    }

    [XmlRoot("Transformer")]
    public class TransformerEntry
    {
        [XmlElement("Name")]
        public String? Name { get; set; }

        [XmlElement("State")]
        public TransformerState State { get; set; } = TransformerState.Off;
    }

    [XmlRoot("Config")]
    public class ConfigData
    {
        [XmlElement("BaseConfig")]
        public BaseConfig? BaseConfig { get; set; }

        [XmlArray("Transformers")]
        [XmlArrayItem("Transformer")]
        public TransformerEntry[]? Transformers { get; set; }
    }
}
