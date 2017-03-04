using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Defaults
{
    [Serializable]
    public class PageLayoutMusicXML
    {
        private double pageHeight;
        private double pageWidth;
        private List<PageMarginsMusicXML> pageMargins;

        [XmlElement("page-height")]
        public double PageHeight
        {
            get
            {
                return pageHeight;
            }

            set
            {
                pageHeight = value;
            }
        }
        [XmlElement("page-width")]
        public double PageWidth
        {
            get
            {
                return pageWidth;
            }

            set
            {
                pageWidth = value;
            }
        }
        [XmlElement("page-margins")]
        public List<PageMarginsMusicXML> PageMargins
        {
            get
            {
                return pageMargins;
            }

            set
            {
                pageMargins = value;
            }
        }
    }
    [Serializable]
    public class PageMarginsMusicXML
    {
        private double leftMargin;
        private double rightMargin;
        private double topMargin;
        private double bottomMargin;
        private MarginTypeMusicXML marginType;
        private bool marginTypeSpecified;

        [XmlElement("left-margin")]
        public double LeftMargin
        {
            get
            {
                return leftMargin;
            }

            set
            {
                leftMargin = value;
            }
        }
        [XmlElement("right-margin")]
        public double RightMargin
        {
            get
            {
                return rightMargin;
            }

            set
            {
                rightMargin = value;
            }
        }
        [XmlElement("top-margin")]
        public double TopMargin
        {
            get
            {
                return topMargin;
            }

            set
            {
                topMargin = value;
            }
        }
        [XmlElement("bottom-margin")]
        public double BottomMargin
        {
            get
            {
                return bottomMargin;
            }

            set
            {
                bottomMargin = value;
            }
        }
        [XmlAttribute("type")]
        public MarginTypeMusicXML MarginType
        {
            get
            {
                return marginType;
            }

            set
            {
                marginType = value;
            }
        }
        [XmlIgnore]
        public bool MarginTypeSpecified
        {
            get
            {
                return marginTypeSpecified;
            }

            set
            {
                marginTypeSpecified = value;
            }
        }
    }
    public enum MarginTypeMusicXML
    {
        odd,
        even,
        both,
    }
}
