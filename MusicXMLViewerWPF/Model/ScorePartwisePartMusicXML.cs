using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model
{
    [Serializable]
    [XmlType(AnonymousType =true)]
    public class ScorePartwisePartMusicXML
    {
        private List<ScorePartwisePartMeasureMusicXML> measure;
        private string id;

        [XmlElement("measure")]
        public List<ScorePartwisePartMeasureMusicXML> Measure
        {
            get
            {
                return measure;
            }

            set
            {
                measure = value;
            }
        }
        [XmlAttribute("id")]
        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }
    }
}
