using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.Attributes
{
    [Serializable]
    [XmlType(AnonymousType =true)]
    public class AttributesDirectiveMusicXML
    {
        private string lang;
        private string value;

        public AttributesDirectiveMusicXML()
        {

        }

        [XmlAttribute("lang")] //! Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://www.w3.org/XML/1998/namespace")
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

        [XmlText]
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