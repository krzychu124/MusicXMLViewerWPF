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
using MusicXMLViewerWPF;
using MusicXMLScore.Helpers;

namespace MusicXMLScore.ViewModel
{
    public enum TimeSigBeatTime
    {
        one = 1,
        two = 2,
        four = 4,
        eight = 8,
        sixteen = 16,
        twentyfour = 24,
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
        private ClefTypeOptions currentclefoption = ViewModel.ClefTypeOptions.regularclef;
        private Dictionary<int, TimeSigBeatTime> timebeatlist = new Dictionary<int, TimeSigBeatTime>() { [1] = TimeSigBeatTime.one, [2] = TimeSigBeatTime.two, [4] = TimeSigBeatTime.four, [8] = TimeSigBeatTime.eight, [16] = TimeSigBeatTime.sixteen, [24] = TimeSigBeatTime.twentyfour, [32] = TimeSigBeatTime.thirtytwo, [64] = TimeSigBeatTime.sixtyfour };
        private Helpers.PreviewCanvas canvaslist = new Helpers.PreviewCanvas();
        private Helpers.PreviewCanvas keypreview = new Helpers.PreviewCanvas();
        private int measurescount = 32;
        private uint timesigtimeval = 4;
        private KeyValuePair<int, TimeSigBeatTime> selectedtimebeats = new KeyValuePair<int, TimeSigBeatTime>(4, TimeSigBeatTime.four);
        private KeyValuePair<string, ClefType> selectedclef = new KeyValuePair<string, ViewModel.ClefType>(MusicalChars.GClef, ViewModel.ClefType.GClef);
        private static Helpers.PreviewCanvas previewcanvas;
        private static List<string> cleftype_ = new List<string>() { MusicalChars.CClef, MusicalChars.GClef, MusicalChars.FClef};
        private Dictionary<string, ClefType> cleftype = new Dictionary<string, ViewModel.ClefType>() { [MusicalChars.GClef] = ViewModel.ClefType.GClef, [MusicalChars.FClef] = ViewModel.ClefType.FClef, [MusicalChars.CClef] = ViewModel.ClefType.CClef };
        private static ObservableCollection<string> keysymbollist = new ObservableCollection<string>();
        private string selclef = cleftype_.ElementAt(1);
        private string selectedkeymode = "Major";
        private string selectedkeysymbol;
        private string selectedkeytype = "Flat";
        private TimeSigSettingOptions currenttimesig = TimeSigSettingOptions.standard;
        private static MusicXMLViewerWPF.Defaults.Defaults defaults = new MusicXMLViewerWPF.Defaults.Defaults();
        #endregion

        #region properties
        public bool CustomSettings { get { return customsetting; } set { if (value != customsetting) { customsetting = value; OptionsWindowCommand.RiseCanExecuteChanged(); } } }
        public ClefTypeOptions CurrentClefOption { get { return currentclefoption; } set { if (value != currentclefoption) { currentclefoption = value; } } }
        public Dictionary<ImageSource, ClefType> ClefTypeList { get; set; }
        public Dictionary<int, TimeSigBeatTime> TimeBeatList { get { return timebeatlist; } }
        public Helpers.PreviewCanvas ConfigurationPreview { get { return canvaslist; } }
        public Helpers.PreviewCanvas KeyPreview { get { return keypreview; } }
        public Helpers.PreviewCanvas PreviewCanvas { get { return previewcanvas; } set { previewcanvas = value; } }
        public int MeasuresCount { get { return measurescount; } set { measurescount = value; } }
        public uint TimeSigTime { get { return timesigtimeval; } set { if (value != timesigtimeval) { timesigtimeval = value; NotifyPropertyChanged(nameof(TimeSigTime)); } } }
        public KeyValuePair<ImageSource, ClefType> SelectedClefType { get; set; }
        public KeyValuePair<int, TimeSigBeatTime> SelectedTimeBeats { get { return selectedtimebeats; } set { selectedtimebeats = value; NotifyPropertyChanged(nameof(SelectedTimeBeats)); } }
        public List<string> ClefType { get { return cleftype_; } }
        public Dictionary<string, ClefType> ClefTypeListS { get { return cleftype; } }
        public ObservableCollection<string> KeySymbolList { get { return keysymbollist; } set { if (keysymbollist == value) return; keysymbollist = value; NotifyPropertyChanged("KeySymbolList"); } }
        public RelayCommand AddVisualCommand { get; set; }
        public RelayCommand CanvasClick { get; set; }
        public RelayCommand OptionsWindowCommand { get; set; }
        public string SelectedClef { get { return selclef; } set { selclef = value; NotifyPropertyChanged(nameof(SelectedClef)); } }
        public KeyValuePair<string, ClefType> SelectedClefS { get { return selectedclef; } set { selectedclef = value; NotifyPropertyChanged(nameof(SelectedClef)); } }
        public string SelectedKeyMode { get { return selectedkeymode; } set { if (selectedkeymode == value) return; selectedkeymode = value; NotifyPropertyChanged(nameof(SelectedKeyMode)); } }
        public string SelectedKeySymbol { get { return selectedkeysymbol; } set { if (selectedkeysymbol == value) return; selectedkeysymbol = value; NotifyPropertyChanged(nameof(SelectedKeySymbol)); } }
        public string SelectedKeyType { get { return selectedkeytype; } set { if (selectedkeytype == value) return; selectedkeytype = value; NotifyPropertyChanged(nameof(SelectedKeyType)); } }
        public string TimeSigTimeSource { get; set; }
        public TimeSigSettingOptions CurrentTimeSigOption { get { return currenttimesig; } set { if (value != currenttimesig) { currenttimesig = value; NotifyPropertyChanged(nameof(CurrentTimeSigOption)); } } }
        

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void NotifyPropertyChanged(string name) { PropertyChanged(this, new PropertyChangedEventArgs(name)); }
        #endregion

