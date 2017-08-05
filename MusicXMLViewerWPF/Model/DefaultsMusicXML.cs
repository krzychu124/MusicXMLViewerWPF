using MusicXMLScore.Model.Defaults;
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
    public class DefaultsMusicXML //TODO_H lyricfont[] lyriclanguage
    {
        private ScalingMusicXML scaling;
        private PageLayoutMusicXML pageLayout;
        private SystemLayoutMusicXML systemLayout;
        private List<StaffLayoutMusicXML> staffLayout;
        private AppearanceMusicXML appearance;
        private EmptyFontMusicXML musicFont;
        private EmptyFontMusicXML wordFont;

        [XmlElement("scaling")]
        public ScalingMusicXML Scaling
        {
            get
            {
                return scaling;
            }

            set
            {
                scaling = value;
            }
        }

        [XmlElement("page-layout")]
        public PageLayoutMusicXML PageLayout
        {
            get
            {
                return pageLayout;
            }

            set
            {
                pageLayout = value;
            }
        }

        [XmlElement("system-layout")]
        public SystemLayoutMusicXML SystemLayout
        {
            get
            {
                return systemLayout;
            }

            set
            {
                systemLayout = value;
            }
        }

        [XmlElement("music-font")]
        public EmptyFontMusicXML MusicFont
        {
            get
            {
                return musicFont;
            }

            set
            {
                musicFont = value;
            }
        }

        [XmlElement("word-font")]
        public EmptyFontMusicXML WordFont
        {
            get
            {
                return wordFont;
            }

            set
            {
                wordFont = value;
            }
        }

        [XmlElement("appearance")]
        public AppearanceMusicXML Appearance
        {
            get
            {
                return appearance;
            }

            set
            {
                appearance = value;
            }
        }

        [XmlElement("staff-layout")]
        public List<StaffLayoutMusicXML> StaffLayout
        {
            get
            {
                return staffLayout;
            }

            set
            {
                staffLayout = value;
            }
        }
    }
}
