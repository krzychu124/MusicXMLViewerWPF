using MusicXMLScore.Model.Helpers;
using MusicXMLScore.Model.MeasureItems.Attributes;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems
{
    public class AttributesMusicXML
    {
        //footnote
        //level
        private double divisions;
        private bool divisionsSpecified;
        private KeyMusicXML key;
        //time
        private string staves;
        //partsymbol
        private string instruments;
        //clef
        //staffdetails
        //transpose
        //attriburesdirective
        //measurestyle

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
        public KeyMusicXML Key
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
        [XmlElement(DataType = "nonNegativeInteger")]
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
    }
}