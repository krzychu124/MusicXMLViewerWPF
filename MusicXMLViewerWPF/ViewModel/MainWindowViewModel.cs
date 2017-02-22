//#define LOADSPEEDTEST
using MusicXMLScore.Helpers;
using MusicXMLViewerWPF;
using SimpleLogger;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MusicXMLScore.ViewModel
{
    public class MainWindowViewModel : ViewModelBase //TODO_I closing all tabitems cause binding break
    {
        public MainWindowViewModel()
        {
            PropertyChanged += MainWindowViewModel_PropertyChanged;
            AddMeasureCommand = new RelayCommand(OnAddMeasure);
            CloseFileCommand = new RelayCommand(OnCloseFile, () => SelectedTabItem != null ? true : false); //? XmlFileLoaded);
            TestButtonCommand = new RelayCommand(OnTestButtonCommand);
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
            var name = SelectedTabItem.DataContext as ViewModel.PagesControllerViewModel;
            Log.LoggIt.Log($"File {name.Title} has been closed");
            tabsLayouts.Remove(name.MusicScore.ID);
            TabsCreated.Remove(SelectedTabItem); // TODO_L Refactor commands to work on selected tab (loaded file/new score)
            if (TabsCreated.Count == 0)
            {
                CreateBlankTab();
                tabsLayouts.TryGetValue("default", out currentTabLayout);
            }
        }

        private void OnTestButtonCommand()
        {
            LayoutStyle.Layout layout = new LayoutStyle.Layout();
            XmlSerializer xml = new XmlSerializer(layout.GetType());
            TextWriter txtw = new StreamWriter(@".\DefaultLayoutStyle.xml");
            xml.Serialize(txtw, layout);
            txtw.Close(); 
        }

        private void OnExitApp()
        {
            Application.Current.Shutdown();
        }
        private T TempMethod<T>(string parameter) where T : new ()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(parameter);
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
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
                SelectedTabItem.DataContext = new PagesControllerViewModel(1);
                Console.WriteLine("defaultscore black document");
            }
            else
            {
                TabItem tab = new TabItem() { Header = header, Content = new PagesControllerView(), DataContext = new PagesControllerViewModel(1) };
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
            XmlDataProvider dataprovider = new XmlDataProvider() { Source = new Uri(filedestination, UriKind.RelativeOrAbsolute), XPath = "./*" };
            Log.LoggIt.Log($"File {filedestination} been loaded", Log.LogType.Info);
            XDocument XDoc = XDocument.Load(filedestination, LoadOptions.SetLineInfo); // std + add line info(number)
            MusicScore musicscore = new MusicScore();
            ScorePartwiseMusicXML xml = TempMethod<ScorePartwiseMusicXML>(filedestination);

#if LOADSPEEDTEST
             var sw = new Stopwatch();
             sw.Start();
            for (int i = 0; i < 1; i++)
            {
                musicscore = new MusicScore(XDoc);
            }
            sw.Stop();
            Console.WriteLine($"file loaded in: {sw.ElapsedMilliseconds / 100}");
            sw = new Stopwatch();
            sw.Start();
#endif
            musicscore = new MusicScore(XDoc);

            var vm = (PagesControllerViewModel)SelectedTabItem.DataContext;
            if (vm.IsBlank) //! check if currently selected tab is blank
            {
                GenerateLayout(musicscore);
                vm.AddMusicScore(musicscore);
                TabsCreated.Remove(SelectedTabItem);
                TabItem newTab = new TabItem() { Header = filedestination, Content = new PagesControllerView(), DataContext = vm };
                TabsCreated.Add(newTab);
                SelectedTabItem = newTab;
            }
            else
            {
                GenerateLayout(musicscore);
                TabItem newTab = new TabItem() { Header = filedestination, Tag=musicscore.ID, Content = new PagesControllerView(), DataContext = new PagesControllerViewModel(musicscore) };
                TabsCreated.Add(newTab);
                SelectedTabItem = newTab;
            }
#if LOADSPEEDTEST
            sw.Stop();
            Console.WriteLine($"Layout generated in: {sw.ElapsedMilliseconds} ms");
#endif
        }

        private void GenerateLayout(MusicScore musicscore)
        {
            LayoutControl.LayoutGeneral layout = new LayoutControl.LayoutGeneral(musicscore);
            tabsLayouts.Add(musicscore.ID, layout);
            CurrentTabLayout = layout;
        }

        private void OnOpenOptionWindow()
        {
            ViewModel.ConfigurationView optionswindow = new ViewModel.ConfigurationView();
            optionswindow.ShowDialog();
        }
        private void SaveDefaultPageProperties(PageProperties pp)
        {
            using (var stream = File.Create(@".\defaultPage.xml"))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter(); //! new BinaryFormatter();
                formatter.Serialize(stream, pp);
            }
        }
        private void LoadDefaultPageProperties()
        {
            using (var stream = File.OpenRead(@".\defaultPage.xml"))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter(); //! new BinaryFormatter();
                PageProperties pp2 = (PageProperties)formatter.Deserialize(stream);
            }
        }
        #region Fields
        private Orientation orientation = Orientation.Horizontal;
        private ObservableCollection<PageView> pages = new ObservableCollection<PageView>();
        private TabItem selectedTabItem;
        private ObservableCollection<TabItem> tabscreated = new ObservableCollection<TabItem>();
        private Dictionary<string, LayoutControl.LayoutGeneral> tabsLayouts = new Dictionary<string, LayoutControl.LayoutGeneral>() { { "default", new LayoutControl.LayoutGeneral() } };
        private LayoutControl.LayoutGeneral currentTabLayout = new LayoutControl.LayoutGeneral();
        private bool xmlfileloaded;
#endregion

#region Properties, Commands
        public RelayCommand AddMeasureCommand { get; set; }
        public RelayCommand CloseFileCommand { get; set; }
        public RelayCommand TestButtonCommand { get; set; }
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
        public PageProperties CurrentPageProperties
        {
            get { return CurrentTabLayout.PageProperties; }
        }

        public LayoutControl.LayoutGeneral CurrentTabLayout { get { return currentTabLayout; } set { currentTabLayout = value; } }
        public TabItem SelectedTabItem
        {
            get { return selectedTabItem; }
            set
            {
                if (SelectedTabItem != null)
                {
                    if (value != null)
                    {
                        PagesControllerViewModel pcvm = (PagesControllerViewModel)value.DataContext;
                        LayoutControl.LayoutGeneral layout;
                        tabsLayouts.TryGetValue(pcvm.MusicScore.ID, out layout);
                        CurrentTabLayout = layout;
                    }
                }
                Set(() => SelectedTabItem, ref selectedTabItem, value);
                
            }
        }
        public ObservableCollection<TabItem> TabsCreated { get { return tabscreated; } set { if (value != null) { tabscreated = value; } } }
#endregion
    }
}
