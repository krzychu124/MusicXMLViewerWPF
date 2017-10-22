using System;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.LayoutControl;
using MusicXMLViewerWPF;
using MusicXMLScore.Helpers;
using MusicXMLScore.Model.Factories;
using System.Windows.Controls;
using MusicXMLScore.View;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;

namespace MusicXMLScore.ViewModel
{
    /// <summary>
    /// Controlls loaded content and divide it into parts("Pages") if it won't fit inside one "Page"
    /// Highly dependent to choosen View-style eg. Normal, OneLine, Paginated, OneInstrument etc.
    /// </summary>
    /// PagesController --- Page.1 --:
    ///                              | PartsSegment.1 (Group of Parts) 
    ///                              :
    ///                                                 | Part.1(instrument) ==> Measures[0;10] Collection // 0:10 example range
    ///                                                 :
    ///                                                 | Part.2(instrument) ==> Measures[0;10] Collection
    ///                                                 :
    ///                            --:
    ///                              | PartsSegment.2
    ///                              :
    ///                                                 | Part.1(instrument) ==> Measures[10;21] Collection // 10:21 example range but continuous according to previous PartsSegment
    ///                                                 :
    ///                                                 | Part.2(instrument) ==> Measures[10;21]  Collection
    ///                                                 :
    /// 
    ///                 --- Page.2 --:
    ///                 |||          | PartsSegment.3
    ///                 |||
    ///                 --- Page.n
    class PagesControllerViewModel : ViewModelBase
    {
        private string header;
        private string id;

        private bool isBlank = true;
        private ObservableCollection<UIElement> pageCollection;
        private ScorePartwiseMusicXML partwise;
        private string title = "";
        private AdvancedMeasureLayout advancedLayout;
        public RelayCommand AddPageCommand { get; set; }
        public ContextMenu ContextMenu { get; set; }

        private readonly List<AdvancedPageViewModel> pagesVM = new List<AdvancedPageViewModel>();
        private int currentMeasureNumber = 0;
        private MeasureSegmentContainer measureSegmentContainer;
        public PagesControllerViewModel()
        {
            PagesCollection = new ObservableCollection<UIElement>();
            AddPageCommand = new RelayCommand(OnAddPageCommand);
            var cMenu = new ContextMenu();
            var item = new MenuItem() { Header = "Add New Page", Command = AddPageCommand };
            cMenu.Items.Add(item);
            ContextMenu = cMenu;
        }

        public PagesControllerViewModel(string test)
        {
            PagesCollection = new ObservableCollection<UIElement>();
            AddPageCommand = new RelayCommand(OnAddEmptyPageCommand);
            var cMenu = new ContextMenu();
            var item = new MenuItem() { Header = "Add New Page", Command = AddPageCommand };
            cMenu.Items.Add(item);
            ContextMenu = cMenu;
        }
        public PagesControllerViewModel(ScorePartwiseMusicXML score)
        {
            partwise = score;
            PagesCollection = new ObservableCollection<UIElement>();
            AddPageCommand = new RelayCommand(AddAdvancedEmptyPage);
            var cMenu = new ContextMenu();
            var item = new MenuItem() { Header = "Add New Page", Command = AddPageCommand };
            cMenu.Items.Add(item);
            ContextMenu = cMenu;
            measureSegmentContainer = new MeasureSegmentContainer();
            measureSegmentContainer.GenerateMeasureSegments(score);

        }

        public void OnAddPageCommand()
        {
            AddScorePage(AdvancedLayoutTestFactory.GetScorePage2());
            Console.WriteLine("Page added");
        }

        private void OnAddEmptyPageCommand()
        {
            AddEmptyPage();
        }

        private void OnAddNextMeasure()
        {
            if(currentMeasureNumber < measureSegmentContainer["P1"].Count - 1)
            {
                pagesVM[0].AddNextMeasure(measureSegmentContainer["P1"][currentMeasureNumber++]);
            }
        }

