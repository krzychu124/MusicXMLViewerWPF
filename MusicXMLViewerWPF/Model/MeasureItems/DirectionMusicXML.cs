using MusicXMLScore.Model.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using MusicXMLScore.Model.MeasureItems.Directions;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems
{
    [Serializable]
    [XmlType(TypeName ="direction")]
    public class DirectionMusicXML
    {
        private List<DirectionTypeMusicXML> directionType;
        private OffsetMusicXML offset;
        private FormattedTextMusicXML footnote;
        private LevelMusicXML level;
        private string voice;
        private string staff;
        private SoundMusicXML sound;
        private AboveBelowMusicXML placement;
        private bool placementSpecified;
        private YesNoMusicXML directive;
        private bool directiveSpecified;

        [XmlElement("direction-type")]
        public List<DirectionTypeMusicXML> DirectionType
        {
            get
            {
                return directionType;
            }

            set
            {
                directionType = value;
            }
        }
        [XmlElement("offset")]
        public OffsetMusicXML Offset
        {
            get
            {
                return offset;
            }

            set
            {
                offset = value;
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
        [XmlElement("sound")]
        public SoundMusicXML Sound
        {
            get
            {
                return sound;
            }

            set
            {
                sound = value;
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
        [XmlAttribute("directive")]
        public YesNoMusicXML Directive
        {
            get
            {
                return directive;
            }

            set
            {
                directive = value;
            }
        }
        [XmlIgnore]
        public bool DirectiveSpecified
        {
            get
            {
                return directiveSpecified;
            }

            set
            {
                directiveSpecified = value;
            }
        }
    }

    [Serializable]
    [XmlType(TypeName = "direction-type")]
    public class DirectionTypeMusicXML
    {
        private object[] items;
        private DirectionChoiceTypeMusicXML[] itemsElementName;

        public DirectionTypeMusicXML()
        {

        }

        [XmlElement("accordion-registration", typeof(AccordionRegistrationMusicXML))] //! no-implementation
        [XmlElement("bracket", typeof(BracketMusicXML))] //! no-implementation
        [XmlElement("coda", typeof(EmptyPrintStyleAlignMusicXML))] //? test
        [XmlElement("damp", typeof(EmptyPrintStyleAlignMusicXML))] //! no-implementation
        [XmlElement("damp-all", typeof(EmptyPrintStyleAlignMusicXML))] //! no-implementation
        [XmlElement("dashes", typeof(DashesMusicXML))] //! no-implementation
        [XmlElement("dynamics", typeof(NoteItems.Notations.DynamicsMusicXML))] //? test
        [XmlElement("eyeglasses", typeof(EmptyPrintStyleAlignMusicXML))] //? test
        [XmlElement("harp-pedals", typeof(HarpPedalsMusicXML))] //! no-implementation
        [XmlElement("image", typeof(ImageMusicXML))] //! no-implementation
        [XmlElement("metronome", typeof(MetronomeMusicXML))]
        [XmlElement("octave-shift", typeof(OctaveShiftMusicXML))]
        [XmlElement("other-direction", typeof(OtherDirectionMusicXML))] //! no-implementation
        [XmlElement("pedal", typeof(PedalMusicXML))] //! no-implementation
        [XmlElement("percussion", typeof(PercussionMusicXML))] //! no-implementation
        [XmlElement("principal-voice", typeof(PrincipalVoiceMusicXML))] //! no-implementation
        [XmlElement("rehearsal", typeof(FormattedTextMusicXML))]  //? test
        [XmlElement("scordatura", typeof(ScordaturaMusicXML))] //! no-implementation
        [XmlElement("segno", typeof(EmptyPrintStyleAlignMusicXML))]  //? test
        [XmlElement("string-mute", typeof(StringMuteMusicXML))] //! no-implementation
        [XmlElement("wedge", typeof(WedgeMusicXML))]
        [XmlElement("words", typeof(FormattedTextMusicXML))]  //? test
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
        public DirectionChoiceTypeMusicXML[] ItemsElementName
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
    }


    [Serializable]
    [XmlType(TypeName ="ItemsElementName")]
    public enum DirectionChoiceTypeMusicXML
    {
        [XmlEnum("accordion-registration")]
        accordionregistration,
        bracket,
        coda,
        damp,
        [XmlEnum("damp-all")]
        dampall,
        dashes,
        dynamics,
        eyeglasses,
        [XmlEnum("harp-pedals")]
        harppedals,
        image,
        metronome,
        [XmlEnum("octave-shift")]
        octaveshift,
        [XmlEnum("other-direction")]
        otherdirection,
        pedal,
        percussion,
        [XmlEnum("principal-voice")]
        principalvoice,
        rehearsal,
        scordatura,
        segno,
        [XmlEnum("string-mute")]
        stringmute,
        wedge,
        words,
    }
}