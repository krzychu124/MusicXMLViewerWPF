using MusicXMLScore.Converters;
using MusicXMLScore.Model.Defaults;
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.Model.MeasureItems.Attributes;
using MusicXMLScore.ScoreProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;
using MusicXMLViewerWPF;
using MusicXMLScore.Model;

namespace MusicXMLScore.DrawingHelpers
{
    public class PartProperties //TODO_H refactor: reduce unnecessary dependecies (the best would be all)
    {

        #region Fields

        private bool systemAttributes = true;
        private ClefChangesDictionary clefChanges = new ClefChangesDictionary();
        private Dictionary<string, ClefChangesDictionary> clefPerStaff;
        private Dictionary<string, int> divisionsAttributes;
        private Dictionary<string, Point> coords;
        private double defaultStaffDistance = 0.0;
        private double defaultSystemDistance = 0.0;
        private double defaultTopSystemDistance = 0.0;
        private double partHeight = 0;
        private double stavesDistance = 0.0;
        private int numberOfStaves = 1;
        private int partIndex = 0;
        private KeyChangesDictionary keyChanges = new KeyChangesDictionary();
        private List<List<List<string>>> measuresPerSystemPerPage = new List<List<List<string>>>();
        private List<List<MeasureNumberingMusicXML>> measureNumberingPerPage = new List<List<MeasureNumberingMusicXML>>();
        private List<List<StaffLayoutMusicXML>> staffLayoutPerPage = new List<List<StaffLayoutMusicXML>>();
        private List<List<string>> measuresPerSystem = new List<List<string>>();
        private List<List<SystemLayoutMusicXML>> systemLayoutPerPage = new List<List<SystemLayoutMusicXML>>();
        private List<List<Tuple<string, string>>> partSysemsInPages;
        private List<MeasureNumberingMusicXML> measureNumbering = new List<MeasureNumberingMusicXML>();
        private List<StaffLayoutMusicXML> staffLayout = new List<StaffLayoutMusicXML>();
        private List<SystemLayoutMusicXML> systemLayout = new List<SystemLayoutMusicXML>();
        private List<Tuple<string, string>> measuresRangeInPartSystem = new List<Tuple<string, string>>();
        private ScorePartwisePartMusicXML currentPart;
        private string partId;
        private TimeChangesDictionary timeChanges = new TimeChangesDictionary();

        #endregion Fields

        #region Constructors

        public PartProperties(ScorePartwiseMusicXML score, string partId)
        {
            SetMainFields(score, partId);
            SetDefaultDistances(score);
            List<ScorePartwisePartMeasureMusicXML> measuresInPart = score.Part.ElementAt(partIndex).Measure;
            GetLayoutInfo(measuresInPart);
            SetSystemMeasureRanges();
            SetPartHeight();
            GenerateDivisionChanges();
        }

        #endregion Constructors

        #region Properties

        public ClefChangesDictionary ClefChanges
        {
            get
            {
                return clefChanges;
            }

            set
            {
                clefChanges = value;
            }
        }

        public Dictionary<string, ClefChangesDictionary> ClefPerStaff
        {
            get
            {
                return clefPerStaff;
            }

            set
            {
                clefPerStaff = value;
            }
        }

        public Dictionary<string, Point> Coords
        {
            get
            {
                return coords; 
                //! return new Dictionary<string, Point>(measuresPerSystem.SelectMany((x,i) => x).ToDictionary(item => item, item=> new Point()));
                //! temporary empty list of point 
            }

            set
            {
                coords = value;
            }
        }

        public KeyChangesDictionary KeyChanges
        {
            get
            {
                return keyChanges;
            }

            set
            {
                keyChanges = value;
            }
        }

        public List<List<string>> MeasuresPerSystem
        {
            get
            {
                return measuresPerSystem;
            }

            set
            {
                measuresPerSystem = value;
            }
        }

        public List<List<List<string>>> MeasuresPerSystemPerPage
        {
            get
            {
                return measuresPerSystemPerPage;
            }

            set
            {
                measuresPerSystemPerPage = value;
            }
        }

        public int NumberOfStaves
        {
            get
            {
                return numberOfStaves;
            }

            set
            {
                numberOfStaves = value;
            }
        }

