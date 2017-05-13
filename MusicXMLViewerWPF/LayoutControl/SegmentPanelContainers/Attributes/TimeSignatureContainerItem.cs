using MusicXMLScore.Model.MeasureItems.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MusicXMLScore.Converters;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
using System.Windows.Media;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes
{
    class TimeSignatureContainerItem : IAttributeItemVisual
    {
        private string itemStaff;
        private Canvas itemCanvas;
        private readonly int attributeIndex = 2;
        private double itemWidth;
        private double itemLeftMargin;
        private double itemRightMargin;
        private Rect itemRectBounds;
        private string beatSymbol;
        private string beatValue;
        private string beatTimeSymbol;
        private string beatTimeValue;
        private string symbol;
        private TimeSymbolMusicXML symbolValue;
        private bool isSymbol = false;
        private bool visible = true;
        private bool empty = false;
        private double[] staffLine;

        public TimeSignatureContainerItem(string staff, int fractionPosition, TimeMusicXML timeSignature):this(timeSignature)
        {

        }
        public TimeSignatureContainerItem(double width)
        {
            ItemCanvas.Width = width;
            itemWidth = width;
        }

        public TimeSignatureContainerItem(TimeMusicXML timeSignature)
        {
            itemCanvas = new Canvas();
            isSymbol = timeSignature.TimeSymbolSpecified;
            SetStandardTimeSigMargins();
            GetTime(timeSignature);
            GetStaffLineCoords();
            GetSymbol();
            Draw();
        }

        private void Draw()
        {
            if (isSymbol)
            {
                itemWidth = DrawingMethods.GetTextWidth(symbol, TypeFaces.GetMusicFont());
                DrawingVisualHost canvas = new DrawingVisualHost();
                canvas.AddCharacterGlyph(new Point(0, staffLine[3]), symbol);
                ItemCanvas.Children.Add(canvas);
            }
            if(!isSymbol || symbolValue == TimeSymbolMusicXML.normal)
            {
                char[] beatChars = beatSymbol.ToCharArray();
                double[] beatCharWidths = beatSymbol.ToCharArray().GetCharsVisualWidth();
                double beatWidth = beatCharWidths.Sum();
                DrawingVisualHost canvasBeat = new DrawingVisualHost();
                double offset = 0;
                for (int i = 0; i < beatChars.Length; i++)
                {
                    canvasBeat.AddCharacterGlyph(new Point(offset, 0), beatChars[i].ToString());
                    offset += beatCharWidths[i];
                }
                //beatTime
                char[] beatTimeChars = beatTimeSymbol.ToCharArray();
                double[] beatTimeCharWidths = beatTimeSymbol.ToCharArray().GetCharsVisualWidth();
                double beatTimeWidth = beatTimeCharWidths.Sum();
                DrawingVisualHost canvasBeatTime = new DrawingVisualHost();
                offset = 0.0;
                for (int i = 0; i < beatTimeChars.Length; i++)
                {
                    canvasBeatTime.AddCharacterGlyph(new Point(offset, 0), beatTimeChars[i].ToString());
                    offset += beatTimeCharWidths[i];
                }
                //measure legth + align
                itemWidth = beatWidth > beatTimeWidth ? beatWidth : beatTimeWidth;
                ItemCanvas.Width = itemWidth;
                if (beatWidth > beatTimeWidth)
                {
                    Canvas.SetLeft(canvasBeat, 0);
                    double shift = (beatWidth - beatTimeWidth) / 2;
                    Canvas.SetLeft(canvasBeatTime, shift);
                }
                else
                {
                    Canvas.SetLeft(canvasBeatTime, 0);
                    double shift = (beatTimeWidth - beatWidth) / 2;
                    Canvas.SetLeft(canvasBeat, shift);
                }
                Canvas.SetTop(canvasBeat, staffLine[4]);
                Canvas.SetTop(canvasBeatTime, staffLine[2]);
                ItemCanvas.Children.Add(canvasBeat);
                ItemCanvas.Children.Add(canvasBeatTime);
            }
        }

        private void GetTime(TimeMusicXML timeSignature)
        {
            if (isSymbol)
            {
                symbolValue = timeSignature.TimeSymbol;
            }
            if (!isSymbol || symbolValue == TimeSymbolMusicXML.normal)
            {
                
                beatValue = timeSignature.Items[timeSignature.ItemsElementName.GetValueIndexFromObjectArray(TimeChoiceTypeMusicXML.beats)].ToString();
                beatTimeValue = timeSignature.Items[timeSignature.ItemsElementName.GetValueIndexFromObjectArray(TimeChoiceTypeMusicXML.beattype)].ToString();
            }
        }

        private void GetSymbol()
        {
            if (isSymbol)
            {
                switch (symbolValue)
                {
                    case TimeSymbolMusicXML.common:
                        symbol = MusicSymbols.CommonTime;
                        break;
                    case TimeSymbolMusicXML.cut:
                        symbol = MusicSymbols.CutTime;
                        break;
                    default:
                        symbol = "*";
                        break;
                }
            }
            if(!isSymbol || symbolValue == TimeSymbolMusicXML.normal)
            {
                beatSymbol = MusicSymbols.GetCustomTimeNumber(beatValue);
                beatTimeSymbol = MusicSymbols.GetCustomTimeNumber(beatTimeValue);
            }
        }

        private void GetStaffLineCoords()
        {
            staffLine = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.StaffLineCoords.Values.ToArray();
        }

        private void SetStandardTimeSigMargins()
        {
            LayoutStyle.Layout layout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle;
            SetItemMargins(layout.MeasureStyle.TimeSigLeftOffset.TenthsToWPFUnit(), layout.MeasureStyle.TimeSigRightOffset.TenthsToWPFUnit());
        }

        public void SetItemMargins(double left, double right)
        {
            ItemLeftMargin = left;
            ItemRightMargin = right;
        }
        public Rect ItemRectBounds
        {
            get
            {
                return itemRectBounds;
            }

            set
            {
                itemRectBounds = value;
            }
        }
        public double ItemWidth
        {
            get
            {
                return itemWidth;
            }
        }

        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                visible = value;
            }
        }

        public bool Empty
        {
            get
            {
                return empty;
            }

            set
            {
                empty = value;
            }
        }

        public double ItemWidthMin
        {
            get
            {
                return 0;
            }

            set
            {
            }
        }

        public int AttributeIndex
        {
            get
            {
                return attributeIndex;
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
    }
}
