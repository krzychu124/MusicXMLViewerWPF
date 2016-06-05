using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Barline :  IDrawable, IXMLExtract // TODO_L test class
    {
        
        private BarlineLocation location;
        private BarStyle style;
        private Coda coda;
        private Fermata fermata;
        private Segno segno;
        private Ending ending;
        private Repeat repeat;

        public BarlineLocation Location { get { return location; } } 
        public BarStyle Style { get { return style; } set { style = value; } }
        public Coda Coda { get { return coda; } }
        public Fermata Fermata { get { return fermata; } }
        public Segno Segno  { get { return segno; } }
        public Ending Ending { get { return ending; } }
        public Repeat Repeat { get { return repeat; } }

        public Barline()
        {
            location = BarlineLocation.left;
            coda = null;
            fermata = null;
            segno = null;
            ending = null;
            repeat = null;
        }

        public Barline(XElement x)
        {
            coda = null;
            fermata = null;
            segno = null;
            ending = null;
            repeat = null;
            XMLFiller(x);
        }

        public void XMLFiller(XElement x)
        {
            x = x.Element("barline") != null ? x.Element("barline") : null;
            if (x != null)
            {
                location = x.Attribute("location").Value == "left" ? BarlineLocation.left : x.Attribute("location").Value == "middle" ? BarlineLocation.middle : BarlineLocation.right;
                if (x.Element("bar-style") != null)
                {
                    getStyle(x.Element("bar-style").Value);
                }
                if (x.Element("segno") != null)
                {
                    var attr = from at in x.Element("segno").Attributes() select at;
                    segno = new Segno(attr);
                }
                if (x.Element("coda") != null)
                {
                    var attr = from at in x.Element("coda").Attributes() select at;
                    coda = new Coda(attr);
                }
                if (x.Element("fermata") != null)
                {
                    var attr = from at in x.Element("fermata").Attributes() select at;
                    fermata = new Fermata(attr);
                }
                if (x.Element("ending") != null)
                {
                    var el = x.Element("ending");
                    ending = new Ending(el);
                }
                if (x.Element("repeat") != null)
                {
                    var attr = from at in x.Element("repeat").Attributes() select at;
                    repeat = new Repeat(attr);
                }
            }
        }
        public void Draw(CanvasList surface)
        {
            
        }

        private void getStyle(string s)
        {
            style = BarStyle.regular;
            if (styleDictionary.ContainsKey(s))
            {
                style = styleDictionary[s];
            }
        }
        private Dictionary<string, BarStyle> styleDictionary = new Dictionary<string, BarStyle>()
        {
            {"regular",BarStyle.regular },
            {"dotted",BarStyle.dotted },
            {"dashed",BarStyle.dashed },
            {"heavy",BarStyle.heavy },
            {"light-light",BarStyle.light_light },
            {"light-heavy",BarStyle.light_heavy },
            {"heavy-light",BarStyle.heavy_light },
            {"heavy-heavy",BarStyle.heavy_heavy },
            {"tick",BarStyle.tick },
            {"short",BarStyle.shortened },
        };

        internal enum BarStyle
        {
            regular,
            dotted,
            dashed,
            heavy,
            light_light,
            light_heavy,
            heavy_light,
            heavy_heavy,
            tick,
            shortened,
        }

        internal enum BarlineLocation
        {
            left,
            middle,
            right,
        }

    }
    public class Segno : EmptyPrintStyle
    {
        private string name = "segno";
        public string Name { get { return name; } }
        public Segno(IEnumerable<XAttribute> x) : base(x)
        {
            
        }
    }

    public class Coda : EmptyPrintStyle
    {
        private string name = "coda";
        public string Name { get { return name; } }
        public Coda(IEnumerable<XAttribute> x) : base(x)
        {

        }
    }
    public class Fermata : EmptyPrintStyle
    {
        private string name = "fermata";
        private UprightInverted type;
        public string Name { get { return name; } }
        public Fermata(IEnumerable<XAttribute> x) : base(x)
        {
            
            foreach (var item in x)
            {
                string t = item.Value;
                type = t == "upright" ? UprightInverted.upright : UprightInverted.inverted;
            }
        }
    }
    class Ending : EmptyPrintStyle, IDrawable
    {
        private EndingType type;
        private float end_length;
        private float text_x;
        private float text_y;
        private int[] number;
        private string ending_val; //for now // not tested //

        public EndingType Type { get { return type; } }
        public float EndLength { get { return end_length; } }
        public float TextX { get { return text_x; } }
        public float TextY { get { return text_y; } }
        public int[] Number {  get { return number; } }
        public string Ending_val {  get { return ending_val; } }

        public Ending(XElement z): base(z.Attributes())
        {
            var x = z.Attributes();
            ending_val = z.Value;
            foreach (var item in x)
            {
                string s = item.Name.LocalName;
                switch (s)
                {
                    case "type":
                        GetEndingType(item.Value);
                        break;
                    case "end-length":
                        end_length = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    case "text-x":
                        text_x = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    case "text-y":
                        text_y = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    case "number":
                        number = GetNumbersArray(item.Value); // string value to array: "1, 2, 3" to  []{1,2,3}
                        break;
                    default:
                        break;
                }
                
            }
        }

        private void GetEndingType (string s)
        {
            type = s == "start" ? EndingType.start : s == "stop" ? EndingType.stop : EndingType.discontinue; 
        }

        public void Draw(CanvasList surface)
        {

        }

        private int[] GetNumbersArray(string s)
        {
            int[] temp = s.Split(',').Select(n => Convert.ToInt32(n)).ToArray(); // s.Split(',').Select(int.Parse).ToArray();
            return temp;
        }

        internal enum EndingType 
        {
            start,
            stop,
            discontinue
        }
    }
    
    
    class Repeat : IDrawable
    {
        private int times;
        private RepeatDirection direction;
        private Winged winged;

        public int Times { get { return times; } }
        public RepeatDirection Direction { get { return direction; } }
        public Winged Winged { get { return winged; } }

        public Repeat(IEnumerable<XAttribute> x)
        {
            foreach (var item in x)
            {
                string s = item.Name.LocalName;
                switch (s)
                {
                    case "direction":
                        direction = item.Value == "backward" ? RepeatDirection.backward : RepeatDirection.forward;
                        break;
                    case "winged":
                        winged = new Winged(item.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Draw(CanvasList surface)
        {

        }

        internal enum RepeatDirection
        {
            backward,
            forward
        }
    }
    class Winged :  IDrawable, IXMLExtract
    {
        private WingType type;
        private string s_type;

        public WingType Type { get { return type; } }
        public string Type_s { get { return s_type; } }

        public Winged(string s)
        {
            s_type = s;
        }
        private void getWingType(string s)
        {
            switch (s)
            {
                case "none":
                    type = WingType.none;
                    break;
                case "straight":
                    type = WingType.straight;
                    break;
                case "curved":
                    type = WingType.curved;
                    break;
                case "double-straight":
                    type = WingType.double_straight;
                    break;
                case "double-curved":
                    type = WingType.double_curved;
                    break;
            }
        }
        public Winged()
        {
            
            var xel =  XMLExtractor();
            foreach (var item in xel)
            {
                XMLFiller(item);
            }
            
        }

        public IEnumerable<XElement> XMLExtractor()
        {
            XDocument x = LoadDocToClasses.Document;
            var z = from item in x.Elements() select item;
            return z;
        }

        public void XMLFiller(XElement x)
        {

        }

        public void Draw(CanvasList surface)
        {

        }

        internal enum WingType
        {
            none,
            straight,
            curved,
            double_straight,
            double_curved
        }
    }
    public class EmptyPrintStyle // TODO_L test class
    {
        protected float def_x;
        protected float def_y;
        protected float rel_x;
        protected float rel_y;
        protected float font_size;
        protected string font_weight;
        protected Halign h_align;
        protected Valign v_align;
        protected string color;

        public float DefX { get { return def_x; } }
        public float DefY { get { return def_y; } }
        public float RelX { get { return rel_x; } }
        public float RelY { get { return rel_y; } }
        public Halign HAlign { get { return h_align; } }
        public Valign VAlign { get { return v_align; } }
        public string Color { get { return color; } }

        public EmptyPrintStyle()
        {

        }
        public EmptyPrintStyle(IEnumerable<XAttribute> x)
        {
            FillAttributes(x);
        }
        private void FillAttributes(IEnumerable<XAttribute> x) // TODO_L check if it's working
        {
            foreach (var item in x)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "default-x":
                        def_x = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    case "default-y":
                        def_y = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    case "relative-x":
                        rel_x = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    case "relative-y":
                        rel_y = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    case "font-size":
                        font_size = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    case "font-weight":
                        font_weight = item.Value;
                        break;
                    case "valign":
                        string vval = item.Value;
                        v_align = vval == "top" ? Valign.top : vval == "bottom" ? Valign.bottom : vval == "middle" ? Valign.middle : Valign.baseline;
                        break;
                    case "halign":
                        string hval = item.Value;
                        h_align = hval == "left" ? Halign.left : hval == "center" ? Halign.center : Halign.right;
                        break;
                    case "color":
                        color = item.Value;
                        break;
                    
                }
            }
        }
    }
    public enum Halign
    {
        left,
        center,
        right
    }
    public enum Valign
    {
        top,
        middle,
        bottom,
        baseline
    }
    public enum UprightInverted
    {
        upright,
        inverted
    }
}
