using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
using MusicXMLScore.Log;
using System.Windows.Controls;
using MusicXMLScore.ScoreLayout.MeasureLayouts;
using MusicXMLScore.Converters;
using MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent;

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
        sixtyfour = 64
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

    class NewScoreCreatorViewModel : ViewModelBase
    {
        #region fields
        private bool customsetting;
        private ClefTypeOptions currentclefoption = ClefTypeOptions.regularclef;
        private Dictionary<int, TimeSigBeatTime> timebeatlist = new Dictionary<int, TimeSigBeatTime> { [1] = TimeSigBeatTime.one, [2] = TimeSigBeatTime.two, [4] = TimeSigBeatTime.four, [8] = TimeSigBeatTime.eight, [16] = TimeSigBeatTime.sixteen, [24] = TimeSigBeatTime.twentyfour, [32] = TimeSigBeatTime.thirtytwo, [64] = TimeSigBeatTime.sixtyfour };
        private Canvas canvaslist = new Canvas();
        private PreviewCanvas keypreview = new PreviewCanvas();
        private int measurescount = 32;
        private uint timesigtimeval = 4;
        private KeyValuePair<int, TimeSigBeatTime> selectedtimebeats = new KeyValuePair<int, TimeSigBeatTime>(4, TimeSigBeatTime.four);
        private KeyValuePair<string, ClefType> selectedclef = new KeyValuePair<string, ClefType>(MusicSymbols.GClef, ViewModel.ClefType.GClef);
        private static PreviewCanvas previewcanvas;
        private static List<string> cleftype_ = new List<string> { MusicSymbols.CClef, MusicSymbols.GClef, MusicSymbols.FClef};
        private Dictionary<string, ClefType> cleftype = new Dictionary<string, ClefType> { [MusicSymbols.GClef] = ViewModel.ClefType.GClef, [MusicSymbols.FClef] = ViewModel.ClefType.FClef, [MusicSymbols.CClef] = ViewModel.ClefType.CClef };
        private static ObservableCollection<string> keysymbollist = new ObservableCollection<string>();
        private string selclef = cleftype_[1];
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
        public Canvas ConfigurationPreview { get { return canvaslist; } }
        public PreviewCanvas KeyPreview { get { return keypreview; } }
        public PreviewCanvas PreviewCanvas { get { return previewcanvas; } set { previewcanvas = value; } }
        public int MeasuresCount { get { return measurescount; } set { measurescount = value; } }
        public uint TimeSigTime { get { return timesigtimeval; } set { if (value != timesigtimeval) { if (timesigtimeval != value) { Set(nameof(TimeSigTime), ref timesigtimeval, value); } } } }
        public KeyValuePair<ImageSource, ClefType> SelectedClefType { get; set; }
        public KeyValuePair<int, TimeSigBeatTime> SelectedTimeBeats { get { return selectedtimebeats; } set { Set(nameof(SelectedTimeBeats), ref selectedtimebeats, value); } }
        public List<string> ClefType { get { return cleftype_; } }
        public Dictionary<string, ClefType> ClefTypeListS { get { return cleftype; } }
        public ObservableCollection<string> KeySymbolList { get { return keysymbollist; } set { if (keysymbollist != value) { Set(nameof(KeySymbolList), ref keysymbollist, value); } } }
        public RelayCommand AddVisualCommand { get; set; }
        public RelayCommand CanvasClick { get; set; }
        public RelayCommand OptionsWindowCommand { get; set; }
        public string SelectedClef { get { return selclef; } set { if (selclef != value) { Set(nameof(SelectedClef), ref selclef, value); } } }
        public KeyValuePair<string, ClefType> SelectedClefS { get { return selectedclef; } set { { Set(nameof(SelectedClefS), ref selectedclef, value); } } }
        public string SelectedKeyMode { get { return selectedkeymode; } set { if (selectedkeymode != value) Set(nameof(SelectedKeyMode), ref selectedkeymode, value); } }
        public string SelectedKeySymbol { get { return selectedkeysymbol; } set { if (selectedkeysymbol != value) { Set(nameof(SelectedKeySymbol), ref selectedkeysymbol, value); } } }
        public string SelectedKeyType { get { return selectedkeytype; } set { if (selectedkeytype != value) { Set(nameof(SelectedKeyType),ref selectedkeytype, value); } } }
        public string TimeSigTimeSource { get; set; }
        public TimeSigSettingOptions CurrentTimeSigOption { get { return currenttimesig; } set { if (value != currenttimesig) { Set(nameof(CurrentTimeSigOption), ref currenttimesig, value); } } }

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
            TestPrototype();
            //MusicScore.Defaults.Scale.Set(40);
        }

        private void NewScoreCreatorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedKeyMode":
                    SetKeySymbolList();
                    RaisePropertyChanged(nameof(SelectedKeySymbol));
                    break;
                case "SelectedKeyType":
                    SetKeySymbolList();
                    RaisePropertyChanged(nameof(SelectedKeySymbol));
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
            //TODO refactor
        }

        public DrawingVisual AddVis()
        {
            var visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                Helpers.DrawingHelpers.DrawString(dc, "test2", TypeFaces.TextFont, Brushes.Black, 35f, 45f, 20f);
            }
            return visual;
        }

        private void TestPrototype()
        {
            var staff = new RegularStaff(5, 60.0.TenthsToWPFUnit(), 180.0.TenthsToWPFUnit());
            staff.Update();
            var attributes = new MeasureAttributes(
                true, 
                new MeasureClef(Model.MeasureItems.Attributes.ClefSignMusicXML.G, 2, 0, staff),
                new MeasureKey(),
                new MeasureTime()
                );
            attributes.Update();
            var measureContent = new StandardMeasureContent(
                attributes,
                new MeasureRest(staff.DesiredWidth- attributes.GetVisualWidth(), staff)
                );
            measureContent.Update();
            var measure = new StandardMeasure("0", "0", staff, staff.DesiredWidth, measureContent);
            canvaslist.Children.Add(measure.GetVisualControl());
        }

        private static void OnOpionsWindow()
        {
            ConfigurationView optionswindow = new ConfigurationView();
            optionswindow.ShowDialog();
        }

        private void OnAddVisual()
        {
            //PreviewCanvas.AddVisual(AddVis());
            keypreview.StaffLine();
        }

        private void SetKeySymbolList()
        {
            if (SelectedKeyMode == "Major")
            {
                if (SelectedKeyType == "Sharp")
                {
                    KeySymbolList = new ObservableCollection<string> { "C", "G", "D", "A", "E", "B", "F\u266f", "C\u266f" };
                }
                if (SelectedKeyType == "Flat")
                {
                    KeySymbolList = new ObservableCollection<string> { "C", "F", "B\u266d", "E\u266d", "A\u266d", "D\u266d", "G\u266d", "C\u266d" };
                }
                if (SelectedKeyType == "None")
                {
                    KeySymbolList = new ObservableCollection<string> { "C", "G", "D", "A", "E", "B", "F\u266f", "C\u266f", "C", "F", "B\u266d", "E\u266d", "A\u266d", "D\u266d", "G\u266d", "C\u266d" };
                }
            }
            if (SelectedKeyMode == "Minor")
            {
                if (SelectedKeyType == "Sharp")
                {
                    KeySymbolList = new ObservableCollection<string> { "a", "e", "b", "f\u266f", "c\u266f", "g\u266f", "d\u266f", "b\u266d" };
                }
                if (SelectedKeyType == "Flat")
                {
                    KeySymbolList = new ObservableCollection<string> { "a", "d", "g", "c", "f", "b\u266d", "e\u266d", "g\u266f" };
                }
                if (SelectedKeyType == "None")
                {
                    KeySymbolList = new ObservableCollection<string> { "a", "e", "b", "f\u266f", "c\u266f", "g\u266f", "d\u266f", "b\u266d", "a", "d", "g", "c", "f", "b\u266d", "e\u266d", "g\u266f" };
                }
            }
        }

        private void OnCanvasClick() //! test
        {
            LoggIt.Log("test");
            LoggIt.Log("error1", LogType.Warning);
            LoggIt.Log("warning occured here ------------------------------------------------>", LogType.Warning);
            for (int i = 0; i < 200; i++)
            {
                LoggIt.Log($"test {i}", LogType.Info);
                LoggIt.Log($"test {i}", LogType.Warning);
                LoggIt.Log($"test {i}", LogType.Error);
            }
            //MessageBox.Show(SimpleLogger.SimpleLog.NumberOfLogEntriesWaitingToBeWrittenToFile.ToString() + " " + canvaslist.Count);
        }
    }
}
