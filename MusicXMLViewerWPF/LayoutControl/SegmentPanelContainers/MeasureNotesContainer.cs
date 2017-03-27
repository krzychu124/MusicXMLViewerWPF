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

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers
{
    class MeasureNotesContainer : Canvas
    {
        List<INoteItemVisual> notesVisuals;
        private List<NoteMusicXML> notesList;
        private List<Tuple<int, INoteItemVisual>> notesWithPostition;
        private ScorePartwisePartMeasureMusicXML measure;
        private string measureId = "";
        private string partId;
        private int staveNumber;

        private int divisions;
        private int availableDuration;
        private int numerator;
        private int denominator;
        private TimeMusicXML timeSignature;
        public MeasureNotesContainer(string measureId, string partId, int numberOfStave)
        {
            notesVisuals = new List<INoteItemVisual>();
            notesList = new List<NoteMusicXML>();
            notesWithPostition = new List<Tuple<int, INoteItemVisual>>();
            this.measureId = measureId;
            this.partId = partId;
            staveNumber = numberOfStave;
        }
        public MeasureNotesContainer(ScorePartwisePartMeasureMusicXML measure, string partId, int numberOfStave)
        {
            notesVisuals = new List<INoteItemVisual>();
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
            int count = notesVisuals.Count;
            double accOffset = leftOffset.TenthsToWPFUnit() *0.8;

            foreach (var item in notesVisuals)
            {
                if (item is RestContainterItem && notesVisuals.Count == 1)
                {
                    double itemHalfWidth = item.ItemWidthMin/2;
                    Canvas.SetLeft(item as Canvas, (availableWidth)/2 - itemHalfWidth);
                    continue;
                }
                Canvas.SetLeft(item as Canvas, accOffset);
                accOffset += item.ItemDuration *offset;
            }
        }

        public double ArrangeNotesByDuration(double availableWidth, int measureDuration)
        {
            double offset = (availableWidth -10.0.TenthsToWPFUnit()) / (double)measureDuration;
            foreach (var item in notesWithPostition)
            {
                SetLeft(item.Item2 as Canvas, item.Item1 * offset + 10.0.TenthsToWPFUnit());
                if (item.Item2 is RestContainterItem && notesWithPostition.Count == 1)
                {
                    double itemHalfWidth = item.Item2.ItemWidthMin / 2;
                    SetLeft(item.Item2 as Canvas, (availableWidth) / 2 - itemHalfWidth);
                    continue;
                }
            }
            return 0.0;
        }

        private double CalculatePositions(double availableWidth)
        {
            return availableWidth / availableDuration;
        }

        private int GetShortestDuration()
        {
            int shortest = int.MaxValue;
            foreach (INoteItemVisual durations in notesVisuals)
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
            Tuple<int, INoteItemVisual> noteVisual = new Tuple<int, INoteItemVisual>(cursorPosition, note);
            AddNote(note);
            notesWithPostition.Add(noteVisual);
        }

        public void AppendRest(RestContainterItem rest, int cursorPosition, string voice = "1")
        {
            Tuple<int, INoteItemVisual> restVisual = new Tuple<int, INoteItemVisual>(cursorPosition, rest);
            AddRest(rest);
            notesWithPostition.Add(restVisual);
        }

        public void AppendAttributes(MeasureAttributesContainer attributes, int cursorPosition)//temp
        {
            Tuple<int, INoteItemVisual> attributesVisual = new Tuple<int, INoteItemVisual>(cursorPosition, attributes);
            AddAttributes(attributes);
            notesWithPostition.Add(attributesVisual);
        }
        public void AddNote(NoteContainerItem noteVisual)
        {
            notesVisuals.Add(noteVisual);
            Children.Add(noteVisual);
        }
        public void AddRest(RestContainterItem restVisual)
        {
            notesVisuals.Add(restVisual);
            Children.Add(restVisual);
        }
        public void AddAttributes(MeasureAttributesContainer attributesVisual)//temp
        {
            notesVisuals.Add(attributesVisual);
            Children.Add(attributesVisual);
        }
    }
}
