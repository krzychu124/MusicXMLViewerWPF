using System;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Helpers
{
    [Serializable]
    public class EmptyFontMusicXML
    {
        private string fontFamily;
        private FontStyleMusicXML fontStyle;
        private bool fontStyleSpecified;
        private string fontSize;
        private FontWeightMusicXML fontWeight;
        private bool fontWeightSpecified;

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

        [XmlAttribute("font-style")]
        public FontStyleMusicXML FontStyle
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

        [XmlIgnore]
        public bool FontStyleSpecified
        {
            get
            {
                return fontStyleSpecified;
            }

            set
            {
                fontStyleSpecified = value;
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
        public FontWeightMusicXML FontWeight
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

        [XmlIgnore]
        public bool FontWeightSpecified
        {
            get
            {
                return fontWeightSpecified;
            }

            set
            {
                fontWeightSpecified = value;
            }
        }

        public EmptyFontMusicXML()
        {

        }
    }
}
