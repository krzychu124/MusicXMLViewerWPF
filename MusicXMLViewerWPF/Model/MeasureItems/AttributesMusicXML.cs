using MusicXMLScore.Model.Helpers;
using MusicXMLScore.Model.MeasureItems.Attributes;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems
{
    [Serializable]
    [XmlType(TypeName ="attributes")]
    public class AttributesMusicXML
    {
        //footnote
        //level
        private double divisions;
        private bool divisionsSpecified;
        private List<KeyMusicXML> key;
        private List<TimeMusicXML> time;
        private string staves;
        private PartSymbolMusicXML partSymbol;
        private string instruments;
        private List<ClefMusicXML> clef;
        private List<StaffDetailsMusicXML> staffDetails;
        private List<TransposeMusicXML> transpose;
        private List<AttributesDirectiveMusicXML> attributesDirective;
        private List<MeasureStyleMusicXML> measureStyle;

        public AttributesMusicXML()
        {

        }

        [XmlElement("divisions")]
        public double Divisions
        {
            get
            {
                return divisions;
            }

            set
            {
                divisions = value;
            }
        }
        [XmlIgnore]
        public bool DivisionsSpecified
        {
            get
            {
                return divisionsSpecified;
            }

            set
            {
                divisionsSpecified = value;
            }
        }
        [XmlElement("key")]
        public List<KeyMusicXML> Key
        {
            get
            {
                return key;
            }

            set
            {
                key = value;
            }
        }
        [XmlElement("time")]
        public List<TimeMusicXML> Time
        {
            get
            {
                return time;
            }

            set
            {
                time = value;
            }
        }
        [XmlElement("staves", DataType ="nonNegativeInteger")]
        public string Staves
        {
            get
            {
                return staves;
            }

            set
            {
                staves = value;
            }
        }
        [XmlElement("part-symbol")]
        public PartSymbolMusicXML PartSymbol
        {
            get
            {
                return partSymbol;
            }

            set
            {
                partSymbol = value;
            }
        }
        [XmlElement(DataType = "nonNegativeInteger")]
        public string Instruments
        {
            get
            {
                return instruments;
            }

            set
            {
                instruments = value;
            }
        }
        [XmlElement("clef")]
        public List<ClefMusicXML> Clef
        {
            get
            {
                return clef;
            }

            set
            {
                clef = value;
            }
        }
        [XmlElement("staff-details")]
        public List<StaffDetailsMusicXML> StaffDetails
        {
            get
            {
                return staffDetails;
            }

            set
            {
                staffDetails = value;
            }
        }
        [XmlElement("transpose")]
        public List<TransposeMusicXML> Transpose
        {
            get
            {
                return transpose;
            }

            set
            {
                transpose = value;
            }
        }
        [XmlElement("attributes-directive")]
        public List<AttributesDirectiveMusicXML> AttributesDirective
        {
            get
            {
                return attributesDirective;
            }

            set
            {
                attributesDirective = value;
            }
        }
        [XmlElement("measure-style")]
        public List<MeasureStyleMusicXML> MeasureStyle
        {
            get
            {
                return measureStyle;
            }

            set
            {
                measureStyle = value;
            }
        }
    }
}