using MusicXMLScore.Model.MeasureItems.Attributes;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MusicXMLScore.Converters;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes
{
    class TimeSignatureContainerItem : MeasureAttributeBase, IAttributeItemVisual
    {
        private string itemStaff;
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

        //public TimeSignatureContainerItem(string staff, int fractionPosition, TimeMusicXML timeSignature)
           
        //{
        //}

        //public TimeSignatureContainerItem(double width)
        //{
        //    ItemCanvas.Width = width;
        //    itemWidth = width;
        //}

        public TimeSignatureContainerItem(TimeMusicXML timeSignature)
            : base(AttributeType.time, 1, 0)
        {
            isSymbol = timeSignature.TimeSymbolSpecified;
            GetTime(timeSignature);
            GetStaffLineCoords();
            GetSymbol();
            Update();
        }

        protected override void Update()
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
                        symbol = "";
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
        
    }
}
