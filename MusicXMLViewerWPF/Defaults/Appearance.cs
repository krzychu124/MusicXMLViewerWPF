using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MusicXMLViewerWPF.Defaults
{
    class Appearance // rework, minor changes 
    {
        private static Dictionary<string, float> distances = new Dictionary<string, float>() { };
        private static Dictionary<string, float> lineWidths = new Dictionary<string, float>() { };
        private static Dictionary<string, float> noteSizes = new Dictionary<string, float>() { };

        public static Dictionary<string, float> Distances { get { return distances; } }
        public static Dictionary<string, float> NoteSizes { get { return noteSizes; } }
        public static Dictionary<string, float> LineWidths { get { return lineWidths; } }

        public Appearance()
        {
            initLineWidths();
        }

        public Appearance(XElement x)
        {
            initLineWidths(x);
        }

        public static float GetDistance(string type)
        {
            float x = 0f;
            if (distances.ContainsKey(type))
            {
                x = distances[type]; 
            }
            return x;
        }
        public static float GetLineWidth(string type)
        {
            float x = 0f;
            if (lineWidths.ContainsKey(type))
            {
                x = lineWidths[type];
            }
            return x;
        }

        public static float GetNoteSize(string type)
        {
            float x = 0f;
            if (noteSizes.ContainsKey(type))
            {
                x = noteSizes[type];
            }
            return x;
        }

        public void initLineWidths()
        {
            var x = Misc.LoadFile.Document;
            if (x == null)
            {
                //string s = MethodBase.GetCurrentMethod().Name;
                //Logger.EmptyXDocument(s);
                Logger.Log("XDocument is empty");
            }
            else
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

        public void initLineWidths(XElement x)
        {
            var appearance = x.Elements(); 
                                           
            foreach (var item in appearance)
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
