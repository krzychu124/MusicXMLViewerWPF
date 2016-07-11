using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{   /// <summary>
    /// Main class, storage for everything which needs to be drawn
    /// </summary>
    class MusicScore 
    {
        #region static variables
        protected static CanvasList Surface;
        protected static string title;
        protected static Defaults.Defaults defaults; 
        protected static Dictionary<string, ScoreParts.Part.Part> parts = new Dictionary<string, ScoreParts.Part.Part>() { };
        protected static Identification.Identification identification; // TODO_L tests
        protected static List<Credit.Credit> credits = new List<Credit.Credit>();
        protected static List<PartList> musicscoreparts;// = new List<PartList>(); // TODO_L replaced for tests
        protected static Work.Work work; 
        protected static XElement file; // <<Loaded XML file>>
        protected static bool loaded = false;
        #endregion

        #region helpers
        private static List<Misc.LineBreak> breaks = new List<Misc.LineBreak>();

        public static List<Misc.LineBreak> LineBreaks { get { return breaks; } }
        #endregion

        #region public static properties (read-only)
        public static string Title { get { return title; } }
        public static Defaults.Defaults Defaults { get { return defaults; } }
        public static Dictionary<string, ScoreParts.Part.Part> Parts { get { return parts; } }
        public static Identification.Identification Identification { get { return identification; } }
        public static List<Credit.Credit> CreditList { get { return credits; } }
        public static List<PartList> ScoreParts { get { return musicscoreparts; } }
        public static Work.Work Work { get { return work; } }
        public static XElement File { get { return file; } }
        public static bool isLoaded { get { return loaded; } }
        #endregion

        public MusicScore(XDocument x)
        {
            file = x.Element("score-partwise");
            if (file != null)
            {
                Logger.Log("File Loaded");
                LoadToClasses();
                loaded = true;
            }
            else
            {
                Logger.Log("Error while loading file");
            }

            
        }

        private void LoadToClasses()
        {
            title = file.Element("movement-title") != null ? file.Element("movement-title").Value : "No title" ;
            work = file.Element("work") != null ? new Work.Work(file.Element("work")) : null;
            defaults = new Defaults.Defaults(file.Element("defaults")); 
            identification = new Identification.Identification(file.Element("identification")); 
            foreach (var item in file.Elements("credit"))
            {
                credits.Add(new Credit.Credit(item));
            }
            foreach (var item in file.Elements("part"))
            {
                 parts.Add(item.Attribute("id").Value, new ScoreParts.Part.Part(item));
            }
        }
        
        public static void GetSurfce (CanvasList s)
        {
            Surface = s;
        }

        public static void Draw(CanvasList surface)
        {
            DrawingVisual credits = new DrawingVisual();
            DrawCredits(credits);
            surface.AddVisual(credits);
            // DrawingVisual visual = new DrawingVisual();
            Parts.ElementAt(0).Value.DrawMeasures(surface);

            
        }

        public static void DrawCredits(DrawingVisual visual)
        {
            foreach (var item in credits)
            {
                DrawingVisual credit = new DrawingVisual();
                item.Draw(credit);
                visual.Children.Add(credit);
            }
        }

        public static void Clear()
        {
            loaded = false;
            title = null;
            defaults = null;
            parts.Clear();
            identification = null;
            credits.Clear();
            breaks.Clear();
            work = null;
            file = null;
            MusicXMLViewerWPF.Defaults.Appearance.Clear();
        }

        public static void AddBreak(float x, float y, string type)
        {
            breaks.Add(new Misc.LineBreak(x, y, type));
        }

        public static void DrawPageRectangle(DrawingVisual visual)
        {
            Misc.DrawingHelpers.DrawRectangle(visual, new Point(0, 0), new Point(Defaults.Page.Width, Defaults.Page.Height));
        }

        public static void DrawMusicScoreMargins(DrawingVisual visual)
        {
            Point right_down_margin_corner = new Point(Defaults.Page.Width - Defaults.Page.Margins.Right, Defaults.Page.Height - Defaults.Page.Margins.Bottom);
            Misc.DrawingHelpers.DrawRectangle(visual, new Point(Defaults.Page.Margins.Left, Defaults.Page.Margins.Top), right_down_margin_corner, Brushes.Blue);
        }

        public static void DrawBreaks(DrawingVisual visual) // drawing break for debugging: line, page, etc.
        {
            foreach (var item in breaks)
            {
                item.DrawBreak(visual);
            }
        }

        public static void DrawMusicScoreTitleSpace(DrawingVisual visual)
        {
            float space_height = 0f;
            foreach (var item in CreditList)
            {
                if (item.Type == Credit.CreditType.title)
                {
                    space_height += 50f;
                }
                if (item.Type == Credit.CreditType.subtitle)
                {
                    space_height += 50f;
                }
                if (item.Type == Credit.CreditType.arranger)
                {
                    space_height += 50f;
                }
            }
            if (space_height != 0)
            {
                Point left_up = new Point(Defaults.Page.Margins.Left, Defaults.Page.Margins.Top);
                //Point right_down = new Point(CreditList.Where(i => i.Type == Credit.CreditType.arranger).Select( i => i.CreditWords.DefX).First(), CreditList.Where(i => i.Type == Credit.CreditType.arranger).Select(i => i.CreditWords.DefY).First());
                
                Point right_down = new Point(Defaults.Page.Width - Defaults.Page.Margins.Right, Defaults.Page.Margins.Top + space_height);
                Misc.DrawingHelpers.DrawRectangle(visual, left_up, right_down, Brushes.Green);
                AddBreak((float)right_down.X + 20f, (float)right_down.Y, "title");
            }
        }
    }
}
