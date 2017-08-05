using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.NoteItems
{
    [Serializable]
    [XmlType(TypeName ="note-head")] //! test
    public class NoteHeadMusicXML
    {
        private YesNoMusicXML filled;
        private bool filledSpecified;
        private YesNoMusicXML parentheses;
        private bool parenthesesSpecified;
        private string fontFamily;
        private FontStyleMusicXML fontStyle;
        private bool fontStyleSpecified;
        private string fontSize;
        private FontWeightMusicXML fontWeight;
        private bool fontWeightSpecified;
        private string color;
        private NoteHeadValueMusicXML value;

        public NoteHeadMusicXML()
        {

        }

        [XmlAttribute("")]
        public YesNoMusicXML Filled
        {
            get
            {
                return filled;
            }

            set
            {
                filled = value;
            }
        }

        [XmlIgnore]
        public bool FilledSpecified
        {
            get
            {
                return filledSpecified;
            }

            set
            {
                filledSpecified = value;
            }
        }

        [XmlAttribute("")]
        public YesNoMusicXML Parentheses
        {
            get
            {
                return parentheses;
            }

            set
            {
                parentheses = value;
            }
        }

        [XmlIgnore]
        public bool ParenthesesSpecified
        {
            get
            {
                return parenthesesSpecified;
            }

            set
            {
                parenthesesSpecified = value;
            }
        }

        [XmlAttribute("")]
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

        [XmlAttribute("")]
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

        [XmlAttribute("")]
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

        [XmlAttribute("")]
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

        [XmlAttribute("")]
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

        [XmlText]
        public NoteHeadValueMusicXML Value
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
    }

    [Serializable]
    [XmlType(TypeName ="notehead-value")]
    public enum NoteHeadValueMusicXML
    {
        slash,
        triangle,
        diamond,
        square,
        cross,
        x,
        [XmlEnum("circle-x")]
        circlex,
        [XmlEnum("inverted triangle")]
        invertedtriangle,
        [XmlEnum("arrow down")]
        arrowdown,
        [XmlEnum("arrow up")]
        arrowup,
        slashed,
        [XmlEnum("back slashed")]
        backslashed,
        normal,
        cluster,
        [XmlEnum("circle dot")]
        circledot,
        [XmlEnum("left triangle")]
        lefttriangle,
        rectangle,
        none,
        @do,
        re,
        mi,
        fa,
        [XmlEnum("fa up")]
        faup,
        so,
        la,
        ti,
    }
}
