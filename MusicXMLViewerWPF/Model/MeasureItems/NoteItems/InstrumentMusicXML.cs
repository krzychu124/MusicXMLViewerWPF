using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
