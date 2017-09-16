using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Defaults
{
    [Serializable]
    public class ScalingMusicXML
    {
        private double millimeters;
        private double tenths;

        [XmlElement("millimeters")]
        public double Millimeters
        {
            get
            {
                return millimeters;
            }

            set
            {
                millimeters = value;
            }
        }

        [XmlElement("tenths")]
        public double Tenths
        {
            get
            {
                return tenths;
            }

            set
            {
                tenths = value;
            }
        }
    }
}
