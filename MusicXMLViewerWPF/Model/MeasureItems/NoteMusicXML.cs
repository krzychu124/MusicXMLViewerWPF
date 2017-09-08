using MusicXMLScore.Model.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using MusicXMLScore.Model.MeasureItems.NoteItems;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using MusicXMLScore.Converters;

namespace MusicXMLScore.Model.MeasureItems
{
    [Serializable]
    [XmlType(TypeName = "note")]
    public class NoteMusicXML
    {
        private object[] items;
        private NoteChoiceTypeMusicXML[] itemsElementName;
        private InstrumentMusicXML instrument;
        private FormattedTextMusicXML footnote; //no usage
        private LevelMusicXML level; //no usage
        private string voice;
        private NoteTypeMusicXML type;
        private List<EmptyPlacementMusicXML> dot;
        private AccidentalMusicXML accidental;
        private TimeModificationMusicXML timeModification;
        private StemMusicXML stem;
        private NoteHeadMusicXML noteHead;
        private NoteHeadTextMusicXML noteHeadText;
        private string staff ="1";
        private List<BeamMusicXML> beam;
        private List<NotationsMusicXML> notations;
        private List<LyricMusicXML> lyric;
        //private PlayMusicXML play; no implementation
        private double defaultX;
        private bool defaultXSpecified;
        private double defaultY;
        private bool defaultYSpecified;
        private double relativeX;
        private bool relativeXSpecified;
        private double relativeY;
        private bool relativeYSpecified;
        private string fontFamily;
        private FontStyleMusicXML fontStyle;
        private bool fontStyleSpecified;
        private string fontSize;
        private FontWeightMusicXML fontWeight;
        private bool fontWeightSpecified;
        private string color;
        private YesNoMusicXML printDot;
        private bool printDotSpecified;
        private YesNoMusicXML printLyric;
        private bool printLyricSpecified;

        [XmlElement("chord", typeof(EmptyMusicXML))]
        [XmlElement("cue", typeof(EmptyMusicXML))]
        [XmlElement("duration", typeof(decimal))]
        [XmlElement("grace", typeof(GraceMusicXML))]
        [XmlElement("pitch", typeof(PitchMusicXML))]
        [XmlElement("rest", typeof(RestMusicXML))]
        [XmlElement("tie", typeof(TieMusicXML))]
        [XmlElement("unpitched", typeof(UnpitchedMusicXML))]
        [XmlChoiceIdentifier("ItemsElementName")]
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

        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public NoteChoiceTypeMusicXML[] ItemsElementName
        {
            get
            {
                return itemsElementName;
            }

            set
            {
                itemsElementName = value;
            }
        }

        [XmlElement("instrument")]
        public InstrumentMusicXML Instrument
        {
            get
            {
                return instrument;
            }

            set
            {
                instrument = value;
            }
        }

        [XmlElement("footnote")]
        public FormattedTextMusicXML Footnote
        {
            get
            {
                return footnote;
            }

            set
            {
                footnote = value;
            }
        }

        [XmlElement("level")]
        public LevelMusicXML Level
        {
            get
            {
                return level;
            }

            set
            {
                level = value;
            }
        }

        [XmlElement("voice")]
        public string Voice
        {
            get
            {
                return voice;
            }

            set
            {
                voice = value;
            }
        }

        [XmlElement("type")]
        public NoteTypeMusicXML Type
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

        [XmlElement("dot")]
        public List<EmptyPlacementMusicXML> Dot
        {
            get
            {
                return dot;
            }

            set
            {
                dot = value;
            }
        }

        [XmlElement("accidental")]
        public AccidentalMusicXML Accidental
        {
            get
            {
                return accidental;
            }

            set
            {
                accidental = value;
            }
        }

        [XmlElement("time-modification")]
        public TimeModificationMusicXML TimeModification
        {
            get
            {
                return timeModification;
            }

            set
            {
                timeModification = value;
            }
        }

        [XmlElement("stem")]
        public StemMusicXML Stem
        {
            get
            {
                return stem;
            }

            set
            {
                stem = value;
            }
        }

        [XmlElement("notehead")]
        public NoteHeadMusicXML NoteHead
        {
            get
            {
                return noteHead;
            }

            set
            {
                noteHead = value;
            }
        }

        [XmlElement("notehead-text")]
        public NoteHeadTextMusicXML NoteHeadText
        {
            get
            {
                return noteHeadText;
            }

            set
            {
                noteHeadText = value;
            }
        }

        [XmlElement("staff")]
        public string Staff
        {
            get
            {
                return staff;
            }

            set
            {
                staff = value;
            }
        }

        [XmlElement("beam")]
        public List<BeamMusicXML> Beam
        {
            get
            {
                return beam;
            }

            set
            {
                beam = value;
            }
        }