        /// <summary>
        /// Part height in Tenths
        /// </summary>
        public double PartHeight
        {
            get
            {
                return partHeight;
            }

            set
            {
                partHeight = value;
            }
        }

        public List<List<Tuple<string, string>>> PartSysemsInPages
        {
            get
            {
                return partSysemsInPages;
            }

            set
            {
                partSysemsInPages = value;
            }
        }

        public List<StaffLayoutMusicXML> StaffLayout
        {
            get
            {
                return staffLayout;
            }

            set
            {
                staffLayout = value;
            }
        }

        public List<List<StaffLayoutMusicXML>> StaffLayoutPerPage
        {
            get
            {
                return staffLayoutPerPage;
            }

            set
            {
                staffLayoutPerPage = value;
            }
        }

        public List<List<SystemLayoutMusicXML>> SystemLayoutPerPage
        {
            get
            {
                return systemLayoutPerPage;
            }

            set
            {
                systemLayoutPerPage = value;
            }
        }

        public TimeChangesDictionary TimeChanges
        {
            get
            {
                return timeChanges;
            }

            set
            {
                timeChanges = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds clef to each new System if not specified inside score
        /// </summary>
        public void AddAttributesToEachSystem()
        {
            if (systemAttributes)
            {
                List<string> systems = measuresPerSystem.Select(x => x.ElementAt(0)).ToList();
                foreach (var item in systems)
                {
                    if (!ClefChanges.ContainsKey(item))
                    {
                        ClefChanges clefs = new ScoreProperties.ClefChanges();
                        for (int i = 1; i <= numberOfStaves; i++)
                        {
                            var clef = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetClef(item, partId, i, 0);
                            clefs.Add(i.ToString(), 0, clef);
                        }
                        ClefChanges.Add(item, clefs);
                    }
                    if (ClefChanges.ContainsKey(item))
                    {
                        ClefChanges clefs = ClefChanges[item];
                        if (clefs.ClefsChanges.All(x => x.Item2 != 0))
                        {
                            for (int i = 1; i <= numberOfStaves; i++)
                            {
                                var clef = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetClef(item, partId, i, 0);
                                clefs.Add(i.ToString(), 0, clef);
                            }
                        }
                    }
                    if (!KeyChanges.ContainsKey(item))
                    {
                        KeyChanges keys = new ScoreProperties.KeyChanges();
                        for (int i = 1; i <= numberOfStaves; i++)
                        {
                            var key = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetKeySignature(item, partId, 0);
                            keys.Add(i.ToString(), 0, key);
                        }
                        KeyChanges.Add(item, keys);
                    }
                    if (KeyChanges.ContainsKey(item))
                    {
                        KeyChanges keys = KeyChanges[item];
                        if (keys.KeysChanges.All(x => x.Item2 != 0))
                        {
                            for (int i = 1; i <= numberOfStaves; i++)
                            {
                                var key = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetKeySignature(item, partId, 0);
                                keys.Add(i.ToString(), 0, key);
                            }
                        }
                    }
                }
            }
        }

        public void GenerateAttributes(TimeSignatures timeSignatures)
        {
            string firsMeasureId = currentPart.Measure.FirstOrDefault().Number;
            foreach (var measure in currentPart.Measure)
            {
                var currentTimeSig = timeSignatures.GetTimeSignature(measure.Number);
                int numerator = currentTimeSig.GetNumerator();
                int denominator = currentTimeSig.GetDenominator();
                int divisions = GetDivisionsMeasureId(measure.Number);
                int maxDuration = (int)((4 / (double)denominator) * (divisions * numerator));
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
                            BackupMusicXML b = (BackupMusicXML)measure.Items[i];
                            fractionCursor -= (int)b.Duration;
                            break;
                        case nameof(ForwardMusicXML):
                            ForwardMusicXML f = (ForwardMusicXML)measure.Items[i];
                            fractionCursor += (int)f.Duration;
                            break;
                        case nameof(NoteMusicXML):
                            NoteMusicXML n = (NoteMusicXML)measure.Items[i];
                            fractionCursor += n.IsChord() || n.IsGrace() ? 0 : n.GetDuration();
                            break;
                    }
                }
            }
            GenerateClefPerStaffDictionary();
        }
        
