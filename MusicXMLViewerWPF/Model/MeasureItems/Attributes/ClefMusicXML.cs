using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.Attributes
{
    [Serializable]
    [XmlType(TypeName ="clef")]
    public class ClefMusicXML
    {
        private ClefSignMusicXML sign;
        private string line;
        private string clefOctaveChange;
        private string number;
        private YesNoMusicXML additional;
        private bool additionalSpecified;
        private SymbolSizeMusicXML size;
        private bool sizeSpecified;
        private YesNoMusicXML afterBarline;
        private bool afterBarlineSpecified;
        private YesNoMusicXML printObject;
        private bool printObjectSpecified;

        public ClefMusicXML()
        {

        }

        [XmlElement("sign")]
        public ClefSignMusicXML Sign
        {
            get
            {
                return sign;
            }

            set
            {
                sign = value;
            }
        }
        [XmlElement("line", DataType ="integer")]
        public string Line
        {
            get
            {
                return line;
            }

            set
            {
                line = value;
            }
        }
        [XmlElement("clef-octave-change", DataType ="positiveInteger")]
        public string ClefOctaveChange
        {
            get
            {
                return clefOctaveChange;
            }

            set
            {
                clefOctaveChange = value;
            }
        }
        [XmlAttribute("number", DataType ="positiveInteger")]
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
        [XmlAttribute("additional")]
        public YesNoMusicXML Additional
        {
            get
            {
                return additional;
            }

            set
            {
                additional = value;
            }
        }
        [XmlIgnore]
        public bool AdditionalSpecified
        {
            get
            {
                return additionalSpecified;
            }

            set
            {
                additionalSpecified = value;
            }
        }
        [XmlAttribute("size")]
        public SymbolSizeMusicXML Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }
        [XmlIgnore]
        public bool SizeSpecified
        {
            get
            {
                return sizeSpecified;
            }

            set
            {
                sizeSpecified = value;
            }
        }
        [XmlAttribute("after-barline")]
        public YesNoMusicXML AfterBarline
        {
            get
            {
                return afterBarline;
            }

            set
            {
                afterBarline = value;
            }
        }
        [XmlIgnore]
        public bool AfterBarlineSpecified
        {
            get
            {
                return afterBarlineSpecified;
            }

            set
            {
                afterBarlineSpecified = value;
            }
        }
        [XmlAttribute("print-object")]
        public YesNoMusicXML PrintObject
        {
            get
            {
                return printObject;
            }

            set
            {
                printObject = value;
            }
        }
        [XmlIgnore]
        public bool PrintObjectSpecified
        {
            get
            {
                return printObjectSpecified;
            }

            set
            {
                printObjectSpecified = value;
            }
        }
    }
    [Serializable]
    [XmlType(TypeName ="clef-sign")]
    public enum ClefSignMusicXML
    {
        G,
        F,
        C,
        percussion,
        TAB,
        jianpu,
        none,
    }
}