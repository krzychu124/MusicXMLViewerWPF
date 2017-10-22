using MusicXMLScore.Converters;
using MusicXMLScore.Model.Defaults;
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.Model.MeasureItems.Attributes;
using MusicXMLScore.ScoreProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using MusicXMLViewerWPF;
using MusicXMLScore.Model;
using System.ComponentModel;

namespace MusicXMLScore.DrawingHelpers
{
    public class PartProperties : INotifyPropertyChanged //TODO_H refactor: reduce unnecessary dependecies (the best would be all)
    {
        private bool _systemAttributes = true;
        private ClefChangesDictionary _clefChanges = new ClefChangesDictionary();
        private Dictionary<string, ClefChangesDictionary> _clefPerStaff;
        private Dictionary<string, int> _divisionsAttributes;
        private Dictionary<string, Point> _coords;
        private double _defaultStaffDistance;
        private double _defaultSystemDistance;
        private double _defaultTopSystemDistance;
        private double _partHeight;
        private double _stavesDistance;
        private int _numberOfStaves = 1;
        private int _numberOfLines = 5;
        private int _partIndex;
        private KeyChangesDictionary _keyChanges = new KeyChangesDictionary();
        private List<List<List<string>>> _measuresPerSystemPerPage = new List<List<List<string>>>();
        private List<List<StaffLayoutMusicXML>> _staffLayoutPerPage = new List<List<StaffLayoutMusicXML>>();
        private List<List<string>> _measuresPerSystem = new List<List<string>>();
        private List<List<SystemLayoutMusicXML>> _systemLayoutPerPage = new List<List<SystemLayoutMusicXML>>();
        private List<List<Tuple<string, string>>> _partSysemsInPages;
        private List<MeasureNumberingMusicXML> _measureNumbering = new List<MeasureNumberingMusicXML>();
        private List<StaffLayoutMusicXML> _staffLayout = new List<StaffLayoutMusicXML>();
        private List<SystemLayoutMusicXML> _systemLayout = new List<SystemLayoutMusicXML>();
        private ScorePartwisePartMusicXML _currentPart;
        private string _partId;
        private TimeChangesDictionary _timeChanges = new TimeChangesDictionary();

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public PartProperties(ScorePartwiseMusicXML score, string partId)
        {
            SetMainFields(score, partId);
            SetDefaultDistances(score);
            List<ScorePartwisePartMeasureMusicXML> measuresInPart = score.Part[_partIndex].Measure;
            GetLayoutInfo(measuresInPart);
            SetSystemMeasureRanges();
            SetPartHeight();
            GenerateDivisionChanges();
        }

        public ClefChangesDictionary ClefChanges
        {
            get { return _clefChanges; }

            set { _clefChanges = value; }
        }

        public Dictionary<string, ClefChangesDictionary> ClefPerStaff
        {
            get { return _clefPerStaff; }

            set { _clefPerStaff = value; }
        }

        public Dictionary<string, Point> Coords
        {
            get
            {
                return _coords;
                //! return new Dictionary<string, Point>(measuresPerSystem.SelectMany((x,i) => x).ToDictionary(item => item, item=> new Point()));
                //! temporary empty list of point 
            }

            set { _coords = value; }
        }

        public KeyChangesDictionary KeyChanges
        {
            get { return _keyChanges; }

            set { _keyChanges = value; }
        }

        public List<List<string>> MeasuresPerSystem
        {
            get { return _measuresPerSystem; }

            set { _measuresPerSystem = value; }
        }

        public List<List<List<string>>> MeasuresPerSystemPerPage
        {
            get { return _measuresPerSystemPerPage; }

            set { _measuresPerSystemPerPage = value; }
        }

        public int NumberOfStaves
        {
            get { return _numberOfStaves; }

            set { _numberOfStaves = value; }
        }

        /// <summary>
        /// Part height in Tenths
        /// </summary>
        public double PartHeight
        {
            get { return _partHeight; }

            set { _partHeight = value; }
        }

        public List<List<Tuple<string, string>>> PartSysemsInPages
        {
            get { return _partSysemsInPages; }

            set { _partSysemsInPages = value; }
        }

        public List<StaffLayoutMusicXML> StaffLayout
        {
            get { return _staffLayout; }

            set { _staffLayout = value; }
        }

        public List<List<StaffLayoutMusicXML>> StaffLayoutPerPage
        {
            get { return _staffLayoutPerPage; }

            set { _staffLayoutPerPage = value; }
        }

