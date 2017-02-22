using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.Attributes
{
    [Serializable]
    [XmlType(TypeName ="part-symbol")]
    public class PartSymbolMusicXML //TODO_H test!!!
    {
        private string topStaff;
        private string bottomStaff;
        private decimal defaultX;
        private bool defaultXSpecified;
        private decimal defaultY;
        private bool defaultYSpecified;
        private decimal relativeX;
        private bool relativeXSpecified;
        private decimal relativeY;
        private bool relativeYSpecified;
        private string color;
        private GroupSymbolValueMusicXML value;

        [XmlAttribute("top-staff", DataType ="positiveInteger")]
        public string TopStaff
        {
            get
            {
                return topStaff;
            }

            set
            {
                topStaff = value;
            }
        }
        [XmlAttribute("bottom-staff", DataType ="positiveInteger")]
        public string BottomStaff
        {
            get
            {
                return bottomStaff;
            }

            set
            {
                bottomStaff = value;
            }
        }
        [XmlAttribute("default-x")]
        public decimal DefaultX
        {
            get
            {
                return defaultX;
            }

            set
            {
                defaultX = value;
            }
        }
        [XmlIgnore]
        public bool DefaultXSpecified
        {
            get
            {
                return defaultXSpecified;
            }

            set
            {
                defaultXSpecified = value;
            }
        }
        [XmlAttribute("default-y")]
        public decimal DefaultY
        {
            get
            {
                return defaultY;
            }

            set
            {
                defaultY = value;
            }
        }
        [XmlIgnore]
        public bool DefaultYSpecified
        {
            get
            {
                return defaultYSpecified;
            }

            set
            {
                defaultYSpecified = value;
            }
        }
        [XmlAttribute("relative-x")]
        public decimal RelativeX
        {
            get
            {
                return relativeX;
            }

            set
            {
                relativeX = value;
            }
        }
        [XmlIgnore]
        public bool RelativeXSpecified
        {
            get
            {
                return relativeXSpecified;
            }

            set
            {
                relativeXSpecified = value;
            }
        }
        [XmlAttribute("relative-y")]
        public decimal RelativeY
        {
            get
            {
                return relativeY;
            }

            set
            {
                relativeY = value;
            }
        }
        [XmlIgnore]
        public bool RelativeYSpecified
        {
            get
            {
                return relativeYSpecified;
            }

            set
            {
                relativeYSpecified = value;
            }
        }
        [XmlAttribute("color")]
        public string Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
            }
        }
        [XmlText]
        public GroupSymbolValueMusicXML Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }
    }

    [Serializable]
    [XmlType(TypeName ="group-symbol-value")]
    public enum GroupSymbolValueMusicXML
    {
        none,
        brace,
        line,
        bracket,
        square,
    }
}