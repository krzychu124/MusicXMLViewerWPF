using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.NoteItems
{
    [Serializable]
    [XmlType(TypeName ="beam")]
    public class BeamMusicXML
    {
        private string number;
        private YesNoMusicXML repeater;
        private bool repeaterSpecified;
        private FanMusicXML fan;
        private bool fanSpecified;
        private string color;
        private BeamValueMusicXML value;

        [XmlAttribute("number", DataType ="positiveInteger")]
        [DefaultValue("1")]
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

        [XmlAttribute("repeater")]
        public YesNoMusicXML Repeater
        {
            get
            {
                return repeater;
            }

            set
            {
                repeater = value;
            }
        }

        [XmlIgnore]
        public bool RepeaterSpecified
        {
            get
            {
                return repeaterSpecified;
            }

            set
            {
                repeaterSpecified = value;
            }
        }

        [XmlAttribute("fan")]
        public FanMusicXML Fan
        {
            get
            {
                return fan;
            }

            set
            {
                fan = value;
            }
        }

        [XmlIgnore]
        public bool FanSpecified
        {
            get
            {
                return fanSpecified;
            }

            set
            {
                fanSpecified = value;
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

        public BeamMusicXML()
        {
            this.Number = "1";
        }
    }

    [Serializable]
    public enum FanMusicXML
    {
        accel,
        rit,
        none,
    }
}
