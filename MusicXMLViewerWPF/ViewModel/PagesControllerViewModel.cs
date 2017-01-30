using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLViewerWPF;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

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
    class PagesControllerViewModel : INotifyPropertyChanged
    {
        #region Fields
        private string header;
        private MusicScore musicScore;
        private object content;
        private ObservableCollection<UIElement> pageCollection;
        #endregion
        //TODO_I .. PageCollection<Page>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public ObservableCollection<UIElement> PagesCollection { get { return pageCollection; } set { pageCollection = value; } }
        public List<List<Part>> PagesList { get; set; }
        #region Properties
        public string Header {  get { return header; } private set { header = value; } }
        public object Content {  get { return new object(); } private set { content = value; } }
        public MusicScore MusicScore { get { return musicScore; } private set { if (value != null) { musicScore = value; PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(MusicScore))); } } }
        public string Title {  get { return MusicScore.Title != null ? MusicScore.Title : "no title :/"; } }
        #endregion
        public PagesControllerViewModel()
        {
            PropertyChanged += PagesControllerViewModel_PropertyChanged;
            MusicScore = new MusicScore();
            //TODO generate simple music score using template(eg. one instr. 10-15 measures)
            PagesCollection = new ObservableCollection<UIElement>();
            AddPageToCollection();
            AddPageToCollection();
            AddPageToCollection();
            AddPageToCollection();
            AddPageToCollection();
        }

        private void PagesControllerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public PagesControllerViewModel(MusicScore musicScore)
        {
            PropertyChanged += PagesControllerViewModel_PropertyChanged;
            MusicScore = musicScore;
            ArrangePages();
            PagesCollection = new ObservableCollection<UIElement>();
            foreach (var item in PagesList)
            {
                AddPageToCollection(item);
            }
        }
        public PagesControllerViewModel(int numberOfPages)
        {
            PropertyChanged += PagesControllerViewModel_PropertyChanged;
            MusicScore = new MusicScore();
            PagesCollection = new ObservableCollection<UIElement>();
            for (int i = 0; i < numberOfPages; i++)
            {
                AddPageToCollection(); //! may add current nuber to generated page
            }
        }
        /// <summary>
        /// Generate Pages using PartList from loaded MusicScore (if MusicScore not support NewPageSystem - generate from scratch)
        /// </summary>
        private void ArrangePages()
        {
            PagesList = new List<List<Part>>();
            if (MusicScore.SupportNewPage)
            {
                foreach (var item in MusicScore.PagesList)
                {
                    List<Part> partslist = new List<Part>();
                    foreach (var part in item)
                    {
                        partslist.Add(part);
                    }
                    PagesList.Add(partslist);
                }
            }
            else
            {
                //ToDO generate from scratch
                GeneratePages();
            }
        }

        private void GeneratePages()
        {
            throw new NotImplementedException();
        }

        private void AddPageToCollection()
        {
            PageViewModel pvm = new PageViewModel();
            PagesCollection.Add(new PageView() { DataContext = pvm });
        }
        private void AddPageToCollection(List<Part> partList)
        {

            PageViewModel pvm = new PageViewModel(partList);
            PagesCollection.Add(new PageView() { DataContext = pvm });
        }
    }
}
