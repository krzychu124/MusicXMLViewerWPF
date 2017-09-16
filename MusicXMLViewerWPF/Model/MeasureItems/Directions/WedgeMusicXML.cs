using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.Directions
{
    [Serializable]
    [XmlType(TypeName ="wedge")]
    public class WedgeMusicXML
    {
        private WedgeTypeMusicXML type;
        private string number;
        private double spread;
        private bool spreadSpecified;
        private YesNoMusicXML niente;
        private bool nienteSpecified;
        private LineTypeMusicXML lineType;
        private bool lineTypeSpecified;
        private double dashLength;
        private bool dashLegthSpecified;
        private double spaceLength;
        private bool spaceLengthSpecified;
        private double defaultX;
        private bool defaultXSpecified;
        private double defaultY;
        private bool defaultYSpecified;
        private double relativeX;
        private bool relativeXSpecified;
        private double relativeY;
        private bool relativeYSpecified;
        private string color;

        public WedgeMusicXML()
        {

        }

        [XmlAttribute("type")]
        public WedgeTypeMusicXML Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

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

        [XmlAttribute("spread")]
        public double Spread
        {
            get
            {
                return spread;
            }

            set
            {
                spread = value;
            }
        }

        [XmlIgnore]
        public bool SpreadSpecified
        {
            get
            {
                return spreadSpecified;
            }

            set
            {
                spreadSpecified = value;
            }
        }

        /// <summary>
        /// Niente attribute is yes if a circle appears at the point of the wedge,
        /// indicating a crescendo from nothing or diminuendo to nothing.
        /// </summary>
        [XmlAttribute("niente")]
        public YesNoMusicXML Niente
        {
            get
            {
                return niente;
            }

            set
            {
                niente = value;
            }
        }

        [XmlIgnore]
        public bool NienteSpecified
        {
            get
            {
                return nienteSpecified;
            }

            set
            {
                nienteSpecified = value;
            }
        }

        [XmlAttribute("line-type")]
        public LineTypeMusicXML LineType
        {
            get
            {
                return lineType;
            }

            set
            {
                lineType = value;
            }
        }

        [XmlIgnore]
        public bool LineTypeSpecified
        {
            get
            {
                return lineTypeSpecified;
            }

            set
            {
                lineTypeSpecified = value;
            }
        }

        [XmlAttribute("dash-length")]
        public double DashLength
        {
            get
            {
                return dashLength;
            }

            set
            {
                dashLength = value;
            }
        }

        [XmlIgnore]
        public bool DashLegthSpecified
        {
            get
            {
                return dashLegthSpecified;
            }

            set
            {
                dashLegthSpecified = value;
            }
        }

        [XmlAttribute("space-length")]
        public double SpaceLength
        {
            get
            {
                return spaceLength;
            }

            set
            {
                spaceLength = value;
            }
        }

        [XmlIgnore]
        public bool SpaceLengthSpecified
        {
            get
            {
                return spaceLengthSpecified;
            }

            set
            {
                spaceLengthSpecified = value;
            }
        }

        [XmlAttribute("default-x")]
        public double DefaultX
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
        public double DefaultY
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
        public double RelativeX
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
        public double RelativeY
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
    }

    [Serializable()]
    [XmlType(TypeName = "wedge-type")]
    public enum WedgeTypeMusicXML
    {
        crescendo,
        diminuendo,
        stop,
        @continue,
    }
}
