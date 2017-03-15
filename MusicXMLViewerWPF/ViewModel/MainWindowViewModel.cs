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
using System.Diagnostics;
using System.Collections;
using MusicXMLScore.Converters;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.ScoreProperties;

namespace MusicXMLScore.ViewModel
{
    public class MainWindowViewModel : ViewModelBase //TODO_I closing all tabitems cause binding break
    {
        #region Fields

        private static Hashtable serializers = new Hashtable();
        private bool isBlank = true;
        private Orientation orientation = Orientation.Horizontal;
        private ScorePropertiesContainer scoreProperties = new ScorePropertiesContainer();
        private TabItem selectedTabItem;
        private ObservableCollection<TabItem> tabscreated = new ObservableCollection<TabItem>();

        #endregion Fields

        #region Constructors

        public MainWindowViewModel()
        {
            PropertyChanged += MainWindowViewModel_PropertyChanged;
            AddMeasureCommand = new RelayCommand(OnAddMeasure);
            CloseFileCommand = new RelayCommand(OnCloseFile, () => IsBlank ==false ? true : false);
            TestButtonCommand = new RelayCommand(OnTestButtonCommand);
            ExitCommand = new RelayCommand(OnExitApp);
            NewCustomScoreCreatorCommand = new RelayCommand(OnNewCustomScoreCreator);
            NewDefaultScoreCreatorCommand = new RelayCommand(OnNewDefaultScoreCreator);
            OldViewCommand = new RelayCommand(OnOldViewCommand, () => false);
            OpenFileCommand = new RelayCommand<string>(s => OnOpenFileCommand(s));
            OpenOptionsWindowCommand = new RelayCommand(OnOpenOptionWindow);
            CreateBlankTab();
            CacheXMLSerializer();
        }

        #endregion Constructors

        #region Properties

