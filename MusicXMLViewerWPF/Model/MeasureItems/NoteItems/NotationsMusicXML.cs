using MusicXMLScore.Model.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using MusicXMLScore.Model.MeasureItems.NoteItems.Notations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.NoteItems
{
    [Serializable]
    [XmlType(TypeName ="notations")]
    public class NotationsMusicXML //todo_h notations
    {
        private FormattedTextMusicXML footnote;
        private LevelMusicXML level;
        private object[] items;
        private YesNoMusicXML printObject;
        private bool printObjectSpecified;

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

        [XmlElement("accidental-mark", typeof(AccidentalMarkMusicXML))]
        //[XmlElement("arpeggiate", typeof(arpeggiate))] 
        [XmlElement("articulations", typeof(ArticulationsMusicXML))]
        [XmlElement("dynamics", typeof(DynamicsMusicXML))]
        [XmlElement("fermata", typeof(FermataMusicXML))]
        [XmlElement("glissando", typeof(GlissandoMusicXML))]
        //[XmlElement("non-arpeggiate", typeof(nonarpeggiate))]
        [XmlElement("ornaments", typeof(OrnamentsMusicXML))]
        [XmlElement("other-notation", typeof(OtherNotationMusicXML))]
        [XmlElement("slide", typeof(SlideMusicXML))]
        [XmlElement("slur", typeof(SlurMusicXML))]
        //[XmlElement("technical", typeof(technical))]
        [XmlElement("tied", typeof(TiedMusicXML))]
        [XmlElement("tuplet", typeof(TupletMusicXML))]
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
}
