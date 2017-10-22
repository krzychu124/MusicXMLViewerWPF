using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.Directions
{
    [Serializable]
    [XmlType(TypeName ="octave-shift")]
    public class OctaveShiftMusicXML
    {
        private UpDownStopContinueMusicXML type;
        private string number;
        private string size;
        private double dashLength;
        private bool dashLengthSpecified;
        private double spaceLength;
        private bool spaceLengthSpecified;

        public OctaveShiftMusicXML()
        {
            Size = "8";
        }

        [XmlAttribute("type")]
        public UpDownStopContinueMusicXML Type
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

        [XmlAttribute("size", DataType ="positiveInteger")]
        [System.ComponentModel.DefaultValue("8")]
        public string Size
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
        public bool DashLengthSpecified
        {
            get
            {
                return dashLengthSpecified;
            }

            set
            {
                dashLengthSpecified = value;
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
    }
}
