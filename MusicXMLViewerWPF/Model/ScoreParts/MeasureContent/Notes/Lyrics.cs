using MusicXMLViewerWPF.ScoreParts.MeasureContent;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Lyrics // nothing here now
    {
        #region attributes
        private int number;
        private string name;
        private float default_y;
        private AboveBelow placement;
        #endregion
        private string noteid;
        private string text;
        private SyllabicType syllabic;
        private string level;
        private XElement xmldefinition;
        private static Point lyricpreviousplace;
        #region Properties
        public string NoteID { get { return noteid; } }
        public int Number { get { return number; } }
        public string Name { get { return name; } }
        public float DefaultY { get { return default_y; } }
        public AboveBelow Placement { get { return placement; } }
        public string Text { get { return text; } }
        public SyllabicType Syllabic { get { return syllabic; } }
        public string Level { get { return level; } }
        public XElement XMLDefinition { get { return xmldefinition; } }
        #endregion

        public Lyrics(XElement x, string noteid)
        {
            xmldefinition = x;
            this.noteid = noteid;
            GetAttributes(x.Attributes().ToList());
            level = string.Empty;
            foreach (var item in x.Elements())
            {
                switch (item.Name.LocalName)
                {
                    case "syllabic":
                        syllabic = GetSyllabic(item.Value);
                        break;
                    case "text":
                        text = item.Value;
                        break;
                    case "level":
                        
                        Logger.Log("");
                        break;
                    default:
                        Logger.Log($"Lyrics element.name not found {item.Name.LocalName}");
                        break;
                }
            }
        }

        private SyllabicType GetSyllabic(string s)
        {
            SyllabicType type;
            switch (s)
            {
                case "begin":
                    type = SyllabicType.begin;
                    break;
                case "middle":
                    type = SyllabicType.middle;
                    break;
                case "end":
                    type = SyllabicType.end;
                    break;
                default:
                    type = SyllabicType.single;
                    break;
            }
            return type;
        }
        private void GetAttributes(List<XAttribute> x)
        {
            foreach (var item in x)
            {
                switch (item.Name.LocalName)
                {
                    case "default-y":
                        default_y = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    case "name":
                        name = item.Value;
                        break;
                    case "number":
                        number = int.Parse(item.Value);
                        break;
                    case "placement":
                        placement = item.Value == "above" ? AboveBelow.above : AboveBelow.below;
                        break;
                    default:
                        Logger.Log($"No implementation for {item.Name.LocalName}");
                        break;
                }
            }
        }
        public void Draw(DrawingVisual visual)
        {
            Note actualnote = null;
            Segment actualmeasure = null;
            try
            {
                actualnote = (Note)Misc.ScoreSystem.GetSegment(NoteID);
                actualmeasure = Misc.ScoreSystem.GetMeasureSegment(actualnote.MeasureId);
            }
            catch(Exception e)
            {
                Logger.Log($"Draw Lyric Exception: {e.ToString()}");
            }
            if (actualnote != null && actualmeasure != null)
            {
                Point noteposition = new Point(actualnote.Relative.X + actualnote.Spacer_L, actualnote.Relative.Y);
                Point measureposition = actualmeasure.Relative;
                Point lyricposition = new Point(noteposition.X, measureposition.Y + actualmeasure.Height * 0.85f);
                Point syllabicline = new Point();
                if (Syllabic == SyllabicType.begin)
                {
                    lyricpreviousplace = lyricposition;
                }
                if (Syllabic == SyllabicType.middle)
                {
                    float temp = (float)(lyricposition.X - lyricpreviousplace.X) / 2;
                    syllabicline = new Point(lyricposition.X - temp, lyricposition.Y);
                    lyricpreviousplace = lyricposition;
                }
                if (Syllabic == SyllabicType.end)
                {
                    float temp = (float)(lyricposition.X - lyricpreviousplace.X) / 2;
                    syllabicline = new Point(lyricposition.X - temp, lyricposition.Y);
                    lyricpreviousplace = new Point();
                }
                
                DrawingVisual lyric = new DrawingVisual();
                using (DrawingContext dc = lyric.RenderOpen()) //! ignoring syllabic type //temporary//
                {
                    Misc.DrawingHelpers.DrawText(dc, this.text, lyricposition, 10f, withsub: false, align: Halign.left, valign: Valign.top);
                    if (Syllabic != SyllabicType.single && Syllabic != SyllabicType.begin)
                    {
                        Misc.DrawingHelpers.DrawText(dc, "-", syllabicline, 10f, withsub: false, align: Halign.left, valign: Valign.top);
                    }
                }
                visual.Children.Add(lyric);
            }
            else
            {
                Logger.Log("Exception occured in note/measure try block");
            }
        }
    }
    
    enum AboveBelow
    {
        below,
        above
    }
    enum SyllabicType
    {
        single,
        begin,
        end,
        middle
    }
}
