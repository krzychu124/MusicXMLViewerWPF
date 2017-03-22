using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.Attributes
{
    [Serializable]
    [XmlType(TypeName ="staff-details")]
    public class StaffDetailsMusicXML
    {
        private StaffTypeMusicXML staffType;
        private bool staffTypeSpecified;
        private string staffLines;
        private StaffTuningMusicXML[] staffTuning;
        private string capo;
        private double staffSize;
        private bool staffSizeSpecified;
        private string number;
        private ShowFretsMusicXML showFrets;
        private bool showFretsSpecified;
        private YesNoMusicXML printObject;
        private bool printObjectSpecified;
        private YesNoMusicXML printSpacing;
        private bool printSpacingSpecified;

        [XmlElement("staff-type")]
        public StaffTypeMusicXML StaffType
        {
            get
            {
                return staffType;
            }

            set
            {
                staffType = value;
            }
        }

        [XmlIgnore]
        public bool StaffTypeSpecified
        {
            get
            {
                return staffTypeSpecified;
            }

            set
            {
                staffTypeSpecified = value;
            }
        }

        [XmlElement("staff-lines", DataType ="nonNegativeInteger")]
        public string StaffLines
        {
            get
            {
                return staffLines;
            }

            set
            {
                staffLines = value;
            }
        }

        [XmlElement("staff-tuning")]
        public StaffTuningMusicXML[] StaffTuning
        {
            get
            {
                return staffTuning;
            }

            set
            {
                staffTuning = value;
            }
        }

        [XmlElement("capo")]
        public string Capo
        {
            get
            {
                return capo;
            }

            set
            {
                capo = value;
            }
        }

        [XmlElement("staff-size")]
        public double StaffSize
        {
            get
            {
                return staffSize;
            }

            set
            {
                staffSize = value;
            }
        }

        [XmlIgnore]
        public bool StaffSizeSpecified
        {
            get
            {
                return staffSizeSpecified;
            }

            set
            {
                staffSizeSpecified = value;
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

        [XmlAttribute("show-frets")]
        public ShowFretsMusicXML ShowFrets
        {
            get
            {
                return showFrets;
            }

            set
            {
                showFrets = value;
            }
        }

        [XmlIgnore]
        public bool ShowFretsSpecified
        {
            get
            {
                return showFretsSpecified;
            }

            set
            {
                showFretsSpecified = value;
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

        [XmlAttribute("print-spacing")]
        public YesNoMusicXML PrintSpacing
        {
            get
            {
                return printSpacing;
            }

            set
            {
                printSpacing = value;
            }
        }

        [XmlIgnore]
        public bool PrintSpacingSpecified
        {
            get
            {
                return printSpacingSpecified;
            }

            set
            {
                printSpacingSpecified = value;
            }
        }
    }

    [Serializable]
    [XmlType(TypeName="staff-tuning")]
    public class StaffTuningMusicXML
    {
        private StepMusicXML tuningStep;
        private double tuningAlter;
        private bool tuningAlterSpecified;
        private string tuningOctave;
        private string line;

        [XmlElement("tuning-step")]
        public StepMusicXML TuningStep
        {
            get
            {
                return tuningStep;
            }

            set
            {
                tuningStep = value;
            }
        }

        [XmlElement("tuning-alter")]
        public double TuningAlter
        {
            get
            {
                return tuningAlter;
            }

            set
            {
                tuningAlter = value;
            }
        }

        [XmlIgnore]
        public bool TuningAlterSpecified
        {
            get
            {
                return tuningAlterSpecified;
            }

            set
            {
                tuningAlterSpecified = value;
            }
        }

        [XmlElement("tuning-octave", DataType ="integer")]
        public string TuningOctave
        {
            get
            {
                return tuningOctave;
            }

            set
            {
                tuningOctave = value;
            }
        }

        [XmlAttribute("line", DataType ="integer")]
        public string Line
        {
            get
            {
                return line;
            }

            set
            {
                line = value;
            }
        }
    }

    [Serializable]
    [XmlType(TypeName ="staff-type")]
    public enum StaffTypeMusicXML
    {
        ossia,
        cue,
        editorial,
        regular,
        alternate,
    }

    [Serializable]
    [XmlType(TypeName ="show-frets")]
    public enum ShowFretsMusicXML
    {
        numbers,
        letters,
    }
}