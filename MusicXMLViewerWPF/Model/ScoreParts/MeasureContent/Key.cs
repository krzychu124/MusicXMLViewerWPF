using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using MusicXMLScore.Helpers;
using MusicXMLViewerWPF.Misc;
using System.ComponentModel;

namespace MusicXMLViewerWPF
{
    class Key : Segment, Misc.IDrawableMusicalObject//  MusicalChars //TODO_L implement missing properties 
    {
        #region Fields
        private EmptyPrintStyle additional_attributes;
        private int measure_num;
        private bool isSharp;
        private bool isNatural = false;
        private Fifths fifths;
        private Mode mode;
        private ClefType clef_type;
        private bool loadstatus;
        private CanvasList drawablemusicalobject;
        private DrawableMusicalObjectStatus drawableobjectstatus;
        #endregion
        #region Properties
        public EmptyPrintStyle AdditionalAttributes { get { return additional_attributes; } }
        public int MeasureNumber { get { return measure_num; } }
        public bool IsSharp { get { return isSharp; } }
        public bool IsNatural { get { return isNatural; } }
        public Fifths Fifths { get { return fifths; } }
        public Mode Mode { get { return mode; } }
        public SegmentType CharacterType { get { return SegmentType.KeySig; } }
        public new PropertyChangedEventHandler PropertyChanged = delegate { };

