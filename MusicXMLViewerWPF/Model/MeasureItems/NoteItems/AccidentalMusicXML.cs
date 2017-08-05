using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems.NoteItems
{
    [Serializable]
    [XmlType(TypeName ="accidental")]
    public class AccidentalMusicXML
    {
        private YesNoMusicXML cautionary;
        private bool cautionarySpecified;
        private YesNoMusicXML editorial;
        private bool editorialSpecified;
        private YesNoMusicXML parentheses;
        private bool parenthesesSpecified;
        private YesNoMusicXML bracket;
        private bool bracketSpecified;
        private SymbolSizeMusicXML size;
        private bool sizeSpecified;
        private AccidentalValueMusicXML value;

        [XmlAttribute("cautionary")]
        public YesNoMusicXML Cautionary
        {
            get
            {
                return cautionary;
            }

            set
            {
                cautionary = value;
            }
        }

        [XmlIgnore]
        public bool CautionarySpecified
        {
            get
            {
                return cautionarySpecified;
            }

            set
            {
                cautionarySpecified = value;
            }
        }

        [XmlAttribute("editorial")]
        public YesNoMusicXML Editorial
        {
            get
            {
                return editorial;
            }

            set
            {
                editorial = value;
            }
        }

        [XmlIgnore]
        public bool EditorialSpecified
        {
            get
            {
                return editorialSpecified;
            }

            set
            {
                editorialSpecified = value;
            }
        }

        [XmlAttribute("parentheses")]
        public YesNoMusicXML Parentheses
        {
            get
            {
                return parentheses;
            }

            set
            {
                parentheses = value;
            }
        }

        [XmlIgnore]
        public bool ParenthesesSpecified
        {
            get
            {
                return parenthesesSpecified;
            }

            set
            {
                parenthesesSpecified = value;
            }
        }

        [XmlAttribute("bracket")]
        public YesNoMusicXML Bracket
        {
            get
            {
                return bracket;
            }

            set
            {
                bracket = value;
            }
        }

        [XmlIgnore]
        public bool BracketSpecified
        {
            get
            {
                return bracketSpecified;
            }

            set
            {
                bracketSpecified = value;
            }
        }

        [XmlAttribute("size")]
        public SymbolSizeMusicXML Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }

        [XmlIgnore]
        public bool SizeSpecified
        {
            get
            {
                return sizeSpecified;
            }

            set
            {
                sizeSpecified = value;
            }
        }

        [XmlText]
        public AccidentalValueMusicXML Value
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
