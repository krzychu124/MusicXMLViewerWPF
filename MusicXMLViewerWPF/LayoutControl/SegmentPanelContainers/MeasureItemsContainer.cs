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

        public double ArrangeItemsByDuration(double availableWidth, int measureDuration)
        {
            //double offset = (availableWidth -10.0.TenthsToWPFUnit()) / (double)measureDuration;
            var measureBeginningAttributes = itemsWithPostition.Where(x => x.Item1 == 0 && x.Item2 is IAttributeItemVisual).Select(x=>x.Item2).ToList();
            itemsWithPostition.Sort((a,b)=> a.Item1.CompareTo(b.Item1));
            double beginningAttributesWidth = 0.0;
            if (measureBeginningAttributes.Count != 0)
            {
                double attributesWidth = 0.0;
                foreach (IAttributeItemVisual measureAttribute in measureBeginningAttributes)
                {
                    attributesWidth = ArrangeAttributes(measureAttribute , attributesWidth);
                }
                beginningAttributesWidth = attributesWidth;
            }
            double widthAvaliableForNotes = availableWidth - beginningAttributesWidth;


            double offset = (widthAvaliableForNotes - 1.0.TenthsToWPFUnit()) / (double)measureDuration;
            foreach (var item in itemsWithPostition)
            {
                if (item.Item2 is IAttributeItemVisual)
                {
                    if (item.Item1 != 0)
                    {
                        //if midmeasure attribute (clef, key or timeSignature
                        SetLeft(item.Item2 as Canvas, item.Item1 * offset + beginningAttributesWidth);
                    }
                }
                else
                {
                    SetLeft(item.Item2 as Canvas, item.Item1 * offset + beginningAttributesWidth);
                }
                if (item.Item2 is RestContainterItem && itemsWithPostition.Count == 1)
                {
                    double itemHalfWidth = item.Item2.ItemWidth / 2;
                    SetLeft(item.Item2 as Canvas, (availableWidth) / 2 - itemHalfWidth);
                    continue;
                }
            }
            return 0.0;
        }

        private double ArrangeAttributes(IAttributeItemVisual attributeVisual, double currentPosition = 0.0)
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
