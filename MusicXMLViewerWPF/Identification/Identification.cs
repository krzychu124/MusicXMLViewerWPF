using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF.Identification
{
    class Identification
    {
        private Encode encoding;
        private List<TypedText> creator = new List<TypedText>();
        private List<TypedText> relation = new List<TypedText>();
        private List<TypedText> rights = new List<TypedText>();
        private string miscellaneous;
        private string source;

        public Encode Encoding { get { return encoding; } }
        public List<TypedText> Creator { get { return creator; } }
        public List<TypedText> Relation { get { return relation; } }
        public List<TypedText> Rights { get { return rights; } }
        public string Miscellaneous { get { return miscellaneous; } }
        public string Source { get { return source; } }

        public Identification(XElement x)
        {
            encoding = new Encode(x);
            miscellaneous = x.Element("miscellaneous").Value;
            source = x.Element("source").Value;
            var creators = x.Elements("creator");
            foreach (var item in creators)
            {
                creator.Add(new TypedText(item) { Type_name = "creator" });
            }
            var relations = x.Elements("relation");
            foreach (var item in relations)
            {
                relation.Add(new TypedText(item) { Type_name = "relation" });
            }
            var rights_ = x.Elements("rights");
            foreach (var item in rights_)
            {
                rights.Add(new TypedText(item) { Type_name = "rights" });
            }
        }
    }

    internal class Encode
    {
        private string encoder;
        private string encoding_date;
        private string encoding_descryption;
        private string software;
        private List<Supports> supports;

        public string Encoder { get { return encoder; } }
        public string Encoding_date { get { return encoding_date; } }
        public string Encoding_descryption { get { return encoding_descryption; } }
        public string Software { get { return software; } }
        public List<Supports> Supports { get { return supports; } }

        public Encode(XElement x)
        {
            encoder = x.Element("encoder") != null ? x.Element("encoder").Value : null;
            encoding_date = x.Element("encoding-date") != null ? x.Element("encoding-date").Value : null;
            encoding_descryption= x.Element("encoding-descryption") != null ? x.Element("encoding-descryption").Value : null;
            software = x.Element("software") != null ? x.Element("software").Value : null;
            var temp_support = x.Elements("supports");
            foreach (var item in temp_support)
            {
                supports.Add(new Supports(item));
            }
        }
    }
    internal class Supports
    {
        private bool type;
        private bool value;
        private string attribute;
        private string element;

        public bool Type { get { return type; } }
        public bool Value { get { return value; } }
        public string Attribute { get { return attribute; } }
        public string Element { get { return element; } }

        public Supports(XElement x)
        {
            var attributes = x.Attributes();
            foreach (var item in attributes)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "attribute":
                        attribute = item.Value;
                        break;
                    case "element":
                        element = item.Value;
                        break;
                    case "type":
                        type = item.Value == "yes" ? true : false;
                        break;
                    case "value":
                        value = item.Value == "yes" ? true : false;
                        break;
                    default:
                        break;
                }
            }
        }
    }
    class TypedText
    {
        private string type_name;
        private string value;
        private string type;

        public string Type { get { return type; } }
        public string Value { get { return value; } }
        public string Type_name { get { return type_name; } set { if (value != null) type_name = value; } }

        public TypedText(XElement x)
        {
            type = x.Attribute("type") != null ? x.Attribute("type").Value : null;
            value = x.Value;
        }
    }
}
