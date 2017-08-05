using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Identification
{
    [Serializable]
    [XmlType(TypeName = "miscellaneous-field")]
    public class XmlMiscellaneousField
    {
        private string name;
        private string value;

        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
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

        public XmlMiscellaneousField()
        {

        }
    }
}
