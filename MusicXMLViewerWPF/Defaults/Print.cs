using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    public class Print //TODO_L test, looks good
    {
        //attributes //
        private float staff_spacing;
        private int blank_page;
        private int page_number;
        private YesNo new_page = YesNo.no;
        private YesNo new_system = YesNo.no;
        //additional settings //
        private Page page;
        private Defaults.SystemLayout sys_layout;
        private MeasureNumbering measure_numbering;
        private List<StaffLayout> staff_layout_list = new List<StaffLayout>();

        public bool NewPage { get { return new_page.GetTypeCode() == 0? false : true; } }
        public bool NewSystem { get { return new_system.GetTypeCode() == 0 ? false : true; } }
        public float StaffSpacing { get { return staff_spacing; } }
        public int BlankPage { get { return blank_page; } }
        public int PageNumber { get { return page_number; } }
        public Page Page { get { return page; } }
        public Defaults.SystemLayout SystemLayout { get { return sys_layout; } }
        public MeasureNumbering MeasureNumbering { get { return measure_numbering; } }
        public List<StaffLayout> StaffLayoutList { get { return staff_layout_list; } }

        public Print(XElement x)
        {
            if (x.Element("print") != null)
            {
                var print = x.Element("print");
                if (print.HasAttributes)
                {
                    var attributes = from z in print.Attributes() select z;
                    foreach (var item in attributes)
                    {
                        string name = item.Name.LocalName;
                        switch (name)
                        {
                            case "new-page":
                                new_page = item.Value == "yes" ? YesNo.yes : YesNo.no;
                                break;
                            case "new-system":
                                new_system = item.Value == "yes" ? YesNo.yes : YesNo.no;
                                break;
                            case "staff-spacing":
                                staff_spacing = float.Parse(item.Value, CultureInfo.InvariantCulture);
                                break;
                            case "blank-page":
                                blank_page = int.Parse(item.Value);
                                break;
                            case "page-number":
                                page_number = int.Parse(item.Value);
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (print.HasElements)
                {
                    var elements = from el in print.Elements() select el;
                    foreach (var item in elements)
                    {
                        string name = item.Name.LocalName;
                        switch (name)
                        {
                            case "system-layout":
                                sys_layout = new Defaults.SystemLayout(item);
                                break;
                            case "measure-numbering":
                                measure_numbering = new MeasureNumbering(item);
                                break;
                            case "staff-layout":
                                StaffLayout s = new StaffLayout(item);
                                staff_layout_list.Add(s);
                                break;
                            case "page-layout":
                                page = new Page(item);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
    public class MeasureNumbering
    {
        private MeasureNumberingType type;

        public MeasureNumberingType NumberingType { get { return type; } }

        public MeasureNumbering(XElement x)
        {
            var temp = x; //.Element("measure-numbering");
            type = temp.Value == "measure"? MeasureNumberingType.measure : temp.Value == "system"? MeasureNumberingType.system : MeasureNumberingType.none;
        }

        public enum MeasureNumberingType
        {
            none,
            measure,
            system
        }
    }
    class MeasureDistance
    {
        private float distance;

        public float Distance {  get { return distance; } }

        public MeasureDistance(float dist)
        {
            distance = dist;
        }
    }

    class PartNameDisplay 
    {
        private AccidentalText atext;
        private string accidentalsymbol;
        private string displaytext;

        public AccidentalText AccText { get { return atext; } }
        public string AccidentalSymbol { get { return accidentalsymbol; } }
        public string DisplayText { get { return displaytext; } }

        public PartNameDisplay(XElement x )
        {
            var temp = x.Element("part-name-display");
            displaytext = temp.Element("display-text").Value;
            atext = temp.Element("accidental-text").Value == "flat" ? AccidentalText.flat : temp.Element("accidental-text").Value == "sharp" ? AccidentalText.sharp : AccidentalText.natural;
            accidentalsymbol = atext == AccidentalText.flat ? MusChar.Flat : atext == AccidentalText.sharp ? MusChar.Sharp : MusChar.Natural;
        }
    }

    class PartAbbreviationDisplay 
    {
        private AccidentalText atext;
        private string accidentalsymbol;
        private string displaytext;

        public AccidentalText AccText { get { return atext; } }
        public string AccidentalSymbol { get { return accidentalsymbol; } }
        public string DisplayText { get { return displaytext; } }

        public PartAbbreviationDisplay(XElement x)
        {
            var temp = x.Element("part-abbreviation-display");
            displaytext = temp.Element("display-text").Value;
            atext = temp.Element("accidental-text").Value == "flat" ? AccidentalText.flat : temp.Element("accidental-text").Value == "sharp" ? AccidentalText.sharp : AccidentalText.natural;
            accidentalsymbol = atext == AccidentalText.flat ? MusChar.Flat : atext == AccidentalText.sharp ? MusChar.Sharp : MusChar.Natural;
        }
    }
    public class StaffLayout 
    {
        private float distance;
        private int number;

        public float Distance { get { return distance; } }
        public int Number { get { return number; } }

        public StaffLayout(float distance)
        {
            this.distance = distance;
        }
        public StaffLayout(XElement x)
        {
            var stafflayout = x;//.Element("staff-layout"); //TODO_H need test may crash here
            number = stafflayout.HasAttributes ? int.Parse(stafflayout.Attribute("number").Value) : 1;
            distance = (float)Convert.ToDouble(stafflayout.Element("staff-distance").Value);
        }
    }
    public enum AccidentalText
    {
        natural,
        flat,
        sharp
    }
    public enum YesNo
    {
        no,
        yes
    }
}
