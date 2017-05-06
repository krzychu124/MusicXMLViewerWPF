using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.NoteItems.Notations
{
    [Serializable]
    [XmlType(TypeName ="glissando")]
    public class GlissandoMusicXML 
    {
        private StartStopMusicXML type;
        private string number;
        private LineTypeMusicXML lineType;
        private bool lineTypeSpecified;
        private double dashLength;
        private bool dashLengthSpecified;
        private double spaceLength;
        private bool spaceLengthSpecified;
        private string value;

        public GlissandoMusicXML()
        {
            this.number = "1";
        }

        [XmlAttribute("type")]
        public StartStopMusicXML Type
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

        [XmlAttribute("number", DataType = "positiveInteger")]
        [System.ComponentModel.DefaultValue("1")]
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
        public double Spacelength
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

        [XmlText]
        public string Value
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
}
