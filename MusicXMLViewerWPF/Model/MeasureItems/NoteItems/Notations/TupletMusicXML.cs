using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.NoteItems.Notations
{
    [Serializable]
    [XmlType(TypeName ="tuplet")]
    public class TupletMusicXML
    {
        private TupletPortionMusicXML tupletActual;
        private TupletPortionMusicXML tupletNormal;
        private StartStopMusicXML type;
        private string number;
        private YesNoMusicXML bracket;
        private bool bracketSpecified;
        private ShowTupletMusicXML showNumber;
        private bool showNumberSpecified;
        private ShowTupletMusicXML showType;
        private bool showTypeSpecified;
        private LineShapeMusicXML lineShape;
        private bool lineShapeSpecified;
        private double defaultX;
        private bool defaultXSpecified;
        private double defaultY;
        private bool defaultYSpecified;
        private double relativeX;
        private bool relativeXSpecified;
        private double relativeY;
        private bool relativeYSpecified;
        private AboveBelowMusicXML placement;
        private bool placementSpecified;

        public TupletMusicXML()
        {

        }

        [XmlElement("tuplet-actual")]
        public TupletPortionMusicXML TupletActual
        {
            get
            {
                return tupletActual;
            }

            set
            {
                tupletActual = value;
            }
        }

        [XmlElement("tuplet-number")]
        public TupletPortionMusicXML TupletNormal
        {
            get
            {
                return tupletNormal;
            }

            set
            {
                tupletNormal = value;
            }
        }

        [XmlAttribute("type")]
        public StartStopMusicXML Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        [XmlAttribute("number", DataType="positiveInteger")]
        public string Number
        {
            get
            {
                return number;
            }

            set
            {
                number = value;
            }
        }

        [XmlAttribute("bracket")]
        public YesNoMusicXML Bracket
        {
            get
            {
                return bracket;
            }

            set
            {
                bracket = value;
            }
        }

        [XmlIgnore]
        public bool BracketSpecified
        {
            get
            {
                return bracketSpecified;
            }

            set
            {
                bracketSpecified = value;
            }
        }

        [XmlAttribute("show-number")]
        public ShowTupletMusicXML ShowNumber
        {
            get
            {
                return showNumber;
            }

            set
            {
                showNumber = value;
            }
        }

        [XmlIgnore]
        public bool ShowNumberSpecified
        {
            get
            {
                return showNumberSpecified;
            }

            set
            {
                showNumberSpecified = value;
            }
        }

        [XmlAttribute("show-type")]
        public ShowTupletMusicXML ShowType
        {
            get
            {
                return showType;
            }

            set
            {
                showType = value;
            }
        }

        [XmlIgnore]
        public bool ShowTypeSpecified
        {
            get
            {
                return showTypeSpecified;
            }

            set
            {
                showTypeSpecified = value;
            }
        }

        [XmlAttribute("line-shape")]
        public LineShapeMusicXML LineShape
        {
            get
            {
                return lineShape;
            }

            set
            {
                lineShape = value;
            }
        }

        [XmlIgnore]
        public bool LineShapeSpecified
        {
            get
            {
                return lineShapeSpecified;
            }

            set
            {
                lineShapeSpecified = value;
            }
        }

        [XmlAttribute("default-x")]
        public double DefaultX
        {
            get
            {
                return defaultX;
            }

            set
            {
                defaultX = value;
            }
        }

        [XmlIgnore]
        public bool DefaultXSpecified
        {
            get
            {
                return defaultXSpecified;
            }

            set
            {
                defaultXSpecified = value;
            }
        }

        [XmlAttribute("default-y")]
        public double DefaultY
        {
            get
            {
                return defaultY;
            }

            set
            {
                defaultY = value;
            }
        }

        [XmlIgnore]
        public bool DefaultYSpecified
        {
            get
            {
                return defaultYSpecified;
            }

            set
            {
                defaultYSpecified = value;
            }
        }

        [XmlAttribute("relative-x")]
        public double RelativeX
        {
            get
            {
                return relativeX;
            }

            set
            {
                relativeX = value;
            }
        }

        [XmlIgnore]
        public bool RelativeXSpecified
        {
            get
            {
                return relativeXSpecified;
            }

            set
            {
                relativeXSpecified = value;
            }
        }

        [XmlAttribute("relative-y")]
        public double RelativeY
        {
            get
            {
                return relativeY;
            }

            set
            {
                relativeY = value;
            }
        }

        [XmlIgnore]
        public bool RelativeYSpecified
        {
            get
            {
                return relativeYSpecified;
            }

            set
            {
                relativeYSpecified = value;
            }
        }

        [XmlAttribute("placement")]
        public AboveBelowMusicXML Placement
        {
            get
            {
                return placement;
            }

            set
            {
                placement = value;
            }
        }

        [XmlIgnore]
        public bool PlacementSpecified
        {
            get
            {
                return placementSpecified;
            }

            set
            {
                placementSpecified = value;
            }
        }
    }

    [Serializable]
    [XmlType(TypeName = "show-tuplet")]
    public enum ShowTupletMusicXML
    {
        actual,
        both,
        none,
    }

    [Serializable]
    [XmlType(TypeName = "tuplet-portion")]
    public class TupletPortionMusicXML
    {
        private TupletNumberMusicXML tupletNumber;
        private TupletTypeMusicXML tupleType;
        private TupletDotMusicXML[] tupletDot;

        public TupletPortionMusicXML()
        {

        }

        [XmlElement("tuplet-number")]
        public TupletNumberMusicXML TupletNumber
        {
            get
            {
                return tupletNumber;
            }

            set
            {
                tupletNumber = value;
            }
        }

        [XmlElement("tuplet-type")]
        public TupletTypeMusicXML TupletType
        {
            get
            {
                return tupleType;
            }

            set
            {
                tupleType = value;
            }
        }

        [XmlElement("tuplet-dot")]
        public TupletDotMusicXML[] TupletDot
        {
            get
            {
                return tupletDot;
            }

            set
            {
                tupletDot = value;
            }
        }
    }

        [Serializable]
    [XmlType(TypeName = "tuplet-number")]
    public class TupletNumberMusicXML
    {
        private string fontFamily;
        private FontStyleMusicXML fontStyle;
        private bool fontStyleSpecified;
        private string fontSize;
        private FontWeightMusicXML fontWeight;
        private bool fontWeightSpecified;
        private string color;
        private string value;

        public TupletNumberMusicXML()
        {

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

        [XmlText]
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
    }

    [Serializable]
    [XmlType(TypeName = "tuplet-type")]
    public class TupletTypeMusicXML
    {
        private string fontFamily;
        private FontStyleMusicXML fontStyle;
        private bool fontStyleSpecified;
        private string fontSize;
        private FontWeightMusicXML fontWeight;
        private bool fontWeightSpecified;
        private string color;
        private NoteTypeValueMusicXML value;

        public TupletTypeMusicXML()
        {

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

        [XmlText]
        public NoteTypeValueMusicXML Value
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
    [XmlType(TypeName = "tuplet-dot")]
    public class TupletDotMusicXML
    {
        private string fontFamily;
        private FontStyleMusicXML fontStyle;
        private bool fontStyleSpecified;
        private string fontSize;
        private FontWeightMusicXML fontWeight;
        private bool fontWeightSpecified;
        private string color;

        public TupletDotMusicXML()
        {

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
    }
}