        public RelayCommand AddMeasureCommand { get; set; }
        public RelayCommand CloseFileCommand { get; set; }
        public LayoutControl.LayoutGeneral CurrentLayout { get { return ScoreProperties.CurrentLayoutProperties; } }
        public PageProperties CurrentPageProperties { get { return ScoreProperties.CurrentLayoutProperties.PageProperties;} }
        public ScorePartwiseMusicXML CurrentSelectedScore { get { return ScoreProperties.CurrentScoreProperties.Score; } }
        public RelayCommand ExitCommand { get; set; }
        public bool IsBlank
        {
            get
            {
                return isBlank;
            }

            set
            {
                Set(() => IsBlank, ref isBlank, value);
                CloseFileCommand.RiseCanExecuteChanged();
            }
        }
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
                        ScoreProperties.SelectScore(pcvm.ID);
                    }
                }
                Set(() => SelectedTabItem, ref selectedTabItem, value);

            }
        }
        public ObservableCollection<TabItem> TabsCreated { get { return tabscreated; } set { if (value != null) { tabscreated = value; } } }
        public RelayCommand TestButtonCommand { get; set; }
        internal Dictionary<string, PartProperties> CurrentPartsProperties { get { return ScoreProperties.CurrentScoreProperties.PartProperties;/*currentpartPropertiesContainer;*/ } /*set { currentpartPropertiesContainer = value; }*/ }
        internal MusicXMLScore.ScoreProperties.ScoreProperties CurrentScoreProperties {  get { return scoreProperties.CurrentScoreProperties;  } }
        internal ScorePropertiesContainer ScoreProperties
        {
            get
            {
                return scoreProperties;
            }
            set
            {
                scoreProperties = value;
            }
        }

        #endregion Properties

        #region Methods

        public static T TempMethod<T>(string parameter) where T : new()
        {
            TextReader reader = null;
            try
            {
                XmlSerializer serializer = (XmlSerializer)serializers[typeof(T)];
                if (serializer == null)
                {
                    serializer = new XmlSerializer(typeof(T));
                }
                reader = new StreamReader(parameter);
                return (T)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                Log.LoggIt.Log(ex.InnerException.Message.ToString(), Log.LogType.Exception);
                return default(T);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        private void CacheXMLSerializer()
        {
            object key = typeof(ScorePartwiseMusicXML);
            ScorePartwiseMusicXML scoreTemp = new ScorePartwiseMusicXML();
            XmlSerializer x = new XmlSerializer(typeof(ScorePartwiseMusicXML));
            serializers[key] = x;
        }

        private void CreateBlankTab()
        {
            string header = "New Document";
            TabItem tab = new TabItem() { Header = header, Content = new PagesControllerView(), DataContext = new PagesControllerViewModel() };
            IsBlank = true;
            TabsCreated.Add(tab);
            SelectedTabItem = tab;
        }

        private void GenerateLayout(MusicScore musicscore)
        {
            LayoutControl.LayoutGeneral layout = new LayoutControl.LayoutGeneral(musicscore);
            //tabsLayouts.Add(musicscore.ID, layout);
            //CurrentLayout = layout;
        }

        private void GenerateLayout(ScorePartwiseMusicXML score)
        {
            scoreProperties.AddScore(score);
        }

        private void LoadDefaultPageProperties()
        {
            using (var stream = File.OpenRead(@".\defaultPage.xml"))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter(); //! new BinaryFormatter();
                PageProperties pp2 = (PageProperties)formatter.Deserialize(stream);
            }
        }

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
            scoreProperties.RemoveScore(name.ID);
            TabsCreated.Remove(SelectedTabItem); // TODO_L Refactor commands to work on selected tab (loaded file/new score)
            
            if (TabsCreated.Count == 0)
            {
                CreateBlankTab();
            }
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
            var sw = new Stopwatch();
            sw = new Stopwatch();
            sw.Start();
            ScorePartwiseMusicXML xml = TempMethod<ScorePartwiseMusicXML>(filedestination);
            xml.InitPartsDictionaries();
            xml.SetLargestWidth();
            sw.Stop();
            Log.LoggIt.Log($"XML file deserialization to ScorePartwise object in : {sw.ElapsedMilliseconds} ms", Log.LogType.Exception);
            sw = new Stopwatch();
            sw.Start();
            var vm = (PagesControllerViewModel)SelectedTabItem.DataContext;
            if (vm.IsBlank) //! check if currently selected tab is blank
            {
                GenerateLayout(xml);//! 02.03 GenerateLayout(musicscore);
                vm.AddScorePartwise(xml);
                TabsCreated.Remove(SelectedTabItem);
                TabItem newTab = new TabItem() { Header = filedestination, Content = new PagesControllerView(), DataContext = vm };
                TabsCreated.Add(newTab);
                SelectedTabItem = newTab;

                sw.Stop();
                Log.LoggIt.Log($"Drawing ScorePartwise object in : {sw.ElapsedMilliseconds} ms", Log.LogType.Exception);
                IsBlank = false;
            }
            else
            {
                GenerateLayout(xml);//! 02.03 GenerateLayout(musicscore);
                PagesControllerViewModel pcvm = new PagesControllerViewModel();
                pcvm.AddScorePartwise(xml);
                TabItem newTab = new TabItem() { Header = filedestination, Tag = xml.ID, Content = new PagesControllerView(), DataContext = pcvm };
                TabsCreated.Add(newTab);
                SelectedTabItem = newTab;
                sw.Stop();
                Log.LoggIt.Log($"Drawing ScorePartwise object in : {sw.ElapsedMilliseconds} ms", Log.LogType.Exception);
                IsBlank = false;
            }
        }

        private void OnOpenOptionWindow()
        {
            ViewModel.ConfigurationView optionswindow = new ViewModel.ConfigurationView();
            optionswindow.ShowDialog();
        }

        private void OnTestButtonCommand()
        {
            LayoutStyle.Layout layout = new LayoutStyle.Layout();
            XmlSerializer xml = new XmlSerializer(layout.GetType());
            TextWriter txtw = new StreamWriter(@".\DefaultLayoutStyle.xml");
            xml.Serialize(txtw, layout);
            txtw.Close(); 
        }
        private void SaveDefaultPageProperties(PageProperties pp)
        {
            using (var stream = File.Create(@".\defaultPage.xml"))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter(); //! new BinaryFormatter();
                formatter.Serialize(stream, pp);
            }
        }

        #endregion Methods
    }
}
