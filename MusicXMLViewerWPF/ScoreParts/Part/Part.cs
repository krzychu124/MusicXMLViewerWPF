using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF.ScoreParts.Part
{
    class Part
    {
        private string part_id;
        private List<Measures.Measure> measure_list = new List<Measures.Measure>();

        public string Id { get { return part_id; } }
        public List<Measures.Measure> MeasureList { get { return measure_list; } }
       

        public Part(XElement x)
        {
            part_id = x.Attribute("id").Value;
            var measures = x.Elements();
            foreach (var item in measures)
            {
                measure_list.Add(new Measures.Measure(item));
            }
        }
    }
}
