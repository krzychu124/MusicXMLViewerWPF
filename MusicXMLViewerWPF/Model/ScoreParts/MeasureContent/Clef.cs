using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;
using MusicXMLScore.Helpers;
using MusicXMLViewerWPF.Misc;
using System.ComponentModel;

namespace MusicXMLViewerWPF
{
    class Clef : Segment, IDrawableMusicalObject
    {
        #region Fields
        private EmptyPrintStyle additional_attributes;
        private ClefType sign;
        private int line;
        private int measure_num;
        private static int clef_alter;
        private static ClefType sign_static;
        private bool visible = false;
        private static Clef cl;
        private int number = 0;
        private static int clef_alter_note;
        private CanvasList drawablemusicalobject;
        private DrawableMusicalObjectStatus dmusicalobjectstatus;
        private bool loadstatus;
        #endregion
        #region Properties
        public EmptyPrintStyle AdditionalAttributes { get { return additional_attributes; } }
        public ClefType Sign { get { return sign; } }
        public int Line { get { return line; } }
        public int MeasureId { get { return measure_num; } }
        public static int ClefAlter { get { return clef_alter; } }
        public static ClefType Sign_static { get { return sign_static; } }
        //public SegmentType CharacterType { get { return SegmentType.Clef; } }
        public bool IsVisible { get { return visible; } }
        public static Clef ClefStatic { get { return cl; } }
        public int Number { get { return number; } }
        public static int ClefAlterNote { get { return clef_alter_note; } }
        public new PropertyChangedEventHandler PropertyChanged = delegate { };

        public CanvasList DrawableMusicalObject { get { return drawablemusicalobject; }  set { drawablemusicalobject = value; } }
        public DrawableMusicalObjectStatus DrawableObjectStatus { get { return dmusicalobjectstatus; } private set { if (dmusicalobjectstatus != value) dmusicalobjectstatus = value; PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(DrawableObjectStatus))); } }
        public bool Loaded { get { return loadstatus; } private set { if (loadstatus != value) loadstatus = value; PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Loaded))); } }
        #endregion
        public Clef(XElement x)
        {
            ID = Misc.RandomGenerator.GetRandomHexNumber();
            PropertyChanged += ClefPropertyChanged;
            additional_attributes = x.Attributes()!=null ? new EmptyPrintStyle(x.Attributes()) : null;
            number = x.HasAttributes ? int.Parse(x.Attribute("number").Value) : 0;
            Segment_type = SegmentType.Clef;
            //-----------------------
            var ele = x.Elements();
            foreach (var item in ele)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "sign":
                        sign = new ClefType(item.Value);
                        sign_static = Sign;
                        clef_alter = sign.Sign == ClefType.Clef.GClef ? 0 : sign.Sign == ClefType.Clef.FClef ? -12 : -6;
                        visible = true;
                        break;
                    case "line":
                        line = int.Parse(item.Value);
                        break;
                    case "clef-octave-change":
                        Logger.Log("Clef-octave-change not implemented");
                        break;
                    default:
                        break;
                }
            }
            cl = this;
            SetClefAlterNote();
            Loaded = true;
        }

        private void ClefPropertyChanged(object sender, PropertyChangedEventArgs e)
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

        /// <summary>
        /// Calculate C4 Note position to set notes placement on staff
        /// </summary>
        private void SetClefAlterNote()
        {
            switch (Sign.Sign_s)
            {
                case "Clef C":
                    clef_alter_note = 0 - (line * 2);
                    break;
                case "Clef G":
                    clef_alter_note = 4 - (line * 2);
                    break;
                case "Clef F":
                    clef_alter_note = -4 - (line * 2);
                    break;
                default:
                    break;
            }
        }
        public Clef(string c, int line, int num)
        {
            
          //  base.type = MusSymbolType.Clef;
            this.line = line;
            this.measure_num = num;
            this.sign = new ClefType(c);

            clef_alter = sign.Sign == ClefType.Clef.GClef ? 0 : sign.Sign == ClefType.Clef.FClef? -12: -6;
        }

        public Clef(ClefType ct, int line = 0)
        {
            PropertyChanged += ClefPropertyChanged;
            Segment_type = SegmentType.Clef;
            sign = ct;
            if (line == 0)
            {
                GetDefaultClefLine(ct.Sign);
            }
            Loaded = true;
        }

        public void Draw(DrawingVisual visual)
        {
            DrawingVisual clef = new DrawingVisual();
            using( DrawingContext dc = clef.RenderOpen())
            {
                Brush clefColor = Brushes.Black;//? (SolidColorBrush)new BrushConverter().ConvertFromString(AdditionalAttributes.Color);
                Misc.DrawingHelpers.DrawString(dc, this.Sign.Symbol, TypeFaces.NotesFont, clefColor, Relative_x + Spacer_L, Relative_y, 40); 
                /**
                Experimental, Scale dependent
                **/
            }
            visual.Children.Add(clef);
        }

        public void InitDrawableObject()
        {

            if (Loaded)
            {
                DrawableMusicalObject = new MusicXMLScore.Helpers.CanvasList(this.Width, this.Height);
                DrawingVisual clef = new DrawingVisual();
                Draw(clef);
                DrawableMusicalObject.AddVisual(clef);
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

        private int GetDefaultClefLine(ClefType.Clef clef)
        {
            int result = 2;
            if (DefaultClefLines.ContainsKey(clef))
            {
                result = DefaultClefLines[clef];
            }
            return result;
        }

        private Dictionary<ClefType.Clef, int> DefaultClefLines = new Dictionary<ClefType.Clef, int>()
        {
            { ClefType.Clef.GClef, 2 },
            { ClefType.Clef.FClef, 4},
            { ClefType.Clef.CClef, 3}
        };
    }
}
