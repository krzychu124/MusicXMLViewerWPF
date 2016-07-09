using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Attributes //TODO_L working basics, missing features
    {
        // TODO_L private Divisions divisions;
        // TODO_L private PartSymbol part_symbol;
        // TODO_L private StaffDetails staff_details;
        // TODO_L public PartSymbol PartSymbol { get { return part_symbol; } }
        private Clef clef;
        private Key key;
        private MeasureStyle measure_style;
        private TimeSignature time;
        private Transpose transpose;
        private uint instruments;
        private uint staves = 1; // info how much staffs in part

        public Clef Clef { get { return clef; } }
        public Key Key { get { return key; } }
        public MeasureStyle MeasureStyle { get { return measure_style; } }
        public TimeSignature Time { get { return time; } }
        public Transpose Transpose { get { return transpose; } }
        public uint Instruments { get { return instruments; } }
        public uint Staves { get { return staves; } }

        public Attributes(XElement x) // possible rework from switch to dictionary// not further properties planned
        {
            var ele = x.Element("attributes").Elements();
            foreach (var item in ele)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "clef":
                        clef = new Clef(item);
                        break;
                    case "key":
                        key = new Key(item);
                        break;
                    case "measure-style":
                        measure_style = new MeasureStyle(item);
                        break;
                    case "time":
                        time = new TimeSignature(item);
                        break;
                    case "transpose":
                        transpose = new Transpose(item);
                        break;
                    case "instruments":
                        instruments = uint.Parse(item.Value);
                        break;
                    case "staves":
                        instruments = uint.Parse(item.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Draw(DrawingVisual visual, Point p, bool firstInLine = false) // rework attempt ... quite good
        {
            float currentX = (float)p.X;
            float currentY = (float)p.Y;
            if (Clef != null) // basic drawing done
            {
                currentX += 5f;
                DrawingVisual clefVisual = new DrawingVisual();
                using (DrawingContext dc = clefVisual.RenderOpen())
                {
                    string symbol = Clef.Sign.Symbol;
                    Misc.DrawingHelpers.DrawString(dc, symbol, TypeFaces.MeasuresFont, Brushes.Black, currentX, currentY, MusicScore.Defaults.Scale.Tenths);
                    currentX += 30f;
                }
                visual.Children.Add(clefVisual);
            }
            if (Key != null) // basic drawing done
            {
                DrawingVisual keyVisual = new DrawingVisual();
                if (Clef != null)
                {
                    Key.Draw(keyVisual, new Point(currentX, currentY), Clef.Sign);
                }
                else
                {
                    Key.Draw(keyVisual, new Point(currentX, currentY), Clef.Sign_static);
                }
                visual.Children.Add(keyVisual);
                currentX += Math.Abs((int)key.Fifths) *8;
            }
            if (Time != null) // basic drawing done
            {
                DrawingVisual timeVisual = new DrawingVisual();
                using (DrawingContext dc = timeVisual.RenderOpen())
                {
                    string beats = Time.BeatStr;
                    string beats_type = Time.BeatTypeStr;
                    Misc.DrawingHelpers.DrawString(dc, beats, TypeFaces.TimeNumbers, Brushes.Black, currentX, currentY , MusicScore.Defaults.Scale.Tenths);
                    Misc.DrawingHelpers.DrawString(dc, beats_type, TypeFaces.TimeNumbers, Brushes.Black, currentX, currentY , MusicScore.Defaults.Scale.Tenths);
                }
                visual.Children.Add(timeVisual);
            }
        }
    }
    //class PartSymbol //TODO_L class PartSymbol
    //{
        
    //}
    class Transpose
    {
        private int number; // optional, if not presentet value=0;
        private int diatonic;
        private int chromatic;
        private int octave_change;

        public int Number { get { return number; } }
        public int Diatonic { get { return diatonic; } }
        public int Chromatic { get { return chromatic; } }
        public int OctaveChange { get { return octave_change; } }

        public Transpose(XElement x)
        {
            number = x.HasAttributes ? int.Parse(x.Attribute("number").Value) : 0;
            var ele = from el in x.Elements() select el;
            foreach (var item in ele)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "diatonic":
                        diatonic = int.Parse(item.Value);
                        break;
                    case "chromatic":
                        chromatic = int.Parse(item.Value);
                        break;
                    case "octave-change":
                        octave_change = int.Parse(item.Value);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    class MeasureStyle
    {
        private int multiple_rest;
        private BeatRepeat beat_repeat;
        private MeasureRepeat measure_repeat;
        private Slash slash;

        public int MultipleRest { get { return multiple_rest; } }
        public BeatRepeat BeatRepeats { get { return beat_repeat; } }
        public MeasureRepeat MeasureRepeat { get { return measure_repeat; } }
        public Slash Slash { get { return slash; } }

        public MeasureStyle(XElement x)
        {
            var ele = x.Elements();
            foreach (var item in ele)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "beat-repeat":
                        beat_repeat = new BeatRepeat(item);
                        break;
                    case "measure-repeat":
                        measure_repeat = new MeasureRepeat(item);
                        break;
                    case "multiple-rest":
                        multiple_rest = int.Parse(item.Value);
                        break;
                    case "slash":
                        slash = new Slash(item);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    internal class Slash // TODO_VL missing features // rarely used
    {
        private StartStop type;
        private bool hasDots;
        private bool hasStems;
        private NoteType slash_type;
        private int dot = 0;

        public StartStop Type { get { return type; } }
        public bool HasDots { get { return hasDots; } }
        public bool HasStems { get { return hasStems; } }
        public NoteType SlashType { get { return slash_type; } }
        public int Dot { get { return dot; } }

        public Slash(XElement x)
        {
            var attr = x.Attributes();
            foreach (var item in attr)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "type":
                        type = item.Value == "start" ? StartStop.start : StartStop.stop ;
                        break;
                    case "use-stems":
                        hasStems = item.Value == "no" ? false : true;
                        break;
                    case "use-dots":
                        hasDots = item.Value == "no" ? false : true;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    internal class MeasureRepeat
    {
        private StartStop type;
        private int slashes = 1;
        private int val;
        public StartStop Type { get { return type; } }
        public int Slashes { get { return slashes; } }

        public MeasureRepeat(XElement x)
        {
            val = int.Parse(x.Value); // TODO need tests
            var attr = x.Attributes();
            foreach (var item in attr)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "type":
                        type = item.Value == "start" ? StartStop.start : StartStop.stop;
                        break;
                    case "slashes":
                        slashes = int.Parse(item.Value);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    internal class BeatRepeat // TODO_VL missing features // rarely used
    {
        private StartStop type;
        private int slashes = 1;
        private bool hasDots;
        private NoteType slash_type;
        private int dot = 0;

        public StartStop Type { get { return type; } }
        public int Slashes { get { return slashes; } }
        public bool HasDots { get { return hasDots; } }
        public NoteType SlashType { get { return slash_type; } }
        public int Dot { get { return dot; } }

        public BeatRepeat(XElement x)
        {
            var attr = x.Attributes();
            foreach (var item in attr)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "type":
                        type = item.Value == "start" ? StartStop.start : StartStop.stop;
                        break;
                    case "slashes":
                        slashes = int.Parse(item.Value);
                        break;
                    case "use-dots":
                        hasDots = item.Value == "no" ? false : true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
    enum NoteType
    {
        whole,
        half,
        quarter,
        eighth,
        d16th,
        d32nd,
        d64th,
        d128th,
        d256th,
        d512th,
        d1024th
    }
    public enum StartStop
    {
        start,
        stop
    }
}
