using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Defaults
{
    [Serializable]
    [XmlType("staff-layout")]
    [DebuggerDisplay("Number {number}, StaffDist. {staffDistance} Specified?: {staffDistanceSpecified}")]
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
                staffDistanceSpecified = true;
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
