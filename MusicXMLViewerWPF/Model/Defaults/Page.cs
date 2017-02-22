using System;
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
        private double pageHeight;
        private double pageWidth;
        private PageMargins pageMargins = new PageMargins();
        private Rect content_space;
        private Rect content_space_for_measures;

        public event PropertyChangedEventHandler PropertyChanged;
        public static int num_lines;
        public static List<Margins> line = new List<Margins>();
        public double Width
        {
            get { return pageWidth; }
            set
            {
                if (value != 0)
                {
                    pageWidth = value;
                }
            }
        }
        public double Height
        {
            get { return pageHeight; }
            set
            {
                if (value != 0)
                {
                    pageHeight = value;
                }
            }
        }
        public PageMargins Margins {  get { return pageMargins; } set { if (value != null) { pageMargins = value; } } }
        public Rect ContentSpace
        {
            get { return content_space; }
            set
            {
                content_space = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ContentSpace)));
            }
        }
        /// <summary>
        /// ContentSpace dimensions: x,y,w,h
        /// </summary>
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
        /// <summary>
        /// MeasureContentSpace dimensions: x,y,w,h
        /// </summary>
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

        public Page()
        {
            this.PropertyChanged += Page_PropertyChanged;
            pageWidth = 2100.0; //! A4 format - vertical
            pageHeight = 2970.0;
            pageMargins = new PageMargins(); //! Std. page margins
            //? No usage, to refactoring; CalculateContentSpace();
            //CalculateMeasureContetSpace();
        }

        public void Page_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MeasuresContentSpaceu":
                    Logger.Log($"CSM changed! Nv: {MeasuresContentSpace_str}");
                    break;
                case "ContentSpaceu":
                    Logger.Log($"CS changed! Nv: {ContentSpace_str}");
                    break;
                default:
                    Logger.Log($"Missing action for {e.PropertyName}");
                    break;
            }
        }

        public void CalculateMeasureContetSpace()
        {
            Rect temp = Credit.Titlesegment.Rectangle;
            MeasuresContentSpace = new Rect(new Point(temp.Left, temp.Bottom), content_space.BottomRight);
            //? content_space_measures = 
        }

        private void CalculateContentSpace()
        {
            Point right_bottom = new Point(pageWidth - Margins.Right, pageHeight - Margins.Bottom);
            ContentSpace = new Rect(new Point(Margins.Left, Margins.Top), right_bottom);
            //MusicScore m = new MusicScore() { ContentSpaceCalculated = true };
        }

        public Page(XElement x)
        {
            this.PropertyChanged += Page_PropertyChanged;
            InitPage(x);
            
            CalculateContentSpace(); //todo remove/refactor
        }

        public Page(float h, float w, PageMargins p)
        {
            pageMargins = p;
            pageHeight = h;
            pageWidth = w;
        }
        
        public Page(int measure_num, float l_margin, float y_marg)//todo refactor
        {
            Margins m = new Margins(measure_num,l_margin,y_marg);
            line.Add(m);
        }
        public Page(int num) //todo refactor
        {
            Margins m = new Margins(num);
            
            line.Add(m);
        }
        public void InitPage(XElement xele) // TODO_L more indepth test
        {
            var pageLayoutXElementList = from x in xele.Elements("page-layout") select x;

            foreach (var item in pageLayoutXElementList)//TODO_L tests, check carefully page margins
            {
                Width = double.Parse(item.Element("page-width").Value, CultureInfo.InvariantCulture);
                Height = double.Parse(item.Element("page-height").Value, CultureInfo.InvariantCulture);
                var pmargins = item.Elements("page-margins");
                string type = item.Attribute("type") != null ? item.Attribute("type").Value : "both";
                pageMargins = new PageMargins(type, float.Parse(item.Element("page-margins").Element("left-margin").Value, CultureInfo.InvariantCulture), float.Parse(item.Element("page-margins").Element("right-margin").Value, CultureInfo.InvariantCulture), float.Parse(item.Element("page-margins").Element("top-margin").Value, CultureInfo.InvariantCulture), float.Parse(item.Element("page-margins").Element("bottom-margin").Value, CultureInfo.InvariantCulture));
               // Page page = new Page(w,h,pm);
            }
            
        }
        /// <summary>
        /// Gets string values from Rect object (X,Y,Width,Heigth)
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private string GetString(Rect rect) //? refactor?delete
        {
            string result = $"{rect.X.ToString("0.##")}; {rect.Y.ToString("0.##")}; {rect.Width.ToString("0.##")}; {rect.Height.ToString("0.##")}";
            return result;
        }
    }
    public class Margins //? refactor
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
    public class PageMargins //! looks ok
    {
        private float bottomMargin;
        private float leftMargin;
        private float rightMargin;
        private float topMargin;
        private MarginType type;

        public float Bottom {  get { return bottomMargin; } }
        public float Left { get { return leftMargin;} }
        public float Right { get { return rightMargin; } }
        public float Top {  get { return topMargin; } }
        public MarginType Type { get { return type; } }

        public PageMargins() // default margins
        {
            SetDefaultMargins();
        }

        public PageMargins(string type, float l, float r, float t,float b)
        {
            bottomMargin = b;
            leftMargin = l;
            rightMargin = r;
            this.type = type == "both" ? MarginType.both : type == "odd" ? MarginType.odd : MarginType.even;
            topMargin = t;
        }

        private void SetDefaultMargins()
        {
            bottomMargin = 25f;
            leftMargin = 20f;
            rightMargin = 20f;
            topMargin = 25f;
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
