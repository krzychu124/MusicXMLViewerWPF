using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLScore.Converters;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Model.MeasureItems;
using System.Diagnostics;
using MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes;
using MusicXMLScore.Model.MeasureItems.Attributes;
using MusicXMLScore.LayoutControl.SegmentPanelContainers;
using MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes;
using MusicXMLScore.Helpers;

namespace MusicXMLScore.LayoutControl
{
    class MeasureSegmentController
    {
        private SegmentPanel segmentPanel;
        private int systemIndex =0;
        private int pageIndex;
        private int stavesCount = 1;
        private int maxDuration = 1;
        private double width = 0;
        private PartProperties partProperties;
        //Dictionary<string, SegmentPanelContainers.MeasureItemsContainer> staffs;
        private Tuple<double, double, double> attributesWidths;
        private BeamItemsController beamsController;
        MeasureItemsContainer measureItemsContainer;
        public int MaxDuration
        {
            get
            {
                return maxDuration;
            }

            set
            {
                maxDuration = value;
            }
        }

        public double Width
        {
            get
            {
                return width;
            }

            set
            {
                width = value;
            }
        }
        internal BeamItemsController BeamsController
        {
            get
            {
                return beamsController;
            }

            set
            {
                beamsController = value;
            }
        }

        public MeasureSegmentController(Model.ScorePartwisePartMeasureMusicXML measure, string partID, int stavesCount, int systemIndex, int pageIndex)
        {
            this.systemIndex = systemIndex;
            this.pageIndex = pageIndex;
            this.stavesCount = stavesCount;
            partProperties = ViewModel.ViewModelLocator.Instance.Main.CurrentPartsProperties[partID];
            Stopwatch stopWatch;

            segmentPanel = new SegmentPanel(partID, measure.Number, systemIndex, pageIndex);
            stopWatch = new Stopwatch();

            stopWatch.Start();
            var currentDivisions = partProperties.GetDivisionsMeasureId(measure.Number);
            var currentTime = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetTimeSignature(measure.Number);
            double denominator = currentDivisions ==1 ? 1 : currentTime.GetDenominator();
            double numerator = currentDivisions == 1 ? 1 : currentTime.GetNumerator();
            maxDuration = (int)((4 / (double)denominator) * ( currentDivisions * numerator));
            int durationCursor = 0;
            var measureItems = measure.Items;
            List<BeamItem> beam = new List<BeamItem>();
            measureItemsContainer = new MeasureItemsContainer(measure.Number, partID, stavesCount, stavesCount.ToString());
            List<NoteMusicXML> temporaryChordList = new List<NoteMusicXML>();
            int chordDuration = 0;
            string tempStaffNumber = "1";
            for (int i = 0; i < measure.Items.Length; i++)
            {
                if (temporaryChordList.Count != 0)
                {
                    if (!(measure.Items[i] is NoteMusicXML))
                    {
                        if (measure.Items[i] is DirectionMusicXML)
                        {
                            //continue; // bugfix => when chorded notes sequence is splited by direction items 
                        }

                        var noteContainer = GenerateNoteContainerFromChords(temporaryChordList, durationCursor, partID, measure.Number, tempStaffNumber);
                        temporaryChordList.Clear();
                        if (noteContainer.Item1.Beams != null)
                        {
                            beam.Add(noteContainer.Item1.Beams);
                        }
                        //! add previous notesChord to container
                        measureItemsContainer.AppendNoteWithStaffNumber(noteContainer.Item1, noteContainer.Item2, noteContainer.Item3, noteContainer.Item4);
                        durationCursor += chordDuration;
                    }
                }

                string typeName = measure.Items[i].GetType().Name;
                if (typeName == nameof(AttributesMusicXML))
                {
                    AttributesMusicXML a = (AttributesMusicXML)measureItems[i];
                    if (CheckIfElementsOtherThanClefKeyTime(a, durationCursor))
                    {

                    }
                }
                if (typeName == nameof(BackupMusicXML))
                {
                    BackupMusicXML backward = (BackupMusicXML)measureItems[i];
                    durationCursor -= int.Parse(backward.Duration.ToString());
                }
                if (typeName == nameof(ForwardMusicXML))
                {
                    ForwardMusicXML forward = (ForwardMusicXML)measureItems[i];
                    durationCursor += int.Parse(forward.Duration.ToString());
                }
                if (typeName == nameof(NoteMusicXML))
                {
                    NoteMusicXML note = (NoteMusicXML)measureItems[i];
                    string staffNumber = note.Staff;
                    string voice = note.Voice;
                    if (note.IsRest())
                    {
                        if (temporaryChordList.Count != 0)
                        {
                            var noteContainer = GenerateNoteContainerFromChords(temporaryChordList, durationCursor, partID, measure.Number, tempStaffNumber);
                            temporaryChordList.Clear();
                            if (noteContainer.Item1.Beams != null)
                            {
                                beam.Add(noteContainer.Item1.Beams);
                            }
                            //! add previous notesChord to container
                            measureItemsContainer.AppendNoteWithStaffNumber(noteContainer.Item1, noteContainer.Item2, noteContainer.Item3, noteContainer.Item4);
                            durationCursor += chordDuration;
                        }
                        //! add rest to container
                        measureItemsContainer.AppendRestWithStaffNumber(new RestContainterItem(note, durationCursor, partID, measure.Number, staffNumber), durationCursor, voice, staffNumber);
                        durationCursor += note.GetDuration();
                    }
                    else
                    {
                        if (!note.IsChord())
                        {
                            if (!note.IsGrace())
                            {
                                if (temporaryChordList.Count != 0)
                                {
                                    var noteContainer = GenerateNoteContainerFromChords(temporaryChordList, durationCursor, partID, measure.Number, tempStaffNumber);
                                    temporaryChordList.Clear();
                                    if (noteContainer.Item1.Beams != null)
                                    {
                                        beam.Add(noteContainer.Item1.Beams);
                                    }
                                    //! add previous notesChord to container
                                    measureItemsContainer.AppendNoteWithStaffNumber(noteContainer.Item1, noteContainer.Item2, noteContainer.Item3, noteContainer.Item4);
                                    durationCursor += chordDuration;
                                }
                                chordDuration = note.GetDuration();
                                tempStaffNumber = staffNumber;
                                temporaryChordList.Add(note);

                            }
                            //temporaryChordList.Add(note);
                        }
                        else
                        {
                            temporaryChordList.Add(note);
                        }
                    }
                }
                if (i < measure.Items.Length)
                {
                    if (temporaryChordList.Count != 0)
                    {
                        if (i + 1 < measure.Items.Length)
                        {
                            
                        }
                        if (i + 1 == measure.Items.Length)
                        {
                            var noteContainer = GenerateNoteContainerFromChords(temporaryChordList, durationCursor, partID, measure.Number, tempStaffNumber);
                            temporaryChordList.Clear();
                            if (noteContainer.Item1.Beams != null)
                            {
                                beam.Add(noteContainer.Item1.Beams);
                            }
                            //! add previous notesChord to container
                            measureItemsContainer.AppendNoteWithStaffNumber(noteContainer.Item1, noteContainer.Item2, noteContainer.Item3, noteContainer.Item4);
                            durationCursor += chordDuration;
                        }
                    }
                }
            }
            beamsController = new BeamItemsController(beam);
            GenerateAndAddAttributesContainers(measure.Number, partID);
            width = measure.CalculatedWidth.TenthsToWPFUnit();
            ArrangeContainers(measure.CalculatedWidth.TenthsToWPFUnit(), maxDuration);
            AppendContainersToSegment();

            stopWatch.Stop();
            Log.LoggIt.Log($"Measure content {measure.Number} (Switch) processig done in: {stopWatch.ElapsedMilliseconds}", Log.LogType.Warning);

        }
        private Tuple<NoteContainerItem, int, string, string> GenerateNoteContainerFromChords(List<NoteMusicXML> chordList, int durationCursor, string partId, string measireId, string staffId)
        {
            string chordVoice = chordList.FirstOrDefault().Voice;
            NoteContainerItem noteContainer = new NoteContainerItem(chordList, durationCursor, partId, measireId, staffId);
            return Tuple.Create(noteContainer, durationCursor, chordVoice, staffId);
        }

