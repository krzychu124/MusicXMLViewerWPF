using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Defaults
{
    [Serializable]
    [XmlType("staff-layout")]
    public class StaffLayoutMusicXML
    {
        private double staffDistance;
        private bool staffDistanceSpecified;
        private string number;

        [XmlElement("staff-distance")]
        public double StaffDistance
        {
            get
            {
                return staffDistance;
            }

            set
            {
                staffDistance = value;
            }
        }
        [XmlIgnore]
        public bool StaffDistanceSpecified
        {
            get
            {
                return staffDistanceSpecified;
            }

            set
            {
                staffDistanceSpecified = value;
            }
        }
        [XmlAttribute("number", DataType ="positiveInteger")]
        public string Number
        {
            get
            {
                return number;
            }

            set
            {
                number = value;
            }
        }
    }
}
