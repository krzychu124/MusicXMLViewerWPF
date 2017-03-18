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

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes
{
    class NoteContainerItem : Canvas, INoteItemVisual
    {
        private double itemWidthMin = 0.0;
        private double itemWidthOpt = 0.0;
        private NoteMusicXML noteItem;
        private int noteIndex;
        private string symbol;
        private string measureId;
        private string partId;
        private object pitchObject;
        private NoteChoiceTypeMusicXML noteVisualType = NoteChoiceTypeMusicXML.chord;
        private NoteChoiceTypeMusicXML noteAdditionalType;
        private Dictionary<int, double> staffLine = new Dictionary<int, double>();
        private NoteTypeValueMusicXML noteType = NoteTypeValueMusicXML.whole;
        private bool hasDots = false;
        private bool altered = false;
        private int duration;
        private int pitchedPosition = 10;
        private double pitchedValue = 0.0;
        private double itemWeight;
        private bool needLedgerLines = false;
        private List<double> ledgerLinesPositions;
        public NoteContainerItem(NoteMusicXML note, int index, string partId, string measureId, NoteChoiceTypeMusicXML noteType)
        {
            noteItem = note;
            noteIndex = index;
            noteVisualType = noteType.HasFlag(NoteChoiceTypeMusicXML.chord)? NoteChoiceTypeMusicXML.chord : noteType.HasFlag(NoteChoiceTypeMusicXML.grace) ? NoteChoiceTypeMusicXML.grace : NoteChoiceTypeMusicXML.cue;
            noteAdditionalType = noteType.HasFlag(NoteChoiceTypeMusicXML.pitch) ? NoteChoiceTypeMusicXML.pitch : NoteChoiceTypeMusicXML.unpitched;
            this.measureId = measureId;
            this.partId = partId;
            InitNoteProperties();
        }

        private void InitNoteProperties()
        {
            duration = noteItem.GetDuration();
            staffLine = ViewModel.ViewModelLocator.Instance.Main.CurrentPageProperties.AvaliableIndexLinePositions;
            GetSymbol();
            GetPitch();
            Draw();
        }

        private void Draw()
        {
            bool small = noteVisualType == NoteChoiceTypeMusicXML.cue || noteVisualType == NoteChoiceTypeMusicXML.grace ? true : false;
            CanvasList note = new CanvasList(10, 10);
            note.AddCharacterGlyph(new System.Windows.Point(0, pitchedValue), symbol, small);
            itemWidthMin = DrawingMethods.GetTextWidth(symbol, TypeFaces.GetMusicFont());
            itemWidthOpt = itemWidthMin;
            if (hasDots)
            {

            }
            CheckForLedgerLines();
            if (needLedgerLines)
            {
                Point position = new Point();
                int index = 0;
                foreach (var ledgerPosition in ledgerLinesPositions)
                {
                    note.AddLedgerLine(new Point(position.X, ledgerLinesPositions[index]), itemWidthMin);
                    index++;
                }
            }
            Children.Add(note);
        }

        private void CheckForLedgerLines()
        {
            if (pitchedPosition < -1)
            {
                needLedgerLines = true;
                var value = Math.Abs(pitchedPosition);
                var even = value % 2 == 0 ? true : false;
                int count = even ? (value + 1) /2 : value/2;
                SetLedgerLinesPositions(count, true);
            }
            if (pitchedPosition > 9)
            {
                needLedgerLines = true;
                var value = Math.Abs(pitchedPosition);
                var even = value % 2 == 0 ? true : false;
                int count = even ? (value - 10) /2 + 1 : (value - 9) /2;
                SetLedgerLinesPositions(count, false);
            }
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
            var clef = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetClef(measureId, partId);
            pitchObject = noteItem.Items[noteItem.GetIndexOfType(noteAdditionalType)];
            if (pitchObject is PitchMusicXML)
            {
                PitchMusicXML pitch = (PitchMusicXML)pitchObject;
                altered = pitch.AlterSpecified;
                pitchedPosition = CalculationHelpers.GetPitchIndexStaffLine(pitch, clef);
                pitchedValue = staffLine[pitchedPosition];
            }
            else
            {
                UnpitchedMusicXML pitch = (UnpitchedMusicXML)pitchObject;
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
    }
}
