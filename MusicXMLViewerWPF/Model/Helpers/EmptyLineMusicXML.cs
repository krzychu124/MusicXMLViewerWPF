using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Helpers
{
    [Serializable]
    [XmlType(TypeName ="empty-line")]
    public class EmptyLineMusicXML
    {
        private LineShapeMusicXML lineshape;
        private bool lineshapeSpecified;
        private LineTypeMusicXML linetype;
        private bool linetypeSpecified;
        private double dashlength;
        private bool dashlengthSpecified;
        private double spacelength;
        private bool spacelengthSpecified;
        private AboveBelowMusicXML placement;
        private bool placementSpecified;

        public EmptyLineMusicXML()
        {

        }

        [XmlAttribute("line-shape")]
        public LineShapeMusicXML Lineshape
        {
            get
            {
                return lineshape;
            }

            set
            {
                lineshape = value;
            }
        }

        [XmlIgnore]
        public bool LineshapeSpecified
        {
            get
            {
                return lineshapeSpecified;
            }

            set
            {
                lineshapeSpecified = value;
            }
        }

        [XmlAttribute("line-type")]
        public LineTypeMusicXML Linetype
        {
            get
            {
                return linetype;
            }

            set
            {
                linetype = value;
            }
        }

        [XmlIgnore]
        public bool LinetypeSpecified
        {
            get
            {
                return linetypeSpecified;
            }

            set
            {
                linetypeSpecified = value;
            }
        }

        [XmlAttribute("dash-length")]
        public double Dashlength
        {
            get
            {
                return dashlength;
            }

            set
            {
                dashlength = value;
            }
        }

        [XmlIgnore]
        public bool DashlengthSpecified
        {
            get
            {
                return dashlengthSpecified;
            }

            set
            {
                dashlengthSpecified = value;
            }
        }

        [XmlAttribute("space-length")]
        public double Spacelength
        {
            get
            {
                return spacelength;
            }

            set
            {
                spacelength = value;
            }
        }

        [XmlIgnore]
        public bool SpacelengthSpecified
        {
            get
            {
                return spacelengthSpecified;
            }

            set
            {
                spacelengthSpecified = value;
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
    }
}
