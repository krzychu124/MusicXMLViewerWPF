using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicXMLViewerWPF.Defaults
{
    class Defaults 
    {
        private MusicScore musicscore;
        private Scale scale = new Scale();
        private Page page;
        private SystemLayout system_layout;
        private StaffLayout staff_layout;
        private Appearance appearance;
        private Dictionary<string,ScoreFonts> fonts = new Dictionary<string,ScoreFonts>();
        
        public Scale Scale { get { return scale; } set { scale = value; } }
        public Page Page { get { return page; } }
        public SystemLayout SystemLayout { get { return system_layout; } }    
        public StaffLayout StaffLayout { get { return staff_layout; } }
        public Appearance Appearance { get { return appearance; } }
        public Dictionary<string,ScoreFonts> Fonts { get { return fonts; } }

        public Defaults(System.Xml.Linq.XElement x, MusicScore ms)
        {
            musicscore = ms;
            var temp = x;
            scale = new Scale(x.Element("scaling"));
            page = new Page(x);
            system_layout = x.Element("system-layout") != null ? new SystemLayout(x.Element("system-layout")) : new SystemLayout();
            staff_layout = x.Element("staff-layout") != null ? new StaffLayout(x.Element("staff-layout")) : null;
            appearance = x.Element("appearance") != null ? new Appearance(x.Element("appearance")) : new Appearance();
            if (x.Element("music-font") != null)
            {
                fonts.Add("m",new ScoreFonts(x.Element("music-font")));
            }
            if (x.Element("word-font") != null)
            {
                fonts.Add("w",new ScoreFonts(x.Element("word-font")));
            }
            if (x.Element("lyric-font") != null)
            {
                fonts.Add("l",new ScoreFonts(x.Element("lyric-font")));
            }
        }
        public Defaults()
        {
            scale = new Scale();
            page = new Page();
            system_layout = new SystemLayout();
            staff_layout = null;
            appearance = new Appearance();
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
