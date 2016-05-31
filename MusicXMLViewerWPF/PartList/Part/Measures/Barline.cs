using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Barline : Measures, IDrawable
    {
        
        private BarlineLocation location;
        private BarStyle style;
        private bool coda;
        private bool fermata;
        private bool segno;
        private MeasureEnding ending;

        public BarlineLocation Location {  get { return location; } } 
        public BarStyle Style { get { return style; } }
        public bool IsCoda { get { return coda; } }
        public bool IsFermata { get { return fermata; } }
        public bool IsSegno { get { return segno; } }
        public MeasureEnding Ending { get { return ending; } }

        public void Draw(CanvasList surface)
        {

        }

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

    class MeasureEnding : Barline, IDrawable
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
    
    
    class Repeat : Barline, IDrawable
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
    class Winged : Barline, IDrawable, IXMLExtract
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
}
