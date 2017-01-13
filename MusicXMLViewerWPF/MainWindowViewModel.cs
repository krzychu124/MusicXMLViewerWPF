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
using MusicXMLScore.Page;

namespace MusicXMLScore
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            PropertyChanged += MainWindowViewModel_PropertyChanged;
            SimpleLog.Info("Test logging started.");
            OpenOptionsWindowCommand = new RelayCommand(OnOpenOptionWindow);
            ExitCommand = new RelayCommand(OnExitApp);
            NewScoreCreatorCommand = new RelayCommand(OnNewScoreCreator);
            AddMeasureCommand = new RelayCommand(OnAddMeasure);
            CustomButtonCommand = new RelayCommand(OnCustomButtonCommand);
            OpenFileCommand = new RelayCommand<string>(s => OnOpenFileCommand(s));
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
            int i = TabsCreated.Count + 1;
            TabItem ti = new TabItem() { Header = "Tab " + i, Content= new PagesControllerView(), DataContext = new PagesControllerViewModel()};
            TabsCreated.Add(ti);
        }
        
        private void OnCustomButtonCommand()
        {
            if (Orientation == Orientation.Vertical)
                Orientation = Orientation.Horizontal;
            else
                Orientation = Orientation.Vertical;
        }

        private void OnOpenFileCommand(object parameter)
        {
            XmlRead xmlReader = new XmlRead();
            XmlDataProvider provider = new XmlDataProvider();
            string filedestination = "";
            if (parameter == null)
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = "MusicXML files|*.xml";
                if (dialog.ShowDialog() == true)
                {
                    filedestination = dialog.FileName;
                }
                else { return; }
            }
            else
            {
                filedestination = parameter as string;
            }
            xmlReader.File_path = filedestination;
            Console.WriteLine("Loading file>> " + filedestination);
            Log.LoggIt.Log($"Loading file: {xmlReader.File_path}", Log.LogType.Info);

            provider.Source = new Uri(filedestination, UriKind.Absolute);
            provider.XPath = "./*";
            Mediator.NotifyColleagues("XmlFileLoaded", provider); //! Notify MainWindowVM that file was loaded and send it to display content
            XDocument Doc = XDocument.Load(filedestination, LoadOptions.SetLineInfo);
            Log.LoggIt.Log($"File has been loaded", Log.LogType.Info);
            XmlFileLoaded = true;
            MusicScore mus_score = new MusicScore(Doc);
            UpdateSize();

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
            //Pages.Clear();
            TabsCreated.Clear();
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
        private double w = 1500;
        private double h = 1900;
        private bool xmlfileloaded;
        private ObservableCollection<PageView> pages = new ObservableCollection<PageView>();
        private Orientation orientation = Orientation.Horizontal;
        private ObservableCollection<TabItem> tabscreated = new ObservableCollection<TabItem>();
        #endregion

        #region Properties, Commands
        public RelayCommand AddMeasureCommand { get; set; }
        //public ObservableCollection<Page.PageView> Pages { get { return pages; } set { if (value != null) { pages = value; } } }
        public ObservableCollection<TabItem> TabsCreated { get { return tabscreated; } set { if (value != null) { tabscreated = value; } } }
        public RelayCommand OpenOptionsWindowCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand NewScoreCreatorCommand { get; set; }
        public RelayCommand CustomButtonCommand { get; set; }
        public RelayCommand <string> OpenFileCommand { get; set; }
        public RelayCommand OldViewCommand { get; set; }
        public RelayCommand CloseFileCommand { get; set; }

        public double PageWidth { get { return w; } set {  if (w!= value) { w = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PageWidth))); } } }
        public double PageHeight { get { return h; } set { if (h != value) { h = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PageHeight))); } } } 
       // public double PageWidthL { get { if (Pages.Count != 0) { return Pages.ElementAt(0).Width; } else { return w; } } set { foreach (var p in Pages) { p.Width = value; } } }
        public bool XmlFileLoaded { get { return xmlfileloaded; } set { if (xmlfileloaded != value) xmlfileloaded = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(XmlFileLoaded)));  } }
        public Orientation Orientation { get { return orientation; } set { if (orientation != value) orientation = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Orientation))); } }
        
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
