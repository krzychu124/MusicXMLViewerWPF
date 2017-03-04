using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Helpers
{
    [Serializable]
    public class FormattedTextMusicXML //! Looks ok
    {
        private string lang;
        private string space;
        private string value;

        private string defaultX;
        private string defaultY;
        private string justify;
        private string halign;
        private string valign;

        private string fontFamily;
        private string fontSize;
        private string fontWeight;
        private string fontStyle;
        private string underline;

        private string color;

        [XmlAttribute("lang", Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Lang
        {
            get
            {
                return lang;
            }

            set
            {
                lang = value;
            }
        }
        [XmlAttribute("space", Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Space
        {
            get
            {
                return space;
            }

            set
            {
                space = value;
            }
        }
        [XmlText()]
        public string Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }
        [XmlAttribute("default-x")]
        public string DefaultX
        {
            get
            {
                return defaultX.ToString();
            }

            set
            {
                defaultX = value;
            }
        }
        [XmlAttribute("default-y")]
        public string DefaultY
        {
            get
            {
                return defaultY.ToString();
            }

            set
            {
                defaultY = value;
            }
        }
        [XmlAttribute("justify")]
        public string Justify
        {
            get
            {
                return justify;
            }

            set
            {
                justify = value;
            }
        }
        [XmlAttribute("halign")]
        public string Halign
        {
            get
            {
                return halign;
            }

            set
            {
                halign = value;
            }
        }
        [XmlAttribute("valign")]
        public string Valign
        {
            get
            {
                return valign;
            }

            set
            {
                valign = value;
            }
        }
        [XmlAttribute("font-family")]
        public string FontFamily
        {
            get
            {
                return fontFamily;
            }

            set
            {
                fontFamily = value;
            }
        }
        [XmlAttribute("font-size")]
        public string FontSize
        {
            get
            {
                return fontSize;
            }

            set
            {
                fontSize = value;
            }
        }
        [XmlAttribute("font-weight")]
        public string FontWeight
        {
            get
            {
                return fontWeight;
            }

            set
            {
                fontWeight = value;
            }
        }
        [XmlAttribute("font-style")]
        public string FontStyle
        {
            get
            {
                return fontStyle;
            }

            set
            {
                fontStyle = value;
            }
        }
        [XmlAttribute("underline")]
        public string Underline
        {
            get
            {
                return underline;
            }

            set
            {
                underline = value;
            }
        }
        [XmlAttribute("color")]
        public string Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
            }
        }

        public FormattedTextMusicXML()
        {

        }
    }
}
