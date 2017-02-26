using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Helpers
{
    [Serializable]
    [XmlType(TypeName ="offset")]
    public class OffsetMusicXML
    {
        private YesNoMusicXML sound;
        private bool soundSpecified;
        private double value;


        [XmlAttribute("sound")]
        public YesNoMusicXML Sound
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
        [XmlIgnore]
        public bool SoundSpecified
        {
            get
            {
                return soundSpecified;
            }

            set
            {
                soundSpecified = value;
            }
        }
        [XmlText]
        public double Value
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
