using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class XmlRead
    {
        private static string file_path;
        private static XDocument Doc;
        public string File_path
        {
            get { return file_path; }

            set { file_path = value; }
        }

        public static XDocument Document
        {
            get
            { if(Doc!=null)
                {
                    return Doc;
                }
                else
                {
                    XDocument d = new XDocument();
                    return d;
                }
                
            }

            set { Doc = value; }
        }

        //static void test()
        //{


        //    Console.WriteLine(XmlRead.GetXmlInventory().ToString());
        //    Console.ReadLine();
        //    var s = AddChar(GetXmlInventory());

        //    test(GetXmlInventory());
        //    Measures a = new Measures();
        //    line();
        //    a.display();
        //    //foreach (var i in s)
        //    //{
        //    //    foreach (var c in i)
        //    //    {
        //    //        Console.WriteLine(c.ToString());
        //    //    }
        //    //}
        //    Console.ReadLine();


        //}
        public static XDocument GetXmlInventory()
        {
            try
            {
                XDocument inventoryDoc
                = XDocument.Load(file_path);

                return inventoryDoc;
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine("File not found !",ex.Message);   
                return null;
            }
            catch ( ArgumentNullException)
            {
                Console.WriteLine("invalid path to file");
                return null;
            }
        }

        

        

        internal static XDocument GetXmlInventory(string fileName)
        {
            try
            {
                XDocument inventoryDoc
                = XDocument.Load(fileName);
                
                Document = inventoryDoc;
               
                return inventoryDoc;
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine("File not found !", ex.Message);
                return null;
            }
        }
        //public static List<List<int>> AddChar(XDocument Doc)
        //{
        //    List<List<int>> meas = new List<List<int>>();
        //    var measures = from i in Doc.Descendants("part") select i;

        //    foreach (var i in measures.Elements("measure"))
        //    {
        //        XElement x = i;
        //        int num = Convert.ToInt32(i.Attribute("number").Value);
        //        int width = Convert.ToInt32(i.Attribute("width").Value);
        //        new Measures(i, num, width);
        //        List<int> tab = new List<int>() { num, width };
        //        meas.Add(tab);
        //        //Measures m = new Measures();
        //        // m.measuresm.Add(x);
        //    }

        //    return meas;
        //}



        //public static List<MusicalChars> AddNote(IEnumerable<XElement> note, out List<Notations> u)
        //{
        //    List<MusicalChars> znak = new List<MusicalChars>();
        //    u = null;
        //    List<Notations> n = new List<Notations>();
        //    foreach (var item in note)
        //    {

        //        int pos = Convert.ToInt32(item.Attribute("default-x").Value);

        //        int id = Convert.ToInt32(item.Attribute("id").Value);
        //        Pitch p = new Pitch();
        //        int dur = Convert.ToInt32(item.Element("duration").Value);
        //        string type = item.Element("type").Value;
        //        int voice = Convert.ToInt32(item.Element("voice").Value);

        //        if (item.Element("rest") ==null)
        //        {
        //            bool i = false;
        //            p = new Pitch(item.Element("pitch").Element("step").Value, Convert.ToInt32(item.Element("pitch").Element("octave").Value));
        //            bool direction = (((string)item.Element("stem").Value) == "up") ? true : false;
        //            string stem = (string)item.Element("stem").Attribute("default-y").Value;
        //            Note t = new Note(id, pos, p, dur, voice, type, stem, direction,i);
        //            znak.Add(t);
        //        }
        //        else
        //        {
        //            bool i = true;
        //            Note t = new Note(id, pos, dur, voice, type,i);
        //            znak.Add(t);
        //        }



        //        if (item.Descendants("notations").Any())
        //        {
        //           // Notations n = new Notations();
        //            if (item.Element("notations").Element("slur").Attribute("type").Value == "start")
        //            {
        //                n.Add(new Notations(Convert.ToInt32(item.Attribute("id").Value),item.Element("notations").Element("slur").Name.LocalName, item.Element("notations").Element("slur").Attribute("type").Value, item.Element("notations").Element("slur").Attribute("number").Value, item.Element("notations").Element("slur").Attribute("placement").Value));

        //            }

        //            else
        //            {
        //                n.Add(new Notations(Convert.ToInt32(item.Attribute("id").Value), item.Element("notations").Element("slur").Name.LocalName, item.Element("notations").Element("slur").Attribute("type").Value, item.Element("notations").Element("slur").Attribute("number").Value));

        //            }

        //        }

        //            //  pitch = item.Elements("");
        //            // var pos = from i in note select (int)i.Element("note");


        //        int a = 0;

        //    }
        //    u = n;
        //    return znak;

        //}
        //public static void ch(IEnumerable<XElement> text)
        //{
        //    if(text.Descendants("rest").Any())
        //    {
        //        //int pauza = 0;
        //        string[] pauza = (from i in text.Elements() where i.IsEmpty select (string)i.Parent.Attribute("id").Value).ToArray();
        //        Console.Write("jest w nucie " );
        //        foreach (string i in pauza)
        //        {
        //            Console.Write(i+ ", " );
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("brak");
        //    }
        //}

        //public static XDocument test(XDocument doc)
        //{
        //    string a = "";
        //    var notes = from item in doc.Descendants() select item.Elements("note");

        //    var clef = from i in doc.Descendants("attributes") select i;
        //    var partlist = from i in doc.Descendants("part-list") select i;
        //    var note = from i in doc.Root.Descendants("note") select i;
        //    //var note_id = from i in note.Elements("note") select i.Add(new Attribute("id",) )
        //    int index = 0;
        //    foreach (var item in note)
        //    {
        //        index++;
        //        item.Add(new XAttribute("id", index));
        //    }


        //    //select new
        //    //{
        //    //    Slur = i.Descendants("slur").Elements().Attributes("type"),
        //    //   // Slur2 = i.Element("notations").Element("slur").Attribute("number").Value,
        //    //  //  Slur3 = i.Element("notations").Element("slur").Attribute("placement").Value
        //    //};






        //    //Console.WriteLine(note.ToString());
        //    // where (string)t.Element("step") == make select (string)t;
        //    //select t.Element("octave").Value;

        //    Window1.InsertLine();
        //    //foreach (var item in note)
        //    //{
        //    //    foreach (var i in item)
        //    //    {
        //    //        Console.WriteLine(i);
        //    //    }
        //    //}
        //    Window1.InsertLine();
        //    List<Notations> z;
        //    var x = AddNote(note,out z);
        //    Console.ReadLine();
        //    //foreach (var i in note)
        //    //{
        //    //    Console.WriteLine(i);
        //    //}
        //    DisplayItem(clef);
        //    Window1.InsertLine();
        //    Console.ReadLine();
        //    DisplayItem(note);
        //    Window1.InsertLine();
        //    ch(note);
        //   // AddChar(notes);
        //    Console.ReadLine();

        //    return null;
        //}
    }
}
