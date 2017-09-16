using System;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Helpers
{
    [Serializable]
    [XmlType(TypeName = "empty-print-object-style-align")]
    public class EmptyPrintObjectStyleAlignMusicXML
    {
        private SimpleTypes.YesNoMusicXML printObject;
        private bool printObjectSpecified;

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
