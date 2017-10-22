//#define LOADSPEEDTEST

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using Microsoft.Win32;
using MusicXMLScore.Converters;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
using MusicXMLScore.LayoutControl;
using MusicXMLScore.Log;
using MusicXMLScore.ScoreProperties;
using MusicXMLViewerWPF;
using MusicXMLScore.ScoreLayout;
using MusicXMLScore.ScoreLayout.PageLayouts;
using MusicXMLScore.ScoreLayout.PageLayouts.PageElements;
using MusicXMLScore.Model.Factories;

namespace MusicXMLScore.ViewModel
{
    public class MainWindowViewModel : ViewModelBase //TODO_I closing all tabitems cause binding break
    {
        private static readonly Hashtable Serializers = new Hashtable();
        private bool _allTabsClosed = true;
        private ScorePropertiesContainer _scoreProperties = new ScorePropertiesContainer();
        private TabItem _selectedTabItem;
        private ObservableCollection<TabItem> _tabscreated = new ObservableCollection<TabItem>();
        private RelayCommand addNewPageCommand;

        public MainWindowViewModel()
        {
            PropertyChanged += MainWindowViewModel_PropertyChanged;
            CloseFileCommand = new RelayCommand(OnCloseFile, () => !AllTabsClosed);
            TestButtonCommand = new RelayCommand(OnTestButtonCommand);
            TestButton2Command = new RelayCommand(OnTestButton2Command);
            ExitCommand = new RelayCommand(OnExitApp);
            NewCustomScoreCreatorCommand = new RelayCommand(OnNewCustomScoreCreator);
            NewDefaultScoreCreatorCommand = new RelayCommand(OnNewDefaultScoreCreator);
            AdvancedLayoutScoreCreatorTest = new RelayCommand(OnAdvancedLayoutScoreCreatorTest);
            AdvancedLayoutScoreCreatorTest2 = new RelayCommand(OnAdvancedLayoutScoreCreatorTest2);
            OpenFileCommand = new RelayCommand<string>(OnOpenFileCommand);
            OpenOptionsWindowCommand = new RelayCommand(OnOpenOptionWindow);

            CreateBlankTab();
            CacheXMLSerializer();
        }

        private void OnAdvancedLayoutScoreCreatorTest2()
        {
            const string header = "ADVANCED_LAYOUT_TEST_PANEL";
            var vm = SelectedTabItem.DataContext as PagesControllerViewModel;
            var score = OnOpenFileAdvancedTestCommand(null);
            if(score == null)
            {
                return;
            }
            if (vm?.IsBlank == true)
            {
                SelectedTabItem.Template = null;
                TabsCreated.Remove(SelectedTabItem);
            }
            var pcvm = new PagesControllerViewModel(score);
            var tab = new TabItem
            {
                Header = header,
                Tag = score.ID,
                Content = new PagesControllerView(),
                DataContext = pcvm
            };
            pcvm.IsBlank = false;
            TabsCreated.Add(tab);
            SelectedTabItem = tab;
            AllTabsClosed = false;
        }

        public RelayCommand AddMeasureCommand { get; set; }

        private bool AllTabsClosed
        {
            get { return _allTabsClosed; }

            set
            {
                Set(() => AllTabsClosed, ref _allTabsClosed, value);
                CloseFileCommand.RiseCanExecuteChanged();
            }
        }

        public RelayCommand CloseFileCommand { get; set; }
        public LayoutGeneral CurrentLayout => ScoreProperties.CurrentLayoutProperties;
        public PageProperties CurrentPageLayout => ScoreProperties.CurrentLayoutProperties.PageProperties;
        public ScorePartwiseMusicXML CurrentSelectedScore => ScoreProperties.CurrentScoreProperties.Score;
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand NewCustomScoreCreatorCommand { get; set; }
        public RelayCommand NewDefaultScoreCreatorCommand { get; set; }
        public RelayCommand OldViewCommand { get; set; }
        public RelayCommand<string> OpenFileCommand { get; set; }
        public RelayCommand OpenOptionsWindowCommand { get; set; }
        public RelayCommand AdvancedLayoutScoreCreatorTest { get; set; }
        public RelayCommand AdvancedLayoutScoreCreatorTest2 { get; set; }
        public RelayCommand AddNewPageCommand { get => addNewPageCommand; set => Set(nameof(AddNewPageCommand), ref addNewPageCommand, value); }

        public TabItem SelectedTabItem
        {
            get { return _selectedTabItem; }
            set
            {
                if (SelectedTabItem != null)
                {
                    //! switch current selected score to score of SelectedTab
                    if (value != null)
                    {
                        PagesControllerViewModel pcvm = (PagesControllerViewModel)value.DataContext;
                        ScoreProperties.SelectScore(pcvm.ID);
                        AddNewPageCommand = pcvm.AddPageCommand;
                    }
                }
                Set(() => SelectedTabItem, ref _selectedTabItem, value);
            }
        }

