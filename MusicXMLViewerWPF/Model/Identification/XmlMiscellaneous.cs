using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Identification
{
    [Serializable]
    public class XmlMiscellaneous
    {
        private List<XmlMiscellaneousField> miscellaneousField = new List<XmlMiscellaneousField>();

        [XmlElement("miscellaneous-field")]
        public List<XmlMiscellaneousField> MiscellaneousField
        {
            get
            {
                return miscellaneousField;
            }

            set
            {
                miscellaneousField = value;
            }
        }
    }
}
