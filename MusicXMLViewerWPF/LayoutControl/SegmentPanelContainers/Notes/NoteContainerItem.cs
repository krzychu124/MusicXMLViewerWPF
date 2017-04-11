using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using MusicXMLScore.Model.MeasureItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes
{
    class NoteContainerItem : INoteItemVisual
    {
        private Canvas itemCanvas;
        private static Random r= new Random();
        private int itemDuration = 0;
        private double itemWidth = 0.0;
        private double itemRightMargin = 0.0;
        private double itemWidthMin = 0.0;
        private double itemWidthOpt = 0.0;
        private List<NoteMusicXML> noteItem;
        private bool isChordNote;
        private int fractionPosition;
        private string symbol;
        private string measureId;
        private string partId;
        private List<object> pitchObject;
        private NoteChoiceTypeMusicXML noteVisualType = NoteChoiceTypeMusicXML.chord;
        private NoteChoiceTypeMusicXML noteAdditionalType = NoteChoiceTypeMusicXML.pitch;
        private Dictionary<int, double> staffLine = new Dictionary<int, double>();
        private NoteTypeValueMusicXML noteType = NoteTypeValueMusicXML.whole;
        private bool hasDots = false;
        private Dictionary<int, bool> altered;
        private int duration;
        private Dictionary<int, int> pitchedPosition;
        private Dictionary<int, double> pitchedValue;
        private double itemWeight = 0.0;
        private bool needLedgerLines = false;
        private List<double> ledgerLinesPositions;
        private string staffNumber;
        private Brush color;
        private StemItem stem;
        private BeamItem beams;
        private bool isSmall = false;
        private bool hasBeams = false;
        public NoteContainerItem(NoteMusicXML note, int fractionPosition, string partId, string measureId, string staffId)
        {
            noteItem = new List<NoteMusicXML>();
            noteItem.Add(note);
            isChordNote = false;
            this.fractionPosition = fractionPosition;
            SetVisualType();
            this.measureId = measureId;
            this.partId = partId;
            this.staffNumber = staffId;
            itemCanvas = new Canvas();
            InitNoteProperties();
            if (hasBeams)
            {
                InitBeams();
            }
        }
        public NoteContainerItem(List<NoteMusicXML> chordList, int fractionPosition, string partId, string measureId, string staffId)
        {
            noteItem = chordList;
            isChordNote = true;
            this.fractionPosition = fractionPosition;
            SetVisualType();
            this.measureId = measureId;
            this.partId = partId;
            this.staffNumber = staffId != null ? staffId : "1";
            itemCanvas = new Canvas();
            InitNoteProperties();
            if (hasBeams)
            {
                InitBeams();
            }
        }

        private void InitBeams()
        {
            string voice = noteItem.Where(x => x.Voice != null).Select(x => x.Voice).FirstOrDefault();
            beams = new BeamItem(noteItem.SelectMany(x => x.Beam).ToList(), voice, fractionPosition, stem);
        }

        private void SetVisualType()
        {
            noteVisualType = noteItem.ElementAt(0).ItemsElementName.Contains(NoteChoiceTypeMusicXML.grace) ? NoteChoiceTypeMusicXML.grace :
                noteItem.ElementAt(0).ItemsElementName.Contains(NoteChoiceTypeMusicXML.cue) ? NoteChoiceTypeMusicXML.cue : NoteChoiceTypeMusicXML.chord;
            noteAdditionalType = noteItem.ElementAt(0).ItemsElementName.Contains(NoteChoiceTypeMusicXML.pitch) ? NoteChoiceTypeMusicXML.pitch : NoteChoiceTypeMusicXML.unpitched;
        }
        private void InitNoteProperties()
        {
            
            duration = noteItem.ElementAt(0).GetDuration(); // always first because all notes in chord should have the same duration
            itemDuration = duration;
            staffLine = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.AvaliableIndexLinePositions;
            GetSymbol();
            GetPitch();
            Draw();
            stem = new StemItem(this);
            hasBeams = noteItem.Any(x => x.Beam.Count != 0) ? true : false;
        }

        private void Draw()
        {
            isSmall = noteVisualType == NoteChoiceTypeMusicXML.cue || noteVisualType == NoteChoiceTypeMusicXML.grace ? true : false;
            DrawingVisualHost noteCanvas = new DrawingVisualHost();
            int index = 0;
            foreach (var note in noteItem)
            {
                color = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.Colors[int.Parse(note.Voice)];
                noteCanvas.AddCharacterGlyph(new Point(0, pitchedValue[index]), symbol, isSmall, color);
                
                if (hasDots)
                {

                }
                index++;
            }
            itemWidthMin = DrawingMethods.GetTextWidth(symbol, TypeFaces.GetMusicFont(), isSmall);
            itemWidthOpt = itemWidthMin;
            CheckForLedgerLines();
            if (needLedgerLines)
            {
                Point position = new Point();
                int indexLedgers = 0;
                foreach (var ledgerPosition in ledgerLinesPositions)
                {
                    noteCanvas.AddLedgerLine(new Point(position.X, ledgerLinesPositions[indexLedgers]), itemWidthMin);
                    indexLedgers++;
                }
            }
            ItemCanvas.Children.Add(noteCanvas);
        }

        public void DrawSpace(double length, bool red = false)
        {
            Brush color;
            int shiftY = 50;
            DrawingVisualHost spaceCanvas = new DrawingVisualHost();
            if (red)
            {
                color = Brushes.Red;
                shiftY = 70;
            }
            else
            {
                color = Brushes.Green;
            }
            double y = r.Next(0, 15) + shiftY;
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawLine(new Pen(color, 3.0), new Point(0, y), new Point(length -0.05, y));
            }
            spaceCanvas.AddVisual(dv);
            ItemCanvas.Children.Add(spaceCanvas);
        }
        private void CheckForLedgerLines()
        {
            if (GetLowestPitchPosition() < -1)
            {
                needLedgerLines = true;
                var value = Math.Abs(GetLowestPitchPosition());
                var even = value % 2 == 0 ? true : false;
                int count = even ? (value + 1) /2 : value/2;
                SetLedgerLinesPositions(count, true);
            }
            if (GetHighestPitchPosition() > 9)
            {
                needLedgerLines = true;
                var value = Math.Abs(GetHighestPitchPosition());
                var even = value % 2 == 0 ? true : false;
                int count = even ? (value - 10) /2 + 1 : (value - 9) /2;
                SetLedgerLinesPositions(count, false);
            }
        }
        private int GetHighestPitchPosition()
        {
            return pitchedPosition.Values.Max();
        }
        private int GetLowestPitchPosition()
        {
            return pitchedPosition.Values.Min();
        }


        private void SetLedgerLinesPositions(int count, bool above = false)
        {
            if (above)
            {
                ledgerLinesPositions = new List<double>();
                for (int i = 1; i <= count; i++)
                {
                    int index = -(i * 2);
                    ledgerLinesPositions.Add(staffLine[index]);
                }
            }
            else
            {
                ledgerLinesPositions = new List<double>();
                for (int i = 0; i < count; i++)
                {
                    int index = 10 +(i * 2);
                    ledgerLinesPositions.Add(staffLine[index]);
                }
            }
        }
        private void GetSymbol()
        {
            Tuple<NoteTypeValueMusicXML, bool> value = CalculationHelpers.GetBaseDurationValue(duration, partId, measureId);
            noteType = value.Item1;
            
            hasDots = value.Item2;
            symbol = MusicSymbols.GetNoteHeadSymbolNoteType(noteType);
        }

        private void GetPitch()
        {
            //var clef = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetClef(measureId, partId, int.Parse(staffId));
            var clef = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetClef(measureId, partId, int.Parse(staffNumber), fractionPosition);
            pitchObject = new List<object>();
            altered = new Dictionary<int, bool>();
            pitchedPosition = new Dictionary<int, int>();
            pitchedValue = new Dictionary<int, double>();
            foreach (var note in noteItem)
            {
                pitchObject.Add(note.Items[note.GetIndexOfType(noteAdditionalType)]);
                int pitchIndex = pitchObject.Count -1;
                if (pitchObject[pitchIndex] is PitchMusicXML)
                {
                    PitchMusicXML pitch = (PitchMusicXML)pitchObject[pitchIndex];
                    altered.Add(pitchIndex, pitch.AlterSpecified);
                    pitchedPosition.Add(pitchIndex, CalculationHelpers.GetPitchIndexStaffLine(pitch, clef));
                    pitchedValue.Add(pitchIndex, staffLine[pitchedPosition[pitchIndex]]);
                }
                else
                {
                    UnpitchedMusicXML pitch = (UnpitchedMusicXML)pitchObject[pitchIndex];
                    pitchedPosition.Add(pitchIndex, CalculationHelpers.GetPitchIndexStaffLine(new PitchMusicXML() { Step = pitch.DisplayStep, Octave = pitch.DisplayOctave }, clef));
                    pitchedValue.Add(pitchIndex, staffLine[pitchedPosition[pitchIndex]]);
                }
            }
        }

        private void CalculateWeight()
        {

        }
        public double ItemWidthMin
        {
            get
            {
                return itemWidthMin;
            }

            set
            {
                itemWidthMin = value;
            }
        }

        public double ItemWidthOpt
        {
            get
            {
                return itemWidthOpt;
            }

            set
            {
                itemWidthOpt = value;
            }
        }

        public double ItemWeight
        {
            get
            {
                return itemWeight;
            }
        }

        public int ItemDuration
        {
            get
            {
                return itemDuration;
            }
        }

        public double ItemWidth
        {
            get
            {
                return itemWidth;
            }

            set
            {
                itemWidth = value;
            }
        }

        public Canvas ItemCanvas
        {
            get
            {
                return itemCanvas;
            }

            set
            {
                itemCanvas = value;
            }
        }

        public Dictionary<int, int> PitchedPosition
        {
            get
            {
                return pitchedPosition;
            }

            set
            {
                pitchedPosition = value;
            }
        }

        public List<NoteMusicXML> NoteItem
        {
            get
            {
                return noteItem;
            }

            set
            {
                noteItem = value;
            }
        }

        public Dictionary<int, double> StaffLine
        {
            get
            {
                return staffLine;
            }

            set
            {
                staffLine = value;
            }
        }

        public Brush Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
            }
        }

        public bool IsSmall
        {
            get
            {
                return isSmall;
            }

            set
            {
                isSmall = value;
            }
        }

        public BeamItem Beams
        {
            get
            {
                return beams;
            }

            set
            {
                beams = value;
            }
        }

        public StemItem Stem
        {
            get
            {
                return stem;
            }

            set
            {
                stem = value;
            }
        }
    }
}