        public List<List<SystemLayoutMusicXML>> SystemLayoutPerPage
        {
            get { return _systemLayoutPerPage; }

            set { _systemLayoutPerPage = value; }
        }

        public TimeChangesDictionary TimeChanges
        {
            get { return _timeChanges; }

            set { _timeChanges = value; }
        }

        public int NumberOfLines
        {
            get { return _numberOfLines; }

            set
            {
                _numberOfLines = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(NumberOfLines)));
            }
        }

        /// <summary>
        /// Adds clef to each new System if not specified inside score
        /// </summary>
        public void AddAttributesToEachSystem()
        {
            if (_systemAttributes)
            {
                List<string> systems = _measuresPerSystem.Select(x => x[0]).ToList();
                foreach (var item in systems)
                {
                    if (!ClefChanges.ContainsKey(item))
                    {
                        ClefChanges clefs = new ClefChanges();
                        for (int i = 1; i <= _numberOfStaves; i++)
                        {
                            var clef = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetClef(item, _partId, i, 0);
                            clefs.Add(i.ToString(), 0, clef);
                        }
                        ClefChanges.Add(item, clefs);
                    }
                    if (ClefChanges.ContainsKey(item))
                    {
                        ClefChanges clefs = ClefChanges[item];
                        if (clefs.AttributeChanges.All(x => x.TimeFraction != 0))
                        {
                            for (int i = 1; i <= _numberOfStaves; i++)
                            {
                                var clef = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetClef(item, _partId, i, 0);
                                clefs.Add(i.ToString(), 0, clef);
                            }
                        }
                    }
                    if (!KeyChanges.ContainsKey(item))
                    {
                        KeyChanges keys = new KeyChanges();
                        for (int i = 1; i <= _numberOfStaves; i++)
                        {
                            var key = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetKeySignature(item, _partId);
                            keys.Add(i.ToString(), 0, key);
                        }
                        KeyChanges.Add(item, keys);
                    }
                    if (KeyChanges.ContainsKey(item))
                    {
                        KeyChanges keys = KeyChanges[item];
                        if (keys.AttributeChanges.All(x => x.TimeFraction != 0))
                        {
                            for (int i = 1; i <= _numberOfStaves; i++)
                            {
                                var key = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetKeySignature(item, _partId);
                                keys.Add(i.ToString(), 0, key);
                            }
                        }
                    }
                }
            }
        }

        public void GenerateAttributes(TimeSignatures timeSignatures)
        {
            string firsMeasureId = _currentPart.Measure.FirstOrDefault().Number;
            foreach (var measure in _currentPart.Measure)
            {
                var currentTimeSig = timeSignatures.GetTimeSignature(measure.Number);
                int numerator = currentTimeSig.GetNumerator();
                int denominator = currentTimeSig.GetDenominator();
                int divisions = GetDivisionsMeasureId(measure.Number);
                //int maxDuration = (int)((4 / (double)denominator) * (divisions * numerator));
                int fractionCursor = 0;

                for (int i = 0; i < measure.Items.Length; i++)
                {
                    string typeName = measure.Items[i].GetType().Name;

                    switch (typeName)
                    {
                        case nameof(AttributesMusicXML):
                            if (firsMeasureId == measure.Number && fractionCursor == 0)
                            {
                                AttributesChanged(measure.Items[i] as AttributesMusicXML, fractionCursor, measure.Number, true);
                            }
                            else
                            {
                                AttributesChanged(measure.Items[i] as AttributesMusicXML, fractionCursor, measure.Number);
                            }
                            break;
                        case nameof(BackupMusicXML):
                            BackupMusicXML b = (BackupMusicXML) measure.Items[i];
                            fractionCursor -= (int) b.Duration;
                            break;
                        case nameof(ForwardMusicXML):
                            ForwardMusicXML f = (ForwardMusicXML) measure.Items[i];
                            fractionCursor += (int) f.Duration;
                            break;
                        case nameof(NoteMusicXML):
                            NoteMusicXML n = (NoteMusicXML) measure.Items[i];
                            fractionCursor += n.IsChord() || n.IsGrace() ? 0 : n.GetDuration();
                            break;
                    }
                }
            }
            GenerateClefPerStaffDictionary();
        }

        public int GetDivisionsMeasureId(string measureId)
        {
            string resultKey = _divisionsAttributes.Keys.FirstOrDefault();
            if (measureId.Contains('X'))
            {
                measureId = measureId.Substring(1);
            }
            resultKey = _divisionsAttributes.LastOrDefault(i => int.Parse(i.Key) <= int.Parse(measureId)).Key;
            return _divisionsAttributes[resultKey];
        }

        private void AttributesChanged(AttributesMusicXML attributes, int cursorPosition, string measureNumber, bool firstMeasure = false)
        {
            ClefChanges clefs = new ClefChanges();
            KeyChanges keys = new KeyChanges();
            TimeChanges times = new TimeChanges();
            //! search for clefs
            if (attributes.Clef.Count == 0 && firstMeasure)
            {
                for (var i = 1; i <= _numberOfStaves; i++)
                {
                    clefs.Add(i.ToString(), cursorPosition, new ClefMusicXML() {Sign = ClefSignMusicXML.G, Line = 2.ToString()});
                }
            }
            else
            {
                for (var i = 0; i < attributes.Clef.Count; i++)
                {
                    clefs.Add(attributes.Clef[i].Number, cursorPosition, attributes.Clef[i]);
                }
            }
            //! search for key signatures
            if (attributes.Key.Count == 0 && firstMeasure)
            {
                for (int i = 1; i <= _numberOfStaves; i++)
                {
                    keys.Add(i.ToString(), cursorPosition,
                        new KeyMusicXML {Items = new object[] {0.ToString()}, ItemsElementName = new[] {KeyChoiceTypes.fifths}});
                }
            }
            else
            {
                for (int i = 0; i < attributes.Key.Count; i++)
                {
                    keys.Add(attributes.Key[i].Number, cursorPosition, attributes.Key[i]);
                }
            }
            //! search for time signatures
            if (attributes.Time.Count == 0 && firstMeasure)
            {
                for (int i = 1; i <= _numberOfStaves; i++)
                {
                    times.Add(i.ToString(), cursorPosition,
                        new TimeMusicXML
                        {
                            Items = new object[] {"4", "4"},
                            ItemsElementName = new[] {TimeChoiceTypeMusicXML.beats, TimeChoiceTypeMusicXML.beattype}
                        });
                }
            }
            else
            {
                for (int i = 0; i < attributes.Time.Count; i++)
                {
                    times.Add(attributes.Time[i].Number, cursorPosition, attributes.Time[i]);
                }
            }

            if (clefs.AttributeChanges.Count != 0)
            {
                _clefChanges.Add(measureNumber, clefs);
            }

            if (keys.AttributeChanges.Count != 0)
            {
                _keyChanges.Add(measureNumber, keys);
            }

            if (times.AttributeChanges.Count != 0)
            {
                _timeChanges.Add(measureNumber, times);
            }
        }

        private void GenerateClefPerStaffDictionary()
        {
            if (_clefPerStaff == null)
            {
                _clefPerStaff = new Dictionary<string, ClefChangesDictionary>();
            }
            else
            {
                _clefPerStaff.Clear();
            }
            for (int i = 1; i <= _numberOfStaves; i++)
            {
                var clefs = _clefChanges.Select(
                        (x, z) => new
                        {
                            x = x.Key,
                            z = x.Value.AttributeChanges.Where(c => c.StaffNumber == i.ToString())
                        })
                    .Where(x => x.z.FirstOrDefault() != null);

                ClefChangesDictionary ccdict = new ClefChangesDictionary();
                foreach (var item in clefs)
                {
                    ClefChanges cc = new ClefChanges();
                    foreach (var c in item.z)
                    {
                        cc.Add(c.StaffNumber, c.TimeFraction, c.AttributeEntity);
                    }
                    ccdict.Add(item.x, cc);
                }
                _clefPerStaff.Add(i.ToString(), ccdict);
            }
        }

        private void GenerateDivisionChanges() //TODO_Later refactor based on measure duration fraction
        {
            _divisionsAttributes = new Dictionary<string, int>();
            var firstMeasure = _currentPart.Measure.FirstOrDefault();
            if (firstMeasure.Items.OfType<AttributesMusicXML>().FirstOrDefault().DivisionsSpecified)
            {
                var divisionsValue = firstMeasure.Items.OfType<AttributesMusicXML>().FirstOrDefault().Divisions;
                _divisionsAttributes.Add(firstMeasure.Number, int.Parse(divisionsValue.ToString()));
            }
            for (int i = 1; i < _currentPart.Measure.Count; i++)
            {
                var measure = _currentPart.Measure[i];
                if (measure.Items.OfType<AttributesMusicXML>().FirstOrDefault()?.DivisionsSpecified != null
                    ? measure.Items.OfType<AttributesMusicXML>().FirstOrDefault().DivisionsSpecified
                    : false)
                {
                    _divisionsAttributes.Add(measure.Number,
                        int.Parse(measure.Items.OfType<AttributesMusicXML>().FirstOrDefault().Divisions.ToString()));
                }
            }
        }

        //! Refactor needed - missing default layout if Loaded score does not contain any layout supporting print elements
        private void GetLayoutInfo(List<ScorePartwisePartMeasureMusicXML> part)
        {
            int currPageListIndex = 0;
            int previousPageListIndex = 0;
            foreach (var item in _partSysemsInPages) //! working properly only if score contains layout elements
            {
                int currentSystemIndex = 0;
                foreach (var measureNumber in item)
                {
                    var measure = part.First(i => i.Number == measureNumber.Item1);
                    var printLayouts = measure.Items.OfType<PrintMusicXML>().FirstOrDefault();
                    if (printLayouts != null)
                    {
                        if (printLayouts.SystemLayout != null)
                        {
                            UpdateListWithObjectsOfType(printLayouts.SystemLayout, ref _systemLayout);
                        }
                        else
                        {
                            _systemLayout.Add(new SystemLayoutMusicXML
                            {
                                SystemDistance = _defaultSystemDistance,
                                TopSystemDistance = _defaultTopSystemDistance
                            });
                        }
                        if (printLayouts.MeasureNumbering != null)
                        {
                            UpdateListWithObjectsOfType(printLayouts.MeasureNumbering, ref _measureNumbering);
                        }
                        if (printLayouts.StaffLayout.Count != 0)
                        {
                            if (printLayouts.StaffLayout.Count != 0)
                            {
                                UpdateListWithObjectsOfType(printLayouts.StaffLayout.ElementAtOrDefault(0), ref _staffLayout);
                            }
                        }
                        else
                        {
                            _staffLayout.Add(new StaffLayoutMusicXML {Number = _numberOfStaves.ToString(), StaffDistance = _defaultStaffDistance});
                        }
                    }
                    if (_systemLayout.Count == 0)
                    {
                        _systemLayout.Add(new SystemLayoutMusicXML
                        {
                            SystemDistance = _defaultSystemDistance,
                            TopSystemDistance = _defaultTopSystemDistance
                        });
                    }
                    if (_systemLayout.Count < currentSystemIndex + 1)
                    {
                        _systemLayout.Add(new SystemLayoutMusicXML
                        {
                            SystemDistance = _defaultSystemDistance,
                            SystemMargins = _systemLayout.LastOrDefault().SystemMargins
                        });
                    }
                    if (_staffLayout.Count == 0)
                    {
                        _staffLayout.Add(new StaffLayoutMusicXML {Number = _numberOfStaves.ToString(), StaffDistance = _defaultStaffDistance});
                    }
                    if (_staffLayout.Count < currentSystemIndex + 1)
                    {
                        _staffLayout.Add(_staffLayout.LastOrDefault());
                    }
                    if (_measureNumbering.Count == 0)
                    {
                        _measureNumbering.Add(new MeasureNumberingMusicXML {Value = MeasureNumberingValueMusicXML.system});
                    }
                    if (_measureNumbering.Count != _systemLayout.Count)
                    {
                        _measureNumbering.Add(_measureNumbering.LastOrDefault());
                    }
                    currentSystemIndex++;
                }
                currPageListIndex = _systemLayout.Count - previousPageListIndex;
                _systemLayoutPerPage.Add(_systemLayout.GetRange(previousPageListIndex, currPageListIndex));
                _staffLayoutPerPage.Add(_staffLayout.GetRange(previousPageListIndex, currPageListIndex));
                previousPageListIndex += currPageListIndex;
            }
            _stavesDistance = _staffLayout[0].StaffDistance;
        }

        private void SetDefaultDistances(ScorePartwiseMusicXML score)
        {
            var layout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout;
            _defaultSystemDistance = 2.5 * layout.PageProperties.StaffHeight.MMToTenths();
            _defaultTopSystemDistance = 3 * layout.PageProperties.StaffHeight.MMToTenths();
            _defaultStaffDistance = 1.7 * layout.PageProperties.StaffHeight.MMToTenths();
            if (score.Defaults != null)
            {
                if (score.Defaults.StaffLayout != null)
                {
                    if (score.Defaults.StaffLayout.Count != 0)
                    {
                        _defaultStaffDistance = score.Defaults.StaffLayout[0].StaffDistance;
                    }
                }
                if (score.Defaults.SystemLayout != null)
                {
                    if (score.Defaults.SystemLayout.SystemDistanceSpecified)
                    {
                        _defaultSystemDistance = score.Defaults.SystemLayout.SystemDistance;
                    }
                    if (score.Defaults.SystemLayout.TopSystemDistanceSpecified)
                    {
                        _defaultTopSystemDistance = score.Defaults.SystemLayout.TopSystemDistance;
                    }
                }
            }
        }

        private void SetMainFields(ScorePartwiseMusicXML score, string partIdValue)
        {
            _partId = partIdValue;
            _currentPart = score.Part.FirstOrDefault(i => i.Id == _partId);
            _partIndex = score.Part.FindIndex(i => i.Id == _partId);
            FindPartSystems(score);
            _numberOfStaves = score.Part[_partIndex].GetStavesCount();
        }

        private void FindPartSystems(ScorePartwiseMusicXML score)
        {
            _partSysemsInPages = score.Part[_partIndex].TryGetLinesPerPage();
        }

        private void SetPartHeight()
        {
            double staffHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.StaffHeight.MMToTenths();
            _partHeight = staffHeight * _numberOfStaves + _stavesDistance * (_numberOfStaves - 1);
        }

        public void SetSystemMeasureRanges()
        {
            LayoutControl.LayoutGeneral currentLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout;
            double defaultLeftMargin = currentLayout.PageMargins.LeftMargin;
            double defaultTopMargin = currentLayout.PageMargins.TopMargin;
            var part = _currentPart;
            _measuresPerSystem = new List<List<string>>();
            foreach (var page in _partSysemsInPages)
            {
                List<List<string>> tempList = new List<List<string>>();
                foreach (var item2 in page)
                {
                    _measuresPerSystem.Add(part.TryGetMeasuresIdRange(item2));
                    tempList.Add(part.TryGetMeasuresIdRange(item2));
                }
                _measuresPerSystemPerPage.Add(new List<List<string>>(tempList));
            }
            double previousWidth = 0.0;
            double currentLineY = 0.0;
            _coords = new Dictionary<string, Point>();
            foreach (var measuresLine in _measuresPerSystem)
            {
                int systemIndex = _measuresPerSystem.IndexOf(measuresLine);
                if (systemIndex == 0)
                {
                    previousWidth = 0.0;

                    foreach (var measureId in measuresLine)
                    {
                        _coords.Add(measureId, new Point(previousWidth, currentLineY));
                        previousWidth += part.GetMeasureUsingId(measureId).CalculatedWidth.TenthsToWPFUnit();
                    }
                }
                else
                {
                    previousWidth = 0.0;
                    foreach (var measureId in measuresLine)
                    {
                        _coords.Add(measureId, new Point(previousWidth, currentLineY));
                        previousWidth += part.GetMeasureUsingId(measureId).CalculatedWidth.TenthsToWPFUnit();
                    }
                }
            }
        }

        /// <summary>
        /// Compare and update differences inside layout settings list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newerLayout"></param>
        /// <param name="listToUpdate"></param>
        private void UpdateListWithObjectsOfType<T>(T newerLayout, ref List<T> listToUpdate)
        {
            T currentLayout = Activator.CreateInstance<T>();
            T oldSystemLayout = listToUpdate.LastOrDefault();
            if (listToUpdate.Count == 0)
            {
                listToUpdate.Add(newerLayout);
                return;
            }
            if (ReferenceEquals(oldSystemLayout, newerLayout))
            {
                T copy = currentLayout;
                if (copy != null)
                {
                    XmlSerializer xmls = new XmlSerializer(copy.GetType());
                    System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                    xmls.Serialize(memStream, copy);

                    memStream.Position = 0;
                    xmls = new XmlSerializer(copy.GetType());
                    copy = (T) xmls.Deserialize(memStream);
                }
                listToUpdate.Add(copy);
            }
            else
            {
                foreach (var property in oldSystemLayout.GetType().GetProperties())
                {
                    var currentObjValue = property.GetValue(oldSystemLayout);
                    var newObjValue = property.GetValue(newerLayout) ?? currentObjValue;
                    if (currentObjValue?.Equals(newObjValue) ?? true)
                    {
                        property.SetValue(currentLayout, currentObjValue);
                    }
                    else
                    {
                        property.SetValue(currentLayout, newObjValue);
                    }
                }
                listToUpdate.Add(currentLayout);
            }
        }
    }
}