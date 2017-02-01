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

        #region Private Fields

        private static bool content_space_calculated = false;
        private static bool credits_loaded = false;
        private static bool loaded = false;
        private static bool supports_new_page = false;
        private static bool supports_new_system = false;
        private List<Credit.Credit> credits = new List<Credit.Credit>();
        private Defaults.Defaults defaults;
        private XElement file;
        private Identification.Identification identification;
        private List<List<Part>> llpages = new List<List<Part>>();
        private List<List<Part>> pagesList = new List<List<Part>>();
        private Dictionary<string, Part> parts = new Dictionary<string, Part>() { };
        private string title;
        private Work.Work work;

        #endregion Private Fields

        #region Public Constructors

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

        #endregion Public Constructors

        #region Public Properties

        public bool ContentSpaceCalculated
        {
            get { return content_space_calculated; }
            set
            {
                Set(() => ContentSpaceCalculated, ref content_space_calculated, value);
            }
        }

        public List<Credit.Credit> CreditList { get { return credits; } set { if (value != null) { credits = value; } } }

        public bool CreditsLoaded
        {
            get { return credits_loaded; }
            set
            {
                Set(() => CreditsLoaded, ref credits_loaded, value);
            }
        }

        public Defaults.Defaults Defaults { get { return defaults; } set { if (value != null) { defaults = value; } } }

        public XElement File { get { return file; } set { if (value != null) { file = value; } } }

        public Identification.Identification Identification { get { return identification; } set { if (value != null) { identification = value; } } }

        public bool Loaded
        {
            get { return loaded; }
            set
            {
                Set(() => Loaded, ref loaded, value);
            }
        }
        public List<List<Part>> PagesList { get { return pagesList; } private set { pagesList = value; } }

        public Dictionary<string, Part> Parts { get { return parts; } set { if (value != null) { parts = value; } } }

        public bool SupportNewPage
        {
            get { return supports_new_page; }
            set
            {
                Set(() => SupportNewPage, ref supports_new_page, value);
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
        public string Title { get { return title; } set { if (value != null) { title = value; } } }    
        public Work.Work Work { get { return work; } set { if (value != null) { work = value; } } }

        #endregion Public Properties

        #region Public Methods

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

        #endregion Public Methods

        #region Private Methods

        private void GeneratePages()
        {
            if (SupportNewPage)
            {
                int pages = 1;
                List<int> maxIndexOfPageList = new List<int>();
                // look for new page attribute to calculate number of pages
                for (var i = 0; i < Parts.ElementAt(0).Value.MeasureList.Count; i++)
                {
                    var measure = Parts.ElementAt(0).Value.MeasureList.ElementAt(i);
                    if (measure.PrintProperties != null)
                    {
                        if (measure.PrintProperties.NewPage)
                        {
                            pages += 1;
                            maxIndexOfPageList.Add(i);
                        }
                    }
                }
                /// fill last page with max index
                if (maxIndexOfPageList.LastOrDefault() != Parts.ElementAt(0).Value.MeasureList.Count)
                {
                    maxIndexOfPageList.Add(Parts.ElementAt(0).Value.MeasureList.Count);
                }
                int index = 0;
                foreach (var currentPageLastItemIndex in maxIndexOfPageList)
                {
                    List<Part> listOfParts = new List<Part>();
                    foreach (var currentPart in Parts) // loop through part to get all measures in range <index, currentPageMaxIndex>
                    {
                        Part currentTempPart = new Part(currentPart.Key);
                        for (var i = index; i < currentPageLastItemIndex; i++)
                        {
                            currentTempPart.AddMeasure(currentPart.Value.MeasureList.ElementAt(i));
                        }
                        List<Part> partWithID = new List<Part>();
                        //partWithID.Add();
                        listOfParts.Add(currentTempPart);
                    }
                    PagesList.Add(listOfParts);
                    index = currentPageLastItemIndex; // set index to indexOflastItem to skip them in next pass ( if any items left)
                }
            }
            else
            {// temporary 
                PagesList.Add(Parts.Values.ToList());
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
            GeneratePages();
        }

        #endregion Private Methods
    }
}