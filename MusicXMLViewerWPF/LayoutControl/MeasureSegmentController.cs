using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using MusicXMLScore.Converters;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
using MusicXMLScore.LayoutControl.SegmentPanelContainers;
using MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes;
using MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes;
using MusicXMLScore.Log;
using MusicXMLScore.Model;
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.ViewModel;
using MusicXMLScore.VisualObjectController;

namespace MusicXMLScore.LayoutControl
{
    class MeasureSegmentController
    {
        private int _stavesCount = 1;
        private double _minStavesDistance = 40.0.TenthsToWPFUnit();
        private int _maxDuration = 1;
        private double _width;
        private double _minimalWidth;
        private PartProperties _partProperties;
        private Tuple<double, double, double> _attributesWidths;
        private BeamItemsController _beamsController;
        private MeasureItemsContainer _measureItemsContainer;
        private StaffLineVisualController _staffLineController;

        private ScorePartwisePartMeasureMusicXML _measure;

        private string _measureId;
        private double _minimalWidthWithAttributes;
        private string _partId;

        public int MaxDuration
        {
            get { return _maxDuration; }

            set { _maxDuration = value; }
        }

        public double Width
        {
            get { return _width; }

            set { _width = value; }
        }

        internal BeamItemsController BeamsController
        {
            get { return _beamsController; }

            set { _beamsController = value; }
        }

        public double MinimalWidth
        {
            get { return _minimalWidth; }

            set
            {
                _minimalWidth = value;
                int partIndex = ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.FindIndex(x => x.Id == _partId);
                ViewModelLocator.Instance.Main.CurrentSelectedScore.Part[partIndex].Measure.FirstOrDefault(x => x.Number == _measureId)
                    .CalculatedWidth = _minimalWidth.WPFUnitToTenths();
            }
        }

        public string MeasureId
        {
            get { return _measureId; }

            set { _measureId = value; }
        }

        public double MinimalWidthWithAttributes
        {
            get { return _minimalWidthWithAttributes; }

            set { _minimalWidthWithAttributes = value; }
        }

        public string PartId
        {
            get { return _partId; }

            set { _partId = value; }
        }

        /// <summary>
        /// Measure Segment Controller Contructor
        /// </summary>
        /// <param name="measure">Measure object</param>
        /// <param name="partID">Part ID of measure</param>
        /// <param name="stavesCount">Number of staves in measure(1 is normal, 2 piano staff)</param>
        public MeasureSegmentController(ScorePartwisePartMeasureMusicXML measure, string partID, int stavesCount)
        {
            _partId = partID;
            _stavesCount = stavesCount < 1 ? 1 : stavesCount; //! correction if set to 0
            _measureId = measure.Number;
            _measure = measure;
            _partProperties = ViewModelLocator.Instance.Main.PartsProperties[partID]; //Todo refator: replace/remove (reduce dependencies)
            _minStavesDistance =
                _partProperties.StaffLayoutPerPage.FirstOrDefault().FirstOrDefault().StaffDistance.TenthsToWPFUnit(); //todo replace/improvements

            var currentDivisions = _partProperties.GetDivisionsMeasureId(measure.Number);
            var currentTime = ViewModelLocator.Instance.Main.CurrentScoreProperties.GetTimeSignature(measure.Number);
            double denominator = currentDivisions == 1 ? 1.0 : currentTime.GetDenominator();
            double numerator = currentDivisions == 1 ? 1.0 : currentTime.GetNumerator();
            _maxDuration = (int) (4 / denominator * (currentDivisions * numerator));
            int durationCursor = 0;
            var measureItems = measure.Items;

            //!----------- beams --------------------------
            List<BeamItem> beam = new List<BeamItem>();
            _measureItemsContainer = new MeasureItemsContainer(measure.Number, partID, stavesCount, stavesCount.ToString());
            //!-------------------------------------------

            //!----------- staff lines --------------------
            _staffLineController = new StaffLineVisualController(stavesCount, _width, _partProperties.NumberOfLines, measure, _minStavesDistance);
            _measureItemsContainer.AddStaffLine(_staffLineController.StaffLineCanvas);
            //!--------------------------------------------

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

                        var noteContainer =
                            GenerateNoteContainerFromChords(temporaryChordList, durationCursor, partID, measure.Number, tempStaffNumber);
                        temporaryChordList.Clear();
                        if (noteContainer.Item1.Beams != null)
                        {
                            beam.Add(noteContainer.Item1.Beams);
                        }
                        //! add previous notesChord to container
                        _measureItemsContainer.AppendNoteWithStaffNumber(noteContainer.Item1, noteContainer.Item2, noteContainer.Item3,
                            noteContainer.Item4);
                        durationCursor += chordDuration;
                    }
                }

