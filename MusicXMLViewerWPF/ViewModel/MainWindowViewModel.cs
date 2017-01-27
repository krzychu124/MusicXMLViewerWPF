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
using MusicXMLScore.ViewModel;
using GalaSoft.MvvmLight;

namespace MusicXMLScore.ViewModel
{
    public class MainWindowViewModel : ViewModelBase //TODO_I closing all tabitems cause binding break
    {
        public MainWindowViewModel()
        {
            PropertyChanged += MainWindowViewModel_PropertyChanged;
            SimpleLog.Info("Test logging started.");
            AddMeasureCommand = new RelayCommand(OnAddMeasure);
            CloseFileCommand = new RelayCommand(OnCloseFile, () => SelectedTabItem != null ? true : false); //? XmlFileLoaded);
            CustomButtonCommand = new RelayCommand(OnCustomButtonCommand);
            ExitCommand = new RelayCommand(OnExitApp);
            NewCustomScoreCreatorCommand = new RelayCommand(OnNewCustomScoreCreator);
            NewDefaultScoreCreatorCommand = new RelayCommand(OnNewDefaultScoreCreator);
            OldViewCommand = new RelayCommand(OnOldViewCommand, () => false);
            OpenFileCommand = new RelayCommand<string>(s => OnOpenFileCommand(s));
            OpenOptionsWindowCommand = new RelayCommand(OnOpenOptionWindow);
        }

