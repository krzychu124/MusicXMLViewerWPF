using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.Attributes
{
    [Serializable]
    [XmlType("key")]
    [DebuggerDisplay("{Items[0]} {number}")]
    public class KeyMusicXML
    {
        private object[] items;
        private KeyChoiceTypes[] itemsElementName;
        private List<KeyOctaveMusicXML> keyOctave;
        private string number;
        private YesNoMusicXML printObject;
        private bool printObjectSpecified;

        [XmlElement("cancel", typeof(CancelMusicXML))]
        [XmlElement("fifths", typeof(string), DataType = "integer")]
        [XmlElement("key-accidental", typeof(AccidentalValueMusicXML))]
        [XmlElement("key-alter", typeof(int))]// decimal
        [XmlElement("key-step", typeof(StepMusicXML))]
        [XmlElement("mode", typeof(string))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get { return items; }
            set
            {
                items = value;
            }
        }
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public KeyChoiceTypes[] ItemsElementName
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
        [XmlElement("key-octave")]
        public List<KeyOctaveMusicXML> KeyOctave
        {
            get
            {
                return keyOctave;
            }

            set
            {
                keyOctave = value;
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
        public KeyMusicXML Clone()
        {
            KeyMusicXML new_key = new KeyMusicXML()
            {
                Items = new List<object>(Items).ToArray(), //TODO_ dunno if work as intended
                ItemsElementName = ItemsElementName,
                KeyOctave = KeyOctave,
                Number = Number,
                PrintObject = PrintObject,
                PrintObjectSpecified = PrintObjectSpecified
            };
            return new_key;
        }
    }
    [Serializable]
    [XmlType(TypeName ="cancel")]
    public class CancelMusicXML
    {
        private CancelLocationMusicXML location;
        private bool locationSpecified;
        private string value;
        [XmlAttribute("location")]
        public CancelLocationMusicXML Location
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
        [XmlIgnore]
        public bool LocationSpecified
        {
            get
            {
                return locationSpecified;
            }

            set
            {
                locationSpecified = value;
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
    public enum KeyChoiceTypes
    {
        cancel,
        fifths,
        [XmlEnum("key-accidental")]
        keyAccidental,
        [XmlEnum("key-alter")]
        keyAlter,
        [XmlEnum("key-step")]
        keyStep,
        mode
    }

    [Serializable]
    [XmlType("key-octave")]
    public class KeyOctaveMusicXML
    {
        private string number;
        private YesNoMusicXML cancel;
        private bool cancelSpecified;
        private string value;

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
        [XmlAttribute("cancel")]
        public YesNoMusicXML Cancel
        {
            get
            {
                return cancel;
            }

            set
            {
                cancel = value;
            }
        }
        [XmlIgnore]
        public bool CancelSpecified
        {
            get
            {
                return cancelSpecified;
            }

            set
            {
                cancelSpecified = value;
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