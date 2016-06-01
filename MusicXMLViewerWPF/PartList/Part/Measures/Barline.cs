using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Barline :  IDrawable, IXMLExtract // TODO test class
    {
        
        private BarlineLocation location;
        private BarStyle style;
        private bool coda;
        private bool fermata;
        private Segno segno;
        private MeasureEnding ending;
        private Repeat repeat;
        private Winged winged;

        public BarlineLocation Location {  get { return location; } } 
        public BarStyle Style { get { return style; } }
        public bool IsCoda { get { return coda; } }
        public bool IsFermata { get { return fermata; } }
        public Segno Segno{ get { return segno; } }
        public MeasureEnding Ending { get { return ending; } }
        public Repeat Repeat { get { return repeat; } }
        public Winged Winged { get { return winged; } }

        public Barline(XElement x)
        {
            XMLFiller(x);
        }

        public void XMLFiller(XElement x)
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
    class Segno : EmptyPrintStyle
    {
        private string name = "segno";
        public string Name { get { return name; } }
        public Segno(IEnumerable<XAttribute> x) : base(x)
        {
            
        }
    }
    class MeasureEnding : IDrawable
    {
        private EndingType type;
        private float end_length;
        private float text_x;
        private float text_y;
        private float x_shift;
        private float y_shift;
        private int[] number;
        private string ending_val; //for now // not tested //

        public EndingType Type { get { return type; } }
        public float EndLength { get { return end_length; } }
        public float TextX { get { return text_x; } }
        public float TextY { get { return text_y; } }
        public float X { get { return x_shift; } }
        public float Y { get { return y_shift; } }
        public int[] Number {  get { return number; } }
        public string Ending_val {  get { return ending_val; } }

        public MeasureEnding(XElement x)
        {

        }

        private void getEndingType (string s)
        {
            type = s == "start" ? EndingType.start : s == "stop" ? EndingType.stop : EndingType.discontinue; 
        }

        public new void Draw(CanvasList surface)
        {

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

        public new void Draw(CanvasList surface)
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

        public new void Draw(CanvasList surface)
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
    class EmptyPrintStyle // TODO test class
    {
        float def_x;
        float def_y;
        float rel_x;
        float rel_y;
        Halign h_align;
        Valign v_align;
        string color;
        public EmptyPrintStyle()
        {

        }
        public EmptyPrintStyle(IEnumerable<XAttribute> x)
        {
            FillAttributes(x);
        }
        private void FillAttributes(IEnumerable<XAttribute> x) // TODO check if it's working
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
    enum Halign
    {
        left,
        center,
        right
    }
    enum Valign
    {
        top,
        middle,
        bottom,
        baseline
    }
}
