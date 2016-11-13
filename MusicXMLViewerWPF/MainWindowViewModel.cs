using MusicXMLScore.Helpers;
using MusicXMLViewerWPF;
using SimpleLogger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLScore
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            PropertyChanged += MainWindowViewModel_PropertyChanged;
            //SimpleLog.SetLogFile(".\\Log", "MyLog_", writeText:true);
            SimpleLog.Info("Test logging started.");
            OpenOptionsWindowCommand = new RelayCommand(OnOpenOptionWindow);
            ExitCommand = new RelayCommand(OnExitApp);
            NewScoreCreatorCommand = new RelayCommand(OnNewScoreCreator);
            AddMeasureCommand = new RelayCommand(OnAddMeasure);
            CustomButtonCommand = new RelayCommand(OnCustomButtonCommand);
            OpenFileCommand = new RelayCommand(OnOpenFileCommand);
            OldViewCommand = new RelayCommand(OnOldViewCommand);
            CloseFileCommand = new RelayCommand(OnCloseFile, () => XmlFileLoaded);
        }

        #region PropertyChanged
        private void MainWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "XmlFileLoaded")
            {
                CloseFileCommand.RiseCanExecuteChanged();
                Mediator.NotifyColleagues("IsFileLoaded", XmlFileLoaded);
            }
        }
        #endregion

        #region Methods
        private void OnNewScoreCreator()
        {
            Configuration.NewScoreCreatorView nscreatorwindow = new Configuration.NewScoreCreatorView();
            nscreatorwindow.ShowDialog();
        }

        private void OnOpenOptionWindow()
        {
            Configuration.ConfigurationView optionswindow = new Configuration.ConfigurationView();
            optionswindow.ShowDialog();
        }

        private void OnAddMeasure()
        {
            Page.PageView page = new Page.PageView();
            page.Width = w;
            page.Height = h;
            Pages.Add(page);
        }
        
        private void OnCustomButtonCommand()
        {
            if (Orientation == Orientation.Vertical)
                Orientation = Orientation.Horizontal;
            else
                Orientation = Orientation.Vertical;
        }

        private void OnOpenFileCommand()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "MusicXML files|*.xml";
            if (dialog.ShowDialog() == true)
            {
                XmlRead xmlReader = new XmlRead();
                xmlReader.File_path = dialog.FileName;
                Console.WriteLine("Loading file>> " + dialog.FileName);
                Log.LoggIt.Log($"Loading file: {xmlReader.File_path}", Log.LogType.Info);
                XmlDataProvider provider = new XmlDataProvider();
                provider.Source = new Uri(dialog.FileName, UriKind.Absolute);
                provider.XPath = "./*";
                Mediator.NotifyColleagues("XmlFileLoaded", provider); //! Notify MainWindowVM that file was loaded and send it to display content
                XDocument Doc = XDocument.Load(dialog.FileName, LoadOptions.SetLineInfo);
                Log.LoggIt.Log($"File has been loaded", Log.LogType.Info);
                XmlFileLoaded = true;
                MusicScore mus_score = new MusicScore(Doc);
                UpdateSize();
            }
        }

        private void OnOldViewCommand()
        {
            MusicXMLViewerWPF.MainWindow oldwindow = new MusicXMLViewerWPF.MainWindow();
            oldwindow.ShowDialog();
        }

        private void OnCloseFile()
        {
            MusicScore.Clear();
            Log.LoggIt.Log("File has been closed");
            Mediator.NotifyColleagues("XmlFileLoaded", new XmlDataProvider());
            XmlFileLoaded = false;
        }
        private void OnExitApp()
        {
            Application.Current.Shutdown();
        }

        private void UpdateSize()
        {
            w = MusicScore.Defaults.Page.Width;
            h = MusicScore.Defaults.Page.Height;
        }
        #endregion

        #region Fields
        private float w = 1500;
        private float h = 1900;
        private bool xmlfileloaded;
        private ObservableCollection<Page.PageView> pages = new ObservableCollection<Page.PageView>();
        private Orientation orientation = Orientation.Horizontal;
        #endregion

        #region Properties, Commands
        public RelayCommand AddMeasureCommand { get; set; }
        public ObservableCollection<Page.PageView> Pages { get { return pages; } set { pages = value; } }
        public RelayCommand OpenOptionsWindowCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand NewScoreCreatorCommand { get; set; }
        public RelayCommand CustomButtonCommand { get; set; }
        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand OldViewCommand { get; set; }
        public RelayCommand CloseFileCommand { get; set; }

        public bool XmlFileLoaded { get { return xmlfileloaded; } set { if (xmlfileloaded != value) xmlfileloaded = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(XmlFileLoaded)));  } }
        public Orientation Orientation { get { return orientation; } set { if (orientation != value) orientation = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Orientation))); } }
        
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
