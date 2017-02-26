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
    [XmlType(TypeName ="accidental-mark")]
    public class AccidentalMarkMusicXML
    {
        private AboveBelowMusicXML placement;
        private bool placementSpecified;
        private AccidentalValueMusicXML value;

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
        [XmlText]
        public AccidentalValueMusicXML Value
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
