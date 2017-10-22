using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.NoteItems
{
    [Serializable]
    [XmlType(TypeName ="intrument")]
    public class InstrumentMusicXML
    {
        private string id;

        public InstrumentMusicXML()
        {

        }

        [XmlAttribute("id", DataType ="IDREF")]
        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }
    }
}
