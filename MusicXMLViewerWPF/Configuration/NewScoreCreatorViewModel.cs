using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MusicXMLScore.Configuration
{
    public enum TimeSigBeatTime
    {
        one = 1,
        two = 2,
        four = 4,
        eight = 8,
        sixteen = 16,
        thirtytwo = 32,
        sixtyfour = 64,
    }

    enum TimeSigSettingOptions
    {
        standard,
        common,
        cut
    }

    enum ClefTypeOptions
    {
        regularclef,
        tablature
    }
    enum ClefType
    {
        GClef,
        FClef,
        CClef
    }

    class NewScoreCreatorViewModel : INotifyPropertyChanged
    {
        #region fields
        private bool customsetting;
        private ClefTypeOptions currentclefoption = Configuration.ClefTypeOptions.regularclef;
        private Dictionary<int, TimeSigBeatTime> timebeatlist = new Dictionary<int, TimeSigBeatTime>() { [1] = TimeSigBeatTime.one, [2] = TimeSigBeatTime.two, [4] = TimeSigBeatTime.four, [8] = TimeSigBeatTime.eight, [16] = TimeSigBeatTime.sixteen, [32] = TimeSigBeatTime.thirtytwo, [64] = TimeSigBeatTime.sixtyfour };
        private Helpers.CanvasList canvaslist = new Helpers.CanvasList();
        private Helpers.PreviewCanvas keypreview = new Helpers.PreviewCanvas();
        private int measurescount = 32;
        private int timesigtimeval = 4;
        private KeyValuePair<int, TimeSigBeatTime> selectedtimebeats = new KeyValuePair<int, TimeSigBeatTime>(4, TimeSigBeatTime.four);
        private static Helpers.PreviewCanvas previewcanvas;
        private static List<string> cleftype = new List<string>() { "\ue050","\ue062","\ue05c"};
        private static ObservableCollection<string> keysymbollist = new ObservableCollection<string>();
        private string selclef = cleftype.ElementAt(0);
        private string selectedkeymode = "Major";
        private string selectedkeysymbol;
        private string selectedkeytype = "Flat";
        private TimeSigSettingOptions currenttimesig = TimeSigSettingOptions.standard;
        #endregion

        #region properties
        public bool CustomSettings { get { return customsetting; } set { if (value != customsetting) { customsetting = value; OptionsWindowCommand.RiseCanExecuteChanged(); } } }
        public ClefTypeOptions CurrentClefOption { get { return currentclefoption; } set { if (value != currentclefoption) { currentclefoption = value; } } }
        public Dictionary<ImageSource, ClefType> ClefTypeList { get; set; }
        public Dictionary<int, TimeSigBeatTime> TimeBeatList { get { return timebeatlist; } }
        public Helpers.CanvasList CList { get { return canvaslist; } }
        public Helpers.PreviewCanvas KeyPreview { get { return keypreview; } }
        public Helpers.PreviewCanvas PreviewCanvas { get { return previewcanvas; } set { previewcanvas = value; } }
        public int MeasuresCount { get { return measurescount; } set { measurescount = value; } }
        public int TimeSigTime { get { return timesigtimeval; } set { if (value != timesigtimeval) { timesigtimeval = value; } } }
        public KeyValuePair<ImageSource, ClefType> SelectedClefType { get; set; }
        public KeyValuePair<int, TimeSigBeatTime> SelectedTimeBeats { get { return selectedtimebeats; } set { selectedtimebeats = value; } }
        public List<string> ClefType { get { return cleftype; } }
        public ObservableCollection<string> KeySymbolList { get { return keysymbollist; } set { if (keysymbollist == value) return; keysymbollist = value; NotifyPropertyChanged("KeySymbolList"); } }
        public RelayCommand AddVisualCommand { get; set; }
        public RelayCommand CanvasClick { get; set; }
        public RelayCommand OptionsWindowCommand { get; set; }
        public string SelectedClef { get { return selclef; } set { selclef = value; } }
        public string SelectedKeyMode { get { return selectedkeymode; } set { if (selectedkeymode == value) return; selectedkeymode = value; NotifyPropertyChanged(nameof(SelectedKeyMode)); } }
        public string SelectedKeySymbol { get { return selectedkeysymbol; } set { if (selectedkeysymbol == value) return; selectedkeysymbol = value; NotifyPropertyChanged(nameof(SelectedKeySymbol)); } }
        public string SelectedKeyType { get { return selectedkeytype; } set { if (selectedkeytype == value) return; selectedkeytype = value; NotifyPropertyChanged(nameof(SelectedKeyType)); } }
        public string TimeSigTimeSource { get; set; }
        public TimeSigSettingOptions CurrentTimeSigOption { get { return currenttimesig; } set { if (value != currenttimesig) { currenttimesig = value; } } }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void NotifyPropertyChanged(string name) { PropertyChanged(this, new PropertyChangedEventArgs(name)); }
        #endregion

        public NewScoreCreatorViewModel()
        {
            PropertyChanged += NewScoreCreatorViewModel_PropertyChanged;
            OptionsWindowCommand = new RelayCommand(OnOpionsWindow, () => CustomSettings);
            AddVisualCommand = new RelayCommand(OnAddVisual);
            CanvasClick = new RelayCommand(OnCanvasClick);
            SetKeySymbolList();
        }

        private void NewScoreCreatorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedKeyMode":
                    SetKeySymbolList();
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedKeySymbol)));
                    break;
                case "SelectedKeyType":
                    SetKeySymbolList();
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedKeySymbol)));
                    break;
                case "KeySymbolList":
                    SelectedKeySymbol = KeySymbolList.ElementAt(0);
                    break;
                default:
                    break;
            }
        }

        
        public DrawingVisual AddVis()
        {
            DrawingVisual vis = new DrawingVisual();
            using (DrawingContext dc = vis.RenderOpen())
            {
                Helpers.DrawingMethods.DrawString(dc, "test2", Helpers.TypeFaces.TextFont, Brushes.Black, 35f, 45f, 20f);
            }
            return vis;
        }

        private static void OnOpionsWindow()
        {
            Configuration.ConfigurationView optionswindow = new ConfigurationView();
            optionswindow.ShowDialog();
        }

        private void OnAddVisual()
        {
            //PreviewCanvas.AddVisual(AddVis());
            canvaslist.AddVisual(AddVis());
            keypreview.StaffLine();
            SimpleLogger.SimpleLog.Log("Button test"+ canvaslist.Count);
            //SimpleLogger.SimpleLog.ShowLogFile();
        }

        private void SetKeySymbolList()
        {
            if (SelectedKeyMode == "Major")
            {
                if (SelectedKeyType == "Sharp")
                {
                    KeySymbolList = new ObservableCollection<string>() { "C", "G", "D", "A", "E", "B", "F\u266f", "C\u266f" };
                }
                if (SelectedKeyType == "Flat")
                {
                    KeySymbolList = new ObservableCollection<string>() { "C", "F", "B\u266d", "E\u266d", "A\u266d", "D\u266d", "G\u266d", "C\u266d" };
                }
                if (SelectedKeyType == "None")
                {
                    KeySymbolList = new ObservableCollection<string>() { "C", "G", "D", "A", "E", "B", "F\u266f", "C\u266f", "C", "F", "B\u266d", "E\u266d", "A\u266d", "D\u266d", "G\u266d", "C\u266d" };
                }
            }
            if (SelectedKeyMode == "Minor")
            {
                if (SelectedKeyType == "Sharp")
                {
                    KeySymbolList = new ObservableCollection<string>() { "a", "e", "b", "f\u266f", "c\u266f", "g\u266f", "d\u266f", "b\u266d" };
                }
                if (SelectedKeyType == "Flat")
                {
                    KeySymbolList = new ObservableCollection<string>() { "a", "d", "g", "c", "f", "b\u266d", "e\u266d", "g\u266f" };
                }
                if (SelectedKeyType == "None")
                {
                    KeySymbolList = new ObservableCollection<string>() { "a", "e", "b", "f\u266f", "c\u266f", "g\u266f", "d\u266f", "b\u266d", "a", "d", "g", "c", "f", "b\u266d", "e\u266d", "g\u266f" };
                }
            }
        }

        private void OnCanvasClick()
        {
            Log.LoggIt.Log("test");
            Log.LoggIt.Log("error1", Log.LogType.Warning);
            Log.LoggIt.Log("warning occured here ------------------------------------------------>", Log.LogType.Warning);
            for (int i = 0; i < 200; i++)
            {
                Log.LoggIt.Log($"test {i}", Log.LogType.Info);
                Log.LoggIt.Log($"test {i}", Log.LogType.Warning);
                Log.LoggIt.Log($"test {i}", Log.LogType.Error);
            }
            //MessageBox.Show(SimpleLogger.SimpleLog.NumberOfLogEntriesWaitingToBeWrittenToFile.ToString() + " " + canvaslist.Count);
        }
    }
}
