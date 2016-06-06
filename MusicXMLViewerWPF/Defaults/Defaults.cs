using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicXMLViewerWPF.Defaults
{
    class Defaults //TODO place for instacnes of parent class folder;
    {
        private Scale scale;
        private Page page;
        private SystemLayout system_layout;
        private StaffLayout staff_layout;
        private Appearance appearance;
        private List<ScoreFonts> fonts = new List<ScoreFonts>();
        
        public Scale Scale { get { return scale; } }
        public Page Page { get { return page; } }
        public SystemLayout SystemLayout { get { return system_layout; } }    
        public StaffLayout StaffLayout { get { return staff_layout; } }
        public Appearance Appearance { get { return appearance; } }
        public List<ScoreFonts> Fonts { get { return fonts; } }
        public Defaults(System.Xml.Linq.XElement x)
        {
            var temp = x;
            scale = new Scale(x.Element("scaling"));
            page = new Page(x.Element("page-layout"));
            system_layout = new SystemLayout(x.Element("system-layout"));
            staff_layout = x.Element("staff-layout") != null ? new StaffLayout(x.Element("staff-layout")) : null;
            appearance = new Appearance(x.Element("appearance"));
            if (x.Element("music-font") != null)
            {
                fonts.Add(new ScoreFonts(x.Element("music-font")));
            }
            if (x.Element("word-font") != null)
            {
                fonts.Add(new ScoreFonts(x.Element("word-font")));
            }
            if (x.Element("lyric-font") != null)
            {
                fonts.Add(new ScoreFonts(x.Element("lyric-font")));
            }
        }
    }

    internal class ScoreFonts
    {
        private FontFamily font_family;
        private float font_size;

        public FontFamily Font { get { return font_family; } }
        public float Size { get { return font_size; } }

        public ScoreFonts(System.Xml.Linq.XElement x)
        {
            font_family = new FontFamily(x.Attribute("font-family").Value);
            font_size = float.Parse(x.Attribute("font-size").Value, CultureInfo.InvariantCulture);
        }
    }
}