        public ObservableCollection<TabItem> TabsCreated
        {
            get { return _tabscreated; }
            set
            {
                if (value != null)
                {
                    _tabscreated = value;
                }
            }
        }

        public RelayCommand TestButton2Command { get; set; }
        public RelayCommand TestButtonCommand { get; set; }

        internal Dictionary<string, PartProperties> PartsProperties =>
            ScoreProperties.CurrentScoreProperties.PartProperties;

        internal ScoreProperties.ScoreProperties CurrentScoreProperties => _scoreProperties.CurrentScoreProperties;

        private ScorePropertiesContainer ScoreProperties
        {
            get { return _scoreProperties; }
            set { _scoreProperties = value; }
        }

        private static T DeserializeFile<T>(string parameter) where T : new()
        {
            TextReader reader = null;
            try
            {
                XmlSerializer serializer = (XmlSerializer)Serializers[typeof(T)] ?? new XmlSerializer(typeof(T));
                reader = new StreamReader(parameter);
                var result = (T)serializer.Deserialize(reader);
                if (result is ScorePartwiseMusicXML)
                {
                    ScorePartwiseMusicXML score = result as ScorePartwiseMusicXML;
                    score.InitPartsDictionaries();
                    score.SetLargestMeasureWidth();
                }

                return result;
            }
            catch (Exception exception)
            {
                LoggIt.Log(exception.Message, LogType.Exception);
                MessageBox.Show(exception.Message + " \n " + exception.StackTrace);
                return default(T);
            }
            finally
            {
                reader?.Close();
            }
        }

        private void CacheXMLSerializer()
        {
            object key = typeof(ScorePartwiseMusicXML);
            XmlSerializer x = new XmlSerializer(typeof(ScorePartwiseMusicXML));
            Serializers[key] = x;
        }

        private void CreateBlankTab()
        {
            string header = "New Document";
            TabItem tab = new TabItem
            {
                Header = header,
                Content = new PagesControllerView(),
                DataContext = new PagesControllerViewModel()
            };
            AllTabsClosed = true;
            TabsCreated.Add(tab);
            SelectedTabItem = tab;
        }

        private void GenerateLayout(ScorePartwiseMusicXML score)
        {
            _scoreProperties.AddScore(score);
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
            var name = SelectedTabItem.DataContext as PagesControllerViewModel;
            if (name != null)
            {
                LoggIt.Log($"File {name.Title} has been closed");
                _scoreProperties.RemoveScore(name.ID);
            }
            SelectedTabItem.Template = null;
            TabsCreated.Remove(
                SelectedTabItem); //TODO_L Refactor commands to work on selected tab (loaded file/new score)

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
            NewScoreCreatorView ndefaultscreatorwindow = new NewScoreCreatorView();
            ndefaultscreatorwindow.ShowDialog();
        }

        private void OnAdvancedLayoutScoreCreatorTest()
        {

            string header = "ADVANCED_LAYOUT_TEST_ID";
            var vm = SelectedTabItem.DataContext as PagesControllerViewModel;
            var pageLayout = new WrappedLayout(new List<AbstractPageElement>());
            var scorePage = AdvancedLayoutTestFactory.GetScorePage();
            var scorePage2 = AdvancedLayoutTestFactory.GetScorePage2();
            scorePage.NextPage = scorePage2;
            scorePage2.PreviousPage = scorePage;
            //var scorePage = new StandardScorePage("ADVANCED_LAYOUT_TEST_ID", pageLayout);
            //scorePage.UpdateContent();
            if (vm?.IsBlank == true)
            {
                vm.AddScorePage(scorePage);

                vm.AddScorePage(scorePage2);
                SelectedTabItem.Tag = scorePage.Id;
                SelectedTabItem.Header = header;
                //bug-fix: binding error PagesCollection property not found
                // SelectedTabItem.Content = new PagesControllerView() { DataContext = vm };
                SelectedTabItem.DataContext = vm;
                Console.WriteLine("ADVANCED_LAYOUT_TEST Loaded");
            }
            else
            {
                var pcvm = new PagesControllerViewModel();
                pcvm.AddScorePage(scorePage);
                pcvm.AddScorePage(scorePage2);
                var tab = new TabItem
                {
                    Header = header,
                    Tag = scorePage.Id,
                    Content = new PagesControllerView(),
                    DataContext = pcvm
                };
                TabsCreated.Add(tab);
                SelectedTabItem = tab;
            }
            AllTabsClosed = false;

        }

