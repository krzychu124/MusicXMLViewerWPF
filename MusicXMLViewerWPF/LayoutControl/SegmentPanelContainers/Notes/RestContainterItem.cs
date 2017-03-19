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


namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes
{
    class RestContainterItem : Canvas, INoteItemVisual
    {
        private NoteMusicXML noteItem;
        private int itemIndex;
        private int itemDuration =0;
        private double itemWidthMin = 0.0;
        private double itemWidthOpt = 0.0; // optimal
        private bool measureRest = false;
        private string symbol;
        private string partId;
        private string measureId;
        private int dotCount = 0;
        private Dictionary<int, double> staffLines = new Dictionary<int, double>();
        private NoteTypeValueMusicXML restType = NoteTypeValueMusicXML.whole;
        private double itemWeight = 0.0;

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

        public RestContainterItem(NoteMusicXML note, int itemIndex, string partId, string measureId)
        {
            noteItem = note;
            this.itemIndex = itemIndex;
            this.partId = partId;
            this.measureId = measureId;
            Draw(CheckIfMeasure());
            CalculateMinWidth();
            CalculateOptWidth();
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
            staffLines = ViewModel.ViewModelLocator.Instance.Main.CurrentPageProperties.AvaliableIndexLinePositions;
            CanvasList rest = new CanvasList(10, 10);
            if (measure)
            {
                measureRest = true;
                GetSymbol();
                double positionY =SetPosition(CalculateRestPositionY());
                rest.AddCharacterGlyph(new Point(0, positionY), symbol);
            }
            else
            {
                GetSymbol();
                double positionY = SetPosition(CalculateRestPositionY());
                rest.AddCharacterGlyph(new Point(0, positionY), symbol);
            }
            if(dotCount!= 0)
            {
                Point dotPosition = new Point(14, SetPosition(3));
                rest.AddCharacterGlyph(dotPosition, MusicSymbols.Dot);
            }
            Children.Add(rest);
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
