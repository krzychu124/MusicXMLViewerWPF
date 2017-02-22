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
    [Serializable]
    public class PartList // first idea :/ // can be hard to read, rework possible
    {
        private static SystemLayout systemlayout;
        private static Dictionary<string, ScorePart> score_parts = new Dictionary<string, ScorePart>() { };
        private List<PartGroup> part_group_list = new List<PartGroup>();

        //public static Page page;
        public static SystemLayout Systemlayout { get { return systemlayout; } }
        public List<PartGroup> PartGroup { get { return part_group_list; } }
        
        // private Identyfication_Class;
        public PartList(XElement x)
        {
            //page = new Page(); //TODO test, possible rework :/
            systemlayout = new SystemLayout();
            getPartList(x);
        }

        public void getPartList(XElement x )
        {
            //XDocument doc = LoadDocToClasses.Document; // TODO_H edit Xdoc replace with Xelement parameter // only temp 
            var partlist = x.Element("part-list").Elements();
           
            foreach (var item in partlist)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "score-part":
                        score_parts.Add(item.Attribute("id").Value, new ScorePart(item));
                        break;
                    case "part-group":
                        part_group_list.Add(new PartGroup(item));
                        break;
                    default:
                        break;
                }
               
                
            }
        }
    }

    public class PartGroup // not implemented // searching for MusicXML templates //
    {
        private GroupBarline groupbarline;
        private GroupSymbol symbol;
        private int number;
        private string groupabbreviation;
        private string groupname;
        private string type;

        public GroupBarline GroupBar { get { return groupbarline; } }
        public GroupSymbol GroupSymbol {  get { return symbol; } }
        public int Number { get { return number; } }
        public string GroupAbbreviation { get { return groupabbreviation; } }
        public string GroupName { get { return groupname; } }
        public string Type { get { return type; } }

        public PartGroup(XElement x)
        {
            number = int.Parse(x.Attribute("number").Value);
            type = x.Attribute("type").Value;
            var elements = x.Elements();
            foreach (var item in elements)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "group-name":
                        groupname = item.Value;
                        break;
                    case "group-abbreviation":
                        groupabbreviation = item.Value;
                        break;
                    case "group-symbol":
                        symbol = new GroupSymbol(item);
                        break;
                    case "group-barline":
                        groupbarline = item.Value == "yes" ? GroupBarline.yes : item.Value == "Mensurstrich" ? GroupBarline.mensurstrinch : GroupBarline.no;
                        break;
                    default: Logger.Log("[part-group] not implemented exc. in switch");
                        break;
                }
            }
            if (symbol != null)
            {
                type = symbol.Type_s;
            }
        }
        public PartGroup(int num, string t)
        {
            number = num;
            type = t;
            //isStart = type == "start" ? true : false;
        }
        public PartGroup(int num, string t, string barlines): this(num, t)
        {
            groupbarline = barlines == "yes" ? GroupBarline.yes : barlines == "Mensurstrich" ? GroupBarline.mensurstrinch : GroupBarline.no;
        }
        public void DrawGroupSymbol(DrawingContext dc, Point startMeasure, Point endMeasure, GroupSymbol type ) // possible to move this to measures class or make separate class for drawing everything ?? //
        {
            // later
        }

        public enum GroupBarline
        {
            no,
            yes,
            mensurstrinch
        }

    }
    public class GroupSymbol : PositionHelper
    {
        private PartGroupSymbol type;
        private string typ;

        public PartGroupSymbol Type { get { return type; } }
        public string Type_s { get { return typ; } }
        public GroupSymbol(XElement x) : base (x.Attributes())
        {
            typ = x.Value;
            type = x.Value == "none" ? PartGroupSymbol.none : 
                   x.Value == "square" ? PartGroupSymbol.square :
                   x.Value == "bracket" ? PartGroupSymbol.bracket : 
                   x.Value == "line" ? PartGroupSymbol.line : PartGroupSymbol.brace;
        } 
    }
    public enum PartGroupSymbol
    {
        none,
        square,
        bracket,
        line,
        brace
    }

}
