using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class LoadDocToClasses // Analisys of XML // Making objects // WiP as whole app :)
    {
        public static XDocument doc;
        private static bool CharListLoaded;
        private static bool MListLoaded;
        public static List<Segment> list = new List<Segment>();
        public static List<Measures> MeasuresList = new List<Measures>();
        public bool isCharListLoaded
        {
            get { return CharListLoaded; }
        }
        public bool isMListLoaded
        {
            get { return MListLoaded; }
        }
        public LoadDocToClasses()
        {
            CharListLoaded = false;
            MListLoaded = false;
        }
        public static XDocument Document {
            get
            {
                return doc; 
                
            }
            set
            {
                if (value != null)
                { doc = value; }
            }
        }

        public static List<List<int>> AddMeasuresToXList(XDocument Doc)
        {
            List<List<int>> measures_list = new List<List<int>>();
            var measures = from i in Doc.Descendants("part") select i;

            foreach (var i in measures.Elements("measure"))
            {
                XElement x = i;
                int num = Convert.ToInt32(i.Attribute("number").Value);
                int width = Convert.ToInt32(i.Attribute("width").Value);
                new Measures(i, num, width);
                List<int> tab = new List<int>() { num, width };

            
                measures_list.Add(tab);
                
            }

            return measures_list;
        }
        public static void AddMeasuresToXListV(XDocument Doc)
        {
          //Clear list before adding  Measures.measuresOfXel.Clear();
            List<List<float>> measures_list = new List<List<float>>();
            var measures = from i in Doc.Descendants("part") select i;

            foreach (var i in measures.Elements("measure"))
            {
                XElement x = i;
                int num = Convert.ToInt32(i.Attribute("number").Value);
                float width = float.Parse((i.Attribute("width").Value), CultureInfo.InvariantCulture);
                new Measures(i, num, width);
                List<float> tab = new List<float>() { num, width };


                measures_list.Add(tab);

            }
            
            Console.WriteLine("Added {0} measures to class", measures_list.Count);

            CharListLoaded = true;
            Measures.CoordedMeasures();
        }
        public static List<Segment> LoadCharsFromMeasures()
        {
            
            //list = new List<MusicalChars>();
            if (Measures.measuresOfXel != null)
            {
                AddIdToNote();
                foreach (var i in Measures.measuresOfXel)
                {
                    int measure_number = Convert.ToInt32(i.Attribute("number").Value);
                    if (i.Element("print") != null && i.Element("print").Element("system-layout") != null)
                    {
                        XElement print = i.Element("print").Element("system-layout");
                        float left_m = 0;
                        float interline = print.Element("top-system-distance") != null ? float.Parse((print.Element("top-system-distance").Value), CultureInfo.InvariantCulture) : float.Parse((print.Element("system-distance").Value), CultureInfo.InvariantCulture);
                        if (print.Element("system-margins")!= null)
                        {
                            left_m = float.Parse((print.Element("system-margins").Element("left-margin").Value), CultureInfo.InvariantCulture);
                        }
                        Page p = new Page(measure_number, left_m, interline);
                    }
                    else
                    {
                        if (i.Element("print") != null)
                        {
                            if (i.Element("print").Attribute("new-system").Value == "yes")
                            {
                                Page p = new Page(measure_number);
                            }
                        }
                    }

                    //NEEDIMPROVEMENTS messy as hell

                    //int measure_number = Convert.ToInt32(i.Attribute("number").Value);
                    if (i.Element("attributes") != null)
                    {

                        XElement attr = i.Element("attributes");
                        if (attr.Element("key") != null)
                        {
                            attr = attr.Element("key");
                            string mode = attr.Element("mode") != null ? attr.Element("mode").Value : "major";
                            Key k = new Key(Convert.ToInt32(attr.Element("fifths").Value), mode, measure_number);
                            Logger.Log(" 'key class' missing addition to list");
                           // list.Add(k);
                        }
                        if (attr.Element("clef") != null)
                        {
                            attr = attr.Element("clef");
                            Clef cl = new Clef(attr.Element("sign").Value, Convert.ToInt32(attr.Element("line").Value), measure_number);
                            //list.Add(cl);
                            Logger.Log(" 'clef class' missing addition to list");
                        }
                        if (attr.Element("time") != null)
                        {
                            XElement time = attr.Element("time");
                            string symbol = time.HasAttributes ? time.FirstAttribute.ToString() : "number";
                            TimeSignature t = new TimeSignature(Convert.ToInt32(time.Element("beats").Value), Convert.ToInt32(time.Element("beat-type").Value), symbol, measure_number);
                            // list.Add(t);
                            Logger.Log(" 'sigtime class' missing addition to list");
                        }
                    }
                    //if (i.Element("attributes") != null)
                    //{

                    //    XElement attr = i.Element("attributes");
                    //    if (attr.Element("clef") != null)
                    //    {
                    //        attr = attr.Element("clef");
                    //        Clef cl = new Clef(attr.Element("sign").Value, Convert.ToInt32(attr.Element("line").Value), measure_number);
                    //        //list.Add(cl);
                    //        Logger.Log(" 'clef class' missing addition to list");
                    //    }
                    //}
                    //if (i.Element("attributes") != null)
                    //{

                    //    XElement attr = i.Element("attributes");
                    //    if (attr.Element("time") != null)
                    //    {
                    //        XElement time = attr.Element("time");
                    //        string symbol = time.HasAttributes ? time.FirstAttribute.ToString() : "number";
                    //        TimeSignature t = new TimeSignature(Convert.ToInt32(time.Element("beats").Value), Convert.ToInt32(time.Element("beat-type").Value), symbol, measure_number);
                    //        // list.Add(t);
                    //        Logger.Log(" 'sigtime class' missing addition to list");
                    //    }
                    //}
                            

                   
                    var l = from u in i.Elements("note") select u;
                    foreach (var item in l)
                    {

                        //chech if is <rest/>
                        //add rest
                        if (item.Element("rest") != null)
                        {
                            Rest p;
                            bool b = true;
                            int d = Convert.ToInt32(item.Element("duration").Value);
                            int id = Convert.ToInt32(item.Attribute("id").Value);
                            int v = Convert.ToInt32(item.Element("voice").Value);
                            string t = "measure";
                            if (item.Element("rest").HasAttributes == false)
                            {
                                float pos = item.Attribute("default-x") != null ?float.Parse((item.Attribute("default-x").Value),CultureInfo.InvariantCulture) : 0.0f;
                                t = item.Element("type").Value;
                                b = false;
                                bool dot = item.Element("dot") != null ? true:false;
                                p = new Rest(measure_number, id, d, v, t, pos, b,dot);
                            }
                            else
                            {
                                p = new Rest(measure_number,id, d, v, b);
                            }
                            list.Add(p);
                        }
                        else
                        {
                            //else add note
                            //if(item.Element("note")!=null)
                            //    {
                            List<Notations> n_list = new List<Notations>();
                            Dictionary<int, string> beam_values = new Dictionary<int, string>();
                            bool hasNotations = false;
                            bool hasStemValue = false;
                            bool i_ = false;
                            float pos = float.Parse((item.Attribute("default-x").Value),CultureInfo.InvariantCulture);
                            float stem = 0f;
                            int beam_number = 0;
                            int dot = item.Element("dot") !=null ? 1 : 0;
                            int dur = Convert.ToInt32(item.Element("duration").Value);
                            int id = Convert.ToInt32(item.Attribute("id").Value);
                            int voice = Convert.ToInt32(item.Element("voice").Value);
                            Note t;
                            Pitch pitch = new Pitch();
                            pitch = new Pitch(item.Element("pitch").Element("step").Value, Convert.ToInt32(item.Element("pitch").Element("octave").Value));
                            string beam = "";
                            string direction = "none";
                            string type = item.Element("type").Value;
                            if (item.Element("stem") != null)
                            {
                                direction =item.Element("stem").Value;
                                stem = (item.Element("stem").HasAttributes == false) ? 0f : float.Parse(item.Element("stem").Attribute("default-y").Value,CultureInfo.InvariantCulture);
                                hasStemValue = item.Element("stem").HasAttributes;
                            }

                            if (item.Element("beam") != null)
                            {
                                
                                var z = item.Elements("beam");
                                int v = 0;
                                foreach (var zed in z)
                                {
                                    string b = (string)zed.Value;
                                    beam_values.Add(v,b);
                                    v++;
                                }
                                beam_number = Convert.ToInt32(item.Element("beam").Attribute("number").Value);
                                beam = (string)item.Element("beam").Value;
                               // t = new Note(measure_number, id, pos, pitch, dur, voice, type, stem, direction,hasStemValue, i_, num, beam,beam_values, dot, hasNotations);
                               // list.Add(t);
                            }
                            if (item.Element("notations") != null)
                            {
                                hasNotations = true;
                                
                                var notation = item.Element("notations").Elements();
                                foreach (var n in notation)
                                {
                                    switch (n.Name.LocalName.ToString())
                                    {
                                        case "slur":
                                            if (n.Attribute("placement") != null)
                                            {
                                                Slur s = new Slur(Convert.ToInt16(n.Attribute("number").Value), n.Attribute("type").Value, n.Attribute("placement").Value);
                                                n_list.Add(s);
                                            }
                                            else
                                            {
                                                Slur s = new Slur(Convert.ToInt16(n.Attribute("number").Value), n.Attribute("type").Value);
                                                n_list.Add(s);
                                            }
                                            break;

                                        case "fermata" : FermataTest f = new FermataTest(n.Attribute("placement").Value);
                                            n_list.Add(f);
                                            break;

                                        
                                    }
                                }
                            }
                            t = new Note(measure_number, id, pos, pitch, dur, voice, type, stem, direction, hasStemValue, i_, beam_number, beam, beam_values, dot, hasNotations,n_list);
                                // list.Add(t);
                               // t = new Note(measure_number, id, pos, pitch, dur, voice, type, stem, direction, i_, dot);
                            list.Add(t);
                            

                        }
                        
                    }
                }
               // tb.writeLineToTextBlock(Page.line.Count.ToString());
            }
            else
            {
              //  tb.writeLineToTextBlock("Added " + Page.line.Count.ToString() + " lines to view");
              //  tb.writeLineToTextBlock("!!!<<Zduplikowany Atrybut>>!!!");
            }
            
            return list;
        }
        public static void AddIdToNote()
        {
           // List<XElement> n = new List<XElement>();
            //note = Measures.measuresOfXel;

            int index = 0;
           // List<XElement> item = new List<XElement>();
            var n = from i in Measures.measuresOfXel.Descendants("note") select i;
        //    if(n.Attributes(""))
            if (n.ElementAt(0).Attribute("id") == null)
            {
                foreach (var i in n)
                {
                    index++;
                    try
                    {
                        i.Add(new XAttribute("id", index));
                    }
                    catch (InvalidOperationException ex)
                    {
                        if (ex.Message == "Zduplikowany atrybut.")
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            Console.WriteLine(ex.Message);
                            break;
                            throw;
                        }
                        else
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            Console.WriteLine(ex.Message);
                            break;
                            throw;
                        }

                    }

                }
            }
         
        }
        public void DisplayItem(IEnumerable<XElement> x)
        {

            foreach (var item in x)
            {
                Console.WriteLine(item);
            }

        }
        public void DisplayItem(IEnumerable<IEnumerable<XElement>> x)
        {

            foreach (var item in x)
            {
                foreach (var i in item)
                {
                    Console.WriteLine(i);
                }
            }

        }
        
        public LoadDocToClasses(XDocument Document)
        {
           // Loaded = true;
        }
    }
}
