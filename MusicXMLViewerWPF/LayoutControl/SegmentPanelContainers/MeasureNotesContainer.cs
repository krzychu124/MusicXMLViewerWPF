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

        public void ArrangeNotes(double avaliablewidth)
        {
            int count = notesVisuals.Count;
            double offset = ((avaliablewidth * 0.9) / count);
            double accOffset = offset /2;
            if( count == 1)
            {
                accOffset = avaliablewidth / 2;
            }
            foreach (var item in notesVisuals)
            {
                Canvas.SetLeft(item as Canvas, accOffset);
                accOffset += offset;
            }
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
