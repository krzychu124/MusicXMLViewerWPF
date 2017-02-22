using MusicXMLScore.Model.Helpers;
using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.Attributes
{
    [Serializable]
    [XmlType(TypeName = "transpose")]
    public class TransposeMusicXML
    {
        private string diatonic;
        private double chromatic;
        private string octaveChange;
        private EmptyMusicXML doubleField;
        private string number;

        public TransposeMusicXML()
        {

        }

        [XmlElement("diatonic", DataType ="integer")]
        public string Diatonic
        {
            get
            {
                return diatonic;
            }

            set
            {
                diatonic = value;
            }
        }
        [XmlElement("chromatic")]
        public double Chromatic
        {
            get
            {
                return chromatic;
            }

            set
            {
                chromatic = value;
            }
        }
        [XmlElement("octave-change", DataType ="integer")]
        public string OctaveChange
        {
            get
            {
                return octaveChange;
            }

            set
            {
                octaveChange = value;
            }
        }
        [XmlElement("double")]
        internal EmptyMusicXML DoubleField
        {
            get
            {
                return doubleField;
            }

            set
            {
                doubleField = value;
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
    }
}