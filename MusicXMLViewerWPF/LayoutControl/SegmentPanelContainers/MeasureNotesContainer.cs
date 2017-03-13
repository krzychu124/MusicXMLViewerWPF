using MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes;
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
        public MeasureNotesContainer()
        {
            notesVisuals = new List<INoteItemVisual>();
        }
        public void AddNote(NoteContainerItem noteVisual)
        {
            notesVisuals.Add(noteVisual);
        }
        public void AddRest(RestContainterItem restVisual)
        {
            notesVisuals.Add(restVisual);
        }
    }
}
