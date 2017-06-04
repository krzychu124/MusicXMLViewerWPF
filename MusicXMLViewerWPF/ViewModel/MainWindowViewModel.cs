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
using System.Linq;

namespace MusicXMLScore.ViewModel
{
    public class MainWindowViewModel : ViewModelBase //TODO_I closing all tabitems cause binding break
    {

        #region Fields

        private static Hashtable serializers = new Hashtable();
        private bool allTabsClosed = true;
        private ScorePropertiesContainer scoreProperties = new ScorePropertiesContainer();
        private TabItem selectedTabItem;
        private ObservableCollection<TabItem> tabscreated = new ObservableCollection<TabItem>();

        #endregion Fields

        #region Constructors

        public MainWindowViewModel()
        {
            PropertyChanged += MainWindowViewModel_PropertyChanged;
            CloseFileCommand = new RelayCommand(OnCloseFile, () => AllTabsClosed ==false ? true : false);
            TestButtonCommand = new RelayCommand(OnTestButtonCommand);
            TestButton2Command = new RelayCommand(OnTestButton2Command);
            ExitCommand = new RelayCommand(OnExitApp);
            NewCustomScoreCreatorCommand = new RelayCommand(OnNewCustomScoreCreator);
            NewDefaultScoreCreatorCommand = new RelayCommand(OnNewDefaultScoreCreator);
            OpenFileCommand = new RelayCommand<string>(s => OnOpenFileCommand(s));
            OpenOptionsWindowCommand = new RelayCommand(OnOpenOptionWindow);

            CreateBlankTab();
            CacheXMLSerializer();
        }

        #endregion Constructors

        #region Properties

        public RelayCommand AddMeasureCommand { get; set; }
        public bool AllTabsClosed
        {
            get
            {
                return allTabsClosed;
            }

            set
            {
                Set(() => AllTabsClosed, ref allTabsClosed, value);
                CloseFileCommand.RiseCanExecuteChanged();
            }
        }

        public RelayCommand CloseFileCommand { get; set; }
        public LayoutControl.LayoutGeneral CurrentLayout { get { return ScoreProperties.CurrentLayoutProperties; } }
        public PageProperties CurrentPageLayout { get { return ScoreProperties.CurrentLayoutProperties.PageProperties;} }
        public ScorePartwiseMusicXML CurrentSelectedScore { get { return ScoreProperties.CurrentScoreProperties.Score; } }
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand NewCustomScoreCreatorCommand { get; set; }
        public RelayCommand NewDefaultScoreCreatorCommand { get; set; }
        public RelayCommand OldViewCommand { get; set; }
        public RelayCommand<string> OpenFileCommand { get; set; }
        public RelayCommand OpenOptionsWindowCommand { get; set; }
        public TabItem SelectedTabItem
        {
            get { return selectedTabItem; }
            set
            {
                if (SelectedTabItem != null)
                {
                    if (value != null)//! switch current selected score to score of SelectedTab
                    {
                        PagesControllerViewModel pcvm = (PagesControllerViewModel)value.DataContext;
                        ScoreProperties.SelectScore(pcvm.ID);
                    }
                }
                Set(() => SelectedTabItem, ref selectedTabItem, value);

            }
        }
        public ObservableCollection<TabItem> TabsCreated { get { return tabscreated; } set { if (value != null) { tabscreated = value; } } }
        public RelayCommand TestButton2Command { get; set; }
        public RelayCommand TestButtonCommand { get; set; }
        internal Dictionary<string, PartProperties> CurrentPartsProperties { get { return ScoreProperties.CurrentScoreProperties.PartProperties;/*currentpartPropertiesContainer;*/ } /*set { currentpartPropertiesContainer = value; }*/ }
        internal ScoreProperties.ScoreProperties CurrentScoreProperties {  get { return scoreProperties.CurrentScoreProperties;  } }
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

        public static T DeserializeFile<T>(string parameter) where T : new()
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
                var result = (T)serializer.Deserialize(reader);
                if(result is ScorePartwiseMusicXML)
                {
                    ScorePartwiseMusicXML score = result as ScorePartwiseMusicXML;
                    score.InitPartsDictionaries();
                    score.SetLargestMeasureWidth();
                }
                
                return result;
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
            AllTabsClosed = true;
            TabsCreated.Add(tab);
            SelectedTabItem = tab;
        }

        private void GenerateLayout(ScorePartwiseMusicXML score)
        {
            scoreProperties.AddScore(score);
        }

        private void MainWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "XmlFileLoaded")
            {
                CloseFileCommand.RiseCanExecuteChanged();
            }
        }
        
        private void OnCloseFile()
        {
            var name = SelectedTabItem.DataContext as ViewModel.PagesControllerViewModel;
            Log.LoggIt.Log($"File {name.Title} has been closed");
            scoreProperties.RemoveScore(name.ID);
            SelectedTabItem.Template = null;
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
        

        [STAThread]
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

            Log.LoggIt.Log($"File {filedestination} been loaded", Log.LogType.Info);
           
            ScorePartwiseMusicXML xml = DeserializeFile<ScorePartwiseMusicXML>(filedestination);
            
            var pagesControllerVM = (PagesControllerViewModel)SelectedTabItem.DataContext;
            if (pagesControllerVM.IsBlank) //! check if currently selected tab is blank/empty then add loaded file to it
            {
                GenerateLayout(xml);
                pagesControllerVM.AddScorePartwise(xml);

                SelectedTabItem.Template = null; //bugfix - if last tab removed, binding error: "cannot find source for binding..."
                TabsCreated.Remove(SelectedTabItem);
                TabItem newTab = new TabItem() { Header = filedestination, Content = new PagesControllerView(), DataContext = pagesControllerVM };
                TabsCreated.Add(newTab);
                SelectedTabItem = newTab;
                AllTabsClosed = false;
            }
            else //! if not empty, create new tab add loaded file and select
            {
                GenerateLayout(xml);
                PagesControllerViewModel pagesControllerNew = new PagesControllerViewModel();
                pagesControllerNew.AddScorePartwise(xml);
                TabItem newTab = new TabItem() { Header = filedestination, Tag = xml.ID, Content = new PagesControllerView(), DataContext = pagesControllerNew };
                TabsCreated.Add(newTab);
                SelectedTabItem = newTab;
                AllTabsClosed = false;
            }
        }

        private void OnOpenOptionWindow()
        {
            ViewModel.ConfigurationView optionswindow = new ViewModel.ConfigurationView();
            optionswindow.ShowDialog();
        }

        private void OnTestButton2Command()
        {
            //! switch stretch setting of all system 
            if (CurrentLayout.LayoutStyle.PageStyle.StretchSystemToPageWidth)
            {
                CurrentLayout.LayoutStyle.PageStyle.StretchSystemToPageWidth = false;
            }
            else
            {
                CurrentLayout.LayoutStyle.PageStyle.StretchSystemToPageWidth = true;
            }
        }

        private void OnTestButtonCommand()
        {
            //! switch stretch setting of last system 
            if (CurrentLayout.LayoutStyle.PageStyle.StretchLastSystemOnPage)
            {
                CurrentLayout.LayoutStyle.PageStyle.StretchLastSystemOnPage= false;
            }
            else
            {
                CurrentLayout.LayoutStyle.PageStyle.StretchLastSystemOnPage = true;
            }
        }

        #endregion Methods
    }
}
