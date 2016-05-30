using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLTestViewerWPF
{
    public class Print
    {
        private float staff_spacing;
        private bool new_system;
        private bool new_page;
        private int blank_page;
        private int page_number;
    }
    class MeasureNumbering : Print
    {
        private MeasureNumberingType type;
        public MeasureNumberingType NumberingType { get { return type; } }
        public MeasureNumbering(XElement x)
        {
            var temp = x.Element("measure-numbering");
            type = temp.Value == "measure"? MeasureNumberingType.measure : temp.Value == "system"? MeasureNumberingType.system : MeasureNumberingType.none;
        }
        internal enum MeasureNumberingType
        {
            none,
            measure,
            system
        }
    }
    class MeasureDistance : Print
    {
        private float distance;
        public float Distance {  get { return distance; } }
        public MeasureDistance(float dist)
        {
            distance = dist;
        }
    }

    class PartNameDisplay : Print
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

    class PartAbbreviationDisplay : Print
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
    class StaffLayout : Print
    {
        private float distance;
        public float Distance { get { return distance; } }

        public StaffLayout(float distance)
        {
            this.distance = distance;
        }
        public StaffLayout(XElement x)
        {
            var stafflayout = x.Element("staff-layout");
            distance = (float)Convert.ToDouble(stafflayout.Element("staff-distance").Value);
        }
    }
    public enum AccidentalText
    {
        natural,
        flat,
        sharp
    }
}
