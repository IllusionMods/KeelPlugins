using System.Xml.Serialization;

namespace FreeHDefaults.Koikatu
{
    [XmlRoot]
    public class Savedata
    {
        [XmlElement]
        public string HeroinePath;
        
        [XmlElement]
        public string PartnerPath;
        
        [XmlElement]
        public string PlayerPath;

        [XmlElement]
        public int map;

        [XmlElement]
        public int timeZone;

        [XmlElement]
        public int stageH1;

        [XmlElement]
        public int stageH2;

        [XmlElement]
        public int statusH;

        [XmlElement]
        public bool discovery;

        [XmlElement]
        public int category;

        [XmlElement]
        public bool isAibuSelect = true;

        [XmlElement]
        public float Fov = 23f;
    }
}