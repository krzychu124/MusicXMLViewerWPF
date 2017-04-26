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

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes
{
    class KeyContainerItem : IAttributeItemVisual
    {
        private string itemStaff;
        private Canvas itemCanvas;
        private readonly int attributeIndex = 1;
        private bool empty = false;
        private bool visible = true;
        private double itemWidth;
        private double itemLeftMargin;
        private double itemRightMargin;
        private Rect itemRectBounds;
        private int[] keyIndexes;
        private int fifts;
        private string keySymbol;
        private ClefMusicXML currentClef;
        private string partId;
        
        public KeyContainerItem(KeyMusicXML key, int fractionPosition, string measureId, string partId, string staffNumber)
        {
            itemCanvas = new Canvas();
            this.partId = partId;
            currentClef = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetClef(measureId, partId, int.Parse(staffNumber), fractionPosition);
            fifts = int.Parse(key.Items[key.ItemsElementName.GetValueIndexFromObjectArray(KeyChoiceTypes.fifths)].ToString());
            SetStandardKeySigMargins();
            GenerateKeyIndexes();
            GetSymbol();
            Draw();
        }

        private void Draw()
        {
            var staffLineCoords = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.AvaliableIndexLinePositions;
            DrawingVisualHost cl = new DrawingVisualHost();
            if (fifts == 0)
            {
                itemWidth = 0;
            }
            else
            {
                int symbolsCount = Math.Abs(fifts);
                if (symbolsCount > 7)
                {
                    symbolsCount = 0;
                }
                Point currentPosition = new Point();
                double symbolWidth = DrawingMethods.GetTextWidth(keySymbol, TypeFaces.GetMusicFont());
                double keySignatureWidth = symbolsCount * symbolWidth;
                for (int i = 0; i < symbolsCount; i++)
                {
                    currentPosition = new Point(i * symbolWidth, staffLineCoords[8-keyIndexes[i]]);
                    cl.AddCharacterGlyph(currentPosition, keySymbol);
                }
                itemWidth = keySignatureWidth;
            }
            ItemCanvas.Children.Add(cl);
        }

        private void GetSymbol()
        {
            keySymbol = " ";
            if (fifts> 0)
            {
                keySymbol = MusicSymbols.Sharp;
            }
            if (fifts < 0)
            {
                keySymbol = MusicSymbols.Flat;
            }
        }

        private void GenerateKeyIndexes()
        {
            int clefKeyOffset = currentClef.Sign == ClefSignMusicXML.G ? 0 :
                currentClef.Sign == ClefSignMusicXML.F ? -2 :
                currentClef.Sign == ClefSignMusicXML.C ? -1 : 0;
            if (fifts == 0)
            {
                //nothing or cancel key from previous measure
            }
            if (fifts > 0)
            {
                //higher == sharps
                keyIndexes = KeyMusicXML.DefaultGSharphKeys.Select(i => i + clefKeyOffset).ToArray();
            }
            if (fifts < 0)
            {
                //lower == flats
                keyIndexes = KeyMusicXML.DefaultGFlatKeys.Select(i => i + clefKeyOffset).ToArray();
            }
        }

        private void SetStandardKeySigMargins()
        {
            LayoutStyle.Layout layout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle;
            SetItemMargins(layout.MeasureStyle.KeySigLeftOffset.TenthsToWPFUnit(), layout.MeasureStyle.KeySigRightOffset.TenthsToWPFUnit());
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
    }
}
