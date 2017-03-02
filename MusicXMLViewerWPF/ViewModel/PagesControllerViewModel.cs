using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLViewerWPF;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MusicXMLScore.Helpers;
using System.Xml.Serialization;
using MusicXMLScore.Model;
using System.IO;

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
        #region Fields
        private object content;
        private string header;
        private bool isBlank = true;
        private MusicScore musicScore;
        private ObservableCollection<UIElement> pageCollection;
        private string title = "";

        //!Prototypes=============================
        private string id;
        private ScorePartwiseMusicXML partwise;
        private DefaultsMusicXML defaultsCopy;
        #endregion
        //TODO_I .. PageCollection<Page>
        #region Properties
        //public object Content { get { return new object(); } private set { content = value; } }
        public string Header { get { return header; } private set { header = value; } }
        public bool IsBlank { get { return isBlank; } set { Set(nameof(IsBlank), ref isBlank, value); } }
        public MusicScore MusicScore { get { return musicScore; } private set { if (value != null) { Set(nameof(MusicScore), ref musicScore, value); } } }
        public ObservableCollection<UIElement> PagesCollection { get { return pageCollection; } set { pageCollection = value; } }
        public List<List<Part>> PagesList { get; set; }
        public string Title {  get { return title; } set { Set(nameof(Title), ref title, value); } }
        
        public string ID {  get { return id; } }
        #endregion
        public PagesControllerViewModel()
        {
            PropertyChanged += PagesControllerViewModel_PropertyChanged;
            MessengerInstance.Register<GenericMessage<List<Part>>>(this, "toNextPage", false, RelocatePartsNextPage);
        }
        public PagesControllerViewModel(MusicScore musicScore)
        {
            PropertyChanged += PagesControllerViewModel_PropertyChanged;
            MessengerInstance.Register<GenericMessage<List<Part>>>(this, "toNextPage", false, RelocatePartsNextPage);
            MessengerInstance.Register<GenericMessage<List<Part>>>(this, "toPreviousPage", false, RelocatePartsPreviousPage);
            MusicScore = musicScore;ArrangePages();
            PagesCollection = new ObservableCollection<UIElement>();
            GeneratePages();
        }
        public PagesControllerViewModel(int numberOfPages)
        {
            
            PropertyChanged += PagesControllerViewModel_PropertyChanged;
            
            IsBlank = false;
            MusicScore = new MusicScore();
            PagesCollection = new ObservableCollection<UIElement>();
            for (int i = 0; i < numberOfPages; i++)
            {
                AddPageToCollection(); //! may add current nuber to generated page
            }
        }
        
        public void AddMusicScore(MusicScore musicScore)
        {
            this.MusicScore = musicScore;
            MessengerInstance.Register<GenericMessage<List<Part>>>(this, "toNextPage", false, RelocatePartsNextPage);
            MessengerInstance.Register<GenericMessage<List<Part>>>(this, "toPreviousPage", false, RelocatePartsPreviousPage);
            ArrangePages();
            PagesCollection = new ObservableCollection<UIElement>();
            GeneratePages();
        }

        public void AddScorePartwise(ScorePartwiseMusicXML spmXML)
        {
            IsBlank = false;
            partwise = spmXML;
            DefaultsMusicXML def = partwise.Defaults;
            XmlSerializer xmls = new XmlSerializer(def.GetType());
            MemoryStream memStream = new MemoryStream();
            xmls.Serialize(memStream, def);

            memStream.Position = 0;
            xmls = new XmlSerializer(def.GetType());
            defaultsCopy = ((DefaultsMusicXML)xmls.Deserialize(memStream));
            PagesCollection = new ObservableCollection<UIElement>();
            AddPageToCollection(spmXML);
        }

        private void AddPageToCollection() //default page
        {
            PageViewModel pvm = new PageViewModel();
            PagesCollection.Add(new PageView() { DataContext = pvm });
        }

        private void AddPageToCollection(List<Part> partList, PageProperties pp) // page with given parts
        {
            PageViewModel pvm = new PageViewModel(partList) { PageWidth = pp.PageDimensions.GetPageDimensionsInPx().X, PageHeight = pp.PageDimensions.GetPageDimensionsInPx().Y };

            //PageViewModel pvm = new PageViewModel(partList) { PageWidth = MusicScore.Defaults.Page.Width, PageHeight = MusicScore.Defaults.Page.Height };
            PagesCollection.Add(new PageView() { DataContext = pvm });
        }

        private void AddPageToCollection(ScorePartwiseMusicXML sp) //default page
        {
            id = sp.ID;
            PageViewModel pvm = new PageViewModel(sp, defaultsCopy);
            PagesCollection.Add(new PageView() { DataContext = pvm });
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
                //ToDO_H generate from scratch
                CreatePages();
            }
        }

        private void CreatePages()
        {
            PagesList = MusicScore.PagesList;// throw new NotImplementedException();
        }

        private void GeneratePages()
        {
            PageProperties pp = ViewModelLocator.Instance.Main.CurrentTabLayout.PageProperties;
            if (PagesList.Count != 0)
            {
                foreach (var item in PagesList)
                {
                    AddPageToCollection(item, pp);
                }
            }
        }

        private void PagesControllerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MusicScore))
            {
                if(MusicScore != null)
                {
                    IsBlank = false;
                }
            }
        }
        private void RelocatePartsNextPage(GenericMessage<List<Part>> partListToRelocate)
        {
            Console.WriteLine("relocated to next page");
            //MessengerInstance.Unregister(this);
        }
        private void RelocatePartsPreviousPage(GenericMessage<List<Part>> partListToRelocate)
        {
            Console.WriteLine("relocated to previous page");
        }
    }
}
