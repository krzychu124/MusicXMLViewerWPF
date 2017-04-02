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
using MusicXMLScore.Converters;
using System.Windows.Media;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes
{
    class RestContainterItem : Canvas, INoteItemVisual
    {
        private static Random r = new Random();
        private NoteMusicXML noteItem;
        private int itemIndex;
        private int itemDuration =0;
        private double itemWidth = 0.0;
        private double itemWidthMin = 0.0;
        private double itemWidthOpt = 0.0; // optimal
        private bool measureRest = false;
        private string symbol;
        private string partId;
        private string measureId;
        private string staffId;
        private int dotCount = 0;
        private Dictionary<int, double> staffLines = new Dictionary<int, double>();
        private NoteTypeValueMusicXML restType = NoteTypeValueMusicXML.whole;
        private double itemWeight = 0.0;
        private bool customPitch = false;
        private string customOctave = "4";
        private StepMusicXML customStep = StepMusicXML.B;
        
        public bool MeasureRest
        {
            get
            {
                return measureRest;
            }

            set
            {
                measureRest = value;
            }
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
                return itemWeight;//throw new NotImplementedException();
            }
            set
            {
                itemWeight = value;
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

        public RestContainterItem(NoteMusicXML note, int itemIndex, string partId, string measureId, string staffId)
        {
            noteItem = note;
            this.itemIndex = itemIndex;
            this.partId = partId;
            this.measureId = measureId;
            this.staffId = staffId;
            customPitch = CheckIfCustomPitchSet();
            Draw(CheckIfMeasure());
            CalculateMinWidth();
            CalculateOptWidth();
        }

        private bool CheckIfCustomPitchSet()
        {
            RestMusicXML restElement = noteItem.Items.OfType<RestMusicXML>().FirstOrDefault();
            if (restElement != null && restElement.DisplayOctave != null)
            {
                customStep = restElement.DisplayStep;
                customOctave = restElement.DisplayOctave;
                return true;
            }
            return false;
        }

        private bool CheckIfMeasure()
        {
            if (noteItem.Items.OfType<RestMusicXML>().FirstOrDefault().MeasureSpecified)
            {
                return noteItem.Items.OfType<RestMusicXML>().FirstOrDefault().Measure == YesNoMusicXML.yes ? true : false;
            }
            return false; //false;
        }

        private void Draw(bool measure)
        {
            staffLines = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.AvaliableIndexLinePositions;
            CanvasList rest = new CanvasList(10, 10);
            
            Brush color = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.Colors[int.Parse(noteItem.Voice)];
            double positionY = 0.0;
            if (measure)
            {
                measureRest = true;
                GetSymbol();
                positionY =SetPosition(CalculateRestPositionY());
                rest.AddCharacterGlyph(new Point(0, positionY), symbol, color: color);
            }
            else
            {
                GetSymbol();
                positionY = SetPosition(CalculateRestPositionY());
                rest.AddCharacterGlyph(new Point(0, positionY), symbol, color: color);
            }
            if(dotCount!= 0)
            {
                double shiftUp = (int)restType >= 6 ? SetPosition(1) :
                    restType == NoteTypeValueMusicXML.Item32nd ? SetPosition(3) :
                    restType == NoteTypeValueMusicXML.Item64th ? SetPosition(5) : SetPosition(5);
                Point dotPosition = new Point(DrawingMethods.GetTextWidth(symbol,TypeFaces.GetMusicFont()) +4.0.TenthsToWPFUnit(), positionY -shiftUp);
                rest.AddCharacterGlyph(dotPosition, MusicSymbols.Dot, color: color);
            }
            Children.Add(rest);
        }

        public void DrawSpace(double length, bool red = false)
        {
            Brush color;
            int shiftY = 50;
            CanvasList spaceCanvas = new CanvasList(10, 10);
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
                dc.DrawLine(new Pen(color, 3.0), new Point(0, shiftY), new Point(length - 0.05, shiftY));
            }
            spaceCanvas.AddVisual(dv);
            Children.Add(spaceCanvas);
        }
        private double SetPosition(int customPosition)
        {
            return staffLines[customPosition];
        }

        private void CalculateMinWidth()
        {
            double restWidth = DrawingMethods.GetTextWidth(symbol, TypeFaces.GetMusicFont());
            double dotWidth = DrawingMethods.GetTextWidth(MusicSymbols.Dot, TypeFaces.GetMusicFont());
            double leftFreeSpace = restWidth * 0.05;
            double dotSpaces = dotWidth * 0.25;
            itemWidthMin = leftFreeSpace + restWidth + (dotWidth + dotSpaces) * dotCount;
        }
        private void CalculateOptWidth()
        {
            double restWidth = DrawingMethods.GetTextWidth(symbol, TypeFaces.GetMusicFont());
            double dotWidth = DrawingMethods.GetTextWidth(MusicSymbols.Dot, TypeFaces.GetMusicFont());
            double leftFreeSpace = restWidth * 0.1;
            double dotSpaces = dotWidth * 0.5;

            itemWidthOpt = leftFreeSpace + restWidth + (dotWidth + dotSpaces) * dotCount;
        }
        private void GetSymbol()
        {
            itemDuration = int.Parse(noteItem.Items.OfType<decimal>().FirstOrDefault().ToString());
            Tuple<NoteTypeValueMusicXML, bool> value = CalculationHelpers.GetBaseDurationValue(itemDuration, partId, measureId);
            // item1 =NoteType... item2 true if has dot/dots
            restType = value.Item1;
            if (value.Item2)
            {
                dotCount++;
            }
            symbol = MusicSymbols.GetRestSymbolNoteType(restType);
        }
        private int CalculateRestPositionY()
        {
            if (customPitch)
            {
                var clef = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetClef(measureId, partId, int.Parse(staffId), itemIndex);
                return CalculationHelpers.GetPitchIndexStaffLine(new PitchMusicXML() { Step = customStep, Octave = customOctave }, clef);
            }
            else
            {
                if (restType == NoteTypeValueMusicXML.whole)
                {
                    return 2;
                }
                else
                {
                    return 4;
                }
            }
        }
    }
}