        private void AddAdvancedEmptyPage()
        {
            if (pageCollection.Count > 0)
            {
                var pageVM = new AdvancedPageViewModel(1);
                pagesVM.Add(pageVM);
                var page = pageCollection[pageCollection.Count - 1] as AdvancedPageView;
                if (page != null)
                {
                    var previousPageVM = page.DataContext as AdvancedPageViewModel;
                    if (previousPageVM != null)
                    {
                        previousPageVM.CurrentPanel.NextPanel = pageVM.CurrentPanel;
                    }
                }
                pageCollection.Add(new AdvancedPageView { DataContext = pageVM });
            }
            else
            {
                var pageVM = new AdvancedPageViewModel(0);
                pagesVM.Add(pageVM);
                var cMenu = new ContextMenu();
                var item = new MenuItem() { Header = "Add Next Measure", Command = new RelayCommand(OnAddNextMeasure) };
                var item2 = new MenuItem() { Header = "Add All Measures", Command = new RelayCommand(OnAddAllMeasures) };
                cMenu.Items.Add(item);
                cMenu.Items.Add(item2);
                pageVM.ContextMenu = cMenu;
                pageCollection.Add(new AdvancedPageView { DataContext = pageVM });
            }
        }

        private void OnAddAllMeasures()
        {
            if (currentMeasureNumber < measureSegmentContainer["P1"].Count - 1)
            {
                var list = measureSegmentContainer["P1"].Skip(currentMeasureNumber).ToList();
                pagesVM[0].AddAllNextMeasures(list);
                currentMeasureNumber = measureSegmentContainer["P1"].Count;
            }
        }

        private void AddEmptyPage()
        {
            if (pageCollection.Count > 0)
            {
                var pageVM = new AdvancedPageViewModel("empty");
                var page = pageCollection[pageCollection.Count - 1] as AdvancedPageView;
                if (page != null)
                {
                    var previousPageVM = page.DataContext as AdvancedPageViewModel;
                    if (previousPageVM != null)
                    {
                        previousPageVM.CurrentPanel.NextPanel = pageVM.CurrentPanel;
                    }
                }
                pageCollection.Add(new AdvancedPageView { DataContext = pageVM });
            }
            else
            {
                var pageVM = new AdvancedPageViewModel("test");
                pageCollection.Add(new AdvancedPageView { DataContext = pageVM });
            }
        }

        public PagesControllerViewModel(int numberOfPages,int asd)
        {
            IsBlank = false;
            PagesCollection = new ObservableCollection<UIElement>();
            for (int i = 0; i < numberOfPages; i++)
            {
                AddPageToCollection(); //! may add current nuber to generated page
            }
        }

        public string Header { get { return header; } private set { header = value; } }
        public string ID { get { return Partwise?.ID; } }
        public bool IsBlank { get { return isBlank; } set { Set(nameof(IsBlank), ref isBlank, value); } }
        public ObservableCollection<UIElement> PagesCollection { get { return pageCollection; } set { pageCollection = value; } }
        public ScorePartwiseMusicXML Partwise
        {
            get
            {
                return partwise;
            }

            set
            {
                partwise = value;
            }
        }

        public string Title { get { return title; } set { Set(nameof(Title), ref title, value); } }


        public void AddScorePartwise(ScorePartwiseMusicXML scorePartXML)
        {
            if (scorePartXML == null)
            {
                throw new NullReferenceException("null scorePart object"); //! temp
            }
            IsBlank = false;
            partwise = scorePartXML;
            PagesCollection = new ObservableCollection<UIElement>();
            PartProperties pp = ViewModelLocator.Instance.Main.PartsProperties[scorePartXML.Part[0].Id];
            bool autoLayoutSupport = ViewModelLocator.Instance.Main.CurrentScoreProperties.AutoLayoutSupportByScore;
            //autoLayoutSupport = false;
            if (autoLayoutSupport)
            {
                foreach (var pages in pp.PartSysemsInPages)
                {
                    AddPageToCollection(scorePartXML);
                }
            }
            else
            {
                advancedLayout = new AdvancedMeasureLayout(partwise);
                PagesCollection = advancedLayout.PagesCollection;
            }
        }

        private void AddPageToCollection() //default page
        {
            PageViewModel pvm = new PageViewModel();
            PagesCollection.Add(new PageView { DataContext = pvm });
        }

        private void AddPageToCollection(ScorePartwiseMusicXML sp) //default page
        {
            id = sp.ID;
            int index = pageCollection.Count;
            PageViewModel pvm = new PageViewModel(sp, index);
            PagesCollection.Add(new PageView { DataContext = pvm });
        }

        public void AddScorePage(ScoreLayout.AbstractScorePage scorePage)
        {
            IsBlank = false;
            id = scorePage.Id;
            var number = PagesCollection.Count + 1;
            var pvm = new PageViewModel(scorePage) { PageNumber = number + "", PageNumberAlignment = number % 2 == 0 ? TextAlignment.Right : TextAlignment.Left };
            PagesCollection.Add(new PageView { DataContext = pvm });
        }
    }
}
