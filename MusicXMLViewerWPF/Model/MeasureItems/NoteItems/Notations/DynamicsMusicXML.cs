using MusicXMLScore.Model.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.NoteItems.Notations
{
    [Serializable]
    [XmlType(TypeName ="dynamics")]
    public class DynamicsMusicXML
    {
        private object[] items;
        private DynamicsChoiceTypeMusicXML[] itemsElementName;
        private AboveBelowMusicXML placement;
        private bool placementSpecified;
        private string underline;
        private string overline;
        private string linethrough;
        private EnclosureShapeMusicXML enclosure;
        private bool enclosureSpecified;

        public DynamicsMusicXML()
        {

        }

        [XmlElement("f", typeof(EmptyMusicXML))]
        [XmlElement("ff", typeof(EmptyMusicXML))]
        [XmlElement("fff", typeof(EmptyMusicXML))]
        [XmlElement("ffff", typeof(EmptyMusicXML))]
        [XmlElement("fffff", typeof(EmptyMusicXML))]
        [XmlElement("ffffff", typeof(EmptyMusicXML))]
        [XmlElement("fp", typeof(EmptyMusicXML))]
        [XmlElement("fz", typeof(EmptyMusicXML))]
        [XmlElement("mf", typeof(EmptyMusicXML))]
        [XmlElement("mp", typeof(EmptyMusicXML))]
        [XmlElement("other-dynamics", typeof(string))]
        [XmlElement("p", typeof(EmptyMusicXML))]
        [XmlElement("pp", typeof(EmptyMusicXML))]
        [XmlElement("ppp", typeof(EmptyMusicXML))]
        [XmlElement("pppp", typeof(EmptyMusicXML))]
        [XmlElement("ppppp", typeof(EmptyMusicXML))]
        [XmlElement("pppppp", typeof(EmptyMusicXML))]
        [XmlElement("rf", typeof(EmptyMusicXML))]
        [XmlElement("rfz", typeof(EmptyMusicXML))]
        [XmlElement("sf", typeof(EmptyMusicXML))]
        [XmlElement("sffz", typeof(EmptyMusicXML))]
        [XmlElement("sfp", typeof(EmptyMusicXML))]
        [XmlElement("sfpp", typeof(EmptyMusicXML))]
        [XmlElement("sfz", typeof(EmptyMusicXML))]
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
        public DynamicsChoiceTypeMusicXML[] ItemsElementName
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
        [XmlAttribute("underline")]
        public string Underline
        {
            get
            {
                return underline;
            }

            set
            {
                underline = value;
            }
        }
        [XmlAttribute("overline")]
        public string Overline
        {
            get
            {
                return overline;
            }

            set
            {
                overline = value;
            }
        }
        [XmlAttribute("line-through")]
        public string Linethrough
        {
            get
            {
                return linethrough;
            }

            set
            {
                linethrough = value;
            }
        }
        [XmlAttribute("enclosure")]
        public EnclosureShapeMusicXML Enclosure
        {
            get
            {
                return enclosure;
            }

            set
            {
                enclosure = value;
            }
        }
        [XmlIgnore]
        public bool EnclosureSpecified
        {
            get
            {
                return enclosureSpecified;
            }

            set
            {
                enclosureSpecified = value;
            }
        }
    }

    [Serializable]
    [XmlType(TypeName = "enclosure-shape")]
    public enum EnclosureShapeMusicXML
    {
        rectangle,
        square,
        oval,
        circle,
        bracket,
        triangle,
        diamond,
        none,
    }

    [Serializable]
    public enum DynamicsChoiceTypeMusicXML
    {
        f,
        ff,
        fff,
        ffff,
        fffff,
        ffffff,
        fp,
        fz,
        mf,
        mp,
        [XmlEnum("other-dynamics")]
        otherdynamics,
        p,
        pp,
        ppp,
        pppp,
        ppppp,
        pppppp,
        rf,
        rfz,
        sf,
        sffz,
        sfp,
        sfpp,
        sfz,
    }
}
