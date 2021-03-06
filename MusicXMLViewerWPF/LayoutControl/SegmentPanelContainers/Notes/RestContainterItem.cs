﻿using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using MusicXMLScore.Model.MeasureItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MusicXMLScore.Converters;
using System.Windows.Media;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes
{
    class RestContainterItem : INoteItemVisual
    {
        private Canvas itemCanvas;
        private static Random r = new Random();
        private NoteMusicXML noteItem;
        private int itemIndex;
        private int itemDuration;
        private double itemWidth;
        private double itemWidthMin;
        private double itemLeftMargin;
        private double itemRightMargin;

        private double horizontalOffset;
        private double verticalOffset;

        private bool measureRest;//! todo
        private string symbol;
        private string partId;
        private string measureId;
        private string itemStaff;
        private int dotCount;

        private Dictionary<int, double> staffLines = new Dictionary<int, double>();
        private NoteTypeValueMusicXML restType = NoteTypeValueMusicXML.whole;

        private bool customPitch;
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

        public RestContainterItem(NoteMusicXML note, int itemIndex, string partId, string measureId, string staffId)
        {
            itemCanvas = new Canvas();
            noteItem = note;
            this.itemIndex = itemIndex;
            this.partId = partId;
            this.measureId = measureId;
            this.itemStaff = staffId;
            customPitch = CheckIfCustomPitchSet();
            Draw(CheckIfMeasure());
            CalculateMinWidth();
            CalculateOptWidth();
        }

        private bool CheckIfCustomPitchSet()
        {
            RestMusicXML restElement = noteItem.Items.OfType<RestMusicXML>().FirstOrDefault();
            if (restElement?.DisplayOctave != null)
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
                return noteItem.Items.OfType<RestMusicXML>().FirstOrDefault().Measure == YesNoMusicXML.yes;
            }
            return false;
        }

        private void Draw(bool measure)
        {
            staffLines = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.AvaliableIndexLinePositions;
            DrawingVisualHost rest = new DrawingVisualHost();

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
            ItemCanvas.Children.Add(rest);
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
            itemWidth = itemWidthMin;
        }

        private void CalculateOptWidth()
        {
            double restWidth = DrawingMethods.GetTextWidth(symbol, TypeFaces.GetMusicFont());
            double dotWidth = DrawingMethods.GetTextWidth(MusicSymbols.Dot, TypeFaces.GetMusicFont());
            double leftFreeSpace = restWidth * 0.1;
            double dotSpaces = dotWidth * 0.5;
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
                var clef = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetClef(measureId, partId, int.Parse(itemStaff), itemIndex);
                return new PitchMusicXML() { Step = customStep, Octave = customOctave }.GetPitchIndexStaffLine(clef);
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

        public void SetItemMargins(double left, double right)
        {
            ItemLeftMargin = left;
            ItemRightMargin = right;
        }
    }
}
