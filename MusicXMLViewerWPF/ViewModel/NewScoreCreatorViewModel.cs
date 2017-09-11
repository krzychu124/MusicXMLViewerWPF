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
using MusicXMLScore.Model.MeasureItems.Attributes;
using System;

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
        private static List<string> cleftype_ = new List<string> {
            MusicSymbols.CClef,
            MusicSymbols.GClef,
            MusicSymbols.FClef
        };
        private static ObservableCollection<string> keysymbollist = new ObservableCollection<string>();
        private static PreviewCanvas previewcanvas;
        private Canvas canvaslist = new Canvas();
        private Dictionary<string, ClefSignMusicXML> cleftype = new Dictionary<string, ClefSignMusicXML> {
            [MusicSymbols.GClef] = ClefSignMusicXML.G,
            [MusicSymbols.FClef] = ClefSignMusicXML.F,
            [MusicSymbols.CClef] = ClefSignMusicXML.C
        };
        private ClefTypeOptions currentclefoption = ClefTypeOptions.regularclef;
        private TimeSymbolMusicXML currenttimesig = TimeSymbolMusicXML.normal;
        private bool customsetting;
        private PreviewCanvas keypreview = new PreviewCanvas();
        private int measurescount = 32;
        private string selclef = cleftype_[1];
        private KeyValuePair<string, ClefSignMusicXML> selectedclef = new KeyValuePair<string, ClefSignMusicXML>(MusicSymbols.GClef, ClefSignMusicXML.G);
        private string selectedkeymode = "Major";
        private string selectedkeysymbol;
        private string selectedkeytype = "Flat";
        private KeyValuePair<int, TimeSigBeatTime> selectedtimebeats = new KeyValuePair<int, TimeSigBeatTime>(4, TimeSigBeatTime.four);
        private readonly Dictionary<int, TimeSigBeatTime> timebeatlist = new Dictionary<int, TimeSigBeatTime>
        {
            [1] = TimeSigBeatTime.one,
            [2] = TimeSigBeatTime.two,
            [4] = TimeSigBeatTime.four,
            [8] = TimeSigBeatTime.eight,
            [16] = TimeSigBeatTime.sixteen,
            [24] = TimeSigBeatTime.twentyfour,
            [32] = TimeSigBeatTime.thirtytwo,
            [64] = TimeSigBeatTime.sixtyfour
        };
        private uint timesigtimeval = 4;


        public NewScoreCreatorViewModel()
        {
            PropertyChanged += NewScoreCreatorViewModel_PropertyChanged;
            OptionsWindowCommand = new RelayCommand(OnOpionsWindow, () => CustomSettings);
            AddVisualCommand = new RelayCommand(OnAddVisual);
            CanvasClick = new RelayCommand(OnCanvasClick);
            InitPreview();
            SetKeySymbolList();
        }

        public RelayCommand AddVisualCommand { get; set; }
        public RelayCommand CanvasClick { get; set; }
        public List<string> ClefType { get { return cleftype_; } }
        public Dictionary<ImageSource, ClefType> ClefTypeList { get; set; }
        public Dictionary<string, ClefSignMusicXML> ClefTypeListS { get { return cleftype; } }
        public Canvas ConfigurationPreview { get { return canvaslist; } }
        public ClefTypeOptions CurrentClefOption { get { return currentclefoption; } set { if (value != currentclefoption) { Set(nameof(CurrentClefOption), ref currentclefoption, value); } } }
        public TimeSymbolMusicXML CurrentTimeSigOption { get { return currenttimesig; } set { if (value != currenttimesig) { Set(nameof(CurrentTimeSigOption), ref currenttimesig, value); } } }
        public bool CustomSettings { get { return customsetting; } set { if (value != customsetting) { customsetting = value; OptionsWindowCommand.RiseCanExecuteChanged(); } } }
        public PreviewCanvas KeyPreview { get { return keypreview; } }
        public ObservableCollection<string> KeySymbolList { get { return keysymbollist; } set { if (keysymbollist != value) { Set(nameof(KeySymbolList), ref keysymbollist, value); } } }
        public int MeasuresCount { get { return measurescount; } set { measurescount = value; } }
        public RelayCommand OptionsWindowCommand { get; set; }
        public PreviewCanvas PreviewCanvas { get { return previewcanvas; } set { previewcanvas = value; } }
        public string SelectedClef { get { return selclef; } set { if (selclef != value) { Set(nameof(SelectedClef), ref selclef, value); } } }
        public KeyValuePair<string, ClefSignMusicXML> SelectedClefS { get { return selectedclef; } set { { Set(nameof(SelectedClefS), ref selectedclef, value); } } }
        public KeyValuePair<ImageSource, ClefSignMusicXML> SelectedClefType { get; set; }
        public string SelectedKeyMode { get { return selectedkeymode; } set { if (selectedkeymode != value) Set(nameof(SelectedKeyMode), ref selectedkeymode, value); } }
        public string SelectedKeySymbol { get { return selectedkeysymbol; } set { if (selectedkeysymbol != value) { Set(nameof(SelectedKeySymbol), ref selectedkeysymbol, value); } } }
        public string SelectedKeyType { get { return selectedkeytype; } set { if (selectedkeytype != value) { Set(nameof(SelectedKeyType), ref selectedkeytype, value); } } }
        public KeyValuePair<int, TimeSigBeatTime> SelectedTimeBeats { get { return selectedtimebeats; } set { Set(nameof(SelectedTimeBeats), ref selectedtimebeats, value); } }
        public Dictionary<int, TimeSigBeatTime> TimeBeatList { get { return timebeatlist; } }
        public uint TimeSigTime { get { return timesigtimeval; } set { if (value != timesigtimeval) { if (timesigtimeval != value) { Set(nameof(TimeSigTime), ref timesigtimeval, value); } } } }
        public string TimeSigTimeSource { get; set; }

        public DrawingVisual AddVis()
        {
            var visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                Helpers.DrawingHelpers.DrawString(dc, "test2", TypeFaces.TextFont, Brushes.Black, 35f, 45f, 20f);
            }
            return visual;
        }

        private static void OnOpionsWindow()
        {
            ConfigurationView optionswindow = new ConfigurationView();
            optionswindow.ShowDialog();
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
                case "SelectedClefS":
                    UpdatePreview();
                    break;
                case "TimeSigTime":
                    UpdatePreview();
                    break;
                case "CurrentTimeSigOption":
                    UpdatePreview();
                    break;
                case nameof(CurrentClefOption):
                    UpdatePreview();
                    break;
                default:
                    break;
            }
        }

        private void OnAddVisual()
        {
            //PreviewCanvas.AddVisual(AddVis());
            keypreview.StaffLine();
        }

        private void OnCanvasClick() //! test
        {
            //LoggIt.Log("test");
            //LoggIt.Log("error1", LogType.Warning);
            //LoggIt.Log("warning occured here ------------------------------------------------>", LogType.Warning);
            //for (int i = 0; i < 200; i++)
            //{
            //    LoggIt.Log($"test {i}", LogType.Info);
            //    LoggIt.Log($"test {i}", LogType.Warning);
            //    LoggIt.Log($"test {i}", LogType.Error);
            //}
            //MessageBox.Show(SimpleLogger.SimpleLog.NumberOfLogEntriesWaitingToBeWrittenToFile.ToString() + " " + canvaslist.Count);
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

        private void TestPrototype()
        {
            canvaslist.Children.Clear();
            var staff = new RegularStaff(5, 50.0.TenthsToWPFUnit(), 180.0.TenthsToWPFUnit());
            staff.Update();
            var currentTime = GetSelectedTime();
            var attributes = new MeasureAttributes(
                true,
                new MeasureClef(SelectedClefS.Value, MusicSymbols.GetClefDefaulLine(SelectedClefS.Value), 0, staff),
                new MeasureKey(),
                new MeasureTime(currentTime.Item1.ToString(), currentTime.Item2.ToString(), currentTime.Item3,staff)
                );
            attributes.Update();
            var measureContent = new StandardMeasureContent(
                attributes,
                new MeasureRest(staff.DesiredWidth - attributes.GetVisualWidth(), staff)
                );
            measureContent.Update();
            var measure = new StandardMeasure("0", "0", staff, staff.DesiredWidth, measureContent);
            canvaslist.Children.Add(measure.GetVisualControl());
        }

        private void UpdatePreview()
        {
            TestPrototype();
        }

        private Tuple<int, int, TimeSymbolMusicXML> GetSelectedTime()
        {
            if (CurrentTimeSigOption== TimeSymbolMusicXML.normal)
            {
                return new Tuple<int, int, TimeSymbolMusicXML>((int)TimeSigTime, (int)SelectedTimeBeats.Value, TimeSymbolMusicXML.normal);
            }
            if (CurrentTimeSigOption == TimeSymbolMusicXML.common)
            {
                return new Tuple<int, int, TimeSymbolMusicXML>(4, 4, TimeSymbolMusicXML.common);
            }
            if (CurrentTimeSigOption == TimeSymbolMusicXML.cut)
            {
                return new Tuple<int, int, TimeSymbolMusicXML>(2, 2, TimeSymbolMusicXML.cut);
            }
            return new Tuple<int, int, TimeSymbolMusicXML>(4,4, TimeSymbolMusicXML.normal);
        }
    }
}
