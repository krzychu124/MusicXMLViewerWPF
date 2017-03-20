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
    class TimeSignatureContainerItem : Canvas, IAttributeItemVisual
    {
        private double itemWidth;
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
        public TimeSignatureContainerItem(double width)
        {
            this.Width = width;
            itemWidth = width;
        }

        public TimeSignatureContainerItem(TimeMusicXML timeSignature)
        {
            isSymbol = timeSignature.TimeSymbolSpecified;
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
                CanvasList canvas = new CanvasList(ItemWidth, 10);
                canvas.AddCharacterGlyph(new Point(0, staffLine[3]), symbol);
                Children.Add(canvas);
            }
            if(!isSymbol || symbolValue == TimeSymbolMusicXML.normal)
            {
                char[] beatChars = beatSymbol.ToCharArray();
                double[] beatCharWidths = beatSymbol.ToCharArray().GetCharsVisualWidth();
                double beatWidth = beatCharWidths.Sum();
                CanvasList canvasBeat = new CanvasList(beatWidth, 10);
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
                CanvasList canvasBeatTime = new CanvasList(beatTimeWidth, 10);
                offset = 0.0;
                for (int i = 0; i < beatTimeChars.Length; i++)
                {
                    canvasBeatTime.AddCharacterGlyph(new Point(offset, 0), beatTimeChars[i].ToString());
                    offset += beatTimeCharWidths[i];
                }
                itemWidth = beatWidth > beatTimeWidth ? beatWidth : beatTimeWidth;
                Width = itemWidth;
                if (beatWidth > beatTimeWidth)
                {
                    SetLeft(canvasBeat, 0);
                    double shift = (beatWidth - beatTimeWidth) / 2;
                    SetLeft(canvasBeatTime, shift);
                }
                else
                {
                    SetLeft(canvasBeatTime, 0);
                    double shift = (beatTimeWidth - beatWidth) / 2;
                    SetLeft(canvasBeat, shift);
                }
                SetTop(canvasBeat, staffLine[4]);
                SetTop(canvasBeatTime, staffLine[2]);
                Children.Add(canvasBeat);
                Children.Add(canvasBeatTime);
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
    }
}
