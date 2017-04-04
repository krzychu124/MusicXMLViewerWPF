using MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes;
using MusicXMLScore.Model;
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.Model.MeasureItems.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MusicXMLScore.Converters;
using MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers
{
    class MeasureItemsContainer : Canvas
    {
        List<IMeasureItemVisual> measureItemsVisuals;
        private List<NoteMusicXML> notesList;
        private List<Tuple<int, IMeasureItemVisual>> itemsWithPostition;
        private ScorePartwisePartMeasureMusicXML measure;
        private string measureId = "";
        private string partId;
        private int staveNumber;

        private int divisions;
        private int availableDuration;
        private int numerator;
        private int denominator;
        private TimeMusicXML timeSignature;

        internal List<Tuple<int, IMeasureItemVisual>> ItemsWithPostition
        {
            get
            {
                return itemsWithPostition;
            }

            set
            {
                itemsWithPostition = value;
            }
        }

        public MeasureItemsContainer(string measureId, string partId, int numberOfStave)
        {
            measureItemsVisuals = new List<IMeasureItemVisual>();
            notesList = new List<NoteMusicXML>();
            itemsWithPostition = new List<Tuple<int, IMeasureItemVisual>>();
            this.measureId = measureId;
            this.partId = partId;
            staveNumber = numberOfStave;
        }
        public MeasureItemsContainer(ScorePartwisePartMeasureMusicXML measure, string partId, int numberOfStave)
        {
            measureItemsVisuals = new List<IMeasureItemVisual>();
            notesList = new List<NoteMusicXML>();
            this.measure = measure;
            measureId = measure.Number;
            this.partId = partId;
            staveNumber = numberOfStave;
            notesList = measure.Items.OfType<NoteMusicXML>().ToList();
            List<NoteMusicXML> chordListTemp = new List<NoteMusicXML>();
            for (int i = 0; i < notesList.Count; i++)
            {
                NoteChoiceTypeMusicXML noteType = notesList[i].GetNoteType();
                NoteChoiceTypeMusicXML tempChoice = NoteChoiceTypeMusicXML.none;
                string staff = "1";
                if (int.Parse(notesList[i].Staff) != staveNumber)
                {
                    continue;
                }
                if (notesList[i].ItemsElementName.Contains(NoteChoiceTypeMusicXML.rest))
                {
                    if (chordListTemp.Count != 0)
                    {
                        staff = chordListTemp.ElementAt(0).Staff;
                        NoteContainerItem note = new NoteContainerItem(chordListTemp, i, partId, measure.Number, staff);
                        AddNote(note);
                        chordListTemp.Clear();
                    }
                    RestContainterItem rest = new RestContainterItem(notesList[i], i, partId, measure.Number, staff);
                    AddRest(rest);
                    continue;
                }
                if (notesList[i].ItemsElementName.Contains(NoteChoiceTypeMusicXML.chord))
                {
                    chordListTemp.Add(notesList[i]);
                }
                else
                {
                    if (chordListTemp.Count != 0)
                    {
                        staff = chordListTemp.ElementAt(0).Staff;
                        NoteContainerItem note = new NoteContainerItem(chordListTemp, i, partId, measure.Number, staff);
                        AddNote(note);
                        chordListTemp.Clear();
                        chordListTemp.Add(notesList[i]);
                    }
                    else
                    {
                        chordListTemp.Add(notesList[i]);
                    }
                    tempChoice = noteType;
                }
                if (chordListTemp.Count != 0 && i+1 == notesList.Count)
                {
                    staff = chordListTemp.ElementAt(0).Staff;
                    NoteContainerItem note = new NoteContainerItem(chordListTemp, i, partId, measure.Number, staff);
                    AddNote(note);
                    chordListTemp.Clear();
                }
            }
        }

        public void ArrangeNotes(double availableWidth)
        {
            double leftOffset = 20.0; //in tenths == staffSpace
            GetDivisions();
            GetTimeSignature();
            double offset = CalculatePositions(availableWidth - leftOffset.TenthsToWPFUnit());
            int count = measureItemsVisuals.Count;
            double accOffset = leftOffset.TenthsToWPFUnit() *0.8;

            foreach (var item in measureItemsVisuals)
            {
                if (item is RestContainterItem && measureItemsVisuals.Count == 1)
                {
                    double itemHalfWidth = item.ItemWidthMin/2;
                    Canvas.SetLeft(item as Canvas, (availableWidth)/2 - itemHalfWidth);
                    continue;
                }
                Canvas.SetLeft(item as Canvas, accOffset);
                accOffset += (item as INoteItemVisual).ItemDuration *offset;
            }
        }

        public double ArrangeItemsByDuration(double availableWidth, double attributeWidth, int measureDuration)
        {
            //length between lines of staffline
            double staffSpace = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.StaffSpace.MMToWPFUnit();
            //-----------------------------------
            // Draw measure Attributes 
            //-----------------------------------
            var measureBeginningAttributes = itemsWithPostition.Where(x => x.Item1 == 0 && x.Item2 is IAttributeItemVisual).Select(x => x.Item2).ToList();
            itemsWithPostition.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            double beginningAttributesWidth = 0.0;
            if (measureBeginningAttributes.Count != 0)
            {
                double attributesWidth = attributeWidth;
                //foreach (IAttributeItemVisual measureAttribute in measureBeginningAttributes)
                //{
                //    attributesWidth = ArrangeAttributes(measureAttribute, attributesWidth);
                //}
                beginningAttributesWidth = attributesWidth;
            }
            double firstNoteMargin = 20.0.TenthsToWPFUnit();
            double widthAvaliableForNotes = availableWidth - beginningAttributesWidth - firstNoteMargin;

            //------------------------------------
            //Common shortes note in measure
            //------------------------------------
            Dictionary<int, int> commonShortest = new Dictionary<int, int>();
            foreach (var item in itemsWithPostition)
            {
                var note = item.Item2 as INoteItemVisual;
                if (note != null)
                {
                    if (commonShortest.ContainsKey(note.ItemDuration))
                    {
                        commonShortest[note.ItemDuration] += 1;
                    }
                    else
                    {
                        commonShortest.Add(note.ItemDuration, 1);
                    }
                }
            }
            var commonDuration = commonShortest.FirstOrDefault(x => x.Value == commonShortest.Values.Max()).Key;
            int shortestDuration = commonShortest.Keys.Where(x=>x!=0).Min(x=>x);
            //------------------------------------
            
            List<int> mesaurePositionIndexes = itemsWithPostition.Select(x => x.Item1).Distinct().ToList();
            mesaurePositionIndexes.Sort();

            double position = beginningAttributesWidth + (firstNoteMargin / 2);
            double startingPositionAfterAttributes = beginningAttributesWidth + (firstNoteMargin / 2);
            Dictionary<int, int> durationsOfPosition = new Dictionary<int, int>();
            for (int i = 0; i < mesaurePositionIndexes.Count; i++)
            {
                if (i < mesaurePositionIndexes.Count - 1)
                {
                    durationsOfPosition.Add(mesaurePositionIndexes[i], mesaurePositionIndexes[i + 1] - mesaurePositionIndexes[i]);
                }
                else
                {
                    durationsOfPosition.Add(mesaurePositionIndexes[mesaurePositionIndexes.Count - 1], measureDuration - mesaurePositionIndexes[mesaurePositionIndexes.Count - 1]);
                }
            }
            //----------------------------------------------
            //this vvv should be calculated before arranging when measure of part has more than one staff to prevent different duration positions
            //----------------------------------------------
            Dictionary<int, double> positionCoords = new Dictionary<int, double>();
            Dictionary<int, Tuple<double, double>> positionsCoordsBeforeStretch = new Dictionary<int, Tuple<double, double>>();
            Dictionary<int, Tuple<double, double>> positionsCoordsAfterStretch = new Dictionary<int, Tuple<double, double>>();
            for (int i = 0; i < mesaurePositionIndexes.Count; i++)
            {
                if (i == 0)
                {
                    positionCoords.Add(mesaurePositionIndexes[i], position);
                    int currentDuration = durationsOfPosition[mesaurePositionIndexes[i]];
                    double previewSpacing = staffSpace * SpacingValue(currentDuration, shortestDuration, 0.6);
                    positionsCoordsBeforeStretch.Add(mesaurePositionIndexes[i], Tuple.Create(position, previewSpacing));
                    positionsCoordsAfterStretch.Add(mesaurePositionIndexes[i], Tuple.Create(position, previewSpacing));
                }
                else
                {
                    int currentDuration =  durationsOfPosition[mesaurePositionIndexes[i]];
                    double previewSpacing = staffSpace * SpacingValue(currentDuration, shortestDuration, 0.6);
                    position += previewSpacing;
                    positionCoords.Add(mesaurePositionIndexes[i], position);
                    positionsCoordsBeforeStretch.Add(mesaurePositionIndexes[i], Tuple.Create(position, previewSpacing));
                    positionsCoordsAfterStretch.Add(mesaurePositionIndexes[i], Tuple.Create(position, previewSpacing));
                }
            }
            if (positionCoords.LastOrDefault().Value > availableWidth)
            {
                //------------------------------------------
                //Spacing factor shrink method when items with calculated spacings can't fit inside measure length
                //Currently using small spacing factors to prevent this situation
                //ToDo
                //------------------------------------------
            }
            if (positionsCoordsBeforeStretch.LastOrDefault().Value.Item1 + positionsCoordsBeforeStretch.LastOrDefault().Value.Item2 < availableWidth)
            {
                double maxWidth = availableWidth - startingPositionAfterAttributes;
                double currentFullWidth = positionsCoordsAfterStretch.Sum(x => x.Value.Item2);
                double difference = maxWidth - currentFullWidth;
                for (int i = 0; i < mesaurePositionIndexes.Count; i++)
                {
                    Tuple<double, double> currentTuple = positionsCoordsAfterStretch[mesaurePositionIndexes[i]];
                    double currentPosition = currentTuple.Item1;
                    double correctedSpacing = (currentTuple.Item2 /currentFullWidth) * difference;

                    if (i == 0)
                    {
                        Tuple<double, double> t = Tuple.Create(currentPosition, correctedSpacing + currentTuple.Item2);
                        positionsCoordsAfterStretch[mesaurePositionIndexes[i]]= t;
                    }
                    else
                    {
                        currentPosition = positionsCoordsAfterStretch[mesaurePositionIndexes[i - 1]].Item1 + positionsCoordsAfterStretch[mesaurePositionIndexes[i-1]].Item2;
                        Tuple<double, double> t = Tuple.Create(currentPosition, correctedSpacing + currentTuple.Item2);
                        positionsCoordsAfterStretch[mesaurePositionIndexes[i]] = t;
                    }
                }
            }
            for (int i = 0; i < itemsWithPostition.Count; i++)
            {
                Tuple<int, IMeasureItemVisual> item = itemsWithPostition[i];
                if (item.Item2 is INoteItemVisual)
                {
                    INoteItemVisual note = item.Item2 as INoteItemVisual;
                    if (note.ItemDuration == 0)
                    {
                        continue;
                    }
                    //debug line before stretch
                    note.DrawSpace(positionsCoordsBeforeStretch[item.Item1].Item2);
                    // seting position of note
                    SetLeft(item.Item2 as Canvas, positionsCoordsAfterStretch[item.Item1].Item1);
                    //debug line after stretch
                    note.DrawSpace(positionsCoordsAfterStretch[item.Item1].Item2, true);
                }
            }
            return 0.0; // unusable for now, could be refactored to help finding optimal width of measure
        }

        public void ArrangeUsingDurationTable(Dictionary<int, double> durationTable)
        {
            //-----------------
            // Keys: -3, -2, -1 used for attributes: clef, key and time
            //-----------------
            var measureBeginningAttributes = itemsWithPostition.Where(x => x.Item1 == 0 && x.Item2 is IAttributeItemVisual).Select(x => x.Item2).ToList();
            if (measureBeginningAttributes.Count != 0)
            {
                ClefContainerItem clef = measureBeginningAttributes.Where(x => x is ClefContainerItem).FirstOrDefault() as ClefContainerItem;
                if (clef != null)
                {
                    ArrangeAttributes(clef, durationTable[-3]);
                }
                KeyContainerItem key = measureBeginningAttributes.Where(x => x is KeyContainerItem).FirstOrDefault() as KeyContainerItem;
                if (key != null)
                {
                    ArrangeAttributes(key, durationTable[-2]);
                }
                TimeSignatureContainerItem time = measureBeginningAttributes.Where(x => x is TimeSignatureContainerItem).FirstOrDefault() as TimeSignatureContainerItem;
                if (time != null)
                {
                    ArrangeAttributes(time, durationTable[-1]);
                }
            }
            foreach (var item in itemsWithPostition)
            {
                if (item.Item2 is INoteItemVisual)
                {
                    INoteItemVisual note = item.Item2 as INoteItemVisual;
                    if (note.ItemDuration == 0)
                    {
                        continue;
                    }
                    SetLeft(item.Item2 as Canvas, durationTable[item.Item1]);
                }
            }
        }

        public List<int> GetDurationIndexes()
        {
            return itemsWithPostition.Select(x => x.Item1).Distinct().ToList();
        }

        /// <summary>
        /// Claculates base spacing factor for further proportional stretching of items inside measure
        /// </summary>
        /// <param name="duration">Duration of note/rest</param>
        /// <param name="shortest">Shortest duration in measure or shortest in ie. whole line of measures</param>
        /// <param name="alpha">Spacing parameter usually between {0.4 ; 0.6}</param>
        /// <returns></returns>
        private double SpacingValue(double duration, double shortest, double alpha)
        {
            double result = 1;
            result = 1 + (alpha * (Math.Log(duration / shortest, 2.0)));
            return result;
        }

        public double ArrangeAttributes(IAttributeItemVisual attributeVisual, double currentPosition = 0.0)
        {
            double width = currentPosition;
            LayoutStyle.MeasureLayoutStyle attributesLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.MeasureStyle;

            switch (attributeVisual.AttributeIndex)
            {
                case 0:
                    ClefContainerItem clef = attributeVisual as ClefContainerItem;
                    width += attributesLayout.ClefLeftOffset.TenthsToWPFUnit();
                    SetLeft(clef, width);
                    width += clef.ItemWidth + attributesLayout.ClefRightOffset.TenthsToWPFUnit();
                    break;
                case 1:
                    KeyContainerItem key = attributeVisual as KeyContainerItem;
                    width += attributesLayout.KeySigLeftOffset;
                    SetLeft(key, width);
                    width += key.ItemWidth + (key.ItemWidth != 0 ? attributesLayout.KeySigRightOffset.TenthsToWPFUnit() : 0);
                    break;
                case 2:
                    TimeSignatureContainerItem timeSig = attributeVisual as TimeSignatureContainerItem;
                    width += attributesLayout.TimeSigLeftOffset.TenthsToWPFUnit();
                    SetLeft(timeSig, width);
                    width += timeSig.ItemWidth + attributesLayout.TimeSigRightOffset.TenthsToWPFUnit();
                    break;
            }

            return width;
        }

        private double CalculatePositions(double availableWidth)
        {
            return availableWidth / availableDuration;
        }

        private int GetShortestDuration()
        {
            int shortest = int.MaxValue;
            foreach (INoteItemVisual durations in measureItemsVisuals)
            {
                if (durations.ItemDuration < shortest)
                {
                    shortest = durations.ItemDuration;
                }
            }
            return shortest;
        }
        private void GetTimeSignature()
        {
            timeSignature = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.TimeSignatures.TimeSignaturesDictionary[measureId];
            numerator = timeSignature.GetNumerator();
            denominator = timeSignature.GetDenominator();
            availableDuration =(int) (((4 / (double)denominator)* divisions) * numerator);
        }

        private void GetDivisions()
        {
            divisions = ViewModel.ViewModelLocator.Instance.Main.CurrentPartsProperties[partId].GetDivisionsMeasureId(measureId);
        }
        public void AppendNote(NoteContainerItem note, int cursorPosition, string voice = "1")
        {
            Tuple<int, IMeasureItemVisual> noteVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, note);
            AddNote(note);
            itemsWithPostition.Add(noteVisual);
        }

        public void AppendRest(RestContainterItem rest, int cursorPosition, string voice = "1")
        {
            Tuple<int, IMeasureItemVisual> restVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, rest);
            AddRest(rest);
            itemsWithPostition.Add(restVisual);
        }

        public void AppendAttribute(IAttributeItemVisual attributeItem, int cursorPosition)//temp
        {
            Tuple<int, IMeasureItemVisual> attributesVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, attributeItem);
            AddAttribute(attributeItem);
            itemsWithPostition.Add(attributesVisual);
        }
        public void AddNote(NoteContainerItem noteVisual)
        {
            measureItemsVisuals.Add(noteVisual);
            Children.Add(noteVisual);
        }
        public void AddRest(RestContainterItem restVisual)
        {
            measureItemsVisuals.Add(restVisual);
            Children.Add(restVisual);
        }
        public void AddAttribute(IAttributeItemVisual attributeVisual)//temp
        {
            measureItemsVisuals.Add(attributeVisual);
            Children.Add(attributeVisual as Canvas); //TODO_WiP TEST
        }
    }
}
