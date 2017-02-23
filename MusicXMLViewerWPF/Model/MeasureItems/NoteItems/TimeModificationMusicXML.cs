using MusicXMLScore.Model.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.NoteItems
{
    [Serializable]
    [XmlType(TypeName ="time-modification")]
    public class TimeModificationMusicXML
    {
        private string actualNotes;
        private string normalNotes;
        private NoteTypeValueMusicXML normalType;
        private List<EmptyMusicXML> normalDot;

        [XmlElement("actual-notes", DataType = "nonNegativeInteger")]
        public string ActualNotes
        {
            get
            {
                return actualNotes;
            }

            set
            {
                actualNotes = value;
            }
        }
        [XmlElement("normal-notes", DataType = "nonNegativeInteger")]
        public string NormalNotes
        {
            get
            {
                return normalNotes;
            }

            set
            {
                normalNotes = value;
            }
        }
        [XmlElement("normal-type")]
        public NoteTypeValueMusicXML NormalType
        {
            get
            {
                return normalType;
            }

            set
            {
                normalType = value;
            }
        }
        [XmlElement("normal-dot")]
        public List<EmptyMusicXML> NormalDot
        {
            get
            {
                return normalDot;
            }

            set
            {
                normalDot = value;
            }
        }
    }
}
