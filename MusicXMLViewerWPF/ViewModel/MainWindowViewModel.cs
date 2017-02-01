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
            CreateBlankTab();
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
                //XmlFileLoaded = true;
            }
        }
        #endregion
        
        private void CreateBlankTab()
        {
            string header = "New Document";
            TabItem tab = new TabItem() { Header = header, Content = new PagesControllerView(), DataContext = new PagesControllerViewModel() };
            TabsCreated.Add(tab);
            SelectedTabItem = tab;
        }
        private void OnAddMeasure()
        {
            int i = TabsCreated.Count + 1;
            TabItem ti = new TabItem() { Header = "Tab " + i, Content = new PagesControllerView(), DataContext = new PagesControllerViewModel() };
            TabsCreated.Add(ti);
            SelectedTabItem = ti;
        }

        private void OnCloseFile()
        {
            //MusicScore.Clear();
            var name = SelectedTabItem.DataContext as ViewModel.PagesControllerViewModel;
            Log.LoggIt.Log($"File {name.Title} has been closed");
            XmlFileLoaded = false;
            //Pages.Clear();
            TabsCreated.Remove(SelectedTabItem); // TODO_L Refactor commands to work on selected tab (loaded file/new score)
            if (TabsCreated.Count == 0)
            {
                CreateBlankTab();
            }

            //TabsCreated.Clear();
        }

        private void OnCustomButtonCommand()
        {
            if (Orientation == Orientation.Vertical)
                Orientation = Orientation.Horizontal;
            else
                Orientation = Orientation.Vertical;
        }

        private void OnExitApp()
        {
            Application.Current.Shutdown();
        }

        private void OnNewCustomScoreCreator()
        {
            ViewModel.NewScoreCreatorView ndefaultscreatorwindow = new ViewModel.NewScoreCreatorView();
            ndefaultscreatorwindow.ShowDialog();
        }

        private void OnNewDefaultScoreCreator()
        {
            string header = "New Default Score";
            var vm = SelectedTabItem.DataContext as PagesControllerViewModel;
            if (vm.IsBlank) //! if currently selected tab is empty
            {
                SelectedTabItem.Header = header;
                SelectedTabItem.DataContext = new PagesControllerViewModel(2);
                Console.WriteLine("defaultscore black document");
            }
            else
            {
                TabItem tab = new TabItem() { Header = header, Content = new PagesControllerView(), DataContext = new PagesControllerViewModel(2) };
                TabsCreated.Add(tab);
                SelectedTabItem = tab;
            }
        }

        private void OnOldViewCommand()
        {
            //MusicXMLViewerWPF.MainWindow oldwindow = new MusicXMLViewerWPF.MainWindow();
            //oldwindow.ShowDialog();
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
            var vm = (PagesControllerViewModel)SelectedTabItem.DataContext;
            if (vm.IsBlank) //! check if currently selected tab is blank
            {
                vm.AddMusicScore(musicscore);
                TabsCreated.Remove(SelectedTabItem);
                TabItem newTab = new TabItem() { Header = filedestination, Content = new PagesControllerView(), DataContext = vm };
                TabsCreated.Add(newTab);
                SelectedTabItem = newTab;
            }
            else
            {
                TabItem newTab = new TabItem() { Header = filedestination, Content = new PagesControllerView(), DataContext = new PagesControllerViewModel(musicscore) };
                TabsCreated.Add(newTab);
                SelectedTabItem = newTab;
            }

        }

        private void OnOpenOptionWindow()
        {
            ViewModel.ConfigurationView optionswindow = new ViewModel.ConfigurationView();
            optionswindow.ShowDialog();
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
        #region Fields
        private double h = 1900;
        private Orientation orientation = Orientation.Horizontal;
        private ObservableCollection<PageView> pages = new ObservableCollection<PageView>();
        private TabItem selectedTabItem;
        private ObservableCollection<TabItem> tabscreated = new ObservableCollection<TabItem>();
        private double w = 2000;
        private bool xmlfileloaded;
        #endregion

        #region Properties, Commands
        public RelayCommand AddMeasureCommand { get; set; }
        public RelayCommand CloseFileCommand { get; set; }
        public RelayCommand CustomButtonCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand NewCustomScoreCreatorCommand { get; set; }
        public RelayCommand NewDefaultScoreCreatorCommand { get; set; }
        public RelayCommand OldViewCommand { get; set; }
        public RelayCommand<string> OpenFileCommand { get; set; }
        public RelayCommand OpenOptionsWindowCommand { get; set; }
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

        public double PageWidth
        {
            get { return w; }
            set
            {
                if (w != value)
                {
                    Set(() => PageWidth, ref w, value);
                }
            }
        }

        public TabItem SelectedTabItem
        {
            get { return selectedTabItem; }
            set
            {
                Set(() => SelectedTabItem, ref selectedTabItem, value);
            }
        }

        public ObservableCollection<TabItem> TabsCreated { get { return tabscreated; } set { if (value != null) { tabscreated = value; } } }
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
        #endregion
    }
}
