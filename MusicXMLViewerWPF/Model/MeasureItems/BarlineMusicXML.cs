using MusicXMLScore.Model.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems
{
    [Serializable]
    [XmlType(TypeName = "barline")]
    public class BarlineMusicXML
    {
        private BarStyleColorMusicXML barStyle;
        //formattedText footnote;
        //level levelField;
        //wavyLine //TODO_L
        private EmptyPrintStyleAlignMusicXML segno;
        private EmptyPrintStyleAlignMusicXML coda;
        private FermataMusicXML fermata;
        private EndingMusicXML ending;
        private RepeatMusicXML repeat;
        private RightLeftMiddleMusicXML location;
        private string segno1;
        private string coda1;
        private double divisions;
        private bool divisionsSpecified;

        [XmlElement("bar-style")]
        public BarStyleColorMusicXML BarStyle
        {
            get
            {
                return barStyle;
            }

            set
            {
                barStyle = value;
            }
        }
        [XmlElement("segno")]
        public EmptyPrintStyleAlignMusicXML Segno
        {
            get
            {
                return segno;
            }

            set
            {
                segno = value;
            }
        }
        [XmlElement("coda")]
        public EmptyPrintStyleAlignMusicXML Coda
        {
            get
            {
                return coda;
            }

            set
            {
                coda = value;
            }
        }
        [XmlElement("fermata")]
        public FermataMusicXML Fermata
        {
            get
            {
                return fermata;
            }

            set
            {
                fermata = value;
            }
        }
        [XmlElement("ending")]
        public EndingMusicXML Ending
        {
            get
            {
                return ending;
            }

            set
            {
                ending = value;
            }
        }
        [XmlElement("repeat")]
        public RepeatMusicXML Repeat
        {
            get
            {
                return repeat;
            }

            set
            {
                repeat = value;
            }
        }
        [XmlAttribute("location")]
        [DefaultValue(RightLeftMiddleMusicXML.right)]
        public RightLeftMiddleMusicXML Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
            }
        }
        [XmlAttribute("segno")]
        public string Segno1
        {
            get
            {
                return segno1;
            }

            set
            {
                segno1 = value;
            }
        }
        [XmlAttribute("coda")]
        public string Coda1
        {
            get
            {
                return coda1;
            }

            set
            {
                coda1 = value;
            }
        }
        [XmlElement("divisions")]
        public double Divisions
        {
            get
            {
                return divisions;
            }

            set
            {
                divisions = value;
            }
        }
        [XmlIgnore]
        public bool DivisionsSpecified
        {
            get
            {
                return divisionsSpecified;
            }

            set
            {
                divisionsSpecified = value;
            }
        }

        public BarlineMusicXML()
        {
            location = RightLeftMiddleMusicXML.right;
        }
    }

    [Serializable]
    [XmlType(TypeName ="ending")]
    public class EndingMusicXML
    {
        private string number;
        private StartStopDiscontinueMusicXML type;
        private YesNoMusicXML printObject;
        private bool printObjectSpecified;
        private double endingLength;
        private bool endingLengthSpecified;
        private double textX;
        private bool textXSpecified;
        private double textY;
        private bool textYSpecified;
        private string value;

        public EndingMusicXML()
        {

        }

        [XmlAttribute("number")]
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
        [XmlAttribute("type")]
        public StartStopDiscontinueMusicXML Type
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
        [XmlAttribute("print-object")]
        public YesNoMusicXML PrintObject
        {
            get
            {
                return printObject;
            }

            set
            {
                printObject = value;
            }
        }
        [XmlIgnore]
        public bool PrintObjectSpecified
        {
            get
            {
                return printObjectSpecified;
            }

            set
            {
                printObjectSpecified = value;
            }
        }
        [XmlAttribute("end-length")]
        public double EndingLength
        {
            get
            {
                return endingLength;
            }

            set
            {
                endingLength = value;
            }
        }
        [XmlIgnore]
        public bool EndingLengthSpecified
        {
            get
            {
                return endingLengthSpecified;
            }

            set
            {
                endingLengthSpecified = value;
            }
        }
        [XmlAttribute("text-x")]
        public double TextX
        {
            get
            {
                return textX;
            }

            set
            {
                textX = value;
            }
        }
        [XmlIgnore]
        public bool TextXSpecified
        {
            get
            {
                return textXSpecified;
            }

            set
            {
                textXSpecified = value;
            }
        }
        [XmlAttribute("text-y")]
        public double TextY
        {
            get
            {
                return textY;
            }

            set
            {
                textY = value;
            }
        }
        [XmlIgnore]
        public bool TextYSpecified
        {
            get
            {
                return textYSpecified;
            }

            set
            {
                textYSpecified = value;
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
    [XmlType(TypeName ="repeat")]
    public class RepeatMusicXML
    {
        private BackwardForwardMusicXML direction;
        private string times;
        private WingedMusicXML winged;
        private bool wingedSpecified;

        public RepeatMusicXML()
        {

        }

        [XmlAttribute("direction")]
        public BackwardForwardMusicXML Direction
        {
            get
            {
                return direction;
            }

            set
            {
                direction = value;
            }
        }
        [XmlAttribute("times", DataType ="nonNegativeInteger")]
        public string Times
        {
            get
            {
                return times;
            }

            set
            {
                times = value;
            }
        }
        [XmlAttribute("winged")]
        public WingedMusicXML Winged
        {
            get
            {
                return winged;
            }

            set
            {
                winged = value;
            }
        }
        [XmlIgnore]
        public bool WingedSpecified
        {
            get
            {
                return wingedSpecified;
            }

            set
            {
                wingedSpecified = value;
            }
        }
    }

    [Serializable]
    [XmlType(TypeName ="fermata")]
    public class FermataMusicXML
    {
        private UprightInvertedMusicXML type;
        private bool typeSpecified;
        private FermataShapeMusicXML value;

        public FermataMusicXML()
        {

        }
        [XmlAttribute("type")]
        public UprightInvertedMusicXML Type
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
        [XmlIgnore]
        public bool TypeSpecified
        {
            get
            {
                return typeSpecified;
            }

            set
            {
                typeSpecified = value;
            }
        }
        [XmlText]
        public FermataShapeMusicXML Value
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
    [XmlType(TypeName ="bar-style-color")]
    public class BarStyleColorMusicXML
    {
        private string color;
        private BarStyleMusicXML value;

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
        public BarStyleMusicXML Value
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
    [XmlType(TypeName = "bar-style")]
    public enum BarStyleMusicXML
    {
        regular,
        dotted,
        dashed,
        heavy,
        [XmlEnum("light-light")]
        lightlight,
        [XmlEnum("light-heavy")]
        lightheavy,
        [XmlEnum("heavy-light")]
        heavylight,
        [XmlEnum("heavy-heavy")]
        heavyheavy,
        tick,
        @short,
        none,
    }

    [Serializable]
    [XmlType(TypeName = "fermata-shape")]
    public enum FermataShapeMusicXML
    {
        normal,
        angled,
        square,
        [XmlEnum("")]
        Item,
    }
    
    [Serializable]
    [XmlType(TypeName ="right-left-middle")]
    public enum RightLeftMiddleMusicXML
    {
        right,
        left,
        middle,
    }
    
    [Serializable]
    [XmlType(TypeName = "start-stop-discontuinue")]
    public enum StartStopDiscontinueMusicXML
    {
        start,
        stop,
        discontinue,
    }

    [Serializable]
    [XmlType(TypeName = "upright-inverted")]
    public enum UprightInvertedMusicXML
    {
        upright,
        inverted,
    }

    [Serializable]
    [XmlType(TypeName ="winged")]
    public enum WingedMusicXML
    {
        none,
        straight,
        curved,
        [XmlEnum("double-straight")]
        doublestraight,
        [XmlEnum("double-curved")]
        doublecurved,
    }
}