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
    [XmlType(TypeName ="slur")]
    public class SlurMusicXML
    {
        private StartStopContinueMusicXML type;
        private string number;
        private LineTypeMusicXML lineType;
        private bool lineTypeSpecified;
        private double dashLength;
        private bool dashLengthSpecified;
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
        private AboveBelowMusicXML placement;
        private bool placementSpecified;
        private OverUnderMusicXML orientation;
        private bool orientationSpecified;
        private double bezierOffset;
        private bool bezierOffsetSpecified;
        private double bezierOffset2;
        private bool bezierOffset2Specified;
        private double bezierX;
        private bool bezierXSpecified;
        private double bezierY;
        private bool bezierYSpecified;
        private double bezierX2;
        private bool bezierX2Specified;
        private double bezierY2;
        private bool bezierY2Specified;
        private string color;

        public SlurMusicXML()
        {
            this.number = "1";
        }

        [XmlAttribute("type")]
        public StartStopContinueMusicXML Type
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
        [XmlAttribute("placement")]
        public AboveBelowMusicXML Placement
        {
            get
            {
                return placement;
            }

            set
            {
                placement = value;
            }
        }
        [XmlIgnore]
        public bool PlacementSpecified
        {
            get
            {
                return placementSpecified;
            }

            set
            {
                placementSpecified = value;
            }
        }
        [XmlAttribute("orientation")]
        public OverUnderMusicXML Orientation
        {
            get
            {
                return orientation;
            }

            set
            {
                orientation = value;
            }
        }
        [XmlIgnore]
        public bool OrientationSpecified
        {
            get
            {
                return orientationSpecified;
            }

            set
            {
                orientationSpecified = value;
            }
        }
        [XmlAttribute("bezier-offset")]
        public double BezierOffset
        {
            get
            {
                return bezierOffset;
            }

            set
            {
                bezierOffset = value;
            }
        }
        [XmlIgnore]
        public bool BezierOffsetSpecified
        {
            get
            {
                return bezierOffsetSpecified;
            }

            set
            {
                bezierOffsetSpecified = value;
            }
        }
        [XmlAttribute("bezier-offset2")]
        public double BezierOffset2
        {
            get
            {
                return bezierOffset2;
            }

            set
            {
                bezierOffset2 = value;
            }
        }
        [XmlIgnore]
        public bool BezierOffset2Specified
        {
            get
            {
                return bezierOffset2Specified;
            }

            set
            {
                bezierOffset2Specified = value;
            }
        }
        [XmlAttribute("bezier-x")]
        public double BezierX
        {
            get
            {
                return bezierX;
            }

            set
            {
                bezierX = value;
            }
        }
        [XmlIgnore]
        public bool BezierXSpecified
        {
            get
            {
                return bezierXSpecified;
            }

            set
            {
                bezierXSpecified = value;
            }
        }
        [XmlAttribute("bezier-y")]
        public double BezierY
        {
            get
            {
                return bezierY;
            }

            set
            {
                bezierY = value;
            }
        }
        [XmlIgnore]
        public bool BezierYSpecified
        {
            get
            {
                return bezierYSpecified;
            }

            set
            {
                bezierYSpecified = value;
            }
        }
        [XmlAttribute("bezier-x2")]
        public double BezierX2
        {
            get
            {
                return bezierX2;
            }

            set
            {
                bezierX2 = value;
            }
        }
        [XmlIgnore]
        public bool BezierX2Specified
        {
            get
            {
                return bezierX2Specified;
            }

            set
            {
                bezierX2Specified = value;
            }
        }
        [XmlAttribute("bezier-y2")]
        public double BezierY2
        {
            get
            {
                return bezierY2;
            }

            set
            {
                bezierY2 = value;
            }
        }
        [XmlIgnore]
        public bool BezierY2Specified
        {
            get
            {
                return bezierY2Specified;
            }

            set
            {
                bezierY2Specified = value;
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
}