        public NewScoreCreatorViewModel()
        {
            PropertyChanged += NewScoreCreatorViewModel_PropertyChanged;
            OptionsWindowCommand = new RelayCommand(OnOpionsWindow, () => CustomSettings);
            AddVisualCommand = new RelayCommand(OnAddVisual);
            CanvasClick = new RelayCommand(OnCanvasClick);
            InitPreview();
            SetKeySymbolList();
            
        }

        private void InitPreview()
        {
           //TODO check if necessary ==> MusicScore.Defaults = new MusicXMLViewerWPF.Defaults.Defaults();
            //MusicScore.Defaults.Scale.Set(40);
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
                case "SelectedKeySymbol":
                    UpdatePreview();
                    break;
                case "SelectedClef":
                    UpdatePreview();
                    break;
                case "SelectedTimeBeats":
                    UpdatePreview();
                    break;
                case "TimeSigTime":
                    UpdatePreview();
                    break;
                case "CurrentTimeSigOption":
                    UpdatePreview();
                    break;
                default:
                    break;
            }
        }

        private void UpdatePreview()
        {
            ConfigurationPreview.ClearVisuals();
            float scale = defaults.Scale.Tenths;
            Point key = new Point(2*scale, 0.5 * scale);
            Point clef = new Point(0.5*scale, 0.5 * scale);
            Point timesig = new Point(1.25*scale, 0.5 * scale);
            Point measure = new Point(0.25*scale, 0.5 * scale);
            DrawingVisual k_d = new DrawingVisual();
            DrawingVisual c_d = new DrawingVisual();
            DrawingVisual t_d = new DrawingVisual();
            DrawingVisual b_d = new DrawingVisual();
            DrawingVisual r_d = new DrawingVisual();
            DrawingVisual m = new DrawingVisual();
            MusicXMLViewerWPF.ScoreParts.Part.Measures.Measure meae = new MusicXMLViewerWPF.ScoreParts.Part.Measures.Measure() { Width = 4.5f * scale};
            using(DrawingContext dc= m.RenderOpen())
            {
                meae.Draw_Measure(dc, measure);
            }
            int fifths = (int)MusicXMLViewerWPF.Key.GetFifths(SelectedKeySymbol);
            MusicXMLViewerWPF.Key k = new MusicXMLViewerWPF.Key(fifths,"major",0);
            string cl = SelectedClefS.Value == ViewModel.ClefType.GClef ? "G" : SelectedClefS.Value == ViewModel.ClefType.FClef ? "F" : "C";
            MusicXMLViewerWPF.ClefType ct = new MusicXMLViewerWPF.ClefType(cl);
            Clef c = new Clef(cl,2,0);
            string ttype = CurrentTimeSigOption == TimeSigSettingOptions.standard ? "" : CurrentTimeSigOption.ToString();
            TimeSignature t = new TimeSignature((int)TimeSigTime, (int)SelectedTimeBeats.Value, ttype);
            Barline br = new Barline() { Style = Barline.BarStyle.light_light };
            Rest r = new Rest("whole", new Point( 3.5f* scale, 0.5 * scale));

            br.DrawBarline(b_d, new Point(4.5f * scale , 0.5 * scale), 5f);
            r.Draw(r_d);
            k.Relative = key;
            c.Relative = clef;
            t.Relative = timesig;
            k.Draw(k_d, k.Relative, ct);
            c.Draw(c_d);
            t.Draw(t_d);
            ConfigurationPreview.AddVisual(m);
            ConfigurationPreview.AddVisual(k_d);
            ConfigurationPreview.AddVisual(c_d);
            ConfigurationPreview.AddVisual(t_d);
            ConfigurationPreview.AddVisual(r_d);
            ConfigurationPreview.AddVisual(b_d);
        }
        
        public DrawingVisual AddVis()
        {
            DrawingVisual vis = new DrawingVisual();
            using (DrawingContext dc = vis.RenderOpen())
            {
                Helpers.DrawingHelpers.DrawString(dc, "test2", Helpers.TypeFaces.TextFont, Brushes.Black, 35f, 45f, 20f);
            }
            return vis;
        }

        private static void OnOpionsWindow()
        {
            ViewModel.ConfigurationView optionswindow = new ConfigurationView();
            optionswindow.ShowDialog();
        }

        private void OnAddVisual()
        {
            //PreviewCanvas.AddVisual(AddVis());
            canvaslist.AddVisual(AddVis());
            keypreview.StaffLine();
            SimpleLogger.SimpleLog.Log("Button test"+ canvaslist);
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
