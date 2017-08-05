using MusicXMLScore.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model
{
    [Serializable]
    public class CreditMusicXML
    {
        private string[] creditType;
        private BookmarkMusicXML bookmark;
        private object[] items;
        private string page;

        public CreditMusicXML()
        {

        }

        [XmlElement("credit-type", Order =0)]
        public string[] CreditType
        {
            get
            {
                return creditType;
            }

            set
            {
                creditType = value;
            }
        }

        [XmlElement("bookmark", Order =1)]
        public BookmarkMusicXML Bookmark
        {
            get
            {
                return bookmark;
            }

            set
            {
                bookmark = value;
            }
        }

        [XmlElement("bookmark", typeof(BookmarkMusicXML), Order = 2)]
        [XmlElement("credit-words", typeof(FormattedTextMusicXML), Order = 2)]
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

        [XmlAttribute("page", DataType ="positiveInteger")]
        public string Page
        {
            get
            {
                return page;
            }

            set
            {
                page = value;
            }
        }
    }

    [Serializable]
    public class BookmarkMusicXML
    {
        private string id;
        private string name;
        private string element;
        private string position;

        public BookmarkMusicXML()
        {

        }

        [XmlAttribute("id", DataType ="ID")]
        public string Id
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

        [XmlAttribute("name", DataType ="token")]
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        [XmlAttribute("element", DataType ="NMTOKEN")]
        public string Element
        {
            get
            {
                return element;
            }

            set
            {
                element = value;
            }
        }

        [XmlAttribute("position", DataType ="positiveInteger")]
        public string Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }
    }
}
