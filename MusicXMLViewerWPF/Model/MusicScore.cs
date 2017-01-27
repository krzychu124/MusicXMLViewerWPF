using GalaSoft.MvvmLight;
using MusicXMLScore.Helpers;
using MusicXMLScore.Log;
using MusicXMLViewerWPF.ScoreParts.MeasureContent;
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
    class MusicScore : ObservableObject
    {
        #region Fields
        private string title;
        private Defaults.Defaults defaults; //? = new Defaults.Defaults(); ??
        private Dictionary<string, Part> parts = new Dictionary<string, Part>() { };
        private Identification.Identification identification;
        private List<Credit.Credit> credits = new List<Credit.Credit>();
        private Work.Work work;
        private XElement file; // <<Loaded XML file>>
        private static bool loaded = false;
        private static bool credits_loaded = false;
        private static bool content_space_calculated = false;
        private static bool supports_new_system = false;
        private static bool supports_new_page = false;
        #endregion
        #region properties with Notification
        public bool Loaded
        {
            get { return loaded; }
            set
            {
                Set(() => Loaded, ref loaded, value);
            }
        }
        public bool CreditsLoaded
        {
            get { return credits_loaded; }
            set
            {
                Set(() => CreditsLoaded, ref credits_loaded, value);
            }
        }
        public bool ContentSpaceCalculated
        {
            get { return content_space_calculated; }
            set
            {
                Set(() => ContentSpaceCalculated, ref content_space_calculated, value);
            }
        }
        public bool SupportNewSystem
        {
            get { return supports_new_system; }
            set
            {
                Set(() => SupportNewSystem, ref supports_new_system, value);
            }
        }
        public bool SupportNewPage
        {
            get { return supports_new_page; }
            set
            {
                Set(() => SupportNewPage, ref supports_new_page, value);
            }
        }
        #endregion

        #region public static properties
        public string Title { get { return title; } set { if (value != null) { title = value; } } } //todo inpc
        public Defaults.Defaults Defaults { get { return defaults; } set { if (value != null) { defaults = value; } } } //todo inpc
        public Dictionary<string, Part> Parts { get { return parts; } set { if (value != null) { parts = value; } } } //todo inpc 
        public Identification.Identification Identification { get { return identification; } set { if (value != null) { identification = value; } } }  //todo inpc 
        public List<Credit.Credit> CreditList { get { return credits; } set { if (value != null) { credits = value; } } }  //todo inpc
        public Work.Work Work { get { return work; } set { if (value != null) { work = value; } } } //todo inpc
        public XElement File { get { return file; } set { if (value != null) { file = value; } } } //todo inpc
        #endregion

        #region ctors.
        public MusicScore()
        {
            this.PropertyChanged += MusicScore_PropertyChanged;
        }
        public MusicScore(XDocument x)
        {
            this.PropertyChanged += MusicScore_PropertyChanged;

            File = x.Element("score-partwise");
            if (file != null) //TODO refactor needed => before pass x here, check compatibility
            {
                LoggIt.Log("File Loaded");
                ImportFromXMLFile();
                Loaded = true;
            }
            else
            {
                LoggIt.Log("Error while loading file", LogType.Error);
            }
        }
        #endregion
        /// <summary>
        /// PopertiesChanged switch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MusicScore_PropertyChanged(object sender, PropertyChangedEventArgs e) //TODO refactor/remove
        {
            switch (e.PropertyName)
            {
                case "CreditsLoaded":
                    if (CreditsLoaded != false)
                    {
                        // Defaults.Page.CalculateMeasureContetSpace();
                        Logger.Log("Credits: ready");
                    }
                    else
                    {
                        Logger.Log("Credits: cleared");
                    }
                    break;
                case "Loaded":
                    if (Loaded == true)
                    {
                        Logger.Log("File: ready.");
                    }
                    else
                    {
                        Logger.Log("File: unloaded.");
                    }
                    break;
                case "ContentSpaceCalculated":
                    if (ContentSpaceCalculated == true)
                    {
                        Logger.Log("Calculated Content Space: ready");
                    }

                    else
                    {
                        Logger.Log("Cleared Content Space: not_ready");
                    }
                    break;
                case "SupportNewSystem":
                    if (SupportNewSystem == true)
                    {
                        Logger.Log("\"NewSystem\" feature Supported");
                    }
                    else
                    {
                        Logger.Log("\"NewSystem\" feature NotSupported");
                    }
                    break;
                case "SupportNewPage":
                    if (SupportNewPage == true)
                    {
                        Logger.Log("\"NewPage\" feature Supported");
                    }
                    else
                    {
                        Logger.Log("\"NewPage\" feature NotSupported");
                    }
                    break;
                default:
                    Console.WriteLine($"Not implemented action for {e.PropertyName} property ");
                    break;
            }
        }

        private void ImportFromXMLFile()
        {
            Title = file.Element("movement-title") != null ? file.Element("movement-title").Value : "No title";
            Work = file.Element("work") != null ? new Work.Work(file.Element("work")) : null;
            Defaults = file.Element("defaults") != null ? new Defaults.Defaults(file.Element("defaults"), this) : new Defaults.Defaults();
            Identification = new Identification.Identification(file.Element("identification"));
            if (Identification?.Encoding?.Supports != null) //todo refactor to inpc
            {
                foreach (var item in Identification.Encoding.Supports)
                {
                    switch (item.Attribute)
                    {
                        case "new-system":
                            SupportNewSystem = item.Value;
                            break;
                        case "new-page":
                            SupportNewPage = item.Value;
                            break;
                        default:
                            break;
                    }
                }
            }
            foreach (var item in file.Elements("credit"))
            {
                CreditList.Add(new Credit.Credit(item, Defaults));
            }
            //// Credit.Credit.SetCreditSegment();  refactored
            //! Refactoring InitTitleSpaceSegment(); 

            foreach (var item in file.Elements("part"))
            {
                Parts.Add(item.Attribute("id").Value, new Part(item));
            }
            //if (Parts.Count > 1) //! needs  refactor due to changed drawing system
            //{
            //    RecalculateMeasuresPosInParts();
            //}

        }
        //private void RecalculateMeasuresPosInParts() //TODO_L improve part drawing// better but still no bugless
        //{
        //    float staffDistance = 80f;
        //    float staveDistance = 80f;

        //    int count = Parts.Count;
        //    float clc_stave = 0f;
        //    if (count != 1)
        //    {
        //        clc_stave = staveDistance + (staffDistance * count - 1);
        //    }

        //    for (int i = 0; i < Parts.Count; i++)
        //    {

        //        float tempY = 0;
        //        int segmentCount = Parts.Values.ElementAt(i).MeasureSegmentList.Count;
        //        int firstinline_count = 0;
        //        for (int j = 0; j< segmentCount; j++)
        //        {
        //            Measure segment = Parts.Values.ElementAt(i).MeasureSegmentList.ElementAt(j);
        //            if (j == 0)
        //            {
        //                tempY = staffDistance * (i) + staveDistance * (firstinline_count);
        //                segment.Relative_y += tempY;
        //                segment.Calculated_y += tempY;
        //                firstinline_count++;
        //                //tempY = clc_stave;
        //            }
        //            else
        //            {
        //                if (segment.IsFirstInLine)
        //                {
        //                    tempY = staffDistance * (i) + staveDistance * (firstinline_count);
        //                    segment.Relative_y += tempY;
        //                    segment.Calculated_y += tempY;
        //                    firstinline_count++;
        //                }
        //                else
        //                {
        //                    segment.Relative_y += tempY;
        //                    segment.Calculated_y += tempY;
        //                }
        //            }
        //        }
        //    }
    }

    //public Dictionary<string, Part> GetParts()
    //{
    //    return parts;
    //}

    //public void Draw(CanvasL surface)
    //{
    //    //DrawingVisual credits = new DrawingVisual();
    //    //DrawCredits(credits);
    //    //surface.AddVisual(credits);
    //    // DrawingVisual visual = new DrawingVisual();
    //    Parts.ElementAt(0).Value.DrawMeasures(surface); //! test


    //}

    //public void DrawCredits(DrawingVisual visual)
    //{
    //    foreach (var item in credits)
    //    {
    //        DrawingVisual credit = new DrawingVisual();
    //        item.Draw(credit);
    //        visual.Children.Add(credit);
    //    }
    //}

    //public static void Clear()
    //{
    //    Misc.ScoreSystem.Clear();
    //    MusicScore clear = new MusicScore();
    //    clear.Loaded = false;
    //    clear.CreditsLoaded = false;
    //    clear.ContentSpaceCalculated = false;
    //    credits_loaded = false;
    //    MusicXMLViewerWPF.Defaults.Appearance.Clear();
    //}
    ///// <summary>
    ///// Fill neccesary properties to properly draw Title/Credits segment
    ///// </summary>
    //private void InitTitleSpaceSegment()
    //{
    //    float space_height = 0f;
    //    foreach (var item in CreditList)
    //    {
    //        if (item.Type == Credit.CreditType.title)
    //        {
    //            space_height += item.Height;
    //        }
    //        if (item.Type == Credit.CreditType.subtitle)
    //        {
    //            space_height += item.Height;
    //        }
    //        if (item.Type == Credit.CreditType.arranger)
    //        {
    //            space_height += item.Height;
    //        }
    //    }
    //    Credit.Credit.Titlesegment.Height = (float)Credit.Credit.Titlesegment.Relative.Y + space_height;
    //    MusicScore n = new MusicScore() { CreditsLoaded = true };
    //}

    #region Visual Helpers for easier debugging
    //public static void DrawPageRectangle(DrawingVisual visual)
    //{
    //    //Misc.DrawingHelpers.DrawRectangle(visual, new Point(0, 0), new Point(Defaults.Page.Width, Defaults.Page.Height));
    //}

    //public static void DrawMusicScoreMargins(DrawingVisual visual)
    //{
    //    //Point right_down_margin_corner = new Point(Defaults.Page.Width - Defaults.Page.Margins.Right, Defaults.Page.Height - Defaults.Page.Margins.Bottom);
    //    //Misc.DrawingHelpers.DrawRectangle(visual, new Point(Defaults.Page.Margins.Left, Defaults.Page.Margins.Top), right_down_margin_corner, Brushes.Blue);
    //}
    //public static void DrawMusicScoreMeasuresContentSpace(DrawingVisual visual)
    //{
    //    //Misc.DrawingHelpers.DrawRectangle(visual, Defaults.Page.MeasuresContentSpace, Brushes.Red);
    //}
    //public static void DrawMusicScoreTitleSpace(DrawingVisual visual)
    //{
    //    //InitTitleSpaceSegment();
    //    if (Credit.Credit.Titlesegment.Height != 0)
    //    {
    //        //? var items = from i in CreditList where i.Type == Credit.CreditType.title || i.Type == Credit.CreditType.subtitle || i.Type == Credit.CreditType.arranger || i.Type == Credit.CreditType.composer select i;
    //        //if (items.Contains(Credit.CreditType.title))
    //        //? var title_ = items.Where(z => z.Type == Credit.CreditType.title);

    //        //foreach (var item in CreditList)
    //        //{
    //        //    if (item.Type == Credit.CreditType.title)
    //        //    {
    //        //        DrawingVisual title = new DrawingVisual();
    //        //        item.Draw(title);
    //        //        visual.Children.Add(title);
    //        //        //? DrawingVisual rect = new DrawingVisual();
    //        //        //? item.Draw(rect, Brushes.Cyan, dashtype: DashStyles.Dot);
    //        //        //? visual.Children.Add(rect);
    //        //    }
    //        //    if (item.Type == Credit.CreditType.subtitle)
    //        //    {
    //        //        DrawingVisual subtitle = new DrawingVisual();
    //        //        item.Draw(subtitle);
    //        //        visual.Children.Add(subtitle);
    //        //        //? DrawingVisual rect = new DrawingVisual();
    //        //        //? item.Draw(rect, Brushes.Cyan, dashtype: DashStyles.Dot);
    //        //        //? visual.Children.Add(rect);
    //        //    }
    //        //    if (item.Type == Credit.CreditType.arranger)
    //        //    {
    //        //        DrawingVisual arranger = new DrawingVisual();
    //        //        item.Draw(arranger);
    //        //        visual.Children.Add(arranger);
    //        //        //? DrawingVisual rect = new DrawingVisual();
    //        //        //? item.Draw(rect, Brushes.Cyan, dashtype: DashStyles.Dot);
    //        //        //? visual.Children.Add(rect);
    //        //    }

    //        //}
    //        //Point left_up = new Point(Defaults.Page.Margins.Left, Defaults.Page.Margins.Top);
    //        //Point right_down = new Point(CreditList.Where(i => i.Type == Credit.CreditType.arranger).Select( i => i.CreditWords.DefX).First(), CreditList.Where(i => i.Type == Credit.CreditType.arranger).Select(i => i.CreditWords.DefY).First());
    //        // Misc.DrawingHelpers.DrawRectangle(visual, left_up, right_down, Brushes.Green);
    //        //Point right_down = new Point(Defaults.Page.Width - Defaults.Page.Margins.Right, Defaults.Page.Margins.Top + space_height);
    //        Point left_up = Credit.Credit.Titlesegment.Relative;
    //        Point right_down = Credit.Credit.Titlesegment.Rectangle.BottomRight;//Relative.Y + Credit.Credit.segment.Dimensions.Y);
    //        Credit.Credit.Titlesegment.Draw(visual, Brushes.Green);
    //        Misc.DrawingHelpers.DrawRectangle(visual, Credit.Credit.Titlesegment.Rectangle.TopLeft, Credit.Credit.Titlesegment.Rectangle.BottomRight, Brushes.Crimson);
    //    }

    //}
    #endregion
}