        [XmlElement("notations")]
        public List<NotationsMusicXML> Notations
        {
            get
            {
                return notations;
            }

            set
            {
                notations = value;
            }
        }

        [XmlElement("lyric")]
        public  List<LyricMusicXML> Lyric
        {
            get
            {
                return lyric;
            }

            set
            {
                lyric = value;
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

        [XmlAttribute("print-dot")]
        public YesNoMusicXML PrintDot
        {
            get
            {
                return printDot;
            }

            set
            {
                printDot = value;
            }
        }

        [XmlIgnore]
        public bool PrintDotSpecified
        {
            get
            {
                return printDotSpecified;
            }

            set
            {
                printDotSpecified = value;
            }
        }

        [XmlAttribute("print-lyric")]
        public YesNoMusicXML PrintLyric
        {
            get
            {
                return printLyric;
            }

            set
            {
                printLyric = value;
            }
        }

        [XmlIgnore]
        public bool PrintLyricSpecified
        {
            get
            {
                return printLyricSpecified;
            }

            set
            {
                printLyricSpecified = value;
            }
        }

        public NoteChoiceTypeMusicXML GetNoteType()
        {
            NoteChoiceTypeMusicXML result = NoteChoiceTypeMusicXML.none;
            if (ItemsElementName.Contains(NoteChoiceTypeMusicXML.chord))
            {
                result = result.SetFlags(NoteChoiceTypeMusicXML.chord, true);
            }
            if (ItemsElementName.Contains(NoteChoiceTypeMusicXML.cue))
            {
                result = NoteChoiceTypeMusicXML.cue;
            }
            if (ItemsElementName.Contains(NoteChoiceTypeMusicXML.grace))
            {
                result = NoteChoiceTypeMusicXML.grace;
            }
            
            //Add additional attribute info
           return GetAdditionalType(result);
        }

        public int GetDuration()
        {
            int result =0;
            if (!GetNoteType().HasFlag(NoteChoiceTypeMusicXML.grace)) // grace note don't have duration specified
            {
                int index = GetIndexOfType(NoteChoiceTypeMusicXML.duration);
                result = int.Parse(Items[index].ToString());
            }
            return result;
        }

        public void SetDuration(int durationValue)
        {
            if (!GetNoteType().HasFlag(NoteChoiceTypeMusicXML.grace))
            {
                if (durationValue < 1)
                {
                    throw new Exception("Note::SetDuration Note duration can not be lower or equal zero");
                }
                int index = GetIndexOfType(NoteChoiceTypeMusicXML.duration);
                if (index == -1)
                {
                    Array.Resize(ref itemsElementName, itemsElementName.Length + 1);
                    itemsElementName[itemsElementName.Length - 1] = NoteChoiceTypeMusicXML.duration;
                    Array.Resize(ref items, items.Length + 1);
                    items[items.Length - 1] = durationValue;
                } else
                {
                    items[index] = durationValue;
                }
            } else
            {
                throw new Exception("Note::SetDuration Note of type Grace can not have duration specified");
            }
        }

        public List<TieMusicXML> GetTies()
        {
            List<TieMusicXML> ties = new List<TieMusicXML>();
            for (int i = 0; i < ItemsElementName.Length; i++)
            {
                if (ItemsElementName[i] == NoteChoiceTypeMusicXML.tie)
                {
                    ties.Add(Items[i] as TieMusicXML);
                }
            }
            return ties;
        }

        public NoteChoiceTypeMusicXML GetAdditionalType(NoteChoiceTypeMusicXML type)
        {
            if (ItemsElementName.Contains(NoteChoiceTypeMusicXML.rest))
            {
                type.SetFlags(NoteChoiceTypeMusicXML.rest, true);
            }
            if (ItemsElementName.Contains(NoteChoiceTypeMusicXML.pitch))
            {
                type.SetFlags(NoteChoiceTypeMusicXML.pitch, true);
            }
            else
            {
                type.SetFlags(NoteChoiceTypeMusicXML.unpitched, true);
            }
            return type;
        }

        public int GetIndexOfType(NoteChoiceTypeMusicXML attributeType)
        {
            return Array.IndexOf(ItemsElementName, attributeType);
        }

        public bool IsRest()
        {
            return Items.OfType<RestMusicXML>().Any();
        }

        public bool IsChord()
        {
            return ItemsElementName.Contains(NoteChoiceTypeMusicXML.chord);
        }

        public bool IsGrace()
        {
            return ItemsElementName.Contains(NoteChoiceTypeMusicXML.grace);
        }
    }

    [Serializable]
    public class UnpitchedMusicXML
    {
        private StepMusicXML displayStep;
        private string displayOctave;

        public UnpitchedMusicXML()
        {

        }

        [XmlElement("display-step")]
        public StepMusicXML DisplayStep
        {
            get
            {
                return displayStep;
            }

            set
            {
                displayStep = value;
            }
        }

        [XmlElement("display-octave", DataType ="integer")]
        public string DisplayOctave
        {
            get
            {
                return displayOctave;
            }

            set
            {
                displayOctave = value;
            }
        }
    }

    [Serializable]
    //[XmlType(TypeName ="tie")] //! test
    public class TieMusicXML
    {
        private StartStopMusicXML type;
        private string timeOnly;

        public TieMusicXML()
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

        [XmlAttribute("time-only")]
        public string TimeOnly
        {
            get
            {
                return timeOnly;
            }

            set
            {
                timeOnly = value;
            }
        }
    }

    [Serializable]
    [XmlType(TypeName ="rest")]
    public class RestMusicXML
    {
        private StepMusicXML displayStep;
        private string displayOctave;
        private YesNoMusicXML measure;
        private bool measureSpecified;

        [XmlElement("display-step")]
        public StepMusicXML DisplayStep
        {
            get
            {
                return displayStep;
            }

            set
            {
                displayStep = value;
            }
        }

        [XmlElement("display-octave")]
        public string DisplayOctave
        {
            get
            {
                return displayOctave;
            }

            set
            {
                displayOctave = value;
            }
        }

        [XmlAttribute("measure")]
        public YesNoMusicXML Measure
        {
            get
            {
                return measure;
            }

            set
            {
                measure = value;
            }
        }

        [XmlIgnore]
        public bool MeasureSpecified
        {
            get
            {
                return measureSpecified;
            }

            set
            {
                measureSpecified = value;
            }
        }
    }

    [Serializable]
    [XmlType("pitch")]
    public class PitchMusicXML
    {
        private StepMusicXML step;
        private double alter;
        private bool alterSpecified;
        private string octave;

        public PitchMusicXML()
        {

        }

        [XmlElement("step")]
        public StepMusicXML Step
        {
            get
            {
                return step;
            }

            set
            {
                step = value;
            }
        }

        [XmlElement("alter")]
        public double Alter
        {
            get
            {
                return alter;
            }

            set
            {
                alter = value;
            }
        }

        [XmlIgnore]
        public bool AlterSpecified
        {
            get
            {
                return alterSpecified;
            }

            set
            {
                alterSpecified = value;
            }
        }

        [XmlElement("octave")]
        public string Octave
        {
            get
            {
                return octave;
            }

            set
            {
                octave = value;
            }
        }
    }

    [Serializable]
    [XmlType("grace")]
    public class GraceMusicXML
    {
        private double stealTimePrevious;
        private bool stealTimePreviousSpecified;
        private double stealTimeFollowing;
        private bool stealTimeFollowingSpecified;
        private double makeTime;
        private bool makeTimeSpecified;
        private YesNoMusicXML slash;
        private bool slashSpecified;

        public GraceMusicXML()
        {

        }

        [XmlAttribute("steam-time-previous")]
        public double StealTimePrevious
        {
            get
            {
                return stealTimePrevious;
            }

            set
            {
                stealTimePrevious = value;
            }
        }

        [XmlIgnore]
        public bool StealTimePreviousSpecified
        {
            get
            {
                return stealTimePreviousSpecified;
            }

            set
            {
                stealTimePreviousSpecified = value;
            }
        }

        [XmlAttribute("steal-time-following")]
        public double StealTimeFollowing
        {
            get
            {
                return stealTimeFollowing;
            }

            set
            {
                stealTimeFollowing = value;
            }
        }

        [XmlIgnore]
        public bool StealTimeFollowingSpecified
        {
            get
            {
                return stealTimeFollowingSpecified;
            }

            set
            {
                stealTimeFollowingSpecified = value;
            }
        }

        [XmlAttribute("make-time")]
        public double MakeTime
        {
            get
            {
                return makeTime;
            }

            set
            {
                makeTime = value;
            }
        }

        [XmlIgnore]
        public bool MakeTimeSpecified
        {
            get
            {
                return makeTimeSpecified;
            }

            set
            {
                makeTimeSpecified = value;
            }
        }

        [XmlAttribute("slash")]
        public YesNoMusicXML Slash
        {
            get
            {
                return slash;
            }

            set
            {
                slash = value;
            }
        }

        [XmlIgnore]
        public bool SlashSpecified
        {
            get
            {
                return slashSpecified;
            }

            set
            {
                slashSpecified = value;
            }
        }
    }

    [Serializable]
    [Flags]
    public enum NoteChoiceTypeMusicXML
    {
        none = 0,
        chord = 1,
        cue = 2,
        duration = 4,
        grace = 8,
        pitch = 16,
        rest = 32,
        tie = 64,
        unpitched = 128,
    }
}