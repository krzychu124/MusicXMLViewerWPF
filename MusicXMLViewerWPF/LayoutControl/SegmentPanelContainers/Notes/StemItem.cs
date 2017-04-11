using MusicXMLScore.Converters;
using MusicXMLScore.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using MusicXMLScore.Model.MeasureItems.NoteItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes
{
    class StemItem
    {
        private StemValueMusicXML direction;
        private bool visible = true;
        private Point startPoint;
        private Point endPoint;
        private NoteContainerItem note;
        private double sizeFactor;
        private Dictionary<int, double> positionsTable;

        internal NoteContainerItem NoteReference
        {
            get
            {
                return note;
            }

            set
            {
                note = value;
            }
        }

        public StemItem(NoteContainerItem note)
        {
            this.note = note;
            positionsTable = note.StaffLine;
            sizeFactor = note.IsSmall ? 0.8 : 1.0;
            InitStem();
            Draw();
        }

        private void InitStem()
        {
           if (note.NoteItem.Count == 1)
            {
                var noteItem = note.NoteItem.ElementAt(0);
                var stem = noteItem.Stem;
                if (stem != null)
                {
                    GetDirection(stem);
                    CalculatePosition(note.PitchedPosition.FirstOrDefault().Value);
                }
            }
        }

        private void CalculatePosition(int notePitchedPosition)
        {
            double noteWidth = note.ItemWidthMin;
            double y = positionsTable[notePitchedPosition];
            double length = 35.0.TenthsToWPFUnit() * sizeFactor;
            double yShift = 1.68.TenthsToWPFUnit();
            if(direction == StemValueMusicXML.up)
            {
                startPoint = new Point(noteWidth * sizeFactor, y - yShift);
                endPoint = new Point(noteWidth * sizeFactor, y - length);
            }
            else
            {
                startPoint = new Point(0, y + yShift);
                endPoint = new Point(0, y + length);
            }
        }
        public Point GetStemEnd()
        {
            return endPoint;
        }

        public Point GetStemBegin()
        {
            return startPoint;
        }

        public Brush GetColor()
        {
            return note.Color;
        }

        private void GetDirection(StemMusicXML stem)
        {
            direction = stem.Value;
        }

        private void Draw()
        {
            LayoutStyle.NotesLayoutStyle notesStyle = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.NotesStyle;
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawLine(new Pen(note.Color, notesStyle.StemThickness * sizeFactor), startPoint, endPoint);
            }
            DrawingVisualHost dvh = new DrawingVisualHost();
            dvh.AddVisual(dv);
            note.ItemCanvas.Children.Add(dvh);
        }
    }
}
