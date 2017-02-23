using MusicXMLScore.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.NoteItems
{
    [Serializable]
    [XmlType(TypeName ="notehead-text")]
    public class NoteHeadTextMusicXML
    {
        private object[] items;

        [XmlElement("accidental-text", typeof(AccidentalTextMusicXML))]
        [XmlElement("display-text", typeof(FormattedTextMusicXML))]
        public object[] Items
        {
            get
            {
                return items;
            }

            set
            {
                items = value;
            }
        }
    }
}
