using MusicXMLScore.Model.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MusicXMLScore.Model.MeasureItems.NoteItems.Notations;

namespace MusicXMLScore.Model.MeasureItems.Directions
{
    [Serializable]
    [XmlType(TypeName ="metronome")]
    public class MetronomeMusicXML
    {
        private object[] items;
        private LeftCenterRightMusicXML justify;
        private bool justifySpecified;
        private YesNoMusicXML parentheses;
        private bool parenthesesSpecified;


        [XmlElement("beat-unit", typeof(NoteTypeValueMusicXML))]
        [XmlElement("beat-unit-dot", typeof(EmptyMusicXML))]
        [XmlElement("metronome-note", typeof(MetronomeNoteMusicXML))]
        [XmlElement("metronome-relation", typeof(string))]
        [XmlElement("per-minute", typeof(PerMinuteMusicXML))]
        public object[] Items
        {
            get
            {
                return items;
            }

            set
            {
                items = value;
            }
        }

        [XmlAttribute("justify")]
        public LeftCenterRightMusicXML Justify
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

        [XmlIgnore]
        public bool JustifySpecified
        {
            get
            {
                return justifySpecified;
            }

            set
            {
                justifySpecified = value;
            }
        }

        [XmlAttribute("parentheses")]
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
    }

    [Serializable]
    [XmlType(TypeName ="metronome-type")]
    public class MetronomeNoteMusicXML
    {
        private NoteTypeValueMusicXML metronomeType;
        private List<EmptyMusicXML> metronomeDot;
        private List<MetronomeBeamMusicXML> metronomeBeam;
        private MetronomeTupletMusicXML metronomeTuplet;

        [XmlElement("metronome-type")]
        public NoteTypeValueMusicXML MetronomeType
        {
            get
            {
                return metronomeType;
            }

            set
            {
                metronomeType = value;
            }
        }

        [XmlElement("metronome-dot")]
        public List<EmptyMusicXML> MetronomeDot
        {
            get
            {
                return metronomeDot;
            }

            set
            {
                metronomeDot = value;
            }
        }

        [XmlElement("metronome-beam")]
        public List<MetronomeBeamMusicXML> MetronomeBeam
        {
            get
            {
                return metronomeBeam;
            }

            set
            {
                metronomeBeam = value;
            }
        }

        [XmlElement("metronome-tuplet")]
        public MetronomeTupletMusicXML MetronomeTuplet
        {
            get
            {
                return metronomeTuplet;
            }

            set
            {
                metronomeTuplet = value;
            }
        }
    }

    [Serializable]
    [XmlType(TypeName ="metronome-tuplet")]
    public class MetronomeTupletMusicXML : NoteItems.TimeModificationMusicXML
    {
        private StartStopMusicXML type;
        private YesNoMusicXML bracket;
        private bool bracketSpecified;
        private NoteItems.Notations.ShowTupletMusicXML showNumber;
        private bool showNumberSpecified;

        public MetronomeTupletMusicXML()
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
    }

    [Serializable]
    [XmlType(TypeName ="metronome-beam")]
    public class MetronomeBeamMusicXML
    {
        private string number;
        private BeamValueMusicXML value;

        public MetronomeBeamMusicXML()
        {
            number = "1";
        }

        [XmlAttribute(DataType = "positiveInteger")]
        [System.ComponentModel.DefaultValue("1")]
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

        [XmlText]
        public BeamValueMusicXML Value
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
    [XmlType(TypeName ="per-minute")]
    public class PerMinuteMusicXML
    {
        private string fontFamily;
        private FontStyleMusicXML fontStyle;
        private bool fontStyleSpecified;
        private string fontSize;
        private FontWeightMusicXML fontWeight;
        private bool fontWeightSpecified;
        private string value;

        public PerMinuteMusicXML()
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
}
