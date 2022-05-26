using System.Xml.Serialization;

namespace EasyUpdate
{
    [XmlRoot("updateinfo")]
    public class UpdateInfo
    {
        [XmlElement("version")]
        public string Version;
        [XmlElement("url")]
        public UrlInfo Url;
        [XmlElement("forced")]
        public ForcedMode Forced = ForcedMode.Normal;
        [XmlElement("checksum", IsNullable = true)]
        public Checksum Checksum;
        [XmlElement("changelog", IsNullable = true)]
        public string ChangeLog;
        [XmlIgnore]
        public bool IsUpdateAvailable;
    }

    public class UrlInfo
    {
        [XmlText]
        public string Url;
        [XmlAttribute("encrypted")]
        public bool IsEncrypted = false;
    }

    public enum ForcedMode
    {
        [XmlEnum("normal")]
        Normal,
        [XmlEnum("forced")]
        Forced,
        [XmlEnum("forceddownload")]
        ForcedDownload
    }

    public class Checksum
    {
        [XmlText]
        public string Value;
        [XmlAttribute("algorithm")]
        public string Algorithm;
    }
}
