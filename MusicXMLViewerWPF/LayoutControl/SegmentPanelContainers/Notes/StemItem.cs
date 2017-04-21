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
        private Point calculatedStartPoint;
        private Point endPoint;
        private Point calculatedEndPoint;
        private NoteContainerItem note;
        private double sizeFactor;
        private Dictionary<int, double> positionsTable;
        private double stemLength;
        private bool isDown;
        private double staffLineOffset;

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

        public double StemLength
        {
            get
            {
                return stemLength;
            }

            set
            {
                stemLength = value;
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
                if (stem != null) //! if stem is set for current Note
                {
                    GetDirection(stem);
                    bool hasBeam = note.NoteItem.FirstOrDefault().Beam.Count != 0 ? true : false;
                    CalculatePosition(note.PitchedPosition.FirstOrDefault().Value, hasBeam);
                }
            }
            else
            {
                var notes = note.NoteItem.Where(noteItem => noteItem.IsGrace() == false);
                foreach (var noteItem in notes)
                {
                    //var noteItem = note;
                    var stem = noteItem.Stem;
                    if (stem != null) //! if stem is set for current Note
                    {
                        GetDirection(stem);
                        bool hasBeam = note.NoteItem.FirstOrDefault().Beam.Count != 0 ? true : false;
                        int pitchedPosition = 0;
                        if (isDown)
                        {
                            pitchedPosition = note.PitchedPosition.Max(x => x.Value);
                        }
                        else
                        {
                            pitchedPosition = note.PitchedPosition.Min(x => x.Value);
                        }
                        CalculatePosition(pitchedPosition, hasBeam);
                    }
                }
            }
        }

        private void CalculatePosition(int notePitchedPosition, bool hasBeam = false)
        {
            double noteWidth = note.ItemWidthMin;
            double y = positionsTable[notePitchedPosition];
            double length = 35.0.TenthsToWPFUnit() * sizeFactor;
            double yShift = 1.68.TenthsToWPFUnit();


            if (direction == StemValueMusicXML.up)
            {
                int highestPitch = note.PitchedPosition.Max(x => x.Value);
                double highestPitchPosition = positionsTable[highestPitch];
                startPoint = new Point(noteWidth * sizeFactor, highestPitchPosition - yShift);
                if (notePitchedPosition > 10)
                {
                    //! set end point to middle staff line
                    endPoint = new Point(noteWidth * sizeFactor, positionsTable[4]);
                }
                else
                {
                    if (notePitchedPosition < 4) // lower than middle staffline pitch position
                    {
                        //! shortest length
                        double shortestLength = 25.0.TenthsToWPFUnit() * sizeFactor;
                        if (hasBeam)
                        {
                            //! beam thickness added to length
                            shortestLength = 30.0.TenthsToWPFUnit() * sizeFactor; 
                        }
                        if (notePitchedPosition < -2)
                        {
                            //! set shortest length
                            endPoint = new Point(noteWidth * sizeFactor, y - shortestLength); 
                        }
                        else
                        {
                            //shorten legth according to pitch (between normal length and shortest)
                            double shortestLengthCorrection = ((notePitchedPosition + 2) / (double)6) * 10.0.TenthsToWPFUnit();
                            endPoint = new Point(noteWidth * sizeFactor, y - (shortestLength + shortestLengthCorrection));
                        }
                    }
                    else //! If pitch is between 4 and 10
                    {
                        endPoint = new Point(noteWidth * sizeFactor, y - length);
                    }
                }
                isDown = false;
            }
            else //! stem down
            {
                int highestPitch = note.PitchedPosition.Min(x => x.Value);
                double highestPitchPosition = positionsTable[highestPitch];
                startPoint = new Point(0, highestPitchPosition + yShift);
                if (notePitchedPosition < -2)
                {
                    //! set end point to middle staff line
                    endPoint = new Point(0, positionsTable[4]);
                }
                else
                {
                    if (notePitchedPosition > 4) // higher than middle staffline pitch position
                    {
                        double shortestLength = 25.0.TenthsToWPFUnit() * sizeFactor;
                        if (hasBeam)
                        {
                            //! beam thickness added to length
                            shortestLength = 30.0.TenthsToWPFUnit() * sizeFactor;
                        }
                        if (notePitchedPosition > 10)
                        {
                            //! set shortest length
                            endPoint = new Point(0, y + shortestLength);
                        }
                        else
                        {
                            //! shorten legth according to pitch (between normal length and shortest)
                            double shortestLengthCorrection = ((10 - notePitchedPosition) / (double)5) * 10.0.TenthsToWPFUnit();
                            endPoint = new Point(0, y + (shortestLength + shortestLengthCorrection));
                        }
                    }
                    else //! if pitch is between -2 and 4
                    {
                        endPoint = new Point(0, y + length);
                    }
                }
                isDown = true;
            }
            CalculateStemLength();
        }
        private void CalculateStemLength()
        {
            if (IsDirectionDown())
            {
                stemLength = endPoint.Y - startPoint.Y;
            }
            else
            {
                stemLength = startPoint.Y - endPoint.Y;
            }
        }

        public Point GetStemEnd()
        {
            return endPoint;
        }

        /// <summary>
        /// StemEndPoint corrected with staff number position
        /// </summary>
        /// <returns></returns>
        public Point GetStemEndCalculated()
        {
            return calculatedEndPoint;
        }

        public Point GetStemBegin()
        {
            return startPoint;
        }

        public Brush GetColor()
        {
            return note.Color;
        }

        /// <summary>
        /// Returns true if stem direction is down
        /// </summary>
        /// <returns></returns>
        public bool IsDirectionDown()
        {
            return isDown;
        }

        /// <summary>
        /// Calculates stem start and end point positions according to staffLine placement
        /// </summary>
        /// <param name="offset"></param>
        public void SetStaffOffset(double offset)
        {
            staffLineOffset = offset;
            calculatedStartPoint = new Point(startPoint.X, startPoint.Y + offset);
            calculatedEndPoint = new Point(endPoint.X, endPoint.Y + offset);
        }

        /// <summary>
        /// Sets stem direction using StemMusicXML object
        /// </summary>
        /// <param name="stem"></param>
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
            dvh.Tag = "stem";
            note.AddStem(dvh);
        }

        /// <summary>
        /// Sets new Stem EndPoint Vertical value and updates stem visual
        /// </summary>
        /// <param name="endPointY"></param>
        internal void SetEndPointY(double endPointY)
        {
            calculatedEndPoint.Y = endPointY;
            endPoint.Y = endPointY - staffLineOffset;
            CalculateStemLength();
            Draw();
        }
    }
}
