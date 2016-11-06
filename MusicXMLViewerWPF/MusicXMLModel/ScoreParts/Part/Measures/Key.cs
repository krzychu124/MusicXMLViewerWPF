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
    class Key : Segment, Misc.IDrawableMusicalChar//  MusicalChars //TODO_L implement missing properties 
    {
        #region Fields
        private EmptyPrintStyle additional_attributes;
        private int measure_num;
        private bool isSharp;
        private bool isNatural = false;
        private Fifths fifths;
        private Mode mode;
        private ClefType clef_type;
        #endregion
        #region Properties
        public EmptyPrintStyle AdditionalAttributes { get { return additional_attributes; } }
        public int MeasureNumber { get { return measure_num; } }
        public bool IsSharp { get { return isSharp; } }
        public bool IsNatural { get { return isNatural; } }
        public Fifths Fifths { get { return fifths; } }
        public Mode Mode { get { return mode; } }
        public SegmentType CharacterType { get { return SegmentType.KeySig; } }
        #endregion

        public Key( int fifths, string mode, int num)
        {
            //this.musicalcharacter = fifths < 0 ? "b" : fifths > 0 ? "#" : " ";
            isSharp = false;
            isSharp = fifths > 0 ? true : fifths < 0 ? false : isNatural = true;
            SetFifths(fifths);
            SetMode(mode);
            //this.type = MusSymbolType.Key;
            this.measure_num = num;
        }

        public Key(XElement x)
        {
            ID = Misc.RandomGenerator.GetRandomHexNumber();
            Segment_type = SegmentType.KeySig;
            additional_attributes = new EmptyPrintStyle(x.Attributes());
            this.mode = Mode.major;
            var ele = x.Elements();
            foreach (var item in ele)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "fifths":
                        SetFifths(int.Parse(item.Value));
                        break;
                    case "mode":
                        SetMode(item.Value);
                        break;
                    default:
                        break;
                }
            }
            isNatural = false;
            isSharp = false;
            isSharp = fifths > 0 ? true : fifths < 0 ? false : isNatural = true;
            Width = Math.Abs((int)Fifths) * 7f;
            clef_type = Clef.Sign_static;
            //recalculate_spacers();
        }

        private void SetMode(string s)
        {
            switch (s)
            {
                case "minor":
                    this.mode = Mode.minor;
                    break;
                case "major":
                    this.mode = Mode.major;
                    break;
                default:
                    this.mode = Mode.major;
                    break;
            }
        }

        public void Draw(DrawingVisual visual, Point p, ClefType sign)
        {
            using (DrawingContext dc = visual.RenderOpen())
            {
                Draw_Key(dc,p,sign,(int)Fifths);
            }
            
        }

        public void Draw(DrawingVisual visual)
        {
            DrawingVisual key = new DrawingVisual();
            using (DrawingContext dc = key.RenderOpen())
            {
                Brush KeyColor = (SolidColorBrush)new BrushConverter().ConvertFromString(AdditionalAttributes.Color);
                Draw_Key(dc, Relative, clef_type, (int)Fifths, color:KeyColor);  //! Experimental
            }
            visual.Children.Add(key);
        }
    
        public static void Draw_Key(DrawingContext dc, Point p, ClefType sign, int num = 0, Brush color = null) //TODO improve with clef sign....
        {
            int alt = -16;
            // num = 4;// test
            if (color == null)
            {
                color = (SolidColorBrush)new BrushConverter().ConvertFromString("Black");
            }
            if (num == 0)
            {
                // do nothing if key is sharp/flat-less
            }
            if (sign != null)
            {
                alt = sign.Sign == ClefType.Clef.GClef ? -16 : sign.Sign == ClefType.Clef.CClef ? -12 : -8;// 0= Gclef 4= Cclef 8= Fclef
            }
            else
            {
                bool isSharp = num > 0 ? true : false;  //check if sharp or flat key
                float x = (float)p.X; // x pos of measure
                float y = (float)p.Y; // y pos of measure
                float[] sharp = new float[] { 2, 12, -2, 8, 20, 4, 16 }; // y pos of each sharp symbol
                float[] flat = new float[] { 16, 4, 20, 8, 24, 12, 28 }; // y pos of each flat symbol
                int padding = isSharp ? 8 : 6; // different padding // difference in width of symbol
                float[] test = isSharp ? sharp : flat; // assign table o possitions
                string key = isSharp ? MusicalChars.Sharp : MusicalChars.Flat; // assign unicode symbol
                for (int i = 0; i < Math.Abs(num); i++)
                {
                    Misc.DrawingHelpers.DrawString(dc, key, TypeFaces.NotesFont, color, x + padding * i, y + (test[i] + alt), MusicScore.Defaults.Scale.Tenths); // draw
                }
            }
        }
    

        private void SetFifths(int i)
        {
            if(FifthDic.ContainsKey(i))
            {
                this.fifths = FifthDic[i];
            }
        }
        private static Dictionary<int, Fifths> FifthDic=new Dictionary<int, Fifths> {
            { -1, Fifths.F },
            { -2, Fifths.Bb },
            { -3, Fifths.Eb },
            { -4, Fifths.Ab },
            { -5, Fifths.Db },
            { 0,Fifths.C },
            { 1,Fifths.G },
            { 2,Fifths.D },
            { 3,Fifths.A },
            { 4,Fifths.E },
            { 5,Fifths.B },
            { -6,Fifths.Gb },
        };
    }
    enum Fifths
    {
        C,
        G =1,
        D =2,
        A =3,
        E =4,
        B =5,
        Gb =-6,
        Db =-5,
        Ab =-4,
        Eb =-3,
        Bb =-2,
        F = -1

    
    }
    enum Mode
    {
        major,
        minor,
        unknown
    }
}
