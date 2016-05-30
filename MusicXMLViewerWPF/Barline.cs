using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF
{
    class Barline
    {
        private BarlineLocation location;
        private BarStyle style;
        private bool coda;
        private bool fermata;
        private bool segno;
        private MeasureEnding ending;

        enum BarStyle
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
        enum BarlineLocation
        {
            left,
            middle,
            right,
        }

    }

    class MeasureEnding
    {
        private EndingType type;
        private float end_length;
        private float text_x;
        private float text_y;
        private float x;
        private float y;
        private int[] number;
        private string ending_val; //for now // not tested //

        enum EndingType 
        {
            start,
            stop,
            discontinue
        }
    }
    
    
    class Repeat
    {
        private int times;
        private RepeatDirection direction;
        private Winged winged;

        enum RepeatDirection
        {
            backward,
            forward
        }
    }
    class Winged
    {
        private WingType type;
        public WingType Type { get { return type; } }
        public Winged(string s)
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
                case "double-straight": type = WingType.double_straight;
                    break;
                case "double-curved": type = WingType.double_curved;
                    break;
            }
           
           
        }
        public enum WingType
        {
            none,
            straight,
            curved,
            double_straight,
            double_curved
        }
    }
}
