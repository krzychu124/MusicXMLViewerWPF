using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Xml.Serialization;
using MusicXMLScore.ScoreProperties;

namespace MusicXMLScore.Model.MeasureItems.Attributes
{
    [Serializable]
    [XmlType(TypeName = "time")]
    public class TimeMusicXML : IMeasureAttribute
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

        [XmlAttribute("number", DataType = "positiveInteger")]
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

        [XmlAttribute("symbol")]
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
                printObjectSpecified = true;
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

        public TimeMusicXML Clone()
        {
            TimeMusicXML new_time = new TimeMusicXML()
            {
                Items = Items,
                ItemsElementName = itemsElementName,
                Number = number,
                TimeSymbol = TimeSymbol,
                TimeSymbolSpecified = TimeSymbolSpecified,
                Separator = Separator,
                SeparatorSpecified = SeparatorSpecified,
                PrintObject = PrintObject,
                PrintObjectSpecified = PrintObjectSpecified
            };
            return new_time;
        }

        public int GetNumerator()
        {
            return GetTimeValueOfType(TimeChoiceTypeMusicXML.beats);
        }

        public int GetDenominator()
        {
            return GetTimeValueOfType(TimeChoiceTypeMusicXML.beattype);
        }

        public int GetTimeValueOfType(TimeChoiceTypeMusicXML type)
        {
            int value = 4;
            if (TimeSymbolSpecified)
            {
                if (TimeSymbol == TimeSymbolMusicXML.common)
                {
                    value = 4;
                }
                if (TimeSymbol == TimeSymbolMusicXML.cut)
                {
                    value = 2;
                }
            }
            else
            {
                string stringValue = (string)GetItemOfType(type);
                if (type == TimeChoiceTypeMusicXML.beats)
                {
                    if (stringValue.Contains("+"))
                    {
                        var array = stringValue.Split('+');
                        foreach (var item in array)
                        {
                            value += int.Parse(item);
                        }
                    }
                    else
                    {
                        value = int.Parse(stringValue);
                    }
                }
                else
                {
                    value = int.Parse(stringValue);
                }
            }
            return value;
        }

        private object GetItemOfType(TimeChoiceTypeMusicXML type)
        {
            int index = -1;
            for(int i =0; i< ItemsElementName.Length; i++)
            {
                if (ItemsElementName[i] == type)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
            {
                throw new Exception($"TimeMusicXML ItemsElementName: missing selected type {type.ToString()} in array");
            }
            return Items[index];
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