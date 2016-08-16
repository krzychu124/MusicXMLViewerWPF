﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    public class Page : INotifyPropertyChanged
    {

        //public string part_id;
        private float page_height;
        private float page_width;
        private PageMargins page_margins;
        private Rect content_space;
        private Rect content_space_for_measures;

        public event PropertyChangedEventHandler PropertyChanged;
        public static int num_lines;
        public static List<Margins> line = new List<Margins>();
        public float Width {  get { return page_width; } }
        public float Height {  get { return page_height; } }
        public PageMargins Margins {  get { return page_margins; } }
        public Rect ContentSpace
        {
            get { return content_space; }
            set
            {
                content_space = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ContentSpace)));
            }
        }
        public string ContentSpace_str { get { return GetString(ContentSpace); } }
        public Rect MeasuresContentSpace
        {
            get { return content_space_for_measures; }
            set
            {
                content_space_for_measures = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MeasuresContentSpace)));
            }
        }
        public string MeasuresContentSpace_str { get { return GetString(MeasuresContentSpace); } }
        public static int Num_lines
        {
            get
            {
                return num_lines;
            }

            set
            {
                num_lines = value;
            }
        }
        //public Page()
        //{
        //    GetPageInfo();
        //}
        public Page()
        {
            this.PropertyChanged += Page_PropertyChanged;
            page_width = 2100f;
            page_height = 2970f;
            page_margins = new PageMargins();
            CalculateContentSpace();
            //CalculateMeasureContetSpace();
        }

        public void Page_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MeasuresContentSpace":
                    Logger.Log($"CSM changed! Nv: {MeasuresContentSpace_str}");
                    break;
                case "ContentSpace":
                    Logger.Log($"CS changed! Nv: {ContentSpace_str}");
                    break;
                default:
                    Logger.Log($"Missing action for {e.PropertyName}");
                    break;
            }
        }

        public void CalculateMeasureContetSpace()
        {
            Rect temp = Credit.Credit.Titlesegment.Rectangle;
            MeasuresContentSpace = new Rect(new Point(temp.Left, temp.Bottom), content_space.BottomRight);
            //MusicScore m = new MusicScore() { ContentSpaceCalculated = true };
            //? content_space_measures = 
        }

        private void CalculateContentSpace()
        {
            Point right_bottom = new Point(page_width - Margins.Right, page_height - Margins.Bottom);
            ContentSpace = new Rect(new Point(Margins.Left, Margins.Top), right_bottom);
            MusicScore m = new MusicScore() { ContentSpaceCalculated = true };
        }

        public Page(XElement x)
        {
            this.PropertyChanged += Page_PropertyChanged;
            GetPageInfo(x);
            
            CalculateContentSpace();
        }

        public Page(float h, float w, PageMargins p)
        {
            page_margins = p;
            page_height = h;
            page_width = w;
        }
        
        public Page(int measure_num, float l_margin, float y_marg)
        {
            Margins m = new Margins(measure_num,l_margin,y_marg);
            line.Add(m);
        }
        public Page(int num)
        {
            Margins m = new Margins(num);
            
            line.Add(m);
        }
        public void GetPageInfo(XElement xele) // TODO_L more indepth test
        {
            //XDocument doc = LoadDocToClasses.Document;
            //var p = from z in doc.Descendants("defaults") select z;
            var pg = from x in xele.Elements("page-layout") select x;

            foreach (var item in pg)
            {
                page_width = float.Parse(item.Element("page-width").Value, CultureInfo.InvariantCulture);
                page_height = float.Parse(item.Element("page-height").Value, CultureInfo.InvariantCulture);
                var pmargins = item.Elements("page-margins");
                string type = item.Attribute("type") != null ? item.Attribute("type").Value : "both";
                page_margins = new PageMargins(type, float.Parse(item.Element("page-margins").Element("left-margin").Value, CultureInfo.InvariantCulture), float.Parse(item.Element("page-margins").Element("right-margin").Value, CultureInfo.InvariantCulture), float.Parse(item.Element("page-margins").Element("top-margin").Value, CultureInfo.InvariantCulture), float.Parse(item.Element("page-margins").Element("bottom-margin").Value, CultureInfo.InvariantCulture));
               // Page page = new Page(w,h,pm);
            }
            
        }
        /* //experiments
        public  void Refresh()
        {
            this.InvalidateMeasure();
            
                this.InvalidateArrange();
                this.InvalidateVisual();
            
               
        }

        public static void DrawString(DrawingContext d, string text, Typeface f, Brush b, float xPos, float yPos, float emSize)
        {
            //This function mimics Graphics.DrawString functionality
            d.DrawText(new FormattedText(text, Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, f, emSize, b), new Point(xPos, yPos));

        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            tblock tb = new tblock();
            base.OnRender(drawingContext);
            Console.WriteLine("rendered");
         //   tb.writeLineToTextBlock("rendered");
            DrawString(drawingContext, num_lines.ToString(), TypeFaces.NotesFont, Brushes.Black, 120f, 10.0f, 40.0f);
        }
        */
        /// <summary>
        /// Gets string values from Rect object (X,Y,Width,Heigth)
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private string GetString(Rect rect)
        {
            string result = $"{rect.X.ToString("0.##")}; {rect.Y.ToString("0.##")}; {rect.Width.ToString("0.##")}; {rect.Height.ToString("0.##")}";
            return result;
        }
    }
    public class Margins
    {
        public int measure_num;
        public float left_m;
        public float up_m;
        public static float relX;
        public static float posY_m;
        public static float relY;
        public Margins(int m_id, float l, float p)
        {
            measure_num = m_id;
            relX = l;
            left_m = relX;
            posY_m = p;
            relY = relY+p;
            up_m = relY;
        }
        public Margins(int m_id)
        {
            left_m = relX;
            measure_num = m_id;
            relY = relY + posY_m;
            up_m = relY;
        }
    }
    public class PageMargins // looks ok
    {
        private float bottom_margin;
        private float left_margin;
        private float right_margin;
        private float top_margin;
        private MarginType type;

        public float Bottom {  get { return bottom_margin; } }
        public float Left { get { return left_margin;} }
        public float Right { get { return right_margin; } }
        public float Top {  get { return top_margin; } }
        public MarginType Type { get { return type; } }

        public PageMargins() // default margins
        {
            SetDefaultMargins();
        }

        public PageMargins(string type, float l, float r, float t,float b)
        {
            bottom_margin = b;
            left_margin = l;
            right_margin = r;
            this.type = type == "both" ? MarginType.both : type == "odd" ? MarginType.odd : MarginType.even;
            top_margin = t;
        }

        private void SetDefaultMargins()
        {
            bottom_margin = 25f;
            left_margin = 20f;
            right_margin = 20f;
            top_margin = 25f;
            type = MarginType.both;
        }

        public enum MarginType
        {
            both,
            odd,
            even
        }
    }
    public class RectExtensions
    {

    }
}
