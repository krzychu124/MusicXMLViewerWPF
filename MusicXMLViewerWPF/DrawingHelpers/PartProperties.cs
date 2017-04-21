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

namespace MusicXMLScore.DrawingHelpers
{
    public class PartProperties //TODO_H Clean up unneccessary methods, field and properties
    {

        #region Fields

        private bool systemAttributes = true;
        private Dictionary<string, List<ClefMusicXML>> clefAttributes;
        private Dictionary<string, Point> coords;
        private MusicXMLScore.Model.ScorePartwisePartMusicXML currentPart;
        private double defaultStaffDistance = 0.0;
        private double defaultSystemDistance = 0.0;
        private double defaultTopSystemDistance = 0.0;
        private Dictionary<string, int> divisionsAttributes;
        private Dictionary<string, KeyMusicXML> keyAttributes;
        private List<MeasureNumberingMusicXML> measureNumbering = new List<MeasureNumberingMusicXML>();
        private List<List<MeasureNumberingMusicXML>> measureNumberingPerPage = new List<List<MeasureNumberingMusicXML>>();
        private List<List<string>> measuresPerSystem = new List<List<string>>();
        private List<List<List<string>>> measuresPerSystemPerPage = new List<List<List<string>>>();
        private List<Tuple<string, string>> measuresRangeInPartSystem = new List<Tuple<string, string>>();
        private int numberOfStaffs = 1;
        private double partHeight = 0;
        private string partId;
        private int partIndex = 0;
        private List<List<Tuple<string, string>>> partSysemsInPages;
        private List<StaffLayoutMusicXML> staffLayout = new List<StaffLayoutMusicXML>();
        private List<List<StaffLayoutMusicXML>> staffLayoutPerPage = new List<List<StaffLayoutMusicXML>>();
        private double stavesDistance = 0.0;
        private List<SystemLayoutMusicXML> systemLayout = new List<SystemLayoutMusicXML>();
        private List<List<SystemLayoutMusicXML>> systemLayoutPerPage = new List<List<SystemLayoutMusicXML>>();
        private List<string> firstIdPerSystem;

        private ClefChangesDictionary clefChanges = new ClefChangesDictionary();
        private Dictionary<string, ClefChangesDictionary> clefPerStaff;
        private KeyChangesDictionary keyChanges = new KeyChangesDictionary();
        private TimeChangesDictionary timeChanges = new TimeChangesDictionary();
        #endregion Fields

        #region Constructors

