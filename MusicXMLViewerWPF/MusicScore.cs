using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    class MusicScore : INotifyPropertyChanged
    {
        #region static variables
        protected static CanvasList Surface;
        protected static string title;
        protected static Defaults.Defaults defaults; 
        protected static Dictionary<string, ScoreParts.Part.Part> parts = new Dictionary<string, ScoreParts.Part.Part>() { };
        protected static Identification.Identification identification; 
        protected static List<Credit.Credit> credits = new List<Credit.Credit>();
        protected static List<PartList> musicscoreparts; //= new List<PartList>(); // TODO_L replaced for tests
        protected static Work.Work work; 
        protected static XElement file; // <<Loaded XML file>>
        protected static bool loaded = false;
        protected static bool credits_loaded = false;
        protected static bool content_space_calculated = false;
        #endregion


        #region helpers
        public event PropertyChangedEventHandler PropertyChanged;
        private static List<Misc.LineBreak> breaks = new List<Misc.LineBreak>();
        //private static List<>
        public static List<Misc.LineBreak> LineBreaks { get { return breaks; } }
        public bool Loaded
        {
            get { return loaded; }
            set
            {
                loaded = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Loaded)));
            }
        }
        public bool CreditsLoaded
        {
            get { return credits_loaded; }
            set
            {
                credits_loaded = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CreditsLoaded)));
            }
        }
        public bool ContentSpaceCalculated
        {
            get { return content_space_calculated; }
            set
            {
                content_space_calculated = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ContentSpaceCalculated)));
            }
        }
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

        public MusicScore()
        {
            this.PropertyChanged += MusicScore_PropertyChanged;
        }
        public MusicScore(XDocument x)
        {
            this.PropertyChanged += MusicScore_PropertyChanged;
            file = x.Element("score-partwise");
            if (file != null)
            {
                Logger.Log("File Loaded");
                LoadToClasses();
                Loaded = true;
            }
            else
            {
                Logger.Log("Error while loading file");
            }

            
        }

        private void MusicScore_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CreditsLoaded": if (CreditsLoaded != false)
                    {
                        Defaults.Page.CalculateMeasureContetSpace();
                        Logger.Log("Calculated Content Space for measures");
                    }
                    break;
                case "Loaded": if (Loaded == true)
                    {
                        Logger.Log("File loaded.");
                    }
                    else
                    {
                        Logger.Log("File cleared.");
                    }
                    break;
                case "ContentSpaceLoaded":
                    Logger.Log("Calculated Content Space");
                    break;
                default:
                    Console.WriteLine($"Not implemented task for {e.PropertyName} property ");
                    break;
            }
        }

        private void LoadToClasses()
        {
            title = file.Element("movement-title") != null ? file.Element("movement-title").Value : "No title" ;
            work = file.Element("work") != null ? new Work.Work(file.Element("work")) : null;
            defaults = file.Element("defaults") != null ? new Defaults.Defaults(file.Element("defaults")) : new MusicXMLViewerWPF.Defaults.Defaults(); 
            identification = new Identification.Identification(file.Element("identification")); 
            foreach (var item in file.Elements("credit"))
            {
                credits.Add(new Credit.Credit(item));
            }
            Credit.Credit.SetCreditSegment();
            
            foreach (var item in file.Elements("part"))
            {
                 parts.Add(item.Attribute("id").Value, new ScoreParts.Part.Part(item));
            }
            CreditsLoaded = false;
            CreditsLoaded = true;
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
            MusicScore clear = new MusicScore();
            clear.Loaded = false;
            title = null;
            defaults = null;
            parts.Clear();
            identification = null;
            credits.Clear();
            breaks.Clear();
            work = null;
            file = null;
            credits_loaded = false;
            MusicXMLViewerWPF.Defaults.Appearance.Clear();
        }

        public static void AddBreak(float x, float y, string type)
        {
            breaks.Add(new Misc.LineBreak(x, y, type));
        }
        #region Visual Helpers for easier debugging
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
        /// <summary>
        /// Fill neccesary properties to properly draw Title/Credits segment
        /// </summary>
        private static void InitTitleSpace()
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
            Credit.Credit.segment.Height = (float)Defaults.Page.ContentSpace.Y + space_height;
            MusicScore n = new MusicScore() { CreditsLoaded = true };
        }
        public static void DrawMusicScoreContentSpace(DrawingVisual visual)
        {
            Misc.DrawingHelpers.DrawRectangle(visual, Defaults.Page.MeasuresContentSpace, Brushes.Red);
        }
        public static void DrawMusicScoreTitleSpace(DrawingVisual visual)
        {
            InitTitleSpace();
            if (Credit.Credit.segment.Height != 0)
            {
                //Point left_up = new Point(Defaults.Page.Margins.Left, Defaults.Page.Margins.Top);
                //Point right_down = new Point(CreditList.Where(i => i.Type == Credit.CreditType.arranger).Select( i => i.CreditWords.DefX).First(), CreditList.Where(i => i.Type == Credit.CreditType.arranger).Select(i => i.CreditWords.DefY).First());
                // Misc.DrawingHelpers.DrawRectangle(visual, left_up, right_down, Brushes.Green);
                //Point right_down = new Point(Defaults.Page.Width - Defaults.Page.Margins.Right, Defaults.Page.Margins.Top + space_height);
                Point left_up = Credit.Credit.segment.Relative;
                Point right_down = new Point(Credit.Credit.segment.Rectangle.Left, Credit.Credit.segment.Rectangle.Height);//Relative.Y + Credit.Credit.segment.Dimensions.Y);
                Credit.Credit.segment.Draw(visual, Brushes.Green);
                //Misc.DrawingHelpers.DrawRectangle(visual, Credit.Credit.segment.Rectangle.TopLeft, Credit.Credit.segment.Rectangle.BottomRight, Brushes.Green);
                AddBreak((float)right_down.X + 20f, (float)right_down.Y, "title");
            }

        }
        #endregion
    }
}
