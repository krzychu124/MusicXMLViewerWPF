using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF.Defaults
{
    class Appearance
    {
        private Dictionary<string, float> distances = new Dictionary<string, float>() { };
        private Dictionary<string, float> lineWidths = new Dictionary<string, float>() { };
        private Dictionary<string, float> noteSizes = new Dictionary<string, float>() { };

        public Dictionary<string, float> Distances { get { return distances; } }
        public Dictionary<string, float> NoteSizes { get { return noteSizes; } }
        public Dictionary<string,float> LineWidths { get { return lineWidths; } }

        public Appearance()
        {
            initLineWidths(LoadDocToClasses.Document);
        }
        public void initLineWidths(XDocument x)
        {
            var apperance = from z in x.Descendants("defaults") select z.Element("appearance"); //search for <appearance></appearance>
            var desc = from s in apperance.Elements().Distinct() select s;
            foreach (var item in desc)
            {
                if (item.Name.LocalName == "line-width") //search for <line-width>
                {
                    string s = item.Attribute("type").Value;
                    float v = float.Parse(item.Value, CultureInfo.InvariantCulture);
                    lineWidths.Add(s, v);
                }
                if (item.Name.LocalName == "note-size") //search for <note-size>
                {
                    string s = item.Attribute("type").Value;
                    float v = float.Parse(item.Value, CultureInfo.InvariantCulture);
                    noteSizes.Add(s, v);
                }
                if (item.Name.LocalName == "distance") //search for <ldistance>
                {
                    string s = item.Attribute("type").Value;
                    float v = float.Parse(item.Value, CultureInfo.InvariantCulture);
                    distances.Add(s, v);
                }

            }
            
        }
    }
    
}
