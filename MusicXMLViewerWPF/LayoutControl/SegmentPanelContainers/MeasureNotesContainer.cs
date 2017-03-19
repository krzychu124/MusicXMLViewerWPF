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
        private ScorePartwisePartMeasureMusicXML measure;
        private string partId;
        private int staveNumber;

        private int divisions;
        private int availableDuration;
        private int numerator;
        private int denominator;
        private TimeMusicXML timeSignature;
        public MeasureNotesContainer(ScorePartwisePartMeasureMusicXML measure, string partId, int numberOfStave)
        {
            notesVisuals = new List<INoteItemVisual>();
            notesList = new List<NoteMusicXML>();
            this.measure = measure;
            this.partId = partId;
            staveNumber = numberOfStave;
            notesList = measure.Items.OfType<NoteMusicXML>().ToList();
            
            for (int i = 0; i < notesList.Count; i++)
            {
                var noteType = notesList[i].GetNoteType();
                if (noteType.HasFlag(NoteChoiceTypeMusicXML.rest))
                {
                    RestContainterItem rest = new RestContainterItem(notesList[i], i, partId, measure.Number);
                    AddRest(rest);
                }
                if (noteType.HasFlag(NoteChoiceTypeMusicXML.pitch) || noteType.HasFlag(NoteChoiceTypeMusicXML.unpitched))
                {
                    NoteContainerItem note = new NoteContainerItem(notesList[i], i, partId, measure.Number, noteType);
                    AddNote(note);
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
            timeSignature = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.TimeSignatures.TimeSignaturesDictionary[measure.Number];
            numerator = timeSignature.GetNumerator();
            denominator = timeSignature.GetDenominator();
            availableDuration =(int) (((4 / (double)denominator)* divisions) * numerator);
        }

        private void GetDivisions()
        {
            divisions = ViewModel.ViewModelLocator.Instance.Main.CurrentPartsProperties[partId].GetDivisionsMeasureId(measure.Number);
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
    }
}
