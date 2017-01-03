using MusicXMLViewerWPF.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Rest : Note, IAutoPosition
    {
        // private int dots; inherited
        // private int multimeasure;
        //private bool hasDot; inherited
        //private float posX; inherited
        //private int id; inherited
        private bool ismeasurerest;
        private int measure_num;
        private string duration_symbol;
        public static List<Rest> RestList = new List<Rest>();

        //public bool HasDot { get { return hasDot; } } inherited
        //public int Duration { get { return duration; } } inherited
        //public int MeasureId { get { return measure_num; } } inherited
        //public int Voice { get { return voice; } } inherited
        //public string Symbol { get { return duration_symbol; } } inherited
        public bool IsMeasureRest { get { return ismeasurerest; } }
        public float X { get { return posX; } }
        public int CharId { get { return id; } }
        public event PropertyChangedEventHandler RestPropertyChanged;

        public Rest(XElement x)
        {
            SetSegmentColor(Brushes.DarkMagenta);
            NotePropertyChanged += Rest_RestPropertyChanged;
            ID = Misc.RandomGenerator.GetRandomHexNumber();
            Segment_type = SegmentType.Rest;
            duration = int.Parse(x.Element("duration").Value);
            voice = int.Parse(x.Element("voice").Value);
            SymbolXMLValue = x.Element("type") != null ? x.Element("type").Value : null;
            SymbolType = SymbolXMLValue != null ? SymbolDuration.d_type(SymbolXMLValue) : MusSymbolDuration.Unknown;
            //Symbol = MusChar.getRestSymbol(SymbolXMLValue);
            Width = 10f;
            isRest = true;
            if (x.Element("rest").HasAttributes) //! Checks if rest lasts whole measure duration
            {
                ismeasurerest = x.Element("rest").Attribute("measure").Value == "yes" ? true : false;
                SymbolXMLValue = "whole";
                SymbolType = MusSymbolDuration.Whole;
            }
            else
            {
                ismeasurerest = false;
            }
            
        }

        private void Rest_RestPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SymbolType":
                    Symbol = MusicalChars.getRestSymbol(SymbolXMLValue);
                    Logger.Log($"Rest symbol is {Symbol}");
                    break;
                default:
                    break;
            }
        }

        public Rest()
        {
            NotePropertyChanged += Note_PropertyChanged;
            NotePropertyChanged += Rest_RestPropertyChanged;
            RestPropertyChanged += Rest_RestPropertyChanged;
            RestPropertyChanged += Note_PropertyChanged;
            Segment_type = SegmentType.Rest;
            Width = 10f;
        }
        public Rest(int m_num,int id,int d, int v, string t, float x, bool restType, bool dot = false)
        {
            this.id = id;
            base.Segment_type = SegmentType.Rest;
            measure_num = m_num;
            this.posX = x;
            duration = d;
            voice = v;
            ismeasurerest = restType;
            duration_symbol = MusicalChars.getRestSymbol(t);
            hasDot = dot;
        }
        public Rest(int m_num,int id,int d, int v, bool restType)
        {
            this.id = id;
            base.Segment_type = SegmentType.Rest;
            measure_num = m_num;
            duration = d;
            voice = v;
            ismeasurerest = restType;
        }

        public Rest(string duration, Point pos)
        {
            NotePropertyChanged += Rest_RestPropertyChanged;
            Relative = pos;
            SymbolXMLValue = duration;
            SymbolType = SymbolDuration.d_type(duration);
        }

        public void MeasureRestDuration(int duration)
        {


        }
        public static void ExtractRests()
        {
              RestList = LoadDocToClasses.list.OfType<Rest>().ToList();
        }

        public override void Draw(DrawingVisual visual)
        {
            DrawingVisual rest = new DrawingVisual();
            using (DrawingContext dc = rest.RenderOpen())
            {
                float YPos = Relative_y;
                if (SymbolType == MusSymbolDuration.Whole) //? TEMPORARY SOLUTION
                {
                    YPos -= 7;
                }
                if (SymbolType == MusSymbolDuration.Half) //? TEMPORARY SOLUTION
                {
                    YPos += 1;
                }
                Brush restColor = this.Color;//! (SolidColorBrush)new BrushConverter().ConvertFromString(AdditionalAttributes.Color);
                Misc.DrawingHelpers.DrawString(dc, this.Symbol, TypeFaces.NotesFont, restColor, Relative_x + Spacer_L, YPos, MusicScore.Defaults.Scale.Tenths); //! Experimental
            }
            visual.Children.Add(rest);
        }
    }
}
