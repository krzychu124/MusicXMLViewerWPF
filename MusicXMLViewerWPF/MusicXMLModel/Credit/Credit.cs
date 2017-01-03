using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLViewerWPF.Credit
{
    class Credit : Segment
    {
        #region Attributes
        private int page; // number of page where credit is presented
        #endregion

        #region Elements
        private CreditWords credit_words;
        private string credit_type;
        private CreditType type;
        /// <summary>
        /// Segment sace for title, dubtitle, arranger and composer
        /// </summary>
        public static Segment titlesegment = new Segment();
        #endregion

        #region Attachable Properties
        public static readonly DependencyProperty CreditTypeProperty = DependencyProperty.RegisterAttached("CreditType",
            typeof(CreditType), typeof(Credit), new FrameworkPropertyMetadata(null));

        public static string GetCreditTypeProperty(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (string)element.GetValue(CreditTypeProperty);
        }
        public static void SetCreditTypeProperty(UIElement element, string value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(CreditTypeProperty, value);
        }


        public static readonly DependencyProperty CreditPlacementProperty = DependencyProperty.RegisterAttached("CreditPlacement",
            typeof(string), typeof(Credit), new FrameworkPropertyMetadata(null));

        public static string GetCreditPlacementProperty(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (string)element.GetValue(CreditPlacementProperty);
        }
        public static void SetCreditPlacementProperty(UIElement element, string value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(CreditPlacementProperty, value);
        }
        #endregion

        #region Public properties read-only
        public CreditWords CreditWords { get { return credit_words; } }
        public int PageNumber { get { return page; } }
        public string CreditType { get { return credit_type; } }
        public CreditType Type { get { return type; } }
        public static Segment Titlesegment { get { return titlesegment; } set { titlesegment = value; } }
        #endregion

        public Credit(System.Xml.Linq.XElement x)
        {
            page = x.HasAttributes ? int.Parse(x.Attribute("page").Value) : 1;
            credit_type = x.Element("credit-type") != null ? x.Element("credit-type").Value : null;
            SetCreditType();
            credit_words = new CreditWords(x.Element("credit-words"));
            if (credit_type == null)
            {
                if (credit_words.HAlign == Halign.center)
                {
                    if (credit_words.VAlign == Valign.top)
                    {
                        credit_type = "title";
                        SetCreditType();
                    }
                    if (credit_words.VAlign == Valign.bottom)
                    {
                        credit_type = "copyrights";
                        SetCreditType();
                    }
                }
            }
            UpdateSegmentHeight();
        }
        /// <summary>
        /// Set basic properties of Credit segment
        /// </summary>
        public static void SetCreditSegment()
        {
            Titlesegment.Relative = new Point(MusicScore.Defaults.Page.ContentSpace.X, MusicScore.Defaults.Page.ContentSpace.Y);
            Titlesegment.Width = (float)MusicScore.Defaults.Page.ContentSpace.Width;
            Titlesegment.Segment_type = SegmentType.Title;
        }
        private void SetCreditType()
        {
            if (credit_type != null)
            {
                switch (credit_type)
                {
                    case "title":
                        type = MusicXMLViewerWPF.Credit.CreditType.title;
                        break;
                    case "subtitle":
                        type = MusicXMLViewerWPF.Credit.CreditType.subtitle;
                        break;
                    case "composer":
                        type = MusicXMLViewerWPF.Credit.CreditType.composer;
                        break;
                    case "page number":
                        type = MusicXMLViewerWPF.Credit.CreditType.page_number;
                        break;
                    case "arranger":
                        type = MusicXMLViewerWPF.Credit.CreditType.arranger;
                        break;
                    case "lyricist":
                        type = MusicXMLViewerWPF.Credit.CreditType.lyricist;
                        break;
                    case "rights":
                        type = MusicXMLViewerWPF.Credit.CreditType.rights;
                        break;
                }
            }
            else
            {
                type = MusicXMLViewerWPF.Credit.CreditType.none;
            }
        }

        public void UpdateSegmentHeight()
        {
            FormattedText ft = GetFormattedText();
            Height = (float)ft.Height;
        }

        public void Draw(DrawingVisual visual)
        {
            using (DrawingContext dc = visual.RenderOpen())
            {
                Point pos = new Point(CreditWords.DefX, MusicScore.Defaults.Page.ContentSpace.Bottom - CreditWords.DefY);
                FormattedText ft = GetFormattedText(pos);
                //Misc.DrawingHelpers.DrawText(dc, text, pos, size, align, valign, weight);
                dc.DrawText(ft, pos);
            }
        }

        private FormattedText GetFormattedText(Point pos = new Point())
        {
            string text = CreditWords.Value;
            Halign align = CreditWords.HAlign;
            float size = CreditWords.FontSize;
            string weight = CreditWords.FontWeight;
            Valign valign = CreditWords.VAlign;
            FormattedText ft = new FormattedText(text, System.Threading.Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, TypeFaces.TextFont, size * 1.4, Brushes.Black);
            Misc.DrawingHelpers.SetFontWeight(ft, weight);
            Misc.DrawingHelpers.VerticalAlign(pos, ft, valign);
            Misc.DrawingHelpers.HorizontalAlign(ft, align);
            return ft;
        }

        public void Draw(DrawingVisual visual, Rect rect)
        {
            //Todo refactoring
        }
    }


    enum CreditType
    {
        page_number,
        title,
        subtitle,
        composer = 6,
        lyricist,
        separator = 8,
        arranger,
        intrumentname,
        rights = 20,
        other = 100,
        none = 1000
    }
    
    enum CreditPlacement
    {
        header,
        footer
    }

    internal class CreditWords : EmptyPrintStyle
    {
        private string value;

        public string Value { get { return value; } }

        public CreditWords(System.Xml.Linq.XElement x ) : base (x.Attributes())
        {
            value = x.Value;
        }
    }
}
