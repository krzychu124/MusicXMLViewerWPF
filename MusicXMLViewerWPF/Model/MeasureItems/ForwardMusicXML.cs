using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems
{
    [Serializable]
    [XmlType(TypeName = "forward")]
    public class ForwardMusicXML
    {
        private double duration;
        //formattedTest footnote
        //level levelField;
        private string voice;
        private string staff;

        public ForwardMusicXML()
        {

        }

        [XmlElement("duration")]
        public double Duration
        {
            get
            {
                return duration;
            }

            set
            {
                duration = value;
            }
        }
        [XmlElement("voice")]
        public string Voice
        {
            get
            {
                return voice;
            }

            set
            {
                voice = value;
            }
        }
        [XmlElement("staff", DataType ="positiveInteger")]
        public string Staff
        {
            get
            {
                return staff;
            }

            set
            {
                staff = value;
            }
        }
    }
}