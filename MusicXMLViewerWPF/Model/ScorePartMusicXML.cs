using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model
{
    [Serializable]
    public class ScorePartMusicXML
    {
        private string partId; //! id
        private string partName; //! long name
        private string partNameAbbreviation; //! short name

        //TODO_L score-instrument, partname,partabbreviation - display, midi-instument

        [XmlAttribute("id")]
        public string PartId
        {
            get
            {
                return partId;
            }

            set
            {
                partId = value;
            }
        }

        [XmlElement("part-name")]
        public string PartName
        {
            get
            {
                return partName;
            }

            set
            {
                partName = value;
            }
        }

        [XmlElement("part-abbreviation")]
        public string PartNameAbbreviation
        {
            get
            {
                return partNameAbbreviation;
            }

            set
            {
                partNameAbbreviation = value;
            }
        }
    }
}
