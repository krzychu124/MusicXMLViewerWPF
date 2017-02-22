using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.Attributes
{
    [Serializable]
    [XmlType(TypeName ="time")]
    public class TimeMusicXML
    {
        private object[] items;
        private TimeChoiceTypeMusicXML[] itemsElementName;
        private string number;
        private TimeSymbolMusicXML timeSymbol;
        private bool timeSymbolSpecified;
        private TimeSeparatorMusicXML separator;
        private bool separatorSpecified;
        private YesNoMusicXML printObject;
        private bool printObjectSpecified;

        public TimeMusicXML()
        {

        }

        [XmlElement("beat-type", typeof(string))]
        [XmlElement("beats", typeof(string))]
        [XmlElement("interchangeable", typeof(InterchangeableMusicXML))]
        [XmlElement("senza-misura", typeof(string))]
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
        [XmlIgnore()]
        public TimeChoiceTypeMusicXML[] ItemsElementName
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
        [XmlAttribute("time-symbol")]
        public TimeSymbolMusicXML TimeSymbol
        {
            get
            {
                return timeSymbol;
            }

            set
            {
                timeSymbol = value;
            }
        }
        [XmlIgnore]
        public bool TimeSymbolSpecified
        {
            get
            {
                return timeSymbolSpecified;
            }

            set
            {
                timeSymbolSpecified = value;
            }
        }
        [XmlAttribute("separator")]
        public TimeSeparatorMusicXML Separator
        {
            get
            {
                return separator;
            }

            set
            {
                separator = value;
            }
        }
        [XmlIgnore]
        public bool SeparatorSpecified
        {
            get
            {
                return separatorSpecified;
            }

            set
            {
                separatorSpecified = value;
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
    }

    [Serializable]
    [XmlType(TypeName ="interchangeable")]
    public class InterchangeableMusicXML
    {
        private TimeRelationMusicXML timeRelation;
        private bool timeRelationSpecified;
        private string[] beats;
        private string[] beatsType;
        private TimeSymbolMusicXML symbol;
        private bool timeSymbolSpecified;
        private TimeSeparatorMusicXML separator;
        private bool separatorSpecified;

        public InterchangeableMusicXML()
        {

        }

        [XmlElement("time-relation")]
        public TimeRelationMusicXML TimeRelation
        {
            get
            {
                return timeRelation;
            }

            set
            {
                timeRelation = value;
            }
        }
        [XmlIgnore]
        public bool TimeRelationSpecified
        {
            get
            {
                return timeRelationSpecified;
            }

            set
            {
                timeRelationSpecified = value;
            }
        }
        [XmlElement("beats")]
        public string[] Beats
        {
            get
            {
                return beats;
            }

            set
            {
                beats = value;
            }
        }
        [XmlElement("beats-type")]
        public string[] BeatsType
        {
            get
            {
                return beatsType;
            }

            set
            {
                beatsType = value;
            }
        }
        [XmlAttribute("symbol")]
        public TimeSymbolMusicXML Symbol
        {
            get
            {
                return symbol;
            }

            set
            {
                symbol = value;
            }
        }
        [XmlIgnore]
        public bool TimeSymbolSpecified
        {
            get
            {
                return timeSymbolSpecified;
            }

            set
            {
                timeSymbolSpecified = value;
            }
        }
        [XmlAttribute("separator")]
        public TimeSeparatorMusicXML Separator
        {
            get
            {
                return separator;
            }

            set
            {
                separator = value;
            }
        }
        [XmlIgnore]
        public bool SeparatorSpecified
        {
            get
            {
                return separatorSpecified;
            }

            set
            {
                separatorSpecified = value;
            }
        }
    }
    [Serializable]
    [XmlType(TypeName ="time-relation")]
    public enum TimeRelationMusicXML
    {
        parentheses,
        bracket,
        equals,
        slash,
        space,
        hyphen
    }

    [Serializable]
    public enum TimeChoiceTypeMusicXML
    {
        [XmlEnum("beat-type")]
        beattype,
        beats,
        interchangeable,
        [XmlEnum("senza-misura")]
        senzamisura,
    }

    [Serializable]
    [XmlType("time-symbol")]
    public enum TimeSymbolMusicXML
    {
        common,
        cut,
        [XmlEnum("single-number")]
        singlenumber,
        note,
        [XmlEnum("dotted-note")]
        dottednote,
        normal,
    }
    [Serializable]
    [XmlType(TypeName = "time-separator")]
    public enum TimeSeparatorMusicXML
    {
        none,
        horizontal,
        diagonal,
        vertical,
        adjacent,
    }
}