        private void OnNewDefaultScoreCreator()
        {
            string header = "New Default Score";
            var vm = SelectedTabItem.DataContext as PagesControllerViewModel;
            ScorePartwiseMusicXML score = Model.Factories.BasicScoreFactory.GetScorePartwise();
            score.InitPartsDictionaries();
            score.SetLargestMeasureWidth();
            GenerateLayout(score);
            //! if currently selected tab is empty
            if (vm?.IsBlank == true)
            {
                vm.AddScorePartwise(score);
                SelectedTabItem.Tag = score.ID;
                SelectedTabItem.Header = header;
                //bug-fix: binding error PagesCollection property not found
                //SelectedTabItem.Content = new PagesControllerView(); 
                SelectedTabItem.DataContext = vm;
                Console.WriteLine("Default Score blank document");
            }
            else
            {
                PagesControllerViewModel pcvm = new PagesControllerViewModel();
                pcvm.AddScorePartwise(score);
                TabItem tab = new TabItem
                {
                    Header = header,
                    Tag = score.ID,
                    Content = new PagesControllerView(),
                    DataContext = pcvm
                };
                TabsCreated.Add(tab);
                SelectedTabItem = tab;
            }
            AllTabsClosed = false;
        }

        private ScorePartwiseMusicXML OnOpenFileAdvancedTestCommand(object parameter)
        {
            string filedestination;
            if (parameter != null)
            {
                filedestination = System.AppDomain.CurrentDomain.BaseDirectory + parameter as string;

            }
            else
            {
                var dialog = new OpenFileDialog { Filter = "MusicXML files|*.xml" };
                if (dialog.ShowDialog() == true)
                {
                    filedestination = dialog.FileName;
                    Console.WriteLine("Loaded for custom advanced layout -> " + filedestination);
                }
                else
                {
                    return null; //! return with no action, eg. OpenFileDialog Cancel/Close button clicked
                }
            }

            LoggIt.Log($"File {filedestination} been loaded");

            ScorePartwiseMusicXML xml = DeserializeFile<ScorePartwiseMusicXML>(filedestination);
            xml.InitPartsDictionaries();
            xml.SetLargestMeasureWidth();
            GenerateLayout(xml);
            return xml;
        }

        [STAThread]
        private void OnOpenFileCommand(object parameter) //! For now xml file load avaliable only
        {
            string filedestination;
            if (parameter != null)
            {
                filedestination = System.AppDomain.CurrentDomain.BaseDirectory + parameter as string;

            }
            else
            {
                var dialog = new OpenFileDialog { Filter = "MusicXML files|*.xml" };
                if (dialog.ShowDialog() == true)
                {
                    filedestination = dialog.FileName;
                    Console.WriteLine("Loaded -> " + filedestination);
                }
                else
                {
                    return; //! return with no action, eg. OpenFileDialog Cancel/Close button clicked
                }
            }

            LoggIt.Log($"File {filedestination} been loaded");

            ScorePartwiseMusicXML xml = DeserializeFile<ScorePartwiseMusicXML>(filedestination);

            var pagesControllerVM = (PagesControllerViewModel)SelectedTabItem.DataContext;
            if (pagesControllerVM.IsBlank) //! check if currently selected tab is blank/empty then add loaded file to it
            {
                GenerateLayout(xml);
                pagesControllerVM.AddScorePartwise(xml);

                SelectedTabItem.Template = null;
                //bugfix - if last tab removed, binding error: "cannot find source for binding..."
                TabsCreated.Remove(SelectedTabItem);
                TabItem newTab = new TabItem
                {
                    Header = filedestination,
                    Content = new PagesControllerView(),
                    DataContext = pagesControllerVM
                };
                TabsCreated.Add(newTab);
                SelectedTabItem = newTab;
                AllTabsClosed = false;
            }
            else //! if not empty, create new tab add loaded file and select
            {
                GenerateLayout(xml);
                PagesControllerViewModel pagesControllerNew = new PagesControllerViewModel();
                pagesControllerNew.AddScorePartwise(xml);
                TabItem newTab = new TabItem
                {
                    Header = filedestination,
                    Tag = xml.ID,
                    Content = new PagesControllerView(),
                    DataContext = pagesControllerNew
                };
                TabsCreated.Add(newTab);
                SelectedTabItem = newTab;
                AllTabsClosed = false;
            }
        }

        private void OnOpenOptionWindow()
        {
            ConfigurationView optionswindow = new ConfigurationView();
            optionswindow.ShowDialog();
        }

        private void OnTestButton2Command()
        {
            //! switch stretch setting of all system 
            CurrentLayout.LayoutStyle.PageStyle.StretchSystemToPageWidth = !CurrentLayout.LayoutStyle.PageStyle.StretchSystemToPageWidth;
        }

        private void OnTestButtonCommand()
        {
            //! switch stretch setting of last system 
            CurrentLayout.LayoutStyle.PageStyle.StretchLastSystemOnPage = !CurrentLayout.LayoutStyle.PageStyle.StretchLastSystemOnPage;
        }
    }
}
