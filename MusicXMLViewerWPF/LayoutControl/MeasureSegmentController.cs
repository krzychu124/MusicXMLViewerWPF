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
using MusicXMLScore.LayoutControl.SegmentPanelContainers;
using MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes;
using MusicXMLScore.Helpers;
using System.Windows.Controls;

namespace MusicXMLScore.LayoutControl
{
    class MeasureSegmentController
    {
        private SegmentPanel segmentPanel;
        private int stavesCount = 1;
        private double minStavesDistance = 40.0.TenthsToWPFUnit();
        private int maxDuration = 1;
        private double width = 0;
        private double minimalWidth;
        private PartProperties partProperties;
        //Dictionary<string, SegmentPanelContainers.MeasureItemsContainer> staffs;
        private Tuple<double, double, double> attributesWidths;
        private BeamItemsController beamsController;
        MeasureItemsContainer measureItemsContainer;
        private string measureID;
        private double minimalWidthWithAttributes;
        private string partId;

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

        public double MinimalWidth
        {
            get
            {
                return minimalWidth;
            }

            set
            {
                minimalWidth = value;
                int partIndex = ViewModel.ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.FindIndex(x => x.Id == partId);
                ViewModel.ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.ElementAt(partIndex).Measure.Where(x=>x.Number== measureID).FirstOrDefault().CalculatedWidth = minimalWidth.WPFUnitToTenths();
            }
        }

        public string MeasureID
        {
            get
            {
                return measureID;
            }

            set
            {
                measureID = value;
            }
        }

        public double MinimalWidthWithAttributes
        {
            get
            {
                return minimalWidthWithAttributes;
            }

            set
            {
                minimalWidthWithAttributes = value;
            }
        }

        public string PartId
        {
            get
            {
                return partId;
            }

            set
            {
                partId = value;
            }
        }

        /// <summary>
        /// Measure Segment Controller Contructor
        /// </summary>
        /// <param name="measure">Measure object</param>
        /// <param name="partID">Part ID of measure</param>
        /// <param name="stavesCount">Number of staves in measure(1 is normal, 2 piano staff)</param>
        public MeasureSegmentController(Model.ScorePartwisePartMeasureMusicXML measure, string partID, int stavesCount)
        {
            this.partId = partID;
            this.stavesCount = stavesCount <1 ? 1: stavesCount; //! correction if set to 0
            this.measureID = measure.Number;
            partProperties = ViewModel.ViewModelLocator.Instance.Main.CurrentPartsProperties[partID]; //Todo refator: replace/remove (reduce dependencies)
            minStavesDistance = partProperties.StaffLayoutPerPage.FirstOrDefault().FirstOrDefault().StaffDistance.TenthsToWPFUnit(); //todo replace/improvements
            Stopwatch stopWatch;

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
            for (int i = 0; i < measure.Items.Length; i++) //Todo improve algorithm
            {
                if (temporaryChordList.Count != 0)
                {
                    if (!(measure.Items[i] is NoteMusicXML))
                    {
                        if (measure.Items[i] is DirectionMusicXML)
                        {
                            //! Todo music direction feature
                            continue; // bugfix => when chorded notes sequence is splited by direction items 
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
                        //Todo refactor
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
                        if (!note.IsChord()) //TODO switch grace and chord conditions order... (grace could be chord too :) )
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
            measureItemsContainer.ArrangeStaffs(minStavesDistance);
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
        public void ArrangeUsingDurationTable(Dictionary<int, double> durationTable, bool update = false)
        {
            measureItemsContainer.ArrangeUsingDurationTable(durationTable, update);
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
                        if (times.ContainsKey(item.Key))
                        {
                            Log.LoggIt.Log($"Argument exception key {item.Key}", Log.LogType.Exception);
                            times[item.Key] = attribute as TimeSignatureContainerItem;
                        }
                        else
                        {
                            times.Add(item.Key, attribute as TimeSignatureContainerItem);
                        }
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
            //segmentPanel.AddMeasureContainer(measureItemsContainer, stavesCount);
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
            return attributesWidths ?? Tuple.Create(0.0, 0.0, 0.0);
        }
        public Canvas GetMeasureCanvas()
        {
            return measureItemsContainer;
        }
        public void AddBeams(List<DrawingVisualHost> beams)
        {
            if (measureItemsContainer.Beams == null)
            {
                measureItemsContainer.Beams = new List<DrawingVisualHost>();
            }
            if (measureItemsContainer.Beams.Count != 0)
            {
                //! if not empty - remove beams to update
                foreach (var item in measureItemsContainer.Beams)
                {
                    measureItemsContainer.Children.Remove(item);
                }
                measureItemsContainer.Beams.Clear();
            }
            foreach (var item in beams)
            {
                measureItemsContainer.Beams.Add(item); //! reference used for update 
                measureItemsContainer.Children.Add(item);
            }
            //if (segmentPanel.Beams == null) 
            //{
            //    segmentPanel.Beams = new List<DrawingVisualHost>();
            //}
            //if (segmentPanel.Beams.Count != 0)
            //{
            //    //! if not empty - remove beams to update
            //    foreach (var item in segmentPanel.Beams)
            //    {
            //        segmentPanel.Children.Remove(item);
            //    }
            //    segmentPanel.Beams.Clear();
            //}
            //foreach (var item in beams)
            //{
            //    segmentPanel.Beams.Add(item); //! reference used for update 
            //    segmentPanel.Children.Add(item);
            //}
        }
        public double MinimalContentWidth()
        {
            var contentItemsWidth = measureItemsContainer.GetMinimalContentWidth();
            var attributesWidth = GetAttributesWidths();
            minimalWidthWithAttributes = ( contentItemsWidth + (attributesWidths.Item1 + attributesWidths.Item2 + attributesWidths.Item3)) *1.5;
            ViewModel.ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.FirstOrDefault().MeasuresByNumber[measureID].CalculatedWidth = MinimalWidthWithAttributes.WPFUnitToTenths();//! Test
            //! calculate optimal width using spacing values
            return contentItemsWidth ;
        }
        public List<AntiCollisionHelper> GetContentItemsProperties(int shortestDuration = 1)
        {
            List<AntiCollisionHelper> antiCollHelpers = new List<AntiCollisionHelper>();
            var allfractions = measureItemsContainer.GetDurationIndexes();
            foreach (var fraction in allfractions)
            {
                List<IMeasureItemVisual> fractionVisuals = measureItemsContainer.ItemsWithPostition.Where(x => x.Item1 == fraction && x.Item2 is INoteItemVisual).Select(item=>item.Item2).ToList();
                foreach (var itemVisual in fractionVisuals)
                {
                    INoteItemVisual note = itemVisual as INoteItemVisual;
                    double itemFractionFactor = LayoutHelpers.SpacingValue(note.ItemDuration, shortestDuration);
                    double factor = note.ItemDuration;
                    antiCollHelpers.Add(new AntiCollisionHelper(fraction, factor, itemFractionFactor, note.ItemWidth, note.ItemLeftMargin, note.ItemRightMargin));
                }
            }
            return antiCollHelpers;
        }
        public int GetMinDuration()
        {
            var allfractions = measureItemsContainer.GetDurationIndexes();
            var durationsOfPostion = LayoutHelpers.GetDurationOfPosition(maxDuration, allfractions);
            return durationsOfPostion.Values.Where(x => x > 0).Min();
        }
    }
}
