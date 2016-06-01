using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using MusicXMLViewerWPF.Defaults;

namespace MusicXMLViewerWPF
{
    class ScorePart : MusicScore // first idea :/ // can be hard to read, rework possible
    {
        private List<ScoreInstrument> instrumentList = new List<ScoreInstrument>();
        //private List<ScoreParts> scoreparts;
        private static List<Part> partList = new List<Part>();
        private string Id;
        private string part_group; // not sure it should be collection or smth // Low priority // I'll add/test this later
        public List<Part> PartList {  get { return partList; } }
        public List<ScoreInstrument> InstrumentList { get { return instrumentList; } }
        public static Page page;
        public static SystemLayout systemlayout;
        public string ID {  get { return Id; } }
        public string PartGroup { get { return part_group; } }
        
        // private Identyfication_Class;
        
        public void getPartList()
        {
            XDocument doc = LoadDocToClasses.Document;
            var p = from z in doc.Descendants("part-list") select z;
           // XElement s;
            List<XElement> slist = new List<XElement>();
            foreach (var item in p)
            {
                slist.Add(item.Element("score-part"));
            }
            foreach (var el in slist)
            {
                Part part = new Part(el);
                partList.Add(part);
                ScoreInstrument instr = new ScoreInstrument(el);
                instrumentList.Add(instr);
                ScorePart sp = new ScorePart(el);
                scoreparts.Add(sp);
            }
        }
        public ScorePart()
        {
            //page = new Page(); //TODO test, possible rework :/
            systemlayout = new SystemLayout();
            getPartList();
        }
        public ScorePart(XElement x)
        {
            Id = x.Attribute("id").Value;
        }
    }

    class PartGroup // not implemented // searching for MusicXML templates //
    {
        private int number;
        private string type;
        private bool isStart;
        private GroupSymbol symbol;
        private GroupBarline groupbarline;
        private string groupname;
        private string groupabbreviation;

        public PartGroup(int num, string t)
        {
            number = num;
            type = t;
            isStart = type == "start" ? true : false;
        }
        public PartGroup(int num, string t, string barlines): this(num, t)
        {
            groupbarline = barlines == "yes" ? GroupBarline.yes : barlines == "Mensurstrich" ? GroupBarline.mensurstrinch : GroupBarline.no;
        }
        public void DrawGroupSymbol(DrawingContext dc, Point startMeasure, Point endMeasure, GroupSymbol type ) // possible to move this to measures class or make separate class for drawing everything ?? //
        {
            // later
        }

        internal enum GroupSymbol
        {
            none,
            square,
            bracket,
            line,
            brace
        }
        internal enum GroupBarline
        {
            no,
            yes,
            mensurstrinch
        }

    }

}
