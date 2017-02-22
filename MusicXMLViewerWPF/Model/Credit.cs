using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace MusicXMLViewerWPF
{
    [Serializable]
    public class Credit : Segment
    {
        #region Attributes
        private int page; // number of page where credit is presented
        #endregion

        #region Elements
        private CreditWords creditWords;
        private string creditType;
        private MusicXMLScore.Model.Helpers.FormattedTextMusicXML cw;
        private CreditType type;
        private Defaults.Defaults defaults;
        /// <summary>
        /// Segment sace for title, dubtitle, arranger and composer
        /// </summary>
        public static Segment titleSegment = new Segment();
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
        public CreditWords CreditWords { get { return creditWords; } }
        public int PageNumber { get { return page; } }

        [XmlElement("credit-type")]
        public string CreditTyp { get { return creditType; } set { creditType = value; } }

        [XmlElement("credit-words")]
        public MusicXMLScore.Model.Helpers.FormattedTextMusicXML CreditW { get { return cw; } set { cw = value; } }
        public CreditType Type { get { return type; } }
        public static Segment Titlesegment { get { return titleSegment; } set { titleSegment = value; } } //todo remove/refactor
        #endregion

        public Credit()
        {

        }

        public Credit(System.Xml.Linq.XElement x) //! , Defaults.Defaults d = null )
        {
            //defaults = d == null? new Defaults.Defaults(): d;
            page = x.HasAttributes ? int.Parse(x.Attribute("page").Value) : 1;
            creditType = x.Element("credit-type") != null ? x.Element("credit-type").Value : null;
            SetCreditType();
            creditWords = new CreditWords(x.Element("credit-words"));
            if (creditType == null)
            {
                if (creditWords.HAlign == Halign.center)
                {
                    if (creditWords.VAlign == Valign.top)
                    {
                        creditType = "title";
                        SetCreditType();
                    }
                    if (creditWords.VAlign == Valign.bottom)
                    {
                        creditType = "copyrights";
                        SetCreditType();
                    }
                }
            }
            UpdateSegmentHeight(); //todo remove/refactor
        }

        private void SetCreditType() //! converts string credit type to enum
        {
            if (creditType != null)
            {
                switch (creditType)
                {
                    case "title":
                        this.type = CreditType.title;
                        break;
                    case "subtitle":
                        type = CreditType.subtitle;
                        break;
                    case "composer":
                        type = CreditType.composer;
                        break;
                    case "page number":
                        type = CreditType.page_number;
                        break;
                    case "arranger":
                        type = CreditType.arranger;
                        break;
                    case "lyricist":
                        type = CreditType.lyricist;
                        break;
                    case "rights":
                        type = CreditType.rights;
                        break;
                }
            }
            else
            {
                type = CreditType.none;
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
                Point pos = new Point(CreditWords.DefX, defaults.Page.ContentSpace.Bottom - CreditWords.DefY); //! **Refactored** Point pos = new Point(CreditWords.DefX, MusicScore.Defaults.Page.ContentSpace.Bottom - CreditWords.DefY);
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
            //Todo_L refactoring
        }
    }
    
    public enum CreditType
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
    
    public enum CreditPlacement
    {
        header,
        footer
    }

    public class CreditWords : EmptyPrintStyle //! refactoring/removing possible later
    {
        private string value;

        public string Value { get { return value; } }

        public CreditWords(System.Xml.Linq.XElement x ) : base (x.Attributes())
        {
            this.value = x.Value;
        }
    }
}
