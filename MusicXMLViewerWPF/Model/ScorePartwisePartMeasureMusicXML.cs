using MusicXMLScore.Model.Helpers.SimpleTypes;
using MusicXMLScore.Model.MeasureItems;
using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model
{
    [Serializable]
    [XmlType(AnonymousType =true)]
    public class ScorePartwisePartMeasureMusicXML
    {
        private object[] items;
        private string number;
        private YesNoMusicXML implicitField = YesNoMusicXML.no;
        private bool implicitFieldSpecified;
        private YesNoMusicXML nonControlling = YesNoMusicXML.no;
        private bool nonControllingSpecified;
        private double width;
        private bool widthSpecified;
        private double calculatedWidth;


        [XmlAttribute("number")]
        public string Number
        {
            get
            {
                return number;
            }

            set
            {
                number = value;
            }
        }
        [XmlAttribute("implicit")]
        public YesNoMusicXML ImplicitField
        {
            get
            {
                return implicitField;
            }

            set
            {
                implicitField = value;
            }
        }
        [XmlIgnore]
        public bool ImplicitFieldSpecified
        {
            get
            {
                return implicitFieldSpecified;
            }

            set
            {
                implicitFieldSpecified = value;
            }
        }
        [XmlAttribute("non-controlling")]
        public YesNoMusicXML NonControlling
        {
            get
            {
                return nonControlling;
            }

            set
            {
                nonControlling = value;
            }
        }
        [XmlIgnore]
        public bool NonControllingSpecified
        {
            get
            {
                return nonControllingSpecified;
            }

            set
            {
                nonControllingSpecified = value;
            }
        }
        [XmlAttribute("width")]
        public double Width
        {
            get
            {
                return width;
            }

            set
            {
                width = value;
            }
        }
        [XmlIgnore]
        public bool WidthSpecified
        {
            get
            {
                return widthSpecified;
            }

            set
            {
                widthSpecified = value;
            }
        }
        [XmlElement("attributes", typeof(AttributesMusicXML))]
        [XmlElement("backup", typeof(BackupMusicXML))]
        [XmlElement("barline", typeof(BarlineMusicXML))]
        [XmlElement("direction", typeof(DirectionMusicXML))]
        [XmlElement("figured-bass", typeof(FiguredbassMusicXML))]
        [XmlElement("forward", typeof(ForwardMusicXML))]
        [XmlElement("grouping", typeof(GroupingMusicXML))]
        [XmlElement("harmony", typeof(HarmonyMusicXML))]
        [XmlElement("note", typeof(NoteMusicXML))]
        [XmlElement("print", typeof(PrintMusicXML))]
        [XmlElement("sound", typeof(SoundMusicXML))]
        public object[] Items
        {
            get
            {
                return items;
            }

            set
            {
                items = value;
            }
        }
        [XmlIgnore]
        public double CalculatedWidth
        {
            get
            {
                return calculatedWidth;
            }

            set
            {
                calculatedWidth = value;
            }
        }

        public ScorePartwisePartMeasureMusicXML()
        {

        }
    }
}