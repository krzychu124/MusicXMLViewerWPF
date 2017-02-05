using GalaSoft.MvvmLight;
using MusicXMLViewerWPF.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace MusicXMLViewerWPF
{
    public class Segment : ObservableObject//, IDrawableMusicalObject
    {
        #region protected fields
        private float width;
        private float height;
        
        private float relative_x;
        private float relative_y;
        
        private float calculated_x;
        private float calculated_y;
        
        private float offset_x;
        private float offset_y;

        private float space_l; //! left spacer
        private float space_r; //! right spacer 

        private Brush color = Brushes.Black;

        private SegmentType segment_type;

        private Dictionary<string, float> spacer_dict = new Dictionary<string, float>();
        private string missingProperies;
        private string id;
        #endregion
        
        #region public properties
        public Brush Color { get { return color; } set { color = value; } }
        public float Calculated_x { get { return calculated_x; } set { calculated_x = value; } }
        public float Calculated_y { get { return calculated_y; } set { calculated_y = value; } }
        public float Height { get { return height; } set { height = value; } }  
        public float Offset_x { get { return offset_x; } set { offset_x = value; } }
        public float Offset_y { get { return offset_y; } set { offset_y = value; } }
        public float Relative_x { get { return relative_x; } set { relative_x = value; } }
        public float Relative_y { get { return relative_y; } set { relative_y = value; } }
        public float Spacer_L { get { return space_l; } set { space_l = value; } }
        public float Spacer_R { get { return space_r; } set { space_r = value; } }
        public float Width { get { return width; } set { width = value; if (checkIfSet()) { RaisePropertyChanged(nameof(Width)); } } }
        public string ID {
            get {
                return id; }
            set {
                if (id == null)
                {
                    id = value;
                } else
                {
                    Logger.Log($"ID changed from {value} to {id}"); MessageBox.Show($"ID changed from {value} to {id}"); }
            }
        }

        public Point Calculated { get { return new Point(calculated_x, calculated_y); } set { calculated_x = (float)value.X; calculated_y = (float)value.Y; RaisePropertyChanged(nameof(Calculated));  } }
        public Point Dimensions { get { return new Point(width, height); } set { width = (float)value.X; height = (float)value.Y; } }
        public Point Offset { get { return new Point(offset_x, offset_y); } set { offset_x = (float)value.X; offset_y = (float)value.Y; } }
        public Point Relative { get { return new Point(relative_x, relative_y); } set { relative_x = (float)value.X; relative_y = (float)value.Y; RaisePropertyChanged(nameof(Relative)); } }
        public string Relative_str { get { return "("+ relative_x.ToString("0.##") + "; "+ relative_y.ToString("0.##") + ")"; } }
        public Rect Rectangle { get { return new Rect(Relative, Dimensions); } }
        public SegmentType Segment_type { get { return segment_type; } set { segment_type = value; SetSpacers(); if (Width == 0) CalculateDimensions(); } }
        public Dictionary<string,float> SpacerDict { get { return spacer_dict; } set { if (value != null) spacer_dict = value; } }
        public SegmentType CharacterType { get { return Segment_type; } }
        #endregion

        public void segment_Properties_Ready(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Calculated":
                    Logger.Log($"Segment {sender.ToString()}, {e.PropertyName}");
                    Logger.Log($"Set to {Calculated.X.ToString("0.#")}X, {Calculated.Y.ToString("0.#")}Y, {Relative.X.ToString("0.#")}X;  {Relative.Y.ToString("0.#")}Y, Width: {Width}");
                    if (Relative_y == 0)
                    {
                        SetRelativePos(Calculated_x, Calculated_y);
                    }
                    break;
                case "Relative":
                    Logger.Log($"Relative set {Relative_str} {this.ToString(StringType.Type)}");
                    break;
                default:
                    Logger.Log($"Segment {e.PropertyName} ready");
                    Logger.Log($"Set to {Width}, {Height}, {Calculated}, {Relative}");
                    break;
            }
        }
        public Segment()
        {
            PropertyChanged += segment_Properties_Ready;
        }
        
        /// <summary>
        /// Set relative position of segment (eg. extracted from XML file)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetRelativePos(float x, float y)
        {
            Relative_x = x;
            Relative_y = y + 10f;
        }
        /// <summary>
        /// Set calculated position of segment
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetCalculatedPos(float x, float y)
        {
            Calculated_x = x;
            Calculated_y = y;
        }
        /// <summary>
        /// Set user offset of segment
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetOffset(float x, float y)
        {
            Offset_x = x;
            Offset_y = y;
        }
        /// <summary>
        /// Set dimensions of segment
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetDimensions( float width, float height)
        {
            this.width = width;
            Height = height;
        }
        /// <summary>
        /// Calculate dimensions from spacers
        /// </summary>
        public void CalculateDimensions()
        {
            if (Spacer_L != 0 || Spacer_R != 0)
            {
                width = Spacer_L + Spacer_R;
                Height = 60f;
                spacer_dictionary();
                //! DEBUG Logger.Log("SpacersDict generated");
            }
        }
        /// <summary>
        /// Sets segment spacers according to type of segment
        /// </summary>
        public void SetSpacers() 
        {
            float left = 0;
            float right = 0;
            if (Segment_type == SegmentType.Barline)
            {
                left = 0f;
                right = 1.5f;
            }
            if (Segment_type == SegmentType.Chord)
            {
                left = 7f;
                right = 15f;
            }
            if (Segment_type == SegmentType.Clef)
            {
                left = 5f;
                right = 25f;
            }
            if (Segment_type == SegmentType.KeySig)
            {
                left = 5f;
                right = 5f;
            }
            if (Segment_type == SegmentType.TimeSig)
            {
                left = 5f;
                right = 15f;
            }
            if (Segment_type == SegmentType.Rest)
            {
                left = 3f;
                right = 12f;
            }
            if (Segment_type == SegmentType.Direction)
            {
                left = 0f;
                right = 3f;
            }
            Spacer_L = left;
            Spacer_R = right;
        }
        /// <summary>
        /// Set custom spacers of segment (left, right) 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        public void SetSpacers(float l, float r)
        { 
            space_l = l;
            space_r = r;
        }
        /// <summary>
        /// Segment values to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string value =$"<{ID}>||{Segment_type.ToString()}| |XY_r|<{Relative_x.ToString("0.#")}><{Relative_y.ToString("0.#")}> |LR| <{Spacer_L.ToString("0.#")}><{Spacer_R.ToString("0.#")}> |WH| <{Width.ToString("0.#")}><{Height.ToString("0.#")}>";
            return value;
        }
        public string ToString(StringType type = StringType.ToString)
        {
            string result = "";
            switch (type)
            {
                case StringType.Type:
                    result = $"|{Segment_type.ToString()}|";
                    break;
                case StringType.ToString:
                    result = this.ToString();
                    break;
                case StringType.Spacer:
                    result = Spacer_L.ToString() + Spacer_R.ToString();
                    break;
                case StringType.Relative:
                    result = Relative.ToString();
                    break;
                case StringType.Calculated:
                    result = Calculated.ToString();
                    break;
                default:
                    break;
            }
            return result;
        }
        public void Draw(DrawingVisual visual)
        {
            Draw(visual, color:Brushes.DarkBlue);
        }
        /// <summary>
        /// Draws segment rectangle on visual
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="color"></param>
        /// <param name="dashtype"></param>
        public void Draw(DrawingVisual visual , Brush color = null, DashStyle dashtype = null)
        {
            if (color == null)
            {
                color = Brushes.DarkBlue;
            }
            if (dashtype == null)
            {
                dashtype = DashStyles.Solid;
            }
            Point one = new Point(Relative_x,Relative_y);
            Point two = new Point(Relative_x + Width, Relative_y + Height);
            DrawingHelpers.DrawRectangle(visual, one, two, color, dashtype);
        }
        /// <summary>
        /// Generate Spacer Dictionary (spacers proportions)
        /// </summary>
        private void spacer_dictionary()
        {
            if (Width != 0)
            {
                SpacerDict.Clear();
                SpacerDict.Add("L", Spacer_L / Width);
                SpacerDict.Add("R", Spacer_R / Width);
            }
        }
        /// <summary>
        /// Recalculate Spacers according to changed Width
        /// </summary>
        public void recalculate_spacers()
        {
            Spacer_L = SpacerDict["L"] * Width;
            Spacer_R = SpacerDict["R"] * Width;
            spacer_dictionary();
        }
        private bool checkIfSet()
        {
            bool result = false;
            if (Calculated.Y != 0 && Relative.Y != 0 && Width != 0 && Height != 0)
            {
                missingProperies = string.Empty;
                result = true;
            }
            if (result == false)
            {
                if (Calculated.Y == 0) missingProperies += "Calculated ";
                if (Relative.Y == 0) missingProperies += "Relative ";
                if (Width == 0) missingProperies += "Width ";
            }
            return result;
        }
        public enum StringType
        {
            Spacer,
            Relative,
            Calculated,
            Type,
            ToString
        }
    }
    
}