                string typeName = measure.Items[i].GetType().Name;
                if (typeName == nameof(AttributesMusicXML))
                {
                    AttributesMusicXML a = (AttributesMusicXML) measureItems[i];
                    if (CheckIfElementsOtherThanClefKeyTime(a, durationCursor))
                    {
                        //Todo refactor
                    }
                }
                if (typeName == nameof(BackupMusicXML))
                {
                    BackupMusicXML backward = (BackupMusicXML) measureItems[i];
                    durationCursor -= int.Parse(backward.Duration.ToString());
                }
                if (typeName == nameof(ForwardMusicXML))
                {
                    ForwardMusicXML forward = (ForwardMusicXML) measureItems[i];
                    durationCursor += int.Parse(forward.Duration.ToString());
                }
                if (typeName == nameof(NoteMusicXML))
                {
                    NoteMusicXML note = (NoteMusicXML) measureItems[i];
                    string staffNumber = note.Staff;
                    string voice = note.Voice;
                    if (note.IsRest())
                    {
                        if (temporaryChordList.Count != 0)
                        {
                            var noteContainer =
                                GenerateNoteContainerFromChords(temporaryChordList, durationCursor, partID, measure.Number, tempStaffNumber);
                            temporaryChordList.Clear();
                            if (noteContainer.Item1.Beams != null)
                            {
                                beam.Add(noteContainer.Item1.Beams);
                            }
                            //! add previous notesChord to container
                            _measureItemsContainer.AppendNoteWithStaffNumber(noteContainer.Item1, noteContainer.Item2, noteContainer.Item3,
                                noteContainer.Item4);
                            durationCursor += chordDuration;
                        }
                        //! add rest to container
                        _measureItemsContainer.AppendRestWithStaffNumber(
                            new RestContainterItem(note, durationCursor, partID, measure.Number, staffNumber), durationCursor, voice, staffNumber);
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
                                    var noteContainer = GenerateNoteContainerFromChords(temporaryChordList, durationCursor, partID, measure.Number,
                                        tempStaffNumber);
                                    temporaryChordList.Clear();
                                    if (noteContainer.Item1.Beams != null)
                                    {
                                        beam.Add(noteContainer.Item1.Beams);
                                    }
                                    //! add previous notesChord to container
                                    _measureItemsContainer.AppendNoteWithStaffNumber(noteContainer.Item1, noteContainer.Item2, noteContainer.Item3,
                                        noteContainer.Item4);
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
                            var noteContainer =
                                GenerateNoteContainerFromChords(temporaryChordList, durationCursor, partID, measure.Number, tempStaffNumber);
                            temporaryChordList.Clear();
                            if (noteContainer.Item1.Beams != null)
                            {
                                beam.Add(noteContainer.Item1.Beams);
                            }
                            //! add previous notesChord to container
                            _measureItemsContainer.AppendNoteWithStaffNumber(noteContainer.Item1, noteContainer.Item2, noteContainer.Item3,
                                noteContainer.Item4);
                            durationCursor += chordDuration;
                        }
                    }
                }
            }
            _beamsController = new BeamItemsController(beam);
            GenerateAndAddAttributesContainers(measure.Number, partID);
            _width = measure.CalculatedWidth.TenthsToWPFUnit();
            ArrangeContainers(measure.CalculatedWidth.TenthsToWPFUnit(), _maxDuration);
            _measureItemsContainer.ArrangeStaffs(_minStavesDistance);
        }

        private Tuple<NoteContainerItem, int, string, string> GenerateNoteContainerFromChords(List<NoteMusicXML> chordList, int durationCursor,
            string partId, string measireId, string staffId)
        {
            string chordVoice = chordList.FirstOrDefault().Voice;
            NoteContainerItem noteContainer = new NoteContainerItem(chordList, durationCursor, partId, measireId, staffId);
            return Tuple.Create(noteContainer, durationCursor, chordVoice, staffId);
        }

        //TODO finish/refactor
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
                if (a.StaffDetails.Count > 1)
                {
                }
                else
                {
                    if (a.StaffDetails.FirstOrDefault().Number != null)
                    {
                    }
                    else
                    {
                        if (_staffLineController.StaffVisuals.Count == 1)
                        {
                            var number = a.StaffDetails.FirstOrDefault().StaffLines;
                            if (number != null)
                            {
                                _partProperties.NumberOfLines = int.Parse(number);
                            }
                        }
                    }
                }
            }
            if (a.Transpose.Count != 0)
            {
            }
            return false;
        }

        private void GenerateAndAddAttributesContainers(string measureNumber, string partID)
        {
            for (int i = 1; i <= _stavesCount; i++) // missing - remainders for each systemSegment beginning measure
            {
                if (_partProperties.ClefChanges.ContainsKey(measureNumber))
                {
                    var clefList = _partProperties.ClefChanges[measureNumber].AttributeChanges.Where(x => x.StaffNumber == i.ToString()).ToList();
                    if (clefList.Count != 0)
                    {
                        foreach (var clef in clefList)
                        {
                            ClefContainerItem clefContainer = new ClefContainerItem(clef.StaffNumber, clef.TimeFraction, clef.AttributeEntity);
                            _measureItemsContainer.AppendAttributeWithStaffNumber(clefContainer, clef.TimeFraction, clef.StaffNumber);
                        }
                    }
                }
                if (_partProperties.KeyChanges.ContainsKey(measureNumber))
                {
                    var keyList = _partProperties.KeyChanges[measureNumber].AttributeChanges
                        .Where(x => x.StaffNumber == i.ToString() || x.StaffNumber == null).ToList();
                    if (keyList.Count != 0)
                    {
                        foreach (var key in keyList)
                        {
                            string staffNumber = key.StaffNumber ?? i.ToString();
                            KeyContainerItem keyContainer =
                                new KeyContainerItem(key.AttributeEntity, key.TimeFraction, measureNumber, partID, staffNumber);
                            _measureItemsContainer.AppendAttributeWithStaffNumber(keyContainer, key.TimeFraction, staffNumber);
                        }
                    }
                }
                if (_partProperties.TimeChanges.ContainsKey(measureNumber))
                {
                    var timeList = _partProperties.TimeChanges[measureNumber].AttributeChanges
                        .Where(x => x.StaffNumber == i.ToString() || x.StaffNumber == null).ToList();
                    if (timeList.Count != 0)
                    {
                        foreach (var time in timeList)
                        {
                            string staffNumber = time.StaffNumber ?? i.ToString();
                            TimeSignatureContainerItem timeContainer =
                                new TimeSignatureContainerItem(time.StaffNumber, time.TimeFraction, time.AttributeEntity);
                            _measureItemsContainer.AppendAttributeWithStaffNumber(timeContainer, time.TimeFraction, staffNumber);
                        }
                    }
                }
            }
        }

        public void ArrangeUsingDurationTable(Dictionary<int, double> durationTable, bool update = false)
        {
            _measureItemsContainer.ArrangeUsingDurationTable(durationTable, update);
        }

        private void ArrangeContainers(double availableWidth, int maxDuration)
        {
            Dictionary<string, List<Tuple<int, IMeasureItemVisual>>> itemsPerStaff = new Dictionary<string, List<Tuple<int, IMeasureItemVisual>>>();
            for (int i = 1; i <= _stavesCount; i++)
            {
                itemsPerStaff.Add(i.ToString(), _measureItemsContainer.ItemsPositionsPerStaff[i.ToString()]);
            }
            Dictionary<string, List<IMeasureItemVisual>> attributesList = new Dictionary<string, List<IMeasureItemVisual>>();
            foreach (var item in itemsPerStaff)
            {
                var attributes = item.Value.Where(x => x.Item1 == 0 && x.Item2 is IAttributeItemVisual).Select(x => x.Item2).ToList();
                attributesList.Add(item.Key, attributes);
            }
            CalculateBeginningAttributes(attributesList);
        }

        private void CalculateBeginningAttributes(Dictionary<string, List<IMeasureItemVisual>> attributesList)
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
                            LoggIt.Log($"Argument exception key {item.Key}", LogType.Exception);
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
                    clefWidths.Add(_measureItemsContainer.ArrangeAttributes(item.Value, new Dictionary<string, double>()));
                }
                maxClefWidth = clefWidths.Max();
            }
            if (keys.Count != 0)
            {
                List<double> keysWidths = new List<double>();
                foreach (var item in keys)
                {
                    keysWidths.Add(_measureItemsContainer.ArrangeAttributes(item.Value, new Dictionary<string, double>()));
                }
                maxKeyWidth = keysWidths.Max();
            }
            if (times.Count != 0)
            {
                List<double> timesWidths = new List<double>();
                foreach (var item in times)
                {
                    timesWidths.Add(_measureItemsContainer.ArrangeAttributes(item.Value, new Dictionary<string, double>()));
                }
                maxTimeWidth = timesWidths.Max();
            }
            _attributesWidths = Tuple.Create(maxClefWidth, maxKeyWidth, maxTimeWidth);
        }

        public List<int> GetIndexes()
        {
            List<List<int>> indexes = new List<List<int>>();
            indexes.Add(_measureItemsContainer.GetDurationIndexes());
            return indexes.SelectMany(x => x).Distinct().ToList();
        }

        /// <summary>
        /// Returns Tuple of attributes widths {clefWidth, keyWidth, timeWidth}
        /// </summary>
        /// <returns></returns>
        public Tuple<double, double, double> GetAttributesWidths()
        {
            return _attributesWidths ?? Tuple.Create(0.0, 0.0, 0.0);
        }

        public Canvas GetMeasureCanvas()
        {
            return _measureItemsContainer;
        }

        public void AddBeams(List<DrawingVisualHost> beams)
        {
            if (_measureItemsContainer.Beams == null)
            {
                _measureItemsContainer.Beams = new List<DrawingVisualHost>();
            }
            if (_measureItemsContainer.Beams.Count != 0)
            {
                //! if not empty - remove beams to update
                foreach (var item in _measureItemsContainer.Beams)
                {
                    _measureItemsContainer.Children.Remove(item);
                }
                _measureItemsContainer.Beams.Clear();
            }
            foreach (var item in beams)
            {
                _measureItemsContainer.Beams.Add(item); //! reference used for update 
                _measureItemsContainer.Children.Add(item);
            }
        }

        public List<AntiCollisionHelper> GetContentItemsProperties(int shortestDuration = 1)
        {
            List<AntiCollisionHelper> antiCollHelpers = new List<AntiCollisionHelper>();
            var allfractions = _measureItemsContainer.GetDurationIndexes();
            foreach (var fraction in allfractions)
            {
                List<IMeasureItemVisual> fractionVisuals = _measureItemsContainer.ItemsWithPostition
                    .Where(x => x.Item1 == fraction && x.Item2 is INoteItemVisual).Select(item => item.Item2).ToList();
                foreach (var itemVisual in fractionVisuals)
                {
                    INoteItemVisual note = itemVisual as INoteItemVisual;
                    double itemFractionFactor = LayoutHelpers.SpacingValue(note.ItemDuration, shortestDuration);
                    double factor = note.ItemDuration;
                    antiCollHelpers.Add(new AntiCollisionHelper(fraction, factor, itemFractionFactor, note.ItemWidth, note.ItemLeftMargin,
                        note.ItemRightMargin));
                }
            }
            return antiCollHelpers;
        }

        public int GetMinDuration()
        {
            var allfractions = _measureItemsContainer.GetDurationIndexes();
            var durationsOfPostion = LayoutHelpers.GetDurationOfPosition(_maxDuration, allfractions);
            return durationsOfPostion.Values.Where(x => x > 0).Min();
        }
    }
}