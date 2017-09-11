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

    enum ClefTypeOptions
    {
        regularclef,
        tablature
    }

    class NewScoreCreatorViewModel : ViewModelBase
    {
        private readonly List<string> clefTypes = new List<string> {
            MusicSymbols.CClef,
            MusicSymbols.GClef,
            MusicSymbols.FClef
        };
        private ObservableCollection<string> keysymbollist = new ObservableCollection<string>();
        private readonly Canvas canvaslist = new Canvas();
        private readonly Dictionary<string, ClefSignMusicXML> cleftype = new Dictionary<string, ClefSignMusicXML>
        {
            [MusicSymbols.GClef] = ClefSignMusicXML.G,
            [MusicSymbols.FClef] = ClefSignMusicXML.F,
            [MusicSymbols.CClef] = ClefSignMusicXML.C
        };
        private ClefTypeOptions currentclefoption = ClefTypeOptions.regularclef;
        private TimeSymbolMusicXML currenttimesig = TimeSymbolMusicXML.normal;
        private bool customsetting;
        private int measurescount = 32;
        private string selectedClef;
        private KeyValuePair<string, ClefSignMusicXML> selectedClefPair = new KeyValuePair<string, ClefSignMusicXML>(MusicSymbols.GClef, ClefSignMusicXML.G);
        private string selectedKeyMode = "Major";
        private string selectedKeySymbol;
        private string selectedKeyType = "Flat";
        private KeyValuePair<int, TimeSigBeatTime> selectedTimeBeatsPair = new KeyValuePair<int, TimeSigBeatTime>(4, TimeSigBeatTime.four);
        private readonly Dictionary<int, TimeSigBeatTime> timeBeatTable = new Dictionary<int, TimeSigBeatTime>
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
        private uint timeSignatureTimeValue = 4;
        private bool fillPage;

        public NewScoreCreatorViewModel()
        {
            PropertyChanged += NewScoreCreatorViewModel_PropertyChanged;
            selectedClef = clefTypes[1];
            OptionsWindowCommand = new RelayCommand(OnOpionsWindow, () => CustomSettings);
            FinishCommand = new RelayCommand(OnFinish);
            CancelCommand = new RelayCommand(OnCancel);
            InitPreview();
            SetKeySymbolList();
        }
        private void OnFillPage()
        {
            Console.WriteLine($"Fill Page clicked, current value {fillPage}");
        }

        private void OnCancel()
        {
            Console.WriteLine("Cancel clicked");
        }

        public RelayCommand CancelCommand { get; set; }
        public RelayCommand FinishCommand { get; set; }
        public List<string> ClefType { get { return clefTypes; } }
        public Dictionary<string, ClefSignMusicXML> ClefTypeListS { get { return cleftype; } }
        public Canvas ConfigurationPreview { get { return canvaslist; } }
        public ClefTypeOptions CurrentClefOption { get { return currentclefoption; } set { Set(nameof(CurrentClefOption), ref currentclefoption, value); } }
        public TimeSymbolMusicXML CurrentTimeSigOption { get { return currenttimesig; } set { Set(nameof(CurrentTimeSigOption), ref currenttimesig, value); } }
        public bool CustomSettings { get { return customsetting; } set { if (value != customsetting) { customsetting = value; OptionsWindowCommand.RiseCanExecuteChanged(); } } }
        public ObservableCollection<string> KeySymbolList { get { return keysymbollist; } set { Set(nameof(KeySymbolList), ref keysymbollist, value); } }
        public int MeasuresCount { get { return measurescount; } set { measurescount = value; } }
        public RelayCommand OptionsWindowCommand { get; set; }
        public string SelectedClef { get { return selectedClef; } set {  Set(nameof(SelectedClef), ref selectedClef, value);  } }
        public KeyValuePair<string, ClefSignMusicXML> SelectedClefS { get { return selectedClefPair; } set { { Set(nameof(SelectedClefS), ref selectedClefPair, value); } } }
        public string SelectedKeyMode { get { return selectedKeyMode; } set { Set(nameof(SelectedKeyMode), ref selectedKeyMode, value); } }
        public string SelectedKeySymbol { get { return selectedKeySymbol; } set { Set(nameof(SelectedKeySymbol), ref selectedKeySymbol, value); }  }
        public string SelectedKeyType { get { return selectedKeyType; } set { Set(nameof(SelectedKeyType), ref selectedKeyType, value); } }
        public KeyValuePair<int, TimeSigBeatTime> SelectedTimeBeats { get { return selectedTimeBeatsPair; } set { Set(nameof(SelectedTimeBeats), ref selectedTimeBeatsPair, value); } }
        public Dictionary<int, TimeSigBeatTime> TimeBeatList { get { return timeBeatTable; } }
        public uint TimeSigTime { get { return timeSignatureTimeValue; } set { Set(nameof(TimeSigTime), ref timeSignatureTimeValue, value); } }
        public string TimeSigTimeSource { get; set; }
        public bool FillPage { get => fillPage; set => Set(nameof(FillPage), ref fillPage, value); }

        private static void OnOpionsWindow()
        {
            var optionswindow = new ConfigurationView();
            optionswindow.ShowDialog();
        }

        private void InitPreview()
        {
            UpdatePreview();
        }

        private void NewScoreCreatorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(FillPage):
                    Console.WriteLine($"Fill page clicked, current value: {fillPage}");
                    break;
                case nameof(SelectedKeyMode):
                case nameof(SelectedKeyType):
                    SetKeySymbolList();
                    RaisePropertyChanged(nameof(SelectedKeySymbol));
                    break;
                case nameof(KeySymbolList):
                    SelectedKeySymbol = KeySymbolList.ElementAt(0);
                    break;
                case nameof(SelectedKeySymbol):
                case nameof(SelectedClef):
                case nameof(SelectedTimeBeats):
                case nameof(SelectedClefS):
                case nameof(TimeSigTime):
                case nameof(CurrentTimeSigOption):
                case nameof(CurrentClefOption):
                    UpdatePreview();
                    break;
                default:
                    break;
            }
        }

        private void OnFinish()
        {
            Console.WriteLine("Finish Clicked");
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

        private void UpdatePreview()
        {
            canvaslist.Children.Clear();
            var staff = new RegularStaff(5, 50.0.TenthsToWPFUnit(), 480.0.TenthsToWPFUnit());
            staff.Update();
            var currentTime = GetSelectedTime();
            var attributes = new MeasureAttributes(
                true,
                new MeasureClef(SelectedClefS.Value, MusicSymbols.GetClefDefaulLine(SelectedClefS.Value), 0, staff),
                new MeasureKey(),
                new MeasureTime(currentTime.Item1.ToString(), currentTime.Item2.ToString(), currentTime.Item3, staff)
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