        private bool CheckIfElementsOtherThanClefKeyTime(AttributesMusicXML a, int currentFraction)
        {
            if (a.DivisionsSpecified)
            {

            }
            if (a.MeasureStyle.Count != 0)
            {

            }
            if (a.PartSymbol != null)
            {

            }
            if (a.StaffDetails.Count != 0)
            {

            }
            if (a.Transpose.Count != 0)
            {

            }
            return false;
        }

        private void GenerateAndAddAttributesContainers(string measureNumber, string partID)
        {
            for (int i = 1; i <= stavesCount; i++) // missing - remainders for each systemSegment beginning measure
            {
                if (partProperties.ClefChanges.ContainsKey(measureNumber))
                {
                    var clefList = partProperties.ClefChanges[measureNumber].ClefsChanges.Where(x => x.Item1 == i.ToString()).ToList();
                    if (clefList.Count != 0)
                    {
                        foreach (var clef in clefList)
                        {
                            ClefContainerItem clefContainer = new ClefContainerItem(clef.Item1, clef.Item2, clef.Item3);
                            measureItemsContainer.AppendAttributeWithStaffNumber(clefContainer, clef.Item2, clef.Item1);
                        }
                    }
                }
                if (partProperties.KeyChanges.ContainsKey(measureNumber))
                {
                    var keyList = partProperties.KeyChanges[measureNumber].KeysChanges.Where(x => x.Item1 == i.ToString() || x.Item1 == null).ToList();
                    if (keyList.Count != 0)
                    {
                        foreach (var key in keyList)
                        {
                            string staffNumber = key.Item1 != null ? key.Item1 : i.ToString();
                            KeyContainerItem keyContainer = new KeyContainerItem(key.Item3, key.Item2, measureNumber, partID, staffNumber);
                            measureItemsContainer.AppendAttributeWithStaffNumber(keyContainer, key.Item2, staffNumber);
                        }
                    }
                }
                if (partProperties.TimeChanges.ContainsKey(measureNumber))
                {
                    var timeList = partProperties.TimeChanges[measureNumber].TimesChanges.Where(x => x.Item1 == i.ToString() || x.Item1 == null).ToList();
                    if (timeList.Count != 0)
                    {
                        foreach (var time in timeList)
                        {
                            string staffNumber = time.Item1 != null ? time.Item1 : i.ToString();
                            TimeSignatureContainerItem timeContainer = new TimeSignatureContainerItem(time.Item1, time.Item2, time.Item3);
                            measureItemsContainer.AppendAttributeWithStaffNumber(timeContainer, time.Item2, staffNumber);
                        }
                    }
                }
            }
        }
        public void ArrangeUsingDurationTable(Dictionary<int, double> durationTable)
        {
            measureItemsContainer.ArrangeUsingDurationTable(durationTable);
        }

