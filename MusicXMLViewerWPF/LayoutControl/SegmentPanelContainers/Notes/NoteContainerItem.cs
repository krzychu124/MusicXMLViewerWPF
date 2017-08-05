using MusicXMLScore.Converters;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.Model.MeasureItems.NoteItems;
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
        private int itemDuration = 0;
        private double itemWidth = 0.0;
        private double itemRightMargin = 0.0;
        private double itemLeftMargin = 0.0;
        private double itemWidthMin = 0.0;
        private double horizontalOffset = 0.0;
        private double verticalOffset = 0.0;
        private List<NoteMusicXML> noteItem;
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
        private bool needLedgerLines = false;
        private List<double> ledgerLinesPositions;
        private string itemStaff;
        private Brush color;
        private StemItem stem;
        private BeamItem beams;
        private bool isSmall = false;
        private bool hasBeams = false;
        private LayoutStyle.Layout layoutStyle;

        public NoteContainerItem(List<NoteMusicXML> chordList, int fractionPosition, string partId, string measureId, string staffId)
        {
            layoutStyle = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle;
            noteItem = new List<NoteMusicXML>(chordList);
            this.fractionPosition = fractionPosition;
            SetVisualType();
            this.measureId = measureId;
            this.partId = partId;
            this.itemStaff = staffId ?? "1";
            itemCanvas = new Canvas();

            InitNoteProperties();
            InitBeams();
        }

        private void DrawFlag()
        {
            bool isFlagDownwards = stem.IsDirectionDown();
            string flagSymbol = MusicSymbols.GetFlag(noteType, isFlagDownwards);
            Point stemEnd = stem.GetStemEnd();
            DrawingVisualHost flagHost = new DrawingVisualHost();
            flagHost.AddCharacterGlyph(stemEnd, flagSymbol, isSmall, color);
            flagHost.Tag = "flag";
            if (!isFlagDownwards)
            {
                itemRightMargin = DrawingMethods.GetTextWidth(flagSymbol, TypeFaces.GetMusicFont());
            }
            AddFlag(flagHost);
        }

        private void InitBeams(bool update = false)
        {
            if (hasBeams)
            {
                if (update)
                {
                    if (NoteItem.Count != 0)
                    {
                        InitStem();
                    }
                }
                string voice = noteItem.Where(x => x.Voice != null).Select(x => x.Voice).FirstOrDefault();
                beams = new BeamItem(noteItem.SelectMany(x => x.Beam).ToList(), voice, fractionPosition, stem);
            }
            else
            {
                if ((int)noteType < 8)
                {
                    DrawFlag();
                }
            }
        }

        public void UpdateStemsAndBeams()
        {
            InitBeams(true); //! temp,test
        }

        private void SetVisualType()
        {
            noteVisualType = noteItem[0].ItemsElementName.Contains(NoteChoiceTypeMusicXML.grace) ? NoteChoiceTypeMusicXML.grace :
                noteItem[0].ItemsElementName.Contains(NoteChoiceTypeMusicXML.cue) ? NoteChoiceTypeMusicXML.cue : NoteChoiceTypeMusicXML.chord;

            noteAdditionalType = noteItem[0].ItemsElementName.Contains(NoteChoiceTypeMusicXML.pitch) ? NoteChoiceTypeMusicXML.pitch : NoteChoiceTypeMusicXML.unpitched;
        }

        private void InitNoteProperties()
        {
            duration = noteItem[0].GetDuration(); // always first because all notes in chord should have the same duration
            itemDuration = duration;
            staffLine = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.AvaliableIndexLinePositions;
            GetSymbol();
            GetPitch();
            Draw();
            InitStem();
        }

        private void InitStem()
        {
            //! in note type/duration is half/semibreve or shorter
            if ((int)noteType < 10)
            {
                stem = new StemItem(this);
                //! if note type/duration is quarter/crotchet or shorter
                if ((int)noteType < 9)
                {
                    hasBeams = noteItem.Any(x => x.Beam.Count != 0);
                }
            }
        }

        private void Draw()
        {
            isSmall = noteVisualType == NoteChoiceTypeMusicXML.cue || noteVisualType == NoteChoiceTypeMusicXML.grace;
            DrawingVisualHost noteVisualHost = new DrawingVisualHost();
            int index = 0;
            foreach (var note in noteItem)
            {
                if (note.Voice == null)
                {
                    Log.LoggIt.Log($"Missing note voice element, setting to default", Log.LogType.Warning);
                    note.Voice = "1"; //! voice set to "1" if null- bugfix
                }
                color = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.Colors[int.Parse(note.Voice)];
                noteVisualHost.AddCharacterGlyph(new Point(0, pitchedValue[index]), symbol, isSmall, color);
                if (note.Accidental != null)
                {
                    DrawAccidental(note, noteItem.IndexOf(note), noteVisualHost);
                }
                if (hasDots)
                {
                    DrawDots(noteVisualHost, index);
                }
                index++;
            }
            itemWidthMin = DrawingMethods.GetTextWidth(symbol, TypeFaces.GetMusicFont(), isSmall);
            itemWidth = itemWidthMin;
            CheckForLedgerLines();
            if (needLedgerLines)
            {
                Point position = new Point();
                int indexLedgers = 0;
                foreach (var ledgerPosition in ledgerLinesPositions)
                {
                    noteVisualHost.AddLedgerLine(new Point(position.X, ledgerLinesPositions[indexLedgers]), itemWidthMin);
                    indexLedgers++;
                }
            }
            ItemCanvas.Children.Add(noteVisualHost);
        }

        private void DrawDots(DrawingVisualHost noteVisualHost, int index)
        {
            double dotPositionY = pitchedValue[index];
            if (pitchedPosition[index] % 2 == 0)
            {
                double sizeFactor = isSmall ? 0.8 : 1.0;
                double shiftUp = 5.0.TenthsToWPFUnit() * sizeFactor;
                dotPositionY -= shiftUp;
            }
            int dotCount = noteItem[index].Dot.Count;
            double noteWidth = DrawingMethods.GetTextWidth(symbol, TypeFaces.GetMusicFont(), isSmall);
            double dotWidth = DrawingMethods.GetTextWidth(MusicSymbols.Dot, TypeFaces.GetMusicFont(), isSmall);
            Point dotPosition = new Point(noteWidth + layoutStyle.NotesStyle.DotStandardOffset.TenthsToWPFUnit(), dotPositionY);
            for (int i = 0; i < dotCount; i++)
            {
                noteVisualHost.AddCharacterGlyph(dotPosition, MusicSymbols.Dot, isSmall, color);
                dotPosition.X += layoutStyle.NotesStyle.DotStandardOffset.TenthsToWPFUnit() + dotWidth;
            }
        }

        private void DrawAccidental(NoteMusicXML note, int index, DrawingVisualHost noteVisualHost)
        {
            AccidentalMusicXML accidental = note.Accidental;
            string accidentalSymbol = GetAccidentalSymbolString(accidental.Value);
            double size = 1.0;
            double totalAccidentalSize = 0.0;
            bool hasBracket = accidental.BracketSpecified ? accidental.Bracket == YesNoMusicXML.yes: false;
            bool hasParentheses = accidental.ParenthesesSpecified ? accidental.Parentheses == YesNoMusicXML.yes: false;
            if (isSmall)
            {
                size = 0.7;
            }
            if (accidental.SizeSpecified)
            {
                size = accidental.Size == SymbolSizeMusicXML.cue ? 0.7 : accidental.Size == SymbolSizeMusicXML.large ? 1.2 : 1.0;
            }
            if (accidental.CautionarySpecified)
            {
                hasParentheses = accidental.Cautionary == YesNoMusicXML.yes;
                //! missing implementation if yes
            }
            //! accidental.Editiorial skipped...

            double accidentalWidth = DrawingMethods.GetTextWidth(accidentalSymbol, TypeFaces.GetMusicFont(), isSmall);
            double accidentalMargin = layoutStyle.NotesStyle.AccidentalToNoteSpace.TenthsToWPFUnit();
            double noteHeadYPosition = pitchedValue[index];

            if (hasBracket)
            {
                totalAccidentalSize += 2 * DrawingMethods.GetTextWidth(MusicSymbols.AccidentalBracketL, TypeFaces.GetMusicFont(), size);
            }
            if (hasParentheses)
            {
                totalAccidentalSize += 2 * DrawingMethods.GetTextWidth(MusicSymbols.AccidentalParenthesesL, TypeFaces.GetMusicFont(), size);
            }
            if (hasBracket || hasParentheses)
            {
                string left = hasBracket ? MusicSymbols.AccidentalBracketL : MusicSymbols.AccidentalParenthesesL;
                noteVisualHost.AddCharacterGlyph(new Point(0 - (totalAccidentalSize + accidentalWidth + accidentalMargin), noteHeadYPosition), left, isSmall, color);
                string rigth = hasBracket ? MusicSymbols.AccidentalBracketR : MusicSymbols.AccidentalParenthesesR;
                noteVisualHost.AddCharacterGlyph(new Point(0 - (totalAccidentalSize / 2 + accidentalWidth + accidentalMargin), noteHeadYPosition), accidentalSymbol, isSmall, color);
                noteVisualHost.AddCharacterGlyph(new Point(0 - (totalAccidentalSize / 2) - accidentalMargin, noteHeadYPosition), rigth, isSmall, color);
                SetLeftMargin(totalAccidentalSize + accidentalWidth + accidentalMargin);
            }
            else
            {
                noteVisualHost.AddCharacterGlyph(new Point( 0 - accidentalWidth - accidentalMargin, noteHeadYPosition), accidentalSymbol, isSmall, color);
                SetLeftMargin(accidentalWidth + accidentalMargin);
            }
        }

        private string GetAccidentalSymbolString(AccidentalValueMusicXML value)
        {
            switch (value)
            {
                case AccidentalValueMusicXML.sharp:
                    return MusicSymbols.Sharp;
                case AccidentalValueMusicXML.natural:
                    return MusicSymbols.Natural;
                case AccidentalValueMusicXML.flat:
                    return MusicSymbols.Flat;
                case AccidentalValueMusicXML.doublesharp:
                    return MusicSymbols.DoubleSharp;
                case AccidentalValueMusicXML.sharpsharp:
                    return MusicSymbols.SharpSharp;
                case AccidentalValueMusicXML.flatflat:
                    return MusicSymbols.DoubleFlat;
                case AccidentalValueMusicXML.naturalsharp:
                    return MusicSymbols.NaturalSharp;
                case AccidentalValueMusicXML.naturalflat:
                    return MusicSymbols.NaturalFlat;
                default:
                    return "";
            }
        }

        private void CheckForLedgerLines()
        {
            if (GetLowestPitchPosition() < -1)
            {
                needLedgerLines = true;
                var value = Math.Abs(GetLowestPitchPosition());
                var even = value % 2 == 0;
                int count = even ? (value + 1) /2 : value/2;
                SetLedgerLinesPositions(count, true);
            }
            if (GetHighestPitchPosition() > 9)
            {
                needLedgerLines = true;
                var value = Math.Abs(GetHighestPitchPosition());
                var even = value % 2 == 0;
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

        internal void AddStem(DrawingVisualHost dvh)
        {
            var stem = ItemCanvas.Children.OfType<DrawingVisualHost>().FirstOrDefault(x => (string)x.Tag == "stem");
            if (stem == null)
            {
                ItemCanvas.Children.Add(dvh);
            }
        }

        private void AddFlag(DrawingVisualHost flagHost)
        {
            var flag = ItemCanvas.Children.OfType<DrawingVisualHost>().FirstOrDefault(x => (string)x.Tag == "flag");
            if (flag != null)
            {
                ItemCanvas.Children.Remove(flag);
            }
            ItemCanvas.Children.Add(flagHost);
        }

        /// <summary>
        /// Sets ledger lines positions 
        /// </summary>
        /// <param name="count">Number of lines</param>
        /// <param name="above">True if ledger lines are above staff line</param>
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
            if (noteVisualType == NoteChoiceTypeMusicXML.grace)
            {
                noteType = noteItem.FirstOrDefault().Type.Value;
            }
            else
            {
                noteType = value.Item1;
            }
            hasDots = value.Item2;
            symbol = MusicSymbols.GetNoteHeadSymbolNoteType(noteType);
        }

        private void GetPitch()
        {
            var clef = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetClef(measureId, partId, int.Parse(itemStaff), fractionPosition);
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
                    pitchedPosition.Add(pitchIndex, pitch.GetPitchIndexStaffLine(clef));
                    pitchedValue.Add(pitchIndex, staffLine[pitchedPosition[pitchIndex]]);
                }
                else
                {
                    UnpitchedMusicXML pitch = (UnpitchedMusicXML)pitchObject[pitchIndex];
                    pitchedPosition.Add(pitchIndex, new PitchMusicXML() { Step = pitch.DisplayStep, Octave = pitch.DisplayOctave }.GetPitchIndexStaffLine(clef));
                    pitchedValue.Add(pitchIndex, staffLine[pitchedPosition[pitchIndex]]);
                }
            }
            if (altered.Any(x=> x.Value))//bug fix - all chorded notes should have been altered if any (octave shift)
            {
                for (int i = 0; i < altered.Count; i++)
                {
                    altered[i] = true;
                }
            }
        }

        /// <summary>
        /// Sets left margin of note, used for layout anti-collision calculations
        /// </summary>
        /// <param name="value"></param>
        private void SetLeftMargin(double value)
        {
            itemLeftMargin = value > itemLeftMargin ? value : itemLeftMargin;
        }

        public void SetItemMargins(double left, double right)//! wip
        {
            ItemLeftMargin = left;
            ItemRightMargin = right;
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

        /// <summary>
        /// Collection of Note Pitches. Key is index of chord, Value is pitch index 
        /// </summary>
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

        public string ItemStaff
        {
            get
            {
                return itemStaff;
            }

            set
            {
                itemStaff = value;
            }
        }

        public double ItemLeftMargin
        {
            get
            {
                return itemLeftMargin;
            }

            private set
            {
                itemLeftMargin = value;
            }
        }

        public double ItemRightMargin
        {
            get
            {
                return itemRightMargin;
            }

            private set
            {
                itemRightMargin = value;
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return horizontalOffset;
            }

            set
            {
                horizontalOffset = value;
            }
        }

        public double VerticalOffset
        {
            get
            {
                return verticalOffset;
            }

            set
            {
                verticalOffset = value;
            }
        }
    }
}
