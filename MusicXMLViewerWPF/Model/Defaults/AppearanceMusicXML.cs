using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Defaults
{
    [Serializable]
    [XmlType(TypeName ="appearance")]
    public class AppearanceMusicXML
    {
        private List<LineWidthMusicXML> lineWidth;
        private List<NoteSizeMusicXML> noteSize;
        private List<DistanceMusicXML> distance;
        private List<OtherAppearanceMusicXML> otherAppearance;

        [XmlElement("line-width")]
        public List<LineWidthMusicXML> LineWidth
        {
            get
            {
                return lineWidth;
            }

            set
            {
                lineWidth = value;
            }
        }

        [XmlElement("note-size")]
        public List<NoteSizeMusicXML> NoteSize
        {
            get
            {
                return noteSize;
            }

            set
            {
                noteSize = value;
            }
        }

        [XmlElement("distance")]
        public List<DistanceMusicXML> Distance
        {
            get
            {
                return distance;
            }

            set
            {
                distance = value;
            }
        }

        [XmlElement("other-appearance")]
        public List<OtherAppearanceMusicXML> OtherAppearance
        {
            get
            {
                return otherAppearance;
            }

            set
            {
                otherAppearance = value;
            }
        }

        public AppearanceMusicXML()
        {

        }
    }

    [Serializable]
    [XmlType(TypeName = "line-width")]
    public class LineWidthMusicXML
    {
        private string type;
        private double value;

        [XmlAttribute("type")]
        public string Type
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

        [XmlText(DataType ="double")]//! test
        public double Value
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
    [XmlType(TypeName = "note-size")]
    public class NoteSizeMusicXML
    {
        private NoteSizeTypeMusicXML type;
        private double value;

        [XmlAttribute("type")]
        public NoteSizeTypeMusicXML Type
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

        [XmlText(DataType = "double")]//! test
        public double Value
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
    [XmlType(TypeName = "distance")]
    public class DistanceMusicXML
    {
        private string type;
        private double value;

        [XmlAttribute("type")]
        public string Type
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

        [XmlText(DataType = "double")]//! test
        public double Value
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
    [XmlType(TypeName = "other-appearance")]
    public class OtherAppearanceMusicXML
    {
        private string type;
        private string value;

        [XmlAttribute("type")]
        public string Type
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

        [XmlText]//! test
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
