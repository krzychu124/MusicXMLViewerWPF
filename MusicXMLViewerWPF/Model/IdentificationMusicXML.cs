using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLScore.Model.Identification;
using System.Xml.Serialization;

namespace MusicXMLScore.Model
{
    [Serializable]
    public class IdentificationMusicXML //TODO_H encoding-field
    {
        private XmlMiscellaneous miscellaneous;
        private EncodingMusicXML encoding;
        private TypedTextMusicXML creator;
        private TypedTextMusicXML rights;
        private TypedTextMusicXML relation;

        [XmlElement("miscellaneous")] //, IsNullable =false
        public XmlMiscellaneous Miscellaneous
        {
            get
            {
                return miscellaneous;
            }

            set
            {
                miscellaneous = value;
            }
        }

        [XmlElement("encoding")]
        public EncodingMusicXML Encoding
        {
            get
            {
                return encoding;
            }

            set
            {
                encoding = value;
            }
        }

        [XmlElement("creator")]
        public TypedTextMusicXML Creator
        {
            get
            {
                return creator;
            }

            set
            {
                creator = value;
            }
        }

        [XmlElement("rights")]
        public TypedTextMusicXML Rights
        {
            get
            {
                return rights;
            }

            set
            {
                rights = value;
            }
        }

        [XmlElement("relation")]
        public TypedTextMusicXML Relation
        {
            get
            {
                return relation;
            }

            set
            {
                relation = value;
            }
        }

        public IdentificationMusicXML()
        {

        }
    }

    [Serializable]
    public class TypedTextMusicXML
    {
        private string type;
        private string value;
        [XmlAttribute("type")]
        public string Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        [XmlText()]
        public string Value
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
