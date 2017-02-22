using MusicXMLScore.Model.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.Attributes
{
    [Serializable]
    [XmlType(TypeName ="measure-style")]
    public class MeasureStyleMusicXML
    {
        private object item;
        private string number;
        private string fontFamily;
        private FontStyleMusicXML fontStyle;
        private bool fontStyleSpecified;
        private FontWeightMusicXML fontWeight;
        private bool fontWeightSpecified;
        private string color;

        [XmlElement("beat-repeat", typeof(BeatRepeatMusicXML))]
        [XmlElement("measure-repeat", typeof(MeasureRepeatMusicXML))]
        [XmlElement("multiple-rest", typeof(MultipleRestMusicXML))]
        [XmlElement("slash", typeof(SlashMusicXML))]
        public object Item
        {
            get
            {
                return this.item;
            }
            set
            {
                this.item = value;
            }
        }
        [XmlAttribute("number", DataType ="positiveInteger")]
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
    //! =============================================
    [Serializable]
    [XmlType(TypeName = "slash")]
    public class SlashMusicXML
    {
        private NoteTypeValueMusicXML slashType;
        private List<EmptyMusicXML> slashDot;
        private StartStopMusicXML type;
        private YesNoMusicXML useDots;
        private bool useDotsSpecified;
        private YesNoMusicXML useStems;
        private bool useStemsSpecified;

        public SlashMusicXML()
        {

        }

        [XmlElement("slash-type")]
        public NoteTypeValueMusicXML SlashType
        {
            get
            {
                return slashType;
            }

            set
            {
                slashType = value;
            }
        }
        [XmlElement("slash-dot")]
        internal List<EmptyMusicXML> SlashDot
        {
            get
            {
                return slashDot;
            }

            set
            {
                slashDot = value;
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
        [XmlAttribute("use-dots")]
        public YesNoMusicXML UseDots
        {
            get
            {
                return useDots;
            }

            set
            {
                useDots = value;
            }
        }
        [XmlIgnore]
        public bool UseDotsSpecified
        {
            get
            {
                return useDotsSpecified;
            }

            set
            {
                useDotsSpecified = value;
            }
        }
        [XmlAttribute("use-stems")]
        public YesNoMusicXML UseStems
        {
            get
            {
                return useStems;
            }

            set
            {
                useStems = value;
            }
        }
        [XmlIgnore]
        public bool UseStemsSpecified
        {
            get
            {
                return useStemsSpecified;
            }

            set
            {
                useStemsSpecified = value;
            }
        }
    }

    //! =============================================
    [Serializable]
    [XmlType(TypeName = "multiple-rest")]
    public class MultipleRestMusicXML
    {
        private YesNoMusicXML useSymbols;
        private bool useSymbolsSpecified;
        private string value;

        public MultipleRestMusicXML()
        {

        }

        [XmlAttribute("use-symbols")]
        public YesNoMusicXML UseSymbols
        {
            get
            {
                return useSymbols;
            }

            set
            {
                useSymbols = value;
            }
        }
        [XmlIgnore]
        public bool UseSymbolsSpecified
        {
            get
            {
                return useSymbolsSpecified;
            }

            set
            {
                useSymbolsSpecified = value;
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

    //! =============================================
    [Serializable]
    [XmlType(TypeName = "measure-repeat")]
    public class MeasureRepeatMusicXML
    {
        private StartStopMusicXML type;
        private string slashes;
        private string value;

        public MeasureRepeatMusicXML()
        {

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
        [XmlAttribute("slashes", DataType ="positiveInteger")]
        public string Slashes
        {
            get
            {
                return slashes;
            }

            set
            {
                slashes = value;
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

    //! =============================================
    [Serializable]
    [XmlType(TypeName = "beat-repeat")]
    public class BeatRepeatMusicXML
    {
        private NoteTypeValueMusicXML slashType;
        private List<EmptyMusicXML> slashDot;
        private StartStopMusicXML type;
        private string slashes;
        private YesNoMusicXML useDots;
        private bool useDotsSpecified;

        public BeatRepeatMusicXML()
        {

        }

        [XmlElement("slash-type")]
        public NoteTypeValueMusicXML SlashType
        {
            get
            {
                return slashType;
            }

            set
            {
                slashType = value;
            }
        }
        [XmlElement("slash-dot")]
        internal List<EmptyMusicXML> SlashDot
        {
            get
            {
                return slashDot;
            }

            set
            {
                slashDot = value;
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
        [XmlAttribute("slashes", DataType ="positiveInteger")]
        public string Slashes
        {
            get
            {
                return slashes;
            }

            set
            {
                slashes = value;
            }
        }
        [XmlAttribute("use-dots")]
        public YesNoMusicXML UseDots
        {
            get
            {
                return useDots;
            }

            set
            {
                useDots = value;
            }
        }
        [XmlIgnore]
        public bool UseDotsSpecified
        {
            get
            {
                return useDotsSpecified;
            }

            set
            {
                useDotsSpecified = value;
            }
        }
    }
}