using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    public class Clef : Segment, Misc.IDrawableMusicalChar
    {
        #region Fields
        private EmptyPrintStyle additional_attributes;
        private ClefType sign;
        private int line;
        private int measure_num;
        private static int clef_alter;
        private static ClefType sign_static;
        private bool visible = false;
        #endregion
        #region Properties
        public EmptyPrintStyle AdditionalAttributes { get { return additional_attributes; } }
        public ClefType Sign { get { return sign; } }
        public int Line { get { return line; } }
        public int MeasureId { get { return measure_num; } }
        public static int ClefAlter { get { return clef_alter; } }
        public static ClefType Sign_static { get { return sign_static; } }
        public SegmentType CharacterType { get { return SegmentType.Clef; } }
        public bool IsVisible { get { return visible; } }
        #endregion
        public Clef(XElement x)
        {
            ID = Misc.RandomGenerator.GetRandomHexNumber();
            additional_attributes = new EmptyPrintStyle(x.Attributes());
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
        }
        
        public Clef(string c, int line, int num)
        {
            
          //  base.type = MusSymbolType.Clef;
            this.line = line;
            this.measure_num = num;
            this.sign = new ClefType(c);

            clef_alter = sign.Sign == ClefType.Clef.GClef ? 0 : sign.Sign == ClefType.Clef.FClef? -12: -6;
        }

        public void Draw(DrawingVisual visual)
        {
            DrawingVisual clef = new DrawingVisual();
            using( DrawingContext dc = clef.RenderOpen())
            {
                Brush clefColor = (SolidColorBrush)new BrushConverter().ConvertFromString(AdditionalAttributes.Color);
                Misc.DrawingHelpers.DrawString(dc, this.Sign.Symbol, TypeFaces.NotesFont, clefColor, Relative_x + Spacer_L, Relative_y, MusicScore.Defaults.Scale.Tenths); //! Experimental
            }
            visual.Children.Add(clef);
        }
    }
}
