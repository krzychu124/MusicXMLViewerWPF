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
    /// Highly depending with choosen View style eg. Normal, OneLine, Paginated, OneInstrument etc.
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
        }

        private void PagesControllerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public PagesControllerViewModel(MusicScore musicScore)
        {
            PropertyChanged += PagesControllerViewModel_PropertyChanged;
            MusicScore = musicScore;
            PagesCollection = new ObservableCollection<UIElement>();
            AddPageToCollection();
        }

        private void AddPageToCollection()
        {
            PageViewModel pvm = new PageViewModel();
            PagesCollection.Add(new PageView() { DataContext = pvm });
        }
    }
}
