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
        private Dictionary<string, ScorePartwisePartMeasureMusicXML> measures;

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
        /// <summary>
        /// Measures Dictionary, Measure.Number as Key
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, ScorePartwisePartMeasureMusicXML> MeasuresByNumber
        {
            get
            {
                return measures;
            }
            set
            {
                measures = value;
            }
        }
        public void SetMeasuresDictionary()
        {
            measures = new Dictionary<string, ScorePartwisePartMeasureMusicXML>();
            foreach (var m in Measure)
            {
                measures.Add(m.Number, m);
            }
        }

        internal int GetStavesCount()
        {
            int numberOfStaves = 1;
            var staves = measure.FirstOrDefault()?.Items?.OfType<MeasureItems.AttributesMusicXML>()?.FirstOrDefault()?.Staves ?? "1";
            if (staves != null)
            {
                numberOfStaves = int.Parse(staves);
            }
            return numberOfStaves;
        }
    }
}
