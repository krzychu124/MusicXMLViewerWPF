using MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes;
using MusicXMLScore.Model;
using MusicXMLScore.Model.MeasureItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers
{
    class MeasureNotesContainer : Canvas
    {
        List<INoteItemVisual> notesVisuals;
        private List<NoteMusicXML> notesList;
        public MeasureNotesContainer(ScorePartwisePartMeasureMusicXML measure, string partId, int numberOfStave)
        {
            notesVisuals = new List<INoteItemVisual>();
            notesList = new List<NoteMusicXML>();
            notesList = measure.Items.OfType<NoteMusicXML>().ToList();
            
            for (int i = 0; i < notesList.Count; i++)
            {
                if (notesList[i].Items.OfType<RestMusicXML>().Any())
                {
                    RestContainterItem rest = new RestContainterItem(notesList[i], i, partId, measure.Number);
                    AddRest(rest);
                }
            }
            ArrangeNotes(measure.CalculatedWidth);
        }

        private void ArrangeNotes(double width)
        {
            int count = notesVisuals.Count;
            double offset = (width/count) / 2;
            double accOffset = offset;
            foreach (var item in notesVisuals)
            {
                Canvas.SetLeft(item as Canvas, accOffset);
                accOffset += offset + offset;
            }
        }

        public void AddNote(NoteContainerItem noteVisual)
        {
            notesVisuals.Add(noteVisual);
        }
        public void AddRest(RestContainterItem restVisual)
        {
            notesVisuals.Add(restVisual);
            Children.Add(restVisual);
        }
    }
}
