using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
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

        private readonly Dictionary<int, Tuple<ClefSignMusicXML, string, string>> avaliableClefs = new Dictionary<int, Tuple<ClefSignMusicXML, string, string>>
        {
            { 1, Tuple.Create(ClefSignMusicXML.G, "2", "0") },
          { 2, Tuple.Create(ClefSignMusicXML.G, "1", "0")},
          { 3, Tuple.Create(ClefSignMusicXML.F , "4", "0" )},
          { 4, Tuple.Create(ClefSignMusicXML.F , "3", "0" )},
          { 5, Tuple.Create(ClefSignMusicXML.F , "5", "0" )},
          { 6, Tuple.Create(ClefSignMusicXML.C , "3", "0" )},
          { 7, Tuple.Create(ClefSignMusicXML.C , "4", "0" )},
          { 8, Tuple.Create(ClefSignMusicXML.C , "5", "0" )},
          { 9, Tuple.Create(ClefSignMusicXML.C , "2", "0" )},
          { 10, Tuple.Create(ClefSignMusicXML.C , "1", "0") }
        };
        private readonly Canvas canvaslist = new Canvas();
        private Canvas clefConfigurationPreview;
        private readonly Dictionary<string, ClefSignMusicXML> cleftype = new Dictionary<string, ClefSignMusicXML>
        {
            [MusicSymbols.GClef] = ClefSignMusicXML.G,
            [MusicSymbols.FClef] = ClefSignMusicXML.F,
            [MusicSymbols.CClef] = ClefSignMusicXML.C
        };
        private readonly List<string> clefTypes = new List<string> {
            MusicSymbols.CClef,
            MusicSymbols.GClef,
            MusicSymbols.FClef
        };
        private ClefTypeOptions currentclefoption = ClefTypeOptions.regularclef;
        private TimeSymbolMusicXML currenttimesig = TimeSymbolMusicXML.normal;
        private bool customsetting;
        private bool fillPage;
        private readonly ObservableCollection<string> keyMajorAny = new ObservableCollection<string> { "C", "G", "D", "A", "E", "B", "F\u266f", "C\u266f", "C", "F", "B\u266d", "E\u266d", "A\u266d", "D\u266d", "G\u266d", "C\u266d" };
        private readonly ObservableCollection<string> keyMajorFlat = new ObservableCollection<string> { "C", "F", "B\u266d", "E\u266d", "A\u266d", "D\u266d", "G\u266d", "C\u266d" };
        private readonly ObservableCollection<string> keyMajorSharp = new ObservableCollection<string> { "C", "G", "D", "A", "E", "B", "F\u266f", "C\u266f" };
        private readonly ObservableCollection<string> keyMinorAny = new ObservableCollection<string> { "a", "e", "b", "f\u266f", "c\u266f", "g\u266f", "d\u266f", "a\u266f", "a", "d", "g", "c", "f", "b\u266d", "e\u266d", "a\u266d" };
        private readonly ObservableCollection<string> keyMinorFlat = new ObservableCollection<string> { "a", "d", "g", "c", "f", "b\u266d", "e\u266d", "a\u266d" };
        private readonly ObservableCollection<string> keyMinorSharp = new ObservableCollection<string> { "a", "e", "b", "f\u266f", "c\u266f", "g\u266f", "d\u266f", "a\u266f" };
        private ObservableCollection<string> keysymbollist = new ObservableCollection<string>();
        private int measurescount = 32;
        private string selectedClef;
        private KeyValuePair<string, ClefSignMusicXML> selectedClefPair = new KeyValuePair<string, ClefSignMusicXML>(MusicSymbols.GClef, ClefSignMusicXML.G);
        private string selectedKeyMode = "Major";
        private string selectedKeySymbol;
        private string selectedKeyType = "Flat";
        private ClefMusicXML selectedPreviewClef;
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

        public NewScoreCreatorViewModel()
        {
            PropertyChanged += NewScoreCreatorViewModel_PropertyChanged;
            selectedClef = clefTypes[1];
            clefConfigurationPreview = new Canvas
            {
                ContextMenu = GetClefConfigContextMenu()
            };
            selectedPreviewClef = new ClefMusicXML
            {
                Sign = ClefSignMusicXML.G,
                Line = "2",
                ClefOctaveChange = "0"
            };
            GenerateClefPreview(selectedPreviewClef);
            OptionsWindowCommand = new RelayCommand(OnOpionsWindow, () => CustomSettings);
            FinishCommand = new RelayCommand(OnFinish);
            CancelCommand = new RelayCommand(OnCancel);
            InitPreview();
            SetKeySymbolList();
        }

        public RelayCommand CancelCommand { get; set; }
        public Canvas ClefConfigurationPreview { get { return clefConfigurationPreview; } }
        public List<string> ClefType { get { return clefTypes; } }
        public Dictionary<string, ClefSignMusicXML> ClefTypeListS { get { return cleftype; } }
        public Canvas ConfigurationPreview { get { return canvaslist; } }
        public ClefTypeOptions CurrentClefOption { get { return currentclefoption; } set { Set(nameof(CurrentClefOption), ref currentclefoption, value); } }
        public TimeSymbolMusicXML CurrentTimeSigOption { get { return currenttimesig; } set { Set(nameof(CurrentTimeSigOption), ref currenttimesig, value); } }
        public bool CustomSettings { get { return customsetting; } set { if (value != customsetting) { customsetting = value; OptionsWindowCommand.RiseCanExecuteChanged(); } } }
        public bool FillPage { get => fillPage; set => Set(nameof(FillPage), ref fillPage, value); }
        public RelayCommand FinishCommand { get; set; }
        public ObservableCollection<string> KeySymbolList { get { return keysymbollist; } set { Set(nameof(KeySymbolList), ref keysymbollist, value); } }
        public int MeasuresCount { get { return measurescount; } set { measurescount = value; } }
        public RelayCommand OptionsWindowCommand { get; set; }
        public string SelectedClef { get { return selectedClef; } set { Set(nameof(SelectedClef), ref selectedClef, value); } }
        public KeyValuePair<string, ClefSignMusicXML> SelectedClefS { get { return selectedClefPair; } set { { Set(nameof(SelectedClefS), ref selectedClefPair, value); } } }
        public string SelectedKeyMode { get { return selectedKeyMode; } set { Set(nameof(SelectedKeyMode), ref selectedKeyMode, value); } }
        public string SelectedKeySymbol { get { return selectedKeySymbol; } set { Set(nameof(SelectedKeySymbol), ref selectedKeySymbol, value); } }
        public string SelectedKeyType { get { return selectedKeyType; } set { Set(nameof(SelectedKeyType), ref selectedKeyType, value); } }
        public KeyValuePair<int, TimeSigBeatTime> SelectedTimeBeats { get { return selectedTimeBeatsPair; } set { Set(nameof(SelectedTimeBeats), ref selectedTimeBeatsPair, value); } }
        public Dictionary<int, TimeSigBeatTime> TimeBeatList { get { return timeBeatTable; } }
        public uint TimeSigTime { get { return timeSignatureTimeValue; } set { Set(nameof(TimeSigTime), ref timeSignatureTimeValue, value); } }
        public string TimeSigTimeSource { get; set; }

        private static void OnOpionsWindow()
        {
            var optionswindow = new ConfigurationView();
            optionswindow.ShowDialog();
        }

        private void ClefMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (e.Source is MenuItem)
            {
                var menuItem = e.Source as MenuItem;
                if (avaliableClefs.TryGetValue((int)menuItem.Tag, out var tuple))
                {

                    SetSelectedClefPreview(tuple.Item1, tuple.Item2, tuple.Item3);
                }
            }
        }
        private void GenerateClefPreview(ClefMusicXML clef)
        {
            clefConfigurationPreview.Children.Clear();
            var staff = new RegularStaff(5, 50.0.TenthsToWPFUnit(), 100.0.TenthsToWPFUnit());
            staff.Update();
            var currentTime = GetSelectedTime();
            var measureClef = new MeasureClef(clef.Sign, int.Parse(clef.Line), int.Parse(clef.ClefOctaveChange), staff);

            var measureKey = new MeasureKey(SelectedClefS.Value, MusicSymbols.GetClefDefaulLine(SelectedClefS.Value), 0);
            measureKey.SetStaff(staff);
            var attributes = new MeasureAttributes(
                true,
                measureClef,
                measureKey,
                new MeasureTime(currentTime.Item1.ToString(), currentTime.Item2.ToString(), currentTime.Item3, staff) { IsVisible = false }
                );
            attributes.Update();
            var measureContent = new StandardMeasureContent(
                attributes,
                new MeasureRest(staff.DesiredWidth - attributes.GetVisualWidth(), staff)
                );
            measureContent.Update();
            var measure = new StandardMeasure("0", "0", staff, staff.DesiredWidth, measureContent);
            clefConfigurationPreview.Children.Add(measure.GetVisualControl());
        }

        private ContextMenu GetClefConfigContextMenu()
        {
            var contextMenu = new ContextMenu();
            var menuItems1 = new MenuItem
            {
                Header = "G Clef",
                Items = {
                    new MenuItem{ Header = "Treble", Tag = 1},
                    new MenuItem{ Header = "French Violin", Tag = 2}
                }
            };
            menuItems1.Click += ClefMenuItem_Click;
            var menuItems2 = new MenuItem
            {
                Header = "F Clef",
                Items = {
                    new MenuItem{ Header = "Bass", Tag = 3},
                    new MenuItem{ Header = "Baritone", Tag = 4},
                    new MenuItem{ Header = "Sub-Bass", Tag = 5}
                }
            };
            menuItems2.Click += ClefMenuItem_Click;
            var menuItems3 = new MenuItem
            {
                Header = "C Clef",
                Items = {
                    new MenuItem{ Header = "Alto", Tag = 6},
                    new MenuItem{ Header = "Tenor", Tag = 7},
                    new MenuItem{ Header = "Baritone", Tag = 8},
                    new MenuItem{ Header = "Mezzo-soprano", Tag = 9},
                    new MenuItem{ Header = "Soprano", Tag = 10}
                }
            };
            menuItems3.Click += ClefMenuItem_Click;
            contextMenu.Items.Add(menuItems1);
            contextMenu.Items.Add(menuItems2);
            contextMenu.Items.Add(menuItems3);
            return contextMenu;
        }

        private int GetKeyFifths()
        {
            var keyFifths = KeySymbolList.IndexOf(selectedKeySymbol);
            keyFifths = SelectedKeyType.Equals("Flat") ? keyFifths * -1 : keyFifths;
            if (SelectedKeyType.Equals("Any"))
            {
                keyFifths = 0;
                var index = KeySymbolList.IndexOf(selectedKeySymbol);
                if (index != 0)
                {
                    // <1,8> is sharp symbol, <9,16> (converted to neg mod 8) ==> <-1,-8> is flat
                    keyFifths = index > 8 ? index % 8 * -1 : index;
                }
            }
            return keyFifths;
        }

        private Tuple<int, int, TimeSymbolMusicXML> GetSelectedTime()
        {
            if (CurrentTimeSigOption == TimeSymbolMusicXML.normal)
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
            return new Tuple<int, int, TimeSymbolMusicXML>(4, 4, TimeSymbolMusicXML.normal);
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

        private void OnCancel()
        {
            Console.WriteLine("Cancel clicked");
        }

        private void OnFillPage()
        {
            Console.WriteLine($"Fill Page clicked, current value {fillPage}");
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
                    KeySymbolList = keyMajorSharp;
                }
                if (SelectedKeyType == "Flat")
                {
                    KeySymbolList = keyMajorFlat;
                }
                if (SelectedKeyType == "Any")
                {
                    KeySymbolList = keyMajorAny;
                }
            }
            if (SelectedKeyMode == "Minor")
            {
                if (SelectedKeyType == "Sharp")
                {
                    KeySymbolList = keyMinorSharp;
                }
                if (SelectedKeyType == "Flat")
                {
                    KeySymbolList = keyMinorFlat;
                }
                if (SelectedKeyType == "Any")
                {
                    KeySymbolList = keyMinorAny;
                }
            }
        }

        private void SetSelectedClefPreview(ClefSignMusicXML clefSign, string line, string octaveChange)
        {
            selectedPreviewClef = new ClefMusicXML
            {
                Sign = clefSign,
                Line = line,
                ClefOctaveChange = octaveChange
            };
            if (selectedPreviewClef != null)
            {
                GenerateClefPreview(selectedPreviewClef);
            }
        }

        private void UpdatePreview()
        {
            canvaslist.Children.Clear();
            var staff = new RegularStaff(5, 50.0.TenthsToWPFUnit(), 480.0.TenthsToWPFUnit());
            staff.Update();
            var currentTime = GetSelectedTime();
            var measureClef = new MeasureClef(SelectedClefS.Value, MusicSymbols.GetClefDefaulLine(SelectedClefS.Value), 0, staff);

            var measureKey = new MeasureKey(SelectedClefS.Value, MusicSymbols.GetClefDefaulLine(SelectedClefS.Value), GetKeyFifths());
            measureKey.SetStaff(staff);
            var attributes = new MeasureAttributes(
                true,
                measureClef,
                measureKey,
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


    }
}