        public int GetDivisionsMeasureId(string measureId)
        {
            string resultKey = divisionsAttributes.Keys.FirstOrDefault();
            if (measureId.Contains('X'))
            {
                measureId = measureId.Substring(1);
            }
            resultKey = divisionsAttributes.Where(i => int.Parse(i.Key) <= int.Parse(measureId)).LastOrDefault().Key;
            return divisionsAttributes[resultKey];
        }

        private void AttributesChanged(AttributesMusicXML attributes, int cursorPosition, string measureNumber, bool firstMeasure = false)
        {
            ClefChanges clefs = new ClefChanges();
            KeyChanges keys = new KeyChanges();
            TimeChanges times = new TimeChanges();
            //! search for clefs
            if (attributes.Clef.Count == 0 && firstMeasure)
            {
                for (var i = 1; i <= numberOfStaves; i++)
                {
                    clefs.Add(i.ToString(), cursorPosition, new ClefMusicXML() { Sign = ClefSignMusicXML.G, Line = 2.ToString() });
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
                for (int i = 1; i <= numberOfStaves; i++)
                {
                    keys.Add(i.ToString(), cursorPosition, new KeyMusicXML() { Items = new object[] { 0.ToString() }, ItemsElementName = new KeyChoiceTypes[] { KeyChoiceTypes.fifths } });
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
                for (int i = 1; i <= numberOfStaves; i++)
                {
                    times.Add(i.ToString(), cursorPosition, new TimeMusicXML() { Items = new object[] { "4", "4" }, ItemsElementName = new TimeChoiceTypeMusicXML[] { TimeChoiceTypeMusicXML.beats, TimeChoiceTypeMusicXML.beattype } });
                }
            }
            else
            {
                for (int i = 0; i < attributes.Time.Count; i++)
                {
                    times.Add(attributes.Time[i].Number, cursorPosition, attributes.Time[i]);
                }
            }

            if (clefs.ClefsChanges.Count != 0)
            {
                clefChanges.Add(measureNumber, clefs);
            }

            if (keys.KeysChanges.Count != 0)
            {
                keyChanges.Add(measureNumber, keys);
            }

            if (times.TimesChanges.Count != 0)
            {
                timeChanges.Add(measureNumber, times);
            }
        }

        private void GenerateClefPerStaffDictionary()
        {
            if (clefPerStaff == null)
            {
                clefPerStaff = new Dictionary<string, ClefChangesDictionary>();
            }
            else
            {
                clefPerStaff.Clear();
            }
            for (int i = 1; i <= numberOfStaves; i++)
            {
                var clefs = clefChanges.Select(
                    (x, z) => new
                    {
                        x = x.Key,
                        z = x.Value.ClefsChanges.Where(c => c.Item1 == i.ToString())
                    })
                    .Where(x => x.z.FirstOrDefault() != null);

                ClefChangesDictionary ccdict = new ClefChangesDictionary();
                foreach (var item in clefs)
                {
                    ClefChanges cc = new ClefChanges();
                    foreach (var c in item.z)
                    {
                        cc.Add(c.Item1, c.Item2, c.Item3);
                    }
                    ccdict.Add(item.x, cc);
                }
                clefPerStaff.Add(i.ToString(), ccdict);
            }
        }

        private void GenerateDivisionChanges() //TODO_Later refactor based on measure duration fraction
        {
            divisionsAttributes = new Dictionary<string, int>();
            var firstMeasure = currentPart.Measure.FirstOrDefault();
            if (firstMeasure.Items.OfType<AttributesMusicXML>().FirstOrDefault().DivisionsSpecified)
            {
                var divisionsValue = firstMeasure.Items.OfType<AttributesMusicXML>().FirstOrDefault().Divisions;
                divisionsAttributes.Add(firstMeasure.Number, int.Parse(divisionsValue.ToString()));
            }
            for (int i = 1; i < currentPart.Measure.Count; i++)
            {
                var measure = currentPart.Measure[i];
                if (measure.Items.OfType<AttributesMusicXML>().FirstOrDefault()?.DivisionsSpecified != null ? measure.Items.OfType<AttributesMusicXML>().FirstOrDefault().DivisionsSpecified : false)
                {
                    divisionsAttributes.Add(measure.Number, int.Parse(measure.Items.OfType<AttributesMusicXML>().FirstOrDefault().Divisions.ToString()));
                }
            }
        }
        
        //! Refactor needed - missing default layout if Loaded score does not contain any layout supporting print elements
        private void GetLayoutInfo(List<Model.ScorePartwisePartMeasureMusicXML> part)
        {
            int currPageListIndex = 0;
            int previousPageListIndex = 0;
            foreach (var item in partSysemsInPages)//! working properly only if score contains layout elements
            {
                int currentSystemIndex = 0;
                List<SystemLayoutMusicXML> tempSysLayouts = new List<SystemLayoutMusicXML>();
                foreach (var measureNumber in item)
                {
                    var measure = part.First(i => i.Number == measureNumber.Item1);
                    var printLayouts = measure.Items.OfType<PrintMusicXML>().FirstOrDefault();
                    if (printLayouts != null)
                    {
                        if (printLayouts.SystemLayout != null)
                        {
                            UpdateListWithObjectsOfType(printLayouts.SystemLayout, ref systemLayout, 0);
                        }
                        else
                        {
                            systemLayout.Add(new SystemLayoutMusicXML() { SystemDistance = defaultSystemDistance, TopSystemDistance = defaultTopSystemDistance });
                        }
                        if (printLayouts.MeasureNumbering != null)
                        {
                            UpdateListWithObjectsOfType(printLayouts.MeasureNumbering, ref measureNumbering, 0);
                        }
                        if (printLayouts.StaffLayout.Count != 0)
                        {
                            if (printLayouts.StaffLayout.Count != 0)
                            {
                                UpdateListWithObjectsOfType(printLayouts.StaffLayout.ElementAtOrDefault(0), ref staffLayout, 0);
                            }
                        }
                        else
                        {
                            staffLayout.Add(new StaffLayoutMusicXML() { Number = numberOfStaves.ToString(), StaffDistance = defaultStaffDistance });
                        }
                    }
                    if (systemLayout.Count == 0)
                    {
                        systemLayout.Add(new SystemLayoutMusicXML() { SystemDistance = defaultSystemDistance, TopSystemDistance = defaultTopSystemDistance });
                    }
                    if (systemLayout.Count < currentSystemIndex + 1)
                    {
                        systemLayout.Add(new SystemLayoutMusicXML() { SystemDistance = defaultSystemDistance, SystemMargins = systemLayout.LastOrDefault().SystemMargins });
                    }
                    if (staffLayout.Count == 0)
                    {
                        staffLayout.Add(new StaffLayoutMusicXML() { Number = numberOfStaves.ToString(), StaffDistance = defaultStaffDistance });
                    }
                    if (staffLayout.Count < currentSystemIndex + 1)
                    {
                        staffLayout.Add(staffLayout.LastOrDefault());
                    }
                    if (measureNumbering.Count == 0)
                    {
                        measureNumbering.Add(new MeasureNumberingMusicXML() { Value = MeasureNumberingValueMusicXML.system });
                    }
                    if (measureNumbering.Count != systemLayout.Count)
                    {
                        measureNumbering.Add(measureNumbering.LastOrDefault());
                    }
                    currentSystemIndex++;
                }
                currPageListIndex = systemLayout.Count - previousPageListIndex;
                systemLayoutPerPage.Add(systemLayout.GetRange(previousPageListIndex, currPageListIndex));
                staffLayoutPerPage.Add(staffLayout.GetRange(previousPageListIndex, currPageListIndex));
                previousPageListIndex += currPageListIndex;
            }
            stavesDistance = staffLayout.ElementAt(0).StaffDistance;
        }

        private void SetDefaultDistances(ScorePartwiseMusicXML score)
        {
            var layout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout;
            defaultSystemDistance = 2.5 * layout.PageProperties.StaffHeight.MMToTenths();
            defaultTopSystemDistance = 3 * layout.PageProperties.StaffHeight.MMToTenths();
            defaultStaffDistance = 1.7 * layout.PageProperties.StaffHeight.MMToTenths();
            if (score.Defaults != null)
            {
                if (score.Defaults.StaffLayout != null)
                {
                    if (score.Defaults.StaffLayout.Count != 0)
                    {
                        defaultStaffDistance = score.Defaults.StaffLayout.ElementAt(0).StaffDistance;
                    }
                }
                if (score.Defaults.SystemLayout != null)
                {
                    if (score.Defaults.SystemLayout.SystemDistanceSpecified)
                    {
                        defaultSystemDistance = score.Defaults.SystemLayout.SystemDistance;
                    }
                    if (score.Defaults.SystemLayout.TopSystemDistanceSpecified)
                    {
                        defaultTopSystemDistance = score.Defaults.SystemLayout.TopSystemDistance;
                    }
                }
            }
        }

        private void SetMainFields(ScorePartwiseMusicXML score, string partId)
        {
            this.partId = partId;
            currentPart = score.Part.Where(i => i.Id == partId).FirstOrDefault();
            partIndex = score.Part.FindIndex(i => i.Id == partId);
            FindPartSystems(score);
            numberOfStaves = score.Part.ElementAt(partIndex).GetStavesCount();
        }
        private void FindPartSystems(ScorePartwiseMusicXML score)
        {
            partSysemsInPages = score.Part.ElementAt(partIndex).TryGetLinesPerPage();
        }
        private void SetPartHeight()
        {
            double staffHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.StaffHeight.MMToTenths();
            partHeight = staffHeight * numberOfStaves + (stavesDistance * (numberOfStaves - 1));
        }
        public void SetSystemMeasureRanges()
        {
            LayoutControl.LayoutGeneral currentLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout;
            double defaultLeftMargin = currentLayout.PageMargins.LeftMargin;
            double defaultTopMargin = currentLayout.PageMargins.TopMargin;
            var part = currentPart;
            measuresPerSystem = new List<List<string>>();
            foreach (var page in partSysemsInPages)
            {
                List<List<string>> tempList = new List<List<string>>();
                foreach (var item2 in page)
                {
                    measuresPerSystem.Add(part.TryGetMeasuresIdRange(item2));
                    tempList.Add(part.TryGetMeasuresIdRange(item2));
                }
                measuresPerSystemPerPage.Add(new List<List<string>>(tempList));
            }
            var measures = part.Measure;
            double previousWidth = 0.0;
            double currentLineY = 0.0;
            coords = new Dictionary<string, Point>();
            foreach (var measuresLine in measuresPerSystem)
            {
                int systemIndex = measuresPerSystem.IndexOf(measuresLine);
                if (systemIndex == 0)
                {
                    double topMargin = 0.0;
                    double marginL = defaultLeftMargin + systemLayout.ElementAt(systemIndex).SystemMargins.LeftMargin;
                    if (partIndex == 0)
                    {
                        topMargin = defaultTopMargin + systemLayout.ElementAt(systemIndex).TopSystemDistance;
                        // if part is first use topsystemdistance else staffdistance
                    }
                    else
                    {
                        topMargin = defaultTopMargin + staffLayout.ElementAt(systemIndex).StaffDistance;
                    }


                    previousWidth = 0.0; //! test marginL.TenthsToWPFUnit();

                    foreach (var measureId in measuresLine)
                    {
                        coords.Add(measureId, new Point(previousWidth, currentLineY));
                        previousWidth += part.GetMeasureUsingId(measureId).CalculatedWidth.TenthsToWPFUnit();
                    }
                }
                else
                {
                    previousWidth = 0.0; //! test (defaultLeftMargin + systemLayout.ElementAt(systemIndex).SystemMargins.LeftMargin).TenthsToWPFUnit();
                    foreach (var measureId in measuresLine)
                    {
                        coords.Add(measureId, new Point(previousWidth, currentLineY));
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
        /// <param name="currentLayoutIndex"></param>
        private void UpdateListWithObjectsOfType<T>(T newerLayout, ref List<T> listToUpdate, int currentLayoutIndex)
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
                    copy = ((T)xmls.Deserialize(memStream));
                }
                listToUpdate.Add(copy);
                return;
            }
            else
            {
                foreach (var property in oldSystemLayout.GetType().GetProperties())
                {
                    var currentObjValue = property.GetValue(oldSystemLayout);
                    var newObjValue = property?.GetValue(newerLayout) ?? currentObjValue;
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

        #endregion Methods

    }
}