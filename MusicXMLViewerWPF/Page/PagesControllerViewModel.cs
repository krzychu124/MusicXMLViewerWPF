using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLViewerWPF;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace MusicXMLScore.Page
{
    class PagesControllerViewModel : INotifyPropertyChanged
    {
        #region Fields
        private string header;
        private MusicScore musicScore;
        private object content;
        private ObservableCollection<UIElement> pageCollection = new ObservableCollection<UIElement>();
        #endregion

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public ObservableCollection<UIElement> PageCollection { get { return pageCollection; } }
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
        }

        private void PagesControllerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public PagesControllerViewModel(MusicScore musicScore)
        {
            PropertyChanged += PagesControllerViewModel_PropertyChanged;
            MusicScore = musicScore;
        }
    }
}
