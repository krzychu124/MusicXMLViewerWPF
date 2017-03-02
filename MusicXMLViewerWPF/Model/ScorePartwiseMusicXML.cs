using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MusicXMLViewerWPF;
using System.Xml.Serialization;
using MusicXMLScore.Model;
using MusicXMLViewerWPF.Identification;
using MusicXMLViewerWPF.Defaults;

namespace MusicXMLViewerWPF
{
    [Serializable()]
    [XmlRoot("score-partwise", Namespace ="", IsNullable =false)]
    public class ScorePartwiseMusicXML
    {
        private string id;
        private WorkMusicXML work; //! Done
        private string movementNumber; //! Done
        private string movementTitle; //! Done
        private IdentificationMusicXML identification;
        private DefaultsMusicXML defaults;
        private List<Credit> credits = new List<Credit>(); //! Done
        private PartListMusicXML partlist;
        private List<ScorePartwisePartMusicXML> part;
        private string version;

        [XmlIgnore]
        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        
        [XmlElement(elementName:"work")]
        public WorkMusicXML Work
        {
            get
            {
                return work;
            }

            set
            {
                work = value;
            }
        }
        [XmlElement("movement-number", typeof(string))]
        public string MovementNumber
        {
            get
            {
                return movementNumber;
            }

            set
            {
                movementNumber = value;
            }
        }

        [XmlElement("movement-title", typeof(string))]
        public string MovementTitle
        {
            get
            {
                return movementTitle;
            }

            set
            {
                movementTitle = value;
            }
        }
        [XmlElement("credit")]
        public List<Credit> Credits
        {
            get { return credits; }
            set
            {
                credits = value;
            }
        }
        [XmlAttribute("version")]
        [System.ComponentModel.DefaultValue("1.0")]
        public string Version
        {
            get
            {
                return version;
            }

            set
            {
                version = value;
            }
        }
        //[XmlIgnore]
        [XmlElement("part-list")]
        public PartListMusicXML Partlist
        {
            get
            {
                return partlist;
            }

            set
            {
                partlist = value;
            }
        }
        [XmlElement("identification")]
        public IdentificationMusicXML Identification
        {
            get
            {
                return identification;
            }

            set
            {
                identification = value;
            }
        }
        [XmlElement("defaults")]
        public DefaultsMusicXML Defaults
        {
            get
            {
                return defaults;
            }

            set
            {
                defaults = value;
            }
        }
        [XmlElement("part")]
        public List<ScorePartwisePartMusicXML> Part
        {
            get
            {
                return part;
            }

            set
            {
                part = value;
            }
        }

        public ScorePartwiseMusicXML()
        {
            this.version = "1.0";
            ID = Misc.RandomGenerator.GetRandomHexNumber();
        }
    }
}
