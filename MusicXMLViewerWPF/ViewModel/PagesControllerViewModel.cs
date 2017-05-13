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
using MusicXMLScore.LayoutControl;

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
        
        private string header;
        private string id;

        private bool isBlank = true;
        private ObservableCollection<UIElement> pageCollection;
        private ScorePartwiseMusicXML partwise;
        private string title = "";
        private AdvancedMeasureLayout advancedLayout;
        #endregion Fields

        #region Constructors

        public PagesControllerViewModel()
        {
        }
        
        public PagesControllerViewModel(int numberOfPages)
        {
            IsBlank = false;
            PagesCollection = new ObservableCollection<UIElement>();
            for (int i = 0; i < numberOfPages; i++)
            {
                AddPageToCollection(); //! may add current nuber to generated page
            }
        }

        #endregion Constructors

        #region Properties

        public string Header { get { return header; } private set { header = value; } }
        public string ID { get { return id; } }
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

        #endregion Properties

        #region Methods
        

        public void AddScorePartwise(ScorePartwiseMusicXML spmXML)
        {
            IsBlank = false;
            partwise = spmXML;
            PagesCollection = new ObservableCollection<UIElement>();
            DrawingHelpers.PartProperties pp = ViewModelLocator.Instance.Main.CurrentPartsProperties[spmXML.Part.ElementAt(0).Id];
            bool autoLayoutSupport = ViewModelLocator.Instance.Main.CurrentScoreProperties.AutoLayoutSupportByScore;
            //autoLayoutSupport = false;
            if (autoLayoutSupport)
            {
                foreach (var pages in pp.PartSysemsInPages)
                {
                    AddPageToCollection(spmXML);
                }
            }
            else
            {
                advancedLayout = new AdvancedMeasureLayout(partwise);
                //advancedLayout.AddBlankPage();
                advancedLayout.GenerateMeasureSegments();
                advancedLayout.FindOptimalMeasureWidths();
                PagesCollection = advancedLayout.PagesCollection;
            }
        }

        private void AddPageToCollection() //default page
        {
            PageViewModel pvm = new PageViewModel();
            PagesCollection.Add(new PageView() { DataContext = pvm });
        }
        

        private void AddPageToCollection(ScorePartwiseMusicXML sp) //default page
        {
            id = sp.ID;
            int index = pageCollection.Count;
            PageViewModel pvm = new PageViewModel(sp, index);
            PagesCollection.Add(new PageView() { DataContext = pvm });
        }
        
        private void PagesControllerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }
        #endregion Methods
    }
}
