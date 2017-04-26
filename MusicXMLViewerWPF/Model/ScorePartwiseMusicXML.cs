using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MusicXMLScore.Model;

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
        private List<CreditMusicXML> credits = new List<CreditMusicXML>(); //! Done
        private PartListMusicXML partlist;
        private List<ScorePartwisePartMusicXML> part;
        private string version;
        private bool layoutInfoInsideScore = false;
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
        public List<CreditMusicXML> Credits
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

        [XmlIgnore]
        public bool LayoutInsideScore
        {
            get
            {
                return layoutInfoInsideScore;
            }

            set
            {
                layoutInfoInsideScore = value;
            }
        }

        public ScorePartwiseMusicXML()
        {
            this.version = "1.0";
            ID = Misc.RandomGenerator.GetRandomHexNumber();
        }
        public void InitPartsDictionaries()
        {
            if (Part.Count == 1 && part.ElementAt(0).Id == null)
            {
                part.ElementAt(0).Id = Partlist.ScoreParts.ElementAt(0).PartId;
            }
            foreach (var part in Part)
            {
                part.SetMeasuresDictionary();
            }
            SearchForPrintElementsSupport();
        }
        /// <summary>
        /// If Version is lower than 3.0, whole layout system should be calculated and generated
        /// </summary>
        private void SearchForPrintElementsSupport()
        {
            //if (this.Version == "3.0")
            //{
            //    layoutInfoInsideScore = true; // else false, missing new-system, new-page attributes, forces manual layout calculations
            //}
            if (Identification?.Encoding != null)
            {
                var printLayoutSupport = Identification.Encoding.ItemsElementName.Any(x => x == MusicXMLScore.Model.Identification.EncodingChoiceType.supports);
                if (printLayoutSupport)
                {
                    var printSupports = Identification.Encoding.Items.Select(x => x).Where(x => x is MusicXMLScore.Model.Identification.SupportsMusicXML);
                    List<MusicXMLScore.Model.Identification.SupportsMusicXML> supprorts = new List<MusicXMLScore.Model.Identification.SupportsMusicXML>();
                    foreach (var item in printSupports)
                    {
                        supprorts.Add(item as MusicXMLScore.Model.Identification.SupportsMusicXML);
                    }
                    var layoutSupports = supprorts.Where(x => x.Element == "print");
                    if (layoutSupports.Count() != 0)
                    {
                        layoutInfoInsideScore = true;
                    }
                }
            }
            else
            {
                layoutInfoInsideScore = false;
            }
        }
    }
}
