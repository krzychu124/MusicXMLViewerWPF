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
    [XmlType(TypeName ="name-display")]
    public class NameDisplayMusicXML
    {
        private object[] items;
        private YesNoMusicXML printObject;
        private bool printObjectSpecified;

        public NameDisplayMusicXML()
        {

        }

        [XmlElement("accidental-text", typeof(AccidentalTextMusicXML))]
        [XmlElement("display-text", typeof(FormattedTextMusicXML))]
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
        [XmlAttribute("print-object")]
        public YesNoMusicXML PrintObject
        {
            get
            {
                return printObject;
            }

            set
            {
                printObject = value;
            }
        }
        [XmlIgnore]
        public bool PrintObjectSpecified
        {
            get
            {
                return printObjectSpecified;
            }

            set
            {
                printObjectSpecified = value;
            }
        }
    }
}
