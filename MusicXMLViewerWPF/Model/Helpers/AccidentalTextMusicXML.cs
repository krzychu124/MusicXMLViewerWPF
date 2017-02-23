using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Helpers
{
    [Serializable]
    [XmlType(TypeName = "accidental-text")]
    public class AccidentalTextMusicXML
    {
        private string lang;
        private string space;
        private AccidentalValueMusicXML value;

        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Lang
        {
            get
            {
                return lang;
            }

            set
            {
                lang = value;
            }
        }
        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Space
        {
            get
            {
                return space;
            }

            set
            {
                space = value;
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
