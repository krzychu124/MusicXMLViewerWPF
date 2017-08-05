using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MusicXMLScore.Model.Helpers;
using System.Diagnostics;

namespace MusicXMLScore.Model.Defaults
{
    [Serializable]
    [XmlType(TypeName ="system-layout")]
    [DebuggerDisplay("Margins L,P: {systemMargins.leftMargin}, {systemMargins.rightMargin};  S=> {systemDistance} T=> {topSystemDistance}")]
    public class SystemLayoutMusicXML
    {
        private SystemMarginsMusicXML systemMargins = new SystemMarginsMusicXML();
        private bool systemMarginsSpecified;
        private double systemDistance;
        private bool systemDistanceSpecified;
        private double topSystemDistance;
        private bool topSystemDistanceSpecified;
        private SystemDividersMusicXML systemDividers;

        [XmlElement("system-margins")]
        public SystemMarginsMusicXML SystemMargins
        {
            get
            {
                return systemMargins;
            }

            set
            {
                systemMargins = value;
            }
        }
        
        [XmlElement("system-distance")]
        public double SystemDistance
        {
            get
            {
                return systemDistance;
            }

            set
            {
                systemDistance = value;
                systemDistanceSpecified = true;
            }
        }

        [XmlIgnore]
        public bool SystemDistanceSpecified
        {
            get
            {
                return systemDistanceSpecified;
            }

            set
            {
                systemDistanceSpecified = value;
            }
        }

        [XmlElement("top-system-distance")]
        public double TopSystemDistance
        {
            get
            {
                return topSystemDistance;
            }

            set
            {
                topSystemDistance = value;
                topSystemDistanceSpecified = true;
            }
        }

        [XmlIgnore]
        public bool TopSystemDistanceSpecified
        {
            get
            {
                return topSystemDistanceSpecified;
            }

            set
            {
                topSystemDistanceSpecified = value;
            }
        }

        [XmlElement("system-dividers")]
        public SystemDividersMusicXML SystemDividers
        {
            get
            {
                return systemDividers;
            }

            set
            {
                systemDividers = value;
            }
        }
    }

    [Serializable]
    [XmlType(TypeName ="system-margins")]
    public class SystemMarginsMusicXML
    {
        private double leftMargin = 0.0;
        private double rightMargin = 0.0;

        [XmlElement("left-margin")]
        public double LeftMargin
        {
            get
            {
                return leftMargin;
            }

            set
            {
                leftMargin = value;
            }
        }

        [XmlElement("right-margin")]
        public double RightMargin
        {
            get
            {
                return rightMargin;
            }

            set
            {
                rightMargin = value;
            }
        }
    }

    [Serializable]
    [XmlType(TypeName = "system-dividers")]
    public class SystemDividersMusicXML
    {
        private EmptyPrintObjectStyleAlignMusicXML leftDivider;
        private EmptyPrintObjectStyleAlignMusicXML rightDivider;

        [XmlElement("left-divider")]
        public EmptyPrintObjectStyleAlignMusicXML LeftDivider
        {
            get
            {
                return leftDivider;
            }

            set
            {
                leftDivider = value;
            }
        }

        [XmlElement("right-divider")]
        public EmptyPrintObjectStyleAlignMusicXML RightDivider
        {
            get
            {
                return rightDivider;
            }

            set
            {
                rightDivider = value;
            }
        }
    }
}
