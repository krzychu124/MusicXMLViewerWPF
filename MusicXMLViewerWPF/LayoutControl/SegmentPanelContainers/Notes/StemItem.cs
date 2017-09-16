using MusicXMLScore.Converters;
using MusicXMLScore.Helpers;
using MusicXMLScore.Model.MeasureItems.NoteItems;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private DrawingVisualHost stemVisual;
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
                var noteItem = note.NoteItem[0];
                var stem = noteItem.Stem;
                if (stem != null) //! if stem is set for current Note
                {
                    GetDirection(stem);
                    bool hasBeam = note.NoteItem.FirstOrDefault().Beam?.Count != 0;
                    CalculatePosition(note.PitchedPosition.FirstOrDefault().Value, hasBeam);
                }
                else
                {
                    CalculateDirection();
                    bool hasBeam = note.NoteItem.FirstOrDefault().Beam.Count != 0;
                    CalculatePosition(note.PitchedPosition.FirstOrDefault().Value, hasBeam);
                }
            }
            else
            {
                var notes = note.NoteItem.Where(noteItem => !noteItem.IsGrace());
                //! check for stem element to get direction
                if (notes.FirstOrDefault().Stem != null)
                {
                    GetDirection(notes.FirstOrDefault().Stem);
                }
                else
                {
                    CalculateDirection(true);
                }

                foreach (var noteItem in notes)
                {
                    bool hasBeam = note.NoteItem.FirstOrDefault().Beam?.Count != 0;
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

        private void CalculateDirection(bool isChord = false)
        {
            if (!isChord)
            {
                //! Get first pitch (if not chord there is only one pitch 
                var pitch = note.PitchedPosition.FirstOrDefault().Value;
                SetDirection(pitch);
            }
            else
            {
                //! gets indexes of notes excudes grace notes
                var indexes = note.NoteItem.Select((x, i) => new { note = x, index = i }).Where(x => !x.note.IsGrace()).Select(x=>x.index).ToList();
                //! select pitches using indexes
                var pitches = indexes.Select(x => note.PitchedPosition[x]);
                //? var pitches = note.PitchedPosition.Select(x => x.Value); Previous version (grace notes not excluded)

                //! staff line indexes are nubered from 0 from first staff upper staff line
                //! positive if notehead placed below this line
                //! negative if notehead is placed above first staff line of staffLine(ledger lines)
                //! if all noteheads are placed above middle line - direction down
                if (pitches.Max() < 5)
                {
                    direction = StemValueMusicXML.down;
                    return;
                }
                //! if all noteheds below middle staff line
                if (pitches.Min() > 4)
                {
                    direction = StemValueMusicXML.up;
                    return;
                }

                //! none of previous continue calculations

                int[] distanceToMid = new int[pitches.Count()];
                for (int i = 0; i < pitches.Count(); i++)
                {
                    distanceToMid[i] = Math.Abs(pitches.ElementAt(i) - 4);
                }
                var maxDistCount = distanceToMid.Select(x => x).Count(x => x == distanceToMid.Max());
                if (maxDistCount == 0)
                {
                    Log.LoggIt.Log("Chorded notes stem direction calculation exception! Max distance to middle line equals 0", Log.LogType.Exception);
                    throw new NotImplementedException();//! temp debug only
                }
                //! If if two noteheads are placed in same distance from middle line, set direction to down;
                if (maxDistCount == 2)
                {
                    //! sum up notehead above and below
                    var aboveMiddle = pitches.Select(x => x).Count(x => x > 4);
                    var belowMiddle = pitches.Select(x => x).Count(x => x < 4);
                    //! find which direction has more noteheads 

                    if (aboveMiddle > belowMiddle)
                    {
                        direction = StemValueMusicXML.down;
                    }
                    if (aboveMiddle < belowMiddle)
                    {
                        direction = StemValueMusicXML.up;
                    }
                    //! if equal set stem direction down
                    else
                    {
                        direction = StemValueMusicXML.down;
                    }
                }
                else
                {
                    //! Gets max distance index to retrive pitch and set direction 
                    SetDirection(pitches.ElementAt(Array.IndexOf(distanceToMid, distanceToMid.Max())));
                }
            }
        }
        /// <summary>
        /// Compares pitch pitch middle staff line pitch. If lower, direction is up
        /// </summary>
        /// <param name="pitch"></param>
        private void SetDirection(int pitch)
        {
            if (pitch > 4)
            {
                direction = StemValueMusicXML.up;
            }
            else
            {
                direction = StemValueMusicXML.down;
            }
        }

        private void Draw()
        {
            LayoutStyle.NotesLayoutStyle notesStyle = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.NotesStyle;
            DrawingVisual dravingVisualStem = new DrawingVisual();
            using (DrawingContext dc = dravingVisualStem.RenderOpen())
            {
                dc.DrawLine(new Pen(note.Color, notesStyle.StemThickness * sizeFactor), startPoint, endPoint);
            }
            //! check if added previously to prevent unnecessary remove+add calls
            bool stemVisualAdded = false;
            if (stemVisual == null)
            {
                stemVisual = new DrawingVisualHost();
                stemVisual.Tag = "stem";
            }
            else
            {
                stemVisual.ClearVisuals();
                stemVisualAdded = true; //! no need to add to noteVisual
            }
            //! 
            stemVisual.AddVisual(dravingVisualStem);

            //! add visual to noteVisual (one time only)
            if (!stemVisualAdded)
            {
                note.AddStem(stemVisual);
            }
        }
        /// <summary>
        /// Sets new Stem EndPoint Vertical value and updates stem visual
        /// </summary>
        /// <param name="endPointY"></param>
        internal void SetEndPointY(double endPointY)
        {
            if (endPointY != calculatedEndPoint.Y)
            {
                calculatedEndPoint.Y = endPointY;
                endPoint.Y = endPointY - staffLineOffset;
                CalculateStemLength();
                Draw();
            }
        }
    }
}