        public PartProperties(MusicXMLViewerWPF.ScorePartwiseMusicXML score, string partId)
        {
            this.partId = partId;
            SetDefaultDistances(score);
            currentPart = score.Part.Where(i => i.Id == partId).FirstOrDefault();
            partIndex = score.Part.FindIndex(i => i.Id == partId);
            partSysemsInPages = score.Part.ElementAt(partIndex).TryGetLinesPerPage();
            var part = score.Part.ElementAt(partIndex).Measure;
            var staves = part.ElementAt(0)?.Items?.OfType<AttributesMusicXML>()?.FirstOrDefault()?.Staves ?? "1";
            if (staves != null)
            {
                numberOfStaffs = int.Parse(staves);
            }
            GetLayoutInfo(part);
            stavesDistance = staffLayout.ElementAt(0).StaffDistance;
            SetSystemMeasureRanges(score);
            SetPartHeight();
            GenerateKeyAttributes();
            GenerateDivisionChanges();
            GenerateFirstMeasureIdPerSystem();
        }

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
                        for (int i = 1; i <= numberOfStaffs; i++)
                        {
                            var clef = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetClef(item, partId, i, 0);
                            clefs.Add(i.ToString(), 0, clef);
                        }
                        ClefChanges.Add(item, clefs);
                    }
                    if (ClefChanges.ContainsKey(item))
                    {
                        ClefChanges clefs = ClefChanges[item];
                        if (clefs.ClefsChanges.All(x=>x.Item2 != 0))
                        {
                            for (int i = 1; i <= numberOfStaffs; i++)
                            {
                                var clef = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetClef(item, partId, i, 0);
                                clefs.Add(i.ToString(), 0, clef);
                            }
                        }
                    }
                    if (!KeyChanges.ContainsKey(item))
                    {
                        KeyChanges keys = new ScoreProperties.KeyChanges();
                        for (int i = 1; i <= numberOfStaffs; i++)
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
                            for (int i = 1; i <= numberOfStaffs; i++)
                            {
                                var key = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetKeySignature(item, partId, 0);
                                keys.Add(i.ToString(), 0, key);
                            }
                        }
                    }
                }
            }
        }

        private void GetLayoutInfo(List<Model.ScorePartwisePartMeasureMusicXML> part)
        {
            int currPageListIndex = 0;
            int previousPageListIndex = 0;
            foreach (var item in partSysemsInPages)
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
                            //if (item.IndexOf(measureNumber) == 0)
                            //{
                            systemLayout.Add(new SystemLayoutMusicXML() { SystemDistance = defaultSystemDistance, TopSystemDistance = defaultTopSystemDistance });
                            //}
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

                            staffLayout.Add(new StaffLayoutMusicXML() { Number = numberOfStaffs.ToString(), StaffDistance = defaultStaffDistance });
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
                        staffLayout.Add(new StaffLayoutMusicXML() { Number = numberOfStaffs.ToString(), StaffDistance = defaultStaffDistance });
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
        }

        #endregion Constructors

        #region Properties

        public Dictionary<string, List<ClefMusicXML>> ClefAttributes
        {
            get
            {
                return clefAttributes;
            }

            set
            {
                clefAttributes = value;
            }
        }

        public Dictionary<string, Point> Coords
        {
            get
            {
                return coords;
            }

            set
            {
                coords = value;
            }
        }

        public Dictionary<string, int> DivisionsAttributes
        {
            get
            {
                return divisionsAttributes;
            }

            set
            {
                divisionsAttributes = value;
            }
        }

        public Dictionary<string, KeyMusicXML> KeyAttributes
        {
            get
            {
                return keyAttributes;
            }

            set
            {
                keyAttributes = value;
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
                return numberOfStaffs;
            }

            set
            {
                numberOfStaffs = value;
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

        public double StavesDistance
        {
            get
            {
                return stavesDistance;
            }

            set
            {
                stavesDistance = value;
            }
        }

        public List<SystemLayoutMusicXML> SystemLayout
        {
            get
            {
                return systemLayout;
            }

            set
            {
                systemLayout = value;
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

        #endregion Properties

        #region Methods
        public bool IsSystemBeginningMeasure(string measureId)
        {
            if (firstIdPerSystem== null)
            {
                GenerateFirstMeasureIdPerSystem();
            }
            if (firstIdPerSystem.Contains(measureId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GenerateFirstMeasureIdPerSystem()
        {
            if (measuresPerSystem != null || measuresPerSystem.Count != 0)
            {
                if (firstIdPerSystem == null)
                {
                    firstIdPerSystem = new List<string>();
                }
                if (firstIdPerSystem.Count != 0)
                {
                    firstIdPerSystem.Clear();
                }
                firstIdPerSystem = measuresPerSystem.Select(t => t).ElementAt(0).ToList();
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
                            fractionCursor += n.IsChord() ||n.IsGrace() ? 0: n.GetDuration();
                            break;
                    }
                }
            }
            GenerateClefPerStaffDictionary();
            
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
            for (int i = 1; i <= numberOfStaffs; i++)
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

        private void GenerateDivisionChanges2()
        {
            //TODO_Later refactor to duration fraction based
        }
        private void AttributesChanged(AttributesMusicXML attributes, int cursorPosition, string measureNumber, bool first = false)
        {
            ClefChanges clefs = new ClefChanges();
            KeyChanges keys = new KeyChanges();
            TimeChanges times = new TimeChanges();
            //search for clefs
            if (attributes.Clef.Count == 0 && first)
            {
                for (var i = 1; i <= numberOfStaffs; i++)
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
            //search for key signatures
            if (attributes.Key.Count == 0 && first)
            {
                for (int i = 1; i <= numberOfStaffs; i++)
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
            //search for time signatures
            if (attributes.Time.Count == 0 && first)
            {
                for (int i = 1; i <= numberOfStaffs; i++)
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

        public ClefChanges GetCurrentClef(string staffNumber, string measureNumber)
        {
            return new ClefChanges();// ClefAlterations.Select((i, j)=> i = clefAlterations.Keys, j = clefAlterations.Values).Where()
        }
        private void GenerateClefAttributes()
        {
            var firsMeasureInSystem = from zz in measuresPerSystem select zz.Select(t => t).ElementAt(0);
            clefAttributes = new Dictionary<string, List<ClefMusicXML>>();
            ClefMusicXML[] clefsArray = new ClefMusicXML[numberOfStaffs];
            ClefMusicXML[] clefsArrayToClone = new ClefMusicXML[numberOfStaffs];
            string fistMeasureId = currentPart.Measure.FirstOrDefault().Number;
            var firstMeasureClefs = currentPart.Measure.FirstOrDefault().Items.OfType<AttributesMusicXML>().FirstOrDefault()?.Clef.ToArray();
            if (firstMeasureClefs == null || firstMeasureClefs.Length == 0)
            {
                firstMeasureClefs= new ClefMusicXML[numberOfStaffs];
                for (int i = 0; i < numberOfStaffs; i++)
                {
                    firstMeasureClefs[i] = new ClefMusicXML() { Sign = ClefSignMusicXML.G, Line = "2" };  // throw new Exception("First measure in part does not contain Clef attribute"); //TODO_LATER Refactor to check before passing to this class.
                }
            }
            List<ClefMusicXML> clefList = new List<ClefMusicXML>();
            for (int i = 0; i < clefsArray.Length; i++)
            {
                var visibleClef = firstMeasureClefs[i].Clone();
                visibleClef.PrintObject = Model.Helpers.SimpleTypes.YesNoMusicXML.yes;
                visibleClef.PrintObjectSpecified = true;
                clefsArray[i] = visibleClef;
                clefList.Add(clefsArray[i]);
                //clone to copy if missing
                var temp = firstMeasureClefs[i].Clone();
                temp.PrintObject = Model.Helpers.SimpleTypes.YesNoMusicXML.no;
                temp.PrintObjectSpecified = true;
                clefsArrayToClone[i] = temp;
            }
            clefAttributes.Add(fistMeasureId, clefList);
            for (int i = 1; i < currentPart.Measure.Count; i++)
            {
                string measureId = currentPart.Measure[i].Number;
                List<ClefMusicXML> currentMeasureClefList = new List<ClefMusicXML>();
                var currentMeasure = currentPart.Measure[i];
                var attributesClefs = currentMeasure.Items.OfType<AttributesMusicXML>().FirstOrDefault()?.Clef; // nullable clef, if present - clef is list if no will be null
                if (attributesClefs != null ? attributesClefs.Count != 0 : false)
                {
                    var currentMeasureClefArray = currentMeasure.Items.OfType<AttributesMusicXML>().FirstOrDefault().Clef.ToArray();
                    foreach (var clefNumber in currentMeasureClefArray.Select(z => z.Number)) // loop through current measure clefs to find if any changed
                    {
                        int index = int.Parse(clefNumber);
                        var temp = currentMeasureClefArray.Where(n=>n.Number == index.ToString()).FirstOrDefault().Clone();
                        temp.PrintObject = Model.Helpers.SimpleTypes.YesNoMusicXML.yes;
                        temp.PrintObjectSpecified = true;
                        int indexClefs= Array.FindIndex(clefsArrayToClone, n => n.Number == index.ToString());
                        clefsArrayToClone[indexClefs] = temp;
                    }
                    for (int x = 0; x < clefsArray.Length; x++) //add to list<clef>
                    {
                        var temp = clefsArrayToClone[x];
                        currentMeasureClefList.Add(temp);
                    }
                }
                else //if clef didn't changed in this measure
                {
                    for (int c = 0; c < clefsArrayToClone.Length; c++)
                    {
                        var temp = clefsArrayToClone[c].Clone();
                        temp.PrintObject = Model.Helpers.SimpleTypes.YesNoMusicXML.no;
                        if (firsMeasureInSystem.Contains(measureId))
                        {
                            temp.PrintObject = Model.Helpers.SimpleTypes.YesNoMusicXML.yes; // change when measure id fist in system
                        }
                        temp.PrintObjectSpecified = true;
                        currentMeasureClefList.Add(temp);
                    }
                }
                clefAttributes.Add(measureId, currentMeasureClefList); // add clef list to dictionary
            }
            //! test // var test = clefAttributes.Where(i => i.Value.ElementAt(0).PrintObject == Model.Helpers.SimpleTypes.YesNoMusicXML.yes);
        }

        private void GenerateKeyAttributes()
        {
            //MusicXML support behaviour when key is different inside different part staves, 
            //I can't find any reason why this could be usefull since part correspond with one instrument and is impossible to play in eg. two different keySignatures at the same time
            //This feature will be omitted for now, so every staffline in part will have the same key signature.
            var firsMeasureInSystem = from zz in measuresPerSystem select zz.Select(t => t).ElementAt(0);
            keyAttributes = new Dictionary<string, KeyMusicXML>();
            KeyMusicXML currentKeySignature = new KeyMusicXML();
            foreach (var measure in currentPart.Measure)
            {
                if (measure.Items.OfType<AttributesMusicXML>().FirstOrDefault() != null)
                {
                    var attributes = measure.Items.OfType<AttributesMusicXML>().FirstOrDefault();
                    if (attributes.Key.Count != 0)
                    {
                        var keySig = attributes.Key.ElementAt(0);
                        currentKeySignature = keySig;
                        currentKeySignature.PrintObject = Model.Helpers.SimpleTypes.YesNoMusicXML.yes;
                        keyAttributes.Add(measure.Number, currentKeySignature);
                    }
                    else
                    {
                        KeyMusicXML key= currentKeySignature.Clone();
                        key.PrintObject = Model.Helpers.SimpleTypes.YesNoMusicXML.no;
                        keyAttributes.Add(measure.Number, key);
                    }
                }
                else
                {
                    KeyMusicXML key = currentKeySignature.Clone();
                    key.PrintObject = Model.Helpers.SimpleTypes.YesNoMusicXML.no;
                    if (firsMeasureInSystem.Contains(measure.Number))
                    {
                        key.PrintObject = Model.Helpers.SimpleTypes.YesNoMusicXML.yes;
                    }
                    keyAttributes.Add(measure.Number, key);
                }

            }
            //var test = keyAttributes.Select(i => i).Where(i => i.Value.PrintObject == Model.Helpers.SimpleTypes.YesNoMusicXML.yes);
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
        private void SetDefaultDistances(MusicXMLViewerWPF.ScorePartwiseMusicXML score)
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

        private void SetPartHeight()
        {
            double staffHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.StaffHeight.MMToTenths();
            partHeight = staffHeight * numberOfStaffs + (stavesDistance * (numberOfStaffs - 1));
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
        private void SetSystemMeasureRanges(MusicXMLViewerWPF.ScorePartwiseMusicXML score)
        {
            LayoutControl.LayoutGeneral currentLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout;
            double defaultLeftMargin = currentLayout.PageMargins.LeftMargin;
            double defaultTopMargin = currentLayout.PageMargins.TopMargin;
            var part = score.Part.ElementAt(partIndex);
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
            var measures = score.Part.ElementAt(partIndex).Measure;
            score.SetLargestWidth();
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
                    //currentLineY = topMargin.TenthsToWPFUnit();
                    foreach (var measureId in measuresLine)
                    {
                        coords.Add(measureId, new Point(previousWidth, currentLineY));
                        previousWidth += part.GetMeasureUsingId(measureId).CalculatedWidth.TenthsToWPFUnit();
                    }
                }
                else
                {
                    if (partIndex == 0)
                    {
                        //currentLineY += systemLayout.ElementAt(systemIndex).SystemDistance.TenthsToWPFUnit() + currentLayout.PageProperties.StaffHeight.MMToWPFUnit();
                    }
                    else
                    {
                        //currentLineY += staffLayout.ElementAt(systemIndex).StaffDistance.TenthsToWPFUnit() + currentLayout.PageProperties.StaffHeight.MMToWPFUnit();
                    }
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
        /// Compare and update differences
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
    public class AttributesChangesDictionary : Dictionary<string, int>
    {

    }

}