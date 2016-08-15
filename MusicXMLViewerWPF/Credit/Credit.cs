using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLViewerWPF.Credit
{
    class Credit 
    {
        #region Attributes
        private int page; // number of page where credit is presented
        #endregion

        #region Elements
        private CreditWords credit_words;
        private string credit_type;
        private CreditType type;
        public static Segment segment = new Segment();
        #endregion

        #region Public properties read-only
        public CreditWords CreditWords { get { return credit_words; } }
        public int Page { get { return page; } }
        public string CreditType { get { return credit_type; } }
        public CreditType Type { get { return type; } }
        #endregion

        public Credit(System.Xml.Linq.XElement x)
        {
            page = int.Parse(x.Attribute("page").Value);
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
        }
        /// <summary>
        /// Set basic properties of Credit segment
        /// </summary>
        public static void SetCreditSegment()
        {
            segment.Relative = new Point(MusicScore.Defaults.Page.ContentSpace.X, MusicScore.Defaults.Page.ContentSpace.Y);
            segment.Width = (float)MusicScore.Defaults.Page.ContentSpace.Width;
            segment.Segment_type = SegmentType.Title;
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
        public void Draw(DrawingVisual visual)
        {
            
            using(DrawingContext dc = visual.RenderOpen())
            {
                string text = CreditWords.Value;
                Point pos = new Point(CreditWords.DefX, CreditWords.DefY);
                Halign align = CreditWords.HAlign;
                float size = CreditWords.FontSize;
                string weight = CreditWords.FontWeight;
                Valign valign = CreditWords.VAlign;
                //string style = CreditWords.
                Misc.DrawingHelpers.DrawText(dc, text, pos, size, align, valign, weight);
            }
            
        }
    }

    enum CreditType
    {
        none,
        page_number,
        title,
        subtitle,
        composer,
        arranger,
        lyricist,
        rights
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