        public CanvasList DrawableMusicalObject { get { return drawablemusicalobject; }  set { drawablemusicalobject = value; } }
        public DrawableMusicalObjectStatus DrawableObjectStatus { get { return drawableobjectstatus; } private set { if (drawableobjectstatus != value) drawableobjectstatus = value; PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(DrawableObjectStatus))); } }
        public bool Loaded { get { return loadstatus; } private set { if (loadstatus != value) loadstatus = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Loaded))); } }
        #endregion

        public Key( int fifths, string mode ="major", int num = 0):base()
        {
            PropertyChanged += KeyPropertyChanged;
            //this.musicalcharacter = fifths < 0 ? "b" : fifths > 0 ? "#" : " ";
            isSharp = false;
            isSharp = fifths > 0 ? true : fifths < 0 ? false : isNatural = true;
            SetFifths(fifths);
            SetMode(mode);
            //this.type = MusSymbolType.Key;
            Width = Math.Abs((int)Fifths) * 10f;
            Height = 60;
            this.measure_num = num;
            Segment_type = SegmentType.KeySig;
            Loaded = true;
        }

        public Key(XElement x)
        {
            PropertyChanged += KeyPropertyChanged;
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
            Width = Math.Abs((int)Fifths) * 7.5f;
            clef_type = Clef.Sign_static;
            //recalculate_spacers();
            Loaded = true;
        }

        private void KeyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Loaded":
                    if (Loaded)
                    {
                        InitDrawableObject();
                    }
                    break;
                case "DrawableObjectStatus":
                    if (DrawableObjectStatus == DrawableMusicalObjectStatus.reload)
                    {
                        ReloadDrawableObject();
                    }
                    break;
                default:
                    break;
            }
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
            DrawingVisual key = new DrawingVisual();
            using (DrawingContext dc = key.RenderOpen())
            {
                Draw_Key(dc,p,sign,(int)Fifths);
            }
            visual.Children.Add(key);
        }

        public void Draw(DrawingVisual visual)
        {
            DrawingVisual key = new DrawingVisual();
            using (DrawingContext dc = key.RenderOpen())
            {
                Brush KeyColor = Brushes.Black;//? (SolidColorBrush)new BrushConverter().ConvertFromString(AdditionalAttributes.Color);
                Draw_Key(dc, Relative, clef_type, (int)Fifths, color:KeyColor);  //! Experimental
            }
            visual.Children.Add(key);
        }

        public static void Draw_Key(DrawingContext dc, Point p, ClefType sign, int num = 0, Brush color = null) 
        {
            float scale = 40; //TODO refactor ** MusicScore.Defaults.Scale.Tenths;
            float alt = -0.4f * scale;
            // num = 4;// test
            if (color == null)
            {
                color = (SolidColorBrush)new BrushConverter().ConvertFromString("Black");
            }
            if (num == 0)
            {
                // do nothing if key is sharp/flat-less
            }
            if (sign != null) //0.4f * scale
            {
                alt = sign.Sign == ClefType.Clef.GClef ? -0.4f * scale : sign.Sign == ClefType.Clef.CClef ? -0.3f * scale : -0.2f * scale;// 0= Gclef 4= Cclef 8= Fclef
            }

            bool isSharp = num > 0 ? true : false;  //check if sharp or flat key
            float x = (float)p.X; // x pos of measure
            float y = (float)p.Y; // y pos of measure
            float[] sharp = new float[] { 2, 12, -2, 8, 20, 4, 16 }; // y pos of each sharp symbol
            float[] sharp_s = new float[] { 0.05f * scale, 0.3f * scale, -0.05f * scale, 0.2f * scale, 0.5f * scale, 0.1f * scale, 0.4f * scale}; //! with scale factor
            float[] flat = new float[] { 16, 4, 20, 8, 24, 12, 28 }; // y pos of each flat symbol
            float[] flat_s = new float[] { 0.4f * scale, 0.1f * scale, 0.5f * scale, 0.2f * scale, 0.6f * scale, 0.3f * scale, 0.7f * scale}; //! with scale factor
            float padding = isSharp ? 0.2f * scale : 0.15f * scale; // different padding // difference in width of symbol
            float[] test = isSharp ? sharp_s : flat_s; // assign table o possitions
            string key = isSharp ? MusicalChars.Sharp : MusicalChars.Flat; // assign unicode symbol
            for (int i = 0; i < Math.Abs(num); i++)
            {
                Misc.DrawingHelpers.DrawString(dc, key, TypeFaces.NotesFont, color, x + padding * i, y + (test[i] + alt), 40); //TODO MusicScore.Defaults.Scale.Tenths
            }
        }
        
    

        private void SetFifths(int i)
        {
            if(FifthDic.ContainsKey(i))
            {
                this.fifths = FifthDic[i];
            }
        }
        private static Dictionary<string, Fifths> fiftsmajorresolver = new Dictionary<string, Fifths> { ["C"] = Fifths.C, ["G"] = Fifths.G, ["D"] = Fifths.D, ["A"] = Fifths.A, ["E"] = Fifths.E, ["B"] = Fifths.B, ["F\u266f"] = Fifths.Fs, ["C\u266f"] = Fifths.Cs, ["F"] = Fifths.F, ["B\u266d"] = Fifths.Bb, ["E\u266d"] = Fifths.Eb, ["A\u266d"] = Fifths.Ab, ["D\u266d"] = Fifths.Db, ["G\u266d"] = Fifths.Gb, ["C\u266d"] = Fifths.Cb };
        public static Fifths GetFifths(string s)
        {
            Fifths f = Fifths.C;
            if (fiftsmajorresolver.ContainsKey(s))
            {
                f = fiftsmajorresolver[s];
            }
            return f;
        }

        public void InitDrawableObject()
        {
            if (Loaded)
            {
                DrawableMusicalObject = new CanvasList(this.Width, this.Height);
                DrawingVisual key = new DrawingVisual();
                Draw(key);
                DrawableMusicalObject.AddVisual(key);
                DrawableMusicalObject.SetValue(CustomMeasurePanel.StaticPositionProperty, true);
                DrawableObjectStatus = DrawableMusicalObjectStatus.ready;
            }
        }

        public void ReloadDrawableObject()
        {
            DrawableMusicalObject.ClearVisuals();
            DrawableObjectStatus = DrawableMusicalObjectStatus.notready;
            InitDrawableObject();
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
            { 6,Fifths.Fs },
            { 7,Fifths.Cs },
            { -6,Fifths.Gb },
            { -7,Fifths.Cb }
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
        Fs =6,
        Cs = 7,
        Cb =-7,
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