        #region PropertyChanged
        private void MainWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "XmlFileLoaded")
            {
                CloseFileCommand.RiseCanExecuteChanged();
            }
            if (e.PropertyName == "SelectedTabItem")
            {
                XmlFileLoaded = true;
            }
        }
        #endregion

        #region Methods
        private void OnNewCustomScoreCreator()
        {
            ViewModel.NewScoreCreatorView ndefaultscreatorwindow = new ViewModel.NewScoreCreatorView();
            ndefaultscreatorwindow.ShowDialog();
        }

        private void OnNewDefaultScoreCreator()
        {
            string header = "New Default Score";
            TabItem tab = new TabItem() { Header = header, Content = new PagesControllerView(), DataContext = new PagesControllerViewModel() };
            TabsCreated.Add(tab);
            SelectedTabItem = tab;
            //XmlFileLoaded = true;
        }

        private void OnOpenOptionWindow()
        {
            ViewModel.ConfigurationView optionswindow = new ViewModel.ConfigurationView();
            optionswindow.ShowDialog();
        }

        private void OnAddMeasure()
        {
            int i = TabsCreated.Count + 1;
            TabItem ti = new TabItem() { Header = "Tab " + i, Content= new PagesControllerView(), DataContext = new PagesControllerViewModel()};
            TabsCreated.Add(ti);
            SelectedTabItem = ti;
        }
        
        private void OnCustomButtonCommand()
        {
            if (Orientation == Orientation.Vertical)
                Orientation = Orientation.Horizontal;
            else
                Orientation = Orientation.Vertical;
        }

        private void OnOpenFileCommand(object parameter) //! For now xml file load avaliable only
        {
            string filedestination;
            if (parameter != null)
            {
                filedestination = parameter as string;
            }
            else
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = "MusicXML files|*.xml";
                if (dialog.ShowDialog() == true)
                {
                    filedestination = dialog.FileName;
                }
                else
                {
                    return; //! return with no action, eg. OpenFileDialog Cancel/Close button clicked
                }
            }
            XmlDataProvider dataprovider = new XmlDataProvider() { Source = new Uri(filedestination), XPath = "./*" };
            Log.LoggIt.Log($"File {filedestination} been loaded", Log.LogType.Info);
            XDocument XDoc = XDocument.Load(filedestination, LoadOptions.SetLineInfo); // std + add line info(number)
            XmlFileLoaded = true;
            MusicScore musicscore = new MusicScore(XDoc);
            TabItem newTab = new TabItem() { Header = filedestination, Content = new PagesControllerView(), DataContext = new PagesControllerViewModel(musicscore) };
            TabsCreated.Add(newTab);
            SelectedTabItem = newTab;
        }

        //! Old OpenFileCommant Action
        //private void OnOpenFileCommand(object parameter)
        //{
        //    XmlRead xmlReader = new XmlRead();
        //    XmlDataProvider provider = new XmlDataProvider();
        //    string filedestination = "";
        //    if (parameter == null)
        //    {
        //        Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
        //        dialog.Filter = "MusicXML files|*.xml";
        //        if (dialog.ShowDialog() == true)
        //        {
        //            filedestination = dialog.FileName;
        //        }
        //        else { return; }
        //    }
        //    else
        //    {
        //        filedestination = parameter as string;
        //    }
        //    xmlReader.File_path = filedestination;
        //    Console.WriteLine("Loading file>> " + filedestination);
        //    Log.LoggIt.Log($"Loading file: {xmlReader.File_path}", Log.LogType.Info);

        //    provider.Source = new Uri(filedestination, UriKind.Absolute);
        //    provider.XPath = "./*";
        //    Mediator.NotifyColleagues("XmlFileLoaded", provider); //! Notify MainWindowVM that file was loaded and send it to display content
        //    XDocument Doc = XDocument.Load(filedestination, LoadOptions.SetLineInfo);
        //    Log.LoggIt.Log($"File has been loaded", Log.LogType.Info);
        //    XmlFileLoaded = true;
        //    MusicScore mus_score = new MusicScore(Doc);
        //    UpdateSize();

        //}

        private void OnOldViewCommand()
        {
            //MusicXMLViewerWPF.MainWindow oldwindow = new MusicXMLViewerWPF.MainWindow();
            //oldwindow.ShowDialog();
        }

        private void OnCloseFile()
        {
            //MusicScore.Clear();
            var name = SelectedTabItem.DataContext as ViewModel.PagesControllerViewModel;
            Log.LoggIt.Log($"File {name.Title} has been closed");
            XmlFileLoaded = false;
            //Pages.Clear();
            TabsCreated.Remove(SelectedTabItem); // TODO_L Refactor commands to work on selected tab (loaded file/new score)
            //TabsCreated.Clear();
        }
        private void OnExitApp()
        {
            Application.Current.Shutdown();
        }
        
        #endregion

        #region Fields
        private double w = 2000;
        private double h = 1900;
        private bool xmlfileloaded;
        private ObservableCollection<PageView> pages = new ObservableCollection<PageView>();
        private Orientation orientation = Orientation.Horizontal;
        private ObservableCollection<TabItem> tabscreated = new ObservableCollection<TabItem>();
        private TabItem selectedTabItem;
        #endregion

        #region Properties, Commands
        public RelayCommand AddMeasureCommand { get; set; }
        public ObservableCollection<TabItem> TabsCreated { get { return tabscreated; } set { if (value != null) { tabscreated = value; } } }
        public RelayCommand OpenOptionsWindowCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand NewDefaultScoreCreatorCommand { get; set; }
        public RelayCommand NewCustomScoreCreatorCommand { get; set; }
        public RelayCommand CustomButtonCommand { get; set; }
        public RelayCommand <string> OpenFileCommand { get; set; }
        public RelayCommand OldViewCommand { get; set; }
        public RelayCommand CloseFileCommand { get; set; }
        public TabItem SelectedTabItem
        {
            get { return selectedTabItem; }
            set {
                if (value != selectedTabItem)
                {
                    Set(() => SelectedTabItem, ref selectedTabItem, value);
                }
            }
        }
        public double PageWidth
        {
            get { return w; }
            set {
                if (w != value)
                {
                    Set(() => PageWidth, ref w, value);
                }
            }
        }
        public double PageHeight
        {
            get { return h; }
            set
            {
                if (h != value)
                {
                    Set(() => PageHeight, ref h, value);
                }
            }
        }
        public bool XmlFileLoaded
        {
            get { return xmlfileloaded; }
            set
            {
                if (xmlfileloaded != value)
                {
                    Set(() => XmlFileLoaded, ref xmlfileloaded, value);
                }
            }
        }
        public Orientation Orientation
        {
            get { return orientation; }
            set
            {
                if (orientation != value)
                {
                    Set(() => Orientation, ref orientation, value);
                }
            }
        }
        #endregion
    }
}
