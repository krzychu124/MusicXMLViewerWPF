using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model
{
    [Serializable]
    [XmlType(TypeName ="part-list")]
    public class XmlPartList
    {
        //private List<> partGroup = new List<string>(); //partgroup[]

        //private XmlScorePart scorePart; //scorepart

        private List<XmlScorePart> scoreParts = new List<XmlScorePart>();

        //[XmlIgnore]
        //[XmlElement("part-group", Order = 0)]
        //public List<string> partgroup
        //{
        //    get
        //    {
        //        return this.partGroup;
        //    }
        //    set
        //    {
        //        this.partGroup = value;
        //    }
        //}

        //[XmlIgnore]
        //[XmlElement("score-part")]
        //public XmlScorePart Scorepart
        //{
        //    get
        //    {
        //        return scorePart;
        //    }
        //    set
        //    {
        //        scorePart = value;
        //    }
        //}

        //[XmlIgnore]
        //[XmlElement("part-group", Order = 2)]
        [XmlElement("score-part")]
        public List<XmlScorePart> ScoreParts
        {
            get
            {
                return this.scoreParts;
            }
            set
            {
                this.scoreParts = value;
            }
        }
    }
}
