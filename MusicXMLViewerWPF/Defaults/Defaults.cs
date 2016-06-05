using System;
using System.Collections.Generic;
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
            
        public Defaults(System.Xml.Linq.XElement x)
        {
            scale = new Scale(x.Element("scale"));
            page = new Page(x.Element("page-layout"));
            system_layout = new SystemLayout(x.Element("system-layout"));
            staff_layout = new StaffLayout(x.Element("staff-layout"));
            appearance = new Appearance(x.Element("aprearance"));
            fonts.Add(new ScoreFonts(x.Element("music-font")));
            fonts.Add(new ScoreFonts(x.Element("word-font")));
            fonts.Add(new ScoreFonts(x.Element("lyric-font")));
        }
    }

    internal class ScoreFonts
    {
        private FontFamily font_family;
        private int font_size;

        public FontFamily Font { get { return font_family; } }
        public int Size { get { return font_size; } }

        public ScoreFonts(System.Xml.Linq.XElement x)
        {
            font_family = new FontFamily(x.Attribute("font-family").Value);
            font_size = int.Parse(x.Attribute("font-size").Value);
        }
    }
}