        private void ArrangeContainers(double availableWidth, int maxDuration)
        {
            Dictionary<string, List<Tuple<int, IMeasureItemVisual>>> itemsPerStaff = new Dictionary<string, List<Tuple<int, IMeasureItemVisual>>>();
            for (int i = 1; i <= stavesCount; i++)
            {
                itemsPerStaff.Add(i.ToString(), measureItemsContainer.ItemsPositionsPerStaff[i.ToString()]);
            }
            Dictionary<string, List<IMeasureItemVisual>> attributesList = new Dictionary<string, List<IMeasureItemVisual>>();
            foreach (var item in itemsPerStaff)
            {
                var attributes = item.Value.Where(x => x.Item1 == 0 && x.Item2 is IAttributeItemVisual).Select(x => x.Item2).ToList();
                attributesList.Add(item.Key, attributes);
            }
            CalculateBeginningAttributes(attributesList);
        }
        private double CalculateBeginningAttributes(Dictionary<string, List<IMeasureItemVisual>> attributesList)
        {
            Dictionary<string, ClefContainerItem> clefs = new Dictionary<string, ClefContainerItem>();
            Dictionary<string, KeyContainerItem> keys = new Dictionary<string, KeyContainerItem>();
            Dictionary<string, TimeSignatureContainerItem> times = new Dictionary<string, TimeSignatureContainerItem>();
            foreach (var item in attributesList)
            {
                foreach (var attribute in item.Value)
                {
                    if (attribute is ClefContainerItem)
                    {
                        clefs.Add(item.Key, attribute as ClefContainerItem);
                    }
                    if (attribute is KeyContainerItem)
                    {
                        keys.Add(item.Key, attribute as KeyContainerItem);
                    }
                    if (attribute is TimeSignatureContainerItem)
                    {
                        times.Add(item.Key, attribute as TimeSignatureContainerItem);
                    }
                }
            }
            double maxClefWidth = 0.0;
            double maxKeyWidth = 0.0;
            double maxTimeWidth = 0.0;
            if (clefs.Count != 0)
            {
                List<double> clefWidths = new List<double>();
                foreach (var item in clefs)
                {
                    clefWidths.Add(measureItemsContainer.ArrangeAttributes(item.Value as IAttributeItemVisual, new Dictionary<string, double>()));
                }
                maxClefWidth = clefWidths.Max();
            }
            if (keys.Count != 0)
            {
                List<double> keysWidths = new List<double>();
                foreach (var item in keys)
                {
                    keysWidths.Add(measureItemsContainer.ArrangeAttributes(item.Value as IAttributeItemVisual, new Dictionary<string, double>()));
                }
                maxKeyWidth = keysWidths.Max();
            }
            if (times.Count != 0)
            {
                List<double> timesWidths = new List<double>();
                foreach (var item in times)
                {
                    timesWidths.Add(measureItemsContainer.ArrangeAttributes(item.Value as IAttributeItemVisual, new Dictionary<string, double>()));
                }
                maxTimeWidth = timesWidths.Max();
            }
            attributesWidths = Tuple.Create(maxClefWidth, maxKeyWidth, maxTimeWidth);
            return 0.0;
        }
        private void AppendContainersToSegment()
        {
            segmentPanel.AddNotesContainer(measureItemsContainer, stavesCount);
        }
        public List<int> GetIndexes()
        {
            List<List<int>> indexes = new List<List<int>>();
            indexes.Add(measureItemsContainer.GetDurationIndexes());
            return indexes.SelectMany(x => x).Distinct().ToList();
        }

        /// <summary>
        /// Returns Tuple of attributes widths {clefWidth, keyWidth, timeWidth}
        /// </summary>
        /// <returns></returns>
        public Tuple<double, double, double> GetAttributesWidths()
        {
            return attributesWidths;
        }
        public SegmentPanel GetContentPanel()
        {
            return segmentPanel;
        }
        public void AddBeams(List<DrawingVisualHost> beams)
        {
            foreach (var item in beams)
            {
                segmentPanel.Children.Add(item);
            }
        }
    }
}
