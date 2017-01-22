using MusicXMLViewerWPF.ScoreParts.MeasureContent;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Barline :  Segment, IDrawable, IXMLExtract // todo ixmlextrack removeal
    {
        #region Fields
        private BarlineLocation location;
        private BarStyle style;
        private Coda coda;
        private Fermata fermata;
        private Segno segno;
        private Ending ending;
        private Repeat repeat;
        private Point pos;
        private DrawingVisual visual;
        private string measureid;
        #endregion
        #region Properties
        public BarlineLocation Location { get { return location; } set { location = value; } } 
        public BarStyle Style { get { return style; } set { style = value; } }
        public Coda Coda { get { return coda; } }
        public Fermata Fermata { get { return fermata; } }
        public Segno Segno  { get { return segno; } }
        public Ending Ending { get { return ending; } }
        public Repeat Repeat { get { return repeat; } }
        public Point Position { get { return pos; } set { pos = value; } }
        public DrawingVisual Visual { get { return visual; } set { visual = value; } }
        public string MeasureID { get { return measureid; } set { measureid = value; } }
        #endregion

        public Barline()
        {
            ID = Misc.RandomGenerator.GetRandomHexNumber();
            location = BarlineLocation.right;
            coda = null;
            fermata = null;
            segno = null;
            ending = null;
            repeat = null;
        }

        public Barline(XElement x)
        {
            ID = Misc.RandomGenerator.GetRandomHexNumber();
            coda = null;
            fermata = null;
            segno = null;
            ending = null;
            repeat = null;
            ExtractXElement(x);
        }

        public void ExtractXElement(XElement x)
        {
            //x = x.Element("barline") != null ? x.Element("barline") : null;

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
        public void Draw(CanvasL surface, Point p)
        {
            
        }

        public void Draw(DrawingVisual visual, Point p)
        {

        }

        public void Draw(DrawingContext dc, Point p, float Width)
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

        public DrawingVisual DrawBarline(DrawingVisual visual, Point p, float width)
        {
            Position = p;
            float scale = 40; //! Temporary change **needs refactoring** MusicScore.Defaults.Scale.Tenths;
            using (DrawingContext dc = visual.RenderOpen())
            {
                float loc = location == BarlineLocation.left ? (float)p.X : (float)p.X + width;
                switch (style)
                {
                    case BarStyle.regular:
                        Misc.DrawingHelpers.DrawString(dc, MusicalChars.RegularBar, TypeFaces.MeasuresFont, Brushes.Black, loc, (float)p.Y, scale);
                        break;
                    case BarStyle.dotted:
                        Misc.DrawingHelpers.DrawString(dc, MusicalChars.DottedBar, TypeFaces.MeasuresFont, Brushes.Black, loc, (float)p.Y, scale);
                        break;
                    case BarStyle.dashed:
                        Misc.DrawingHelpers.DrawString(dc, MusicalChars.DashedBar, TypeFaces.MeasuresFont, Brushes.Black, loc, (float)p.Y, scale);
                        break;
                    case BarStyle.heavy:
                        Misc.DrawingHelpers.DrawString(dc, MusicalChars.HeavyBar, TypeFaces.MeasuresFont, Brushes.Black, loc, (float)p.Y, scale);
                        break;
                    case BarStyle.light_light:
                        Misc.DrawingHelpers.DrawString(dc, MusicalChars.LightLightBar, TypeFaces.MeasuresFont, Brushes.Black, loc, (float)p.Y, scale);
                        break;
                    case BarStyle.light_heavy:
                        Misc.DrawingHelpers.DrawString(dc, MusicalChars.LightHeavyBar, TypeFaces.MeasuresFont, Brushes.Black, loc - 7, (float)p.Y, scale);
                        break;
                    case BarStyle.heavy_light:
                        Misc.DrawingHelpers.DrawString(dc, MusicalChars.HeavyLightBar, TypeFaces.MeasuresFont, Brushes.Black, loc - 3, (float)p.Y, scale);
                        break;
                    case BarStyle.heavy_heavy:
                        Misc.DrawingHelpers.DrawString(dc, MusicalChars.HeavyHeavyBar, TypeFaces.MeasuresFont, Brushes.Black, loc, (float)p.Y, scale);
                        break;
                    case BarStyle.tick:
                        Misc.DrawingHelpers.DrawString(dc, MusicalChars.TickBar, TypeFaces.MeasuresFont, Brushes.Black, loc, (float)p.Y-4, scale);
                        break;
                    case BarStyle.shortened:
                        Misc.DrawingHelpers.DrawString(dc, MusicalChars.ShortBar, TypeFaces.MeasuresFont, Brushes.Black, loc, (float)p.Y+12, scale);
                        break;
                    default:
                        break;
                }
                if (Repeat != null)
                {
                    DrawingVisual visualForRepeats = new DrawingVisual();
                    using (DrawingContext dc2 = visualForRepeats.RenderOpen())
                    {
                        float location = Repeat.Direction == Repeat.RepeatDirection.forward ? 7f : -12f;
                        if (Repeat.Winged == null) //Repeat.Winged.Type == Winged.WingType.none || 
                        {
                            Misc.DrawingHelpers.DrawString(dc, MusicalChars.RepeatDots, TypeFaces.MeasuresFont, Brushes.Black, loc + location, (float)p.Y, scale);
                        }
                    }
                    visual.Children.Add(visualForRepeats);
                }
                
            }
            Visual = visual;
            return visual;
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
        private string _symbol;
        private string measureid;

        public string Name { get { return name; } }
        public string Symbol { get { return _symbol; } }
        public string MeasureID { get { return measureid; } set { measureid = value; } }

        public Segno(IEnumerable<XAttribute> x) : base(x)
        {
            //TODO_L missing implementation
            _symbol = MusicalChars.Segno;
        }

        public void Draw(DrawingVisual visual, Point p)
        {
            DrawingVisual segno = new DrawingVisual();
            using (DrawingContext dc = segno.RenderOpen())
            {
                Misc.DrawingHelpers.DrawString(dc, Symbol, TypeFaces.MeasuresFont, Brushes.Black, (float)p.X - DefX - 8, (float)p.Y - this.DefY -20, 40 / 1.5f); //TODO Implement scale **Refactor needed** MusicScore.Defaults.Scale.Tenths
            }
            visual.Children.Add(segno);
        }
    }
    public class Coda : EmptyPrintStyle
    {
        private string name = "coda";
        private string _symbol;
        private string measureid;

        public string Name { get { return name; } }
        public string Symbol { get { return _symbol; } }
        public string MeasureID { get { return measureid; } set { measureid = value; } }

        public Coda(IEnumerable<XAttribute> x) : base(x)
        {
            _symbol = MusicalChars.Coda;
        }
        public void Draw(DrawingVisual visual)
        {
            Point position = new Point();
            Measure measure = (Measure)Misc.ScoreSystem.GetMeasureSegment(MeasureID);
            position = measure.Relative;
            position.X += DefX;
            Draw(visual, position);
        }
        public void Draw(DrawingVisual visual, Point p) 
        {
            DrawingVisual coda = new DrawingVisual();
            using (DrawingContext dc = coda.RenderOpen())
            {
                Misc.DrawingHelpers.DrawString(dc, Symbol, TypeFaces.MeasuresFont, Brushes.Black, (float)p.X - this.DefX - 10, (float)p.Y - this.DefY -20, 40 / 1.5f); //TODO Scale refactor - MusicScore.Defaults.Scale.Tenths 
            }
            visual.Children.Add(coda);
        }
    }
    public class Fermata : EmptyPrintStyle
    {
        private string name = "fermata";
        private UprightInverted type;
        private string _symbol;

        public string Name { get { return name; } }
        public string Symbol { get { return _symbol; } }

        public Fermata(IEnumerable<XAttribute> x) : base(x)
        {
            
            foreach (var item in x)
            {
                string t = item.Value;
                type = t == "upright" ? UprightInverted.upright : UprightInverted.inverted;
            }
        }

        public void Draw(DrawingVisual visual, Point p)
        {
            DrawingVisual fermata = new DrawingVisual();
            using (DrawingContext dc = fermata.RenderOpen())
            {
                Misc.DrawingHelpers.DrawString(dc, Symbol, TypeFaces.MeasuresFont, Brushes.Black, (float)p.X - this.DefX, (float)p.Y - this.DefY, 40 / 2); //TODO MusicScore.Defaults.Scale.Tenths
            }
            visual.Children.Add(fermata);
        }
    }
    class Ending : EmptyPrintStyle
    {
        private int measure_number;
        private EndingType type;
        private float end_length;
        private float text_x;
        private float text_y;
        private int number;
        private string ending_val; //for now // not tested //

        public int MeasureNumber { get { return measure_number; } }
        public EndingType Type { get { return type; } }
        public float EndLength { get { return end_length; } }
        public float TextX { get { return text_x; } }
        public float TextY { get { return text_y; } }
        public int Number {  get { return number; } }
        public string Ending_val {  get { return ending_val; } }
        public static List<Endingtemp> EndingTempList = new List<Endingtemp>();

        public Ending(XElement z): base(z.Attributes())
        {
            var x = z.Attributes();
            ending_val = z.Value;
            measure_number = int.Parse(z.Parent.Parent.Attribute("number").Value);
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
                        number = int.Parse(item.Value); // string value to array: "1, 2, 3" to  []{1,2,3}
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
        public void Draw(Point p, float width, CanvasL surface)
        {
            
            
        }
        public DrawingVisual DrawEnding(DrawingVisual visual, Point p, float width)
        {
            EndingTempList.Add(new Endingtemp(p,this,width));
            
            if (EndingTempList[EndingTempList.Count-1].t != EndingType.start)
            {
               
                int current_ending_nr = this.Number;
                var extr = EndingTempList.Select( i => i).Where(i => i.Num == current_ending_nr).ToList();
                if (extr.Count != 1)
                {
                    for (int i = 0; i < extr.Count; i++)
                    {
                        if (extr.ElementAt(i).Ending.Type != EndingType.start)
                        {
                            float ending_length = extr.ElementAt(i-1).Ending.EndLength * 0.6f;
                            Pen pen = new Pen(Brushes.Black, 1);
                            float temp = ending_length;
                            DrawingVisual visualEnding = new DrawingVisual();
                            using (DrawingContext dc = visualEnding.RenderOpen())
                            {
                                float txtX = (float)extr.ElementAt(i - 1).P.X + 4;
                                float txtY = (float)extr.ElementAt(i - 1).P.Y - 2;
                                dc.DrawLine(pen, new Point(extr.ElementAt(i-1).P.X, extr.ElementAt(i-1).P.Y + ending_length), new Point(extr.ElementAt(i-1).P.X, extr.ElementAt(i -1).P.Y - 6));
                                Misc.DrawingHelpers.DrawString(dc, extr.ElementAt(i - 1).Ending.Number + ".", TypeFaces.TextFont, Brushes.Black, txtX, txtY, 12f);
                                dc.DrawLine(pen, new Point(extr.ElementAt(i - 1).P.X, extr.ElementAt(i - 1).P.Y - 6), new Point(extr.ElementAt(i).P.X + extr.ElementAt(i).Width, extr.ElementAt(i).P.Y - 6));
                                if (extr.ElementAt(i).Ending.Type == EndingType.discontinue)
                                {
                                    ending_length = 0f;
                                }
                                dc.DrawLine(pen, new Point(extr.ElementAt(i - 1).P.X, extr.ElementAt(i - 1).P.Y - 6), new Point(extr.ElementAt(i - 1).P.X, extr.ElementAt(i - 1).P.Y + ending_length));
                                ending_length = temp;
                                extr.RemoveAll( r => r.Num == current_ending_nr);
                                EndingTempList.RemoveAll(r => r.Num == current_ending_nr);
                            }
                            visual.Children.Add(visualEnding);
                        }
                    }

                }
            }
            return visual;
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
        internal class Endingtemp
        {
            public Ending Ending;
            public float Width;
            public int Num;
            public Point P;
            public EndingType t;

            public Endingtemp(Point p, Ending ending, float width)
            {
                P = p;
                t = ending.Type;
                Num = ending.Number;
                Ending = ending;
                Width = width;
            }
        }
    }
    class Repeat 
    {
        private int times;
        private RepeatDirection direction;
        private Winged winged;

        public int Times { get { return times; } }
        public RepeatDirection Direction { get { return direction; } }
        public Winged Winged { get { return winged; } }

        public Repeat(IEnumerable<XAttribute> x)
        {
            winged = null;
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

        public void Draw(DrawingVisual visual, Point p)
        {
            if (Winged != null)
            {
                throw new NotImplementedException();
            }
        }

        internal enum RepeatDirection
        {
            backward,
            forward
        }
    }
    class Winged :  IDrawable, IXMLExtract //todo ixmllextract removal
    {
        private WingType type;
        private string s_type;
        private Point pos;
        private DrawingVisual visual;

        public WingType Type { get { return type; } }
        public string Type_s { get { return s_type; } }
        public Point Position { get { return pos; } set { pos = value; } }
        public DrawingVisual Visual { get { return visual; } set { visual = value; } }

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
                ExtractXElement(item);
            }
            
        }

        public IEnumerable<XElement> XMLExtractor()
        {
            XDocument x = LoadDocToClasses.Document;
            var z = from item in x.Elements() select item;
            return z;
        }

        public void ExtractXElement(XElement x)
        {

        }

        public void Draw(DrawingVisual visual, Point p)
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
    public class EmptyPrintStyle
    {
        protected float def_x;
        protected float def_y;
        protected float rel_x;
        protected float rel_y;
        protected float font_size = 14;
        protected string font_weight;
        protected Halign h_align;
        protected Valign v_align;
        protected string color = "black";

        public float DefX { get { return def_x; } }
        public float DefY { get { return def_y; } }
        public float RelX { get { return rel_x; } }
        public float RelY { get { return rel_y; } }
        public float FontSize { get { return font_size; } }
        public string FontWeight { get { return font_weight; } }
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
        private void FillAttributes(IEnumerable<XAttribute> x) 

        {
            h_align = Halign.none;
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
                    case "justify":
                        string hval2 = item.Value;
                        if (h_align == Halign.none)
                        {
                            h_align = hval2 == "left" ? Halign.left : hval2 == "center" ? Halign.center : Halign.right;
                        }
                        
                        break;
                }
            }
        }
    }
    public enum Halign
    {
        none,
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
