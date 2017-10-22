using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Identification
{
    [Serializable]
    public class EncodingMusicXML
    {
        private object[] items;
        private EncodingChoiceType[] itemsElementName;

        [XmlElement("encoder", typeof(TypedTextMusicXML))]
        [XmlElement("encoding-date", typeof(DateTime), DataType = "date")]
        [XmlElement("encoding-description", typeof(string))]
        [XmlElement("software", typeof(string))]
        [XmlElement("supports", typeof(SupportsMusicXML))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.items;
            }
            set
            {
                this.items = value;
            }
        }

        /// <uwagi/>
        [XmlElement("ItemsElementName")]
        [XmlIgnore()]
        public EncodingChoiceType[] ItemsElementName
        {
            get
            {
                return this.itemsElementName;
            }
            set
            {
                this.itemsElementName = value;
            }
        }
    }

    [Serializable]
    [XmlType("supports")]
    public class SupportsMusicXML
    {
        private YesNoMusicXML type;
        private string element;
        private string attribute;
        private string value;

        [XmlAttribute("type")]
        public YesNoMusicXML Type
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

        [XmlAttribute("element")]
        public string Element
        {
            get
            {
                return element;
            }

            set
            {
                element = value;
            }
        }

        [XmlAttribute("attribute")]
        public string Attribute
        {
            get
            {
                return attribute;
            }

            set
            {
                attribute = value;
            }
        }

        [XmlAttribute("value")]
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

    [Serializable()]
    [XmlType(IncludeInSchema = false)]
    public enum EncodingChoiceType
    {
        encoder,
        [XmlEnum("encoding-date")]
        encodingdate,
        [XmlEnum("encoding-description")]
        encodingdescription,
        software,
        supports,
    }
}
