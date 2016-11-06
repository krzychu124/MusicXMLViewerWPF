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
using System.Windows.Media;

namespace MusicXMLScore
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            //SimpleLog.SetLogFile(".\\Log", "MyLog_", writeText:true);
            SimpleLog.Info("Test logging started.");
            OpenOptionsWindowCommand = new RelayCommand(OnOpenOptionWindow);
            ExitCommand = new RelayCommand(OnExitApp);
            NewScoreCreatorCommand = new RelayCommand(OnNewScoreCreator);
            AddMeasureCommand = new RelayCommand(OnAddMeasure);
            CustomButtonCommand = new RelayCommand(OnCustomButtonCommand);
        }
        

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
            page.Width = test;
            page.Height = test;
            Pages.Add(page);
            test = test - 100;
            //Orientation = Orientation.Vertical;

        }
        
        private void OnCustomButtonCommand()
        {
            if (Orientation == Orientation.Vertical)
                Orientation = Orientation.Horizontal;
            else
                Orientation = Orientation.Vertical;
        }
        private int test = 500;
        private void OnExitApp()
        {
            Application.Current.Shutdown();
        }
        public RelayCommand AddMeasureCommand { get; set; }
        public ObservableCollection<Page.PageView> Pages { get { return pages; } set { pages = value; } }
        public RelayCommand OpenOptionsWindowCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand NewScoreCreatorCommand { get; set; }
        public RelayCommand CustomButtonCommand { get; set; }
        public Orientation Orientation { get { return orientation; } set { if (orientation != value) orientation = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Orientation))); } }
        private ObservableCollection<Page.PageView> pages = new ObservableCollection<Page.PageView>();
        private Orientation orientation = Orientation.Horizontal;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
