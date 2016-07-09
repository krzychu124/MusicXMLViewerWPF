using System;
using System.Collections.Generic;
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
    public class Page  //TODO_H provide default margins if not presented in document  // Need to be reworked // Currently not used(maybe partial but not as good as it should be)// Should be the base of Page layout :/ // missing logic for pages, parts, scores, voices layouts
    {

        //public string part_id;
        private static float page_height;
        private static float page_width;
        private static PageMargins page_margins;

        public static int num_lines;
        public static List<Margins> line = new List<Margins>();
        public float Width {  get { return page_width; } }
        public float Height {  get { return page_height; } }
        public PageMargins Margins {  get { return page_margins; } }

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

        public Page(XElement x)
        {
            GetPageInfo(x);
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
}
