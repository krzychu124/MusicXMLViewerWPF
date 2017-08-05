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
    [XmlType(TypeName ="note-type")]
    public class NoteTypeMusicXML
    {
        private SymbolSizeMusicXML size;
        private bool sizeSpecified;
        private NoteTypeValueMusicXML value;

        public NoteTypeMusicXML()
        {

        }

        [XmlAttribute("size")]
        public SymbolSizeMusicXML Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }

        [XmlIgnore]
        public bool SizeSpecified
        {
            get
            {
                return sizeSpecified;
            }

            set
            {
                sizeSpecified = value;
            }
        }

        [XmlText]
        public NoteTypeValueMusicXML Value
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
