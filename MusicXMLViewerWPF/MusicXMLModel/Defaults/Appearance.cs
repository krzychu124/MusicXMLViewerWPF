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

        public void init()
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
                        if (lineWidths.ContainsKey(s))
                        {
                            lineWidths[s] = v;
                        }
                        else
                        {
                            lineWidths.Add(s, v);
                        }
                    }
                    if (item.Name.LocalName == "note-size") //search for <note-size>
                    {
                        string s = item.Attribute("type").Value;
                        float v = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        if (noteSizes.ContainsKey(s))
                        {
                            noteSizes[s] = v;
                        }
                        else
                        {
                            noteSizes.Add(s, v);
                        }
                    }
                    if (item.Name.LocalName == "distance") //search for <ldistance>
                    {
                        string s = item.Attribute("type").Value;
                        float v = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        if (distances.ContainsKey(s))
                        {
                            distances[s] = v;
                        }
                        else
                        {
                            distances.Add(s, v);
                        }
                    }

                }
            }
        }

        public void initFromXElement(XElement x)
        {
            var appearance = x.Elements();

            foreach (var item in appearance)
            {
                if (item.Name.LocalName == "line-width") //search for <line-width>
                {
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
                }
                if (item.Name.LocalName == "note-size") //search for <note-size>
                {
                    string s = item.Attribute("type").Value;
                    float v = float.Parse(item.Value, CultureInfo.InvariantCulture);
                    if (noteSizes.ContainsKey(s))
                    {
                        noteSizes[s] = v;
                    }
                    else
                    {
                        noteSizes.Add(s, v);
                    }
                }
                if (item.Name.LocalName == "distance") //search for <ldistance>
                {
                    string s = item.Attribute("type").Value;
                    float v = float.Parse(item.Value, CultureInfo.InvariantCulture);
                    if (distances.ContainsKey(s))
                    {
                        distances[s] = v;
                    }
                    else
                    {
                        distances.Add(s, v);
                    }
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
