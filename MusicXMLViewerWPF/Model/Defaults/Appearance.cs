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

        public static Dictionary<string, float> Distances { get { return distances; } set { if (value != null) { distances = value; } } }
        public static Dictionary<string, float> NoteSizes { get { return noteSizes; } set { if (value != null) { noteSizes = value; } } }
        public static Dictionary<string, float> LineWidths { get { return lineWidths; } set { if (value != null) { lineWidths = value; } } }

        public Appearance()
        {
            initFromDefaults();
        }

        public Appearance(XElement x)
        {
            initFromXElement(x);
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

        public void initFromXElement(XElement x)
        {
            var appearance = x.Elements();

            foreach (var item in appearance)
            {
                switch (item.Name.LocalName)
                {
                    case "line-width": //search for <line-width>
                        string s = item.Attribute("type").Value;
                        float v = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        if (lineWidths.ContainsKey(s))
                        {
                            lineWidths[s] = v;
                        }
                        else
                        {
                            lineWidths.Add(s, v);
                        }
                        break;
                    case "note-size": //search for <note-size>
                        string s1 = item.Attribute("type").Value;
                        float v1 = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        if (noteSizes.ContainsKey(s1))
                        {
                            noteSizes[s1] = v1;
                        }
                        else
                        {
                            noteSizes.Add(s1, v1);
                        }
                        break;
                    case "distance": //search for <ldistance>
                        string s2 = item.Attribute("type").Value;
                        float v2 = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        if (distances.ContainsKey(s2))
                        {
                            distances[s2] = v2;
                        }
                        else
                        {
                            distances.Add(s2, v2);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        private void initFromDefaults()
        {
            initDefaultDistances();
            initDefaultLineWidths();
            initDefaultNoteSizes();
        }
        private void initDefaultDistances()
        {
            if (Distances.Count == 0)
            {
                distances.Add("hyphen", 60f);
                distances.Add("beam", 8f);
            }
        }

        private void initDefaultNoteSizes()
        {
            if (NoteSizes.Count == 0)
            {
                noteSizes.Add("grace", 60f);
                noteSizes.Add("cue", 60f);
            }
        }

        private void initDefaultLineWidths()
        {
            if (LineWidths.Count == 0)
            {
                lineWidths.Add("stem", 1.4583f);
                lineWidths.Add("beam", 5f);
                lineWidths.Add("staff", 1.4583f);
                lineWidths.Add("light barline", 1.4583f);
                lineWidths.Add("heavy barline", 5f);
                lineWidths.Add("leger", 1.4583f);
                lineWidths.Add("ending", 1.4583f);
                lineWidths.Add("wedge", 1.4583f);
                lineWidths.Add("enclosure", 1.4583f);
                lineWidths.Add("tuplet bracket", 1.4583f);
            }
        }

        public static void Clear()
        {
            distances = null;
            distances = new Dictionary<string, float>();
            lineWidths = null;
            lineWidths = new Dictionary<string, float>();
            noteSizes = null;
            noteSizes = new Dictionary<string, float>();
        }
    }
    
}
