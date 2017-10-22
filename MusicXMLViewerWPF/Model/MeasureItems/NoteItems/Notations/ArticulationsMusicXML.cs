using MusicXMLScore.Model.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.NoteItems.Notations
{
    [Serializable]
    [XmlType(TypeName ="articulations")]
    public class ArticulationsMusicXML
    {
        private object[] items;
        private ArticulationsChoiceTypeMusicXML[] itemsElementName;

        public ArticulationsMusicXML()
        {

        }

        [XmlElement("accent", typeof(EmptyPlacementMusicXML))]
        [XmlElement("breath-mark", typeof(BreathMarkMusicXML))]
        [XmlElement("caesura", typeof(EmptyPlacementMusicXML))]
        [XmlElement("detached-legato", typeof(EmptyPlacementMusicXML))]
        [XmlElement("doit", typeof(EmptyLineMusicXML))]
        [XmlElement("falloff", typeof(EmptyLineMusicXML))]
        [XmlElement("other-articulation", typeof(PlacementTextMusicXML))]
        [XmlElement("plop", typeof(EmptyLineMusicXML))]
        [XmlElement("scoop", typeof(EmptyLineMusicXML))]
        [XmlElement("spiccato", typeof(EmptyPlacementMusicXML))]
        [XmlElement("staccatissimo", typeof(EmptyPlacementMusicXML))]
        [XmlElement("staccato", typeof(EmptyPlacementMusicXML))]
        [XmlElement("stress", typeof(EmptyPlacementMusicXML))]
        [XmlElement("strong-accent", typeof(StrongAccentMusicXML))]
        [XmlElement("tenuto", typeof(EmptyPlacementMusicXML))]
        [XmlElement("unstress", typeof(EmptyPlacementMusicXML))]
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
        public ArticulationsChoiceTypeMusicXML[] ItemsElementName
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
    [XmlType(TypeName = "breath-mark")]
    public class BreathMarkMusicXML
    {
        private AboveBelowMusicXML placement;
        private bool placementSpecified;
        private BreathMarkValue value;

        public BreathMarkMusicXML()
        {

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

        [XmlText]
        public BreathMarkValue Value
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
    [XmlType(TypeName = "breath-mark-value")]
    public enum BreathMarkValue
    {
        [XmlEnum("")]
        Item,
        comma,
        tick,
    }

    [Serializable]
    public enum ArticulationsChoiceTypeMusicXML
    {
        accent,
        [XmlEnum("breath-mark")]
        breathmark,
        caesura,
        [XmlEnum("detached-legato")]
        detachedlegato,
        doit,
        falloff,
        [XmlEnum("other-articulation")]
        otherarticulation,
        plop,
        scoop,
        spiccato,
        staccatissimo,
        staccato,
        stress,
        [XmlEnum("strong-accent")]
        strongaccent,
        tenuto,
        unstress,
    }

    [Serializable]
    [XmlType(TypeName = "strong-accent")]
    public class StrongAccentMusicXML : EmptyPlacementMusicXML
    {
        private UpDownMusicXML type;

        public StrongAccentMusicXML()
        {
            this.type = UpDownMusicXML.up;
        }

        /// <uwagi/>
        [XmlAttribute()]
        [System.ComponentModel.DefaultValue(UpDownMusicXML.up)]
        public UpDownMusicXML Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
    }
}
