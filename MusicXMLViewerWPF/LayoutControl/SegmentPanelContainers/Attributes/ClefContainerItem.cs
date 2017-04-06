using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MusicXMLScore.Model.MeasureItems.Attributes;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.ScoreProperties;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes
{
    class ClefContainerItem : Canvas, IAttributeItemVisual
    {
        private readonly int attributeIndex = 0;
        private double itemWidth;
        private Rect itemRectBounds;
        private Visual visual;
        private ClefSignMusicXML sign;
        private int line;
        private int octaveChange;
        private bool isAdditional =false;
        private string symbol;
        private double staffLine;
        private bool isEmpty = false;
        private bool visible = true;
        private int fractionPosition = 0;
        
        public ClefContainerItem(string staff, int fractionPosition, ClefMusicXML clef)
        {
            this.fractionPosition = fractionPosition;
            this.sign = clef.Sign;
            this.line = clef.Line != null ? int.Parse(clef.Line) : 0;
            this.octaveChange = clef.ClefOctaveChange != null ? int.Parse(clef.ClefOctaveChange) : 0;
            if (clef.AdditionalSpecified)
            {
                isAdditional = clef.Additional == Model.Helpers.SimpleTypes.YesNoMusicXML.yes ? true : false;
            }
            else
            {
                isAdditional = false;
            }
            if (fractionPosition != 0)
            {
                isAdditional = true;
            }
            GetSymbol();
            GetLine();
            Draw();
        }
        public ClefContainerItem(ClefSignMusicXML sign, int line, int octaveChange =0, bool additional = false)
        {
            this.sign = sign;
            this.line = line;
            this.octaveChange = octaveChange;
            isAdditional = additional;
            
            GetSymbol();
            GetLine();

            Draw();
        }
        public ClefContainerItem(ClefMusicXML clef)
        {
            this.sign = clef.Sign;
            this.line = clef.Line != null ? int.Parse(clef.Line) : 0;
            this.octaveChange = clef.ClefOctaveChange != null ? int.Parse(clef.ClefOctaveChange) : 0;
            if (clef.AdditionalSpecified)
            {
                isAdditional = clef.Additional == Model.Helpers.SimpleTypes.YesNoMusicXML.yes ? true : false;
            }
            else
            {
                isAdditional = false;
            }
            if (fractionPosition != 0)
            {
                isAdditional = true;
            }
            GetSymbol();
            GetLine();
            Draw();
        }

        private void Draw(bool visible = true, bool empty = false)
        {
            if (visible)
            {
                double tempLine = staffLine;
                string tempSymbol = symbol;
                if (empty) // used for proper layout spacing, invisible but taking space; //TODO_LATER more test/refactor
                {
                    itemWidth = DrawingMethods.GetTextWidth(MusicSymbols.GClef, Helpers.TypeFaces.GetMusicFont(), isAdditional);
                    tempSymbol = "";
                    tempLine = 0.0;
                }
                else
                {
                    itemWidth = DrawingMethods.GetTextWidth(symbol, Helpers.TypeFaces.GetMusicFont(), isAdditional);
                }
                Helpers.DrawingVisualHost cc = new Helpers.DrawingVisualHost();
                //cc.Width = 10;
                //cc.Height = 10;
                cc.AddCharacterGlyph(new Point(0, tempLine), tempSymbol, isAdditional);
                Children.Add(cc);
            }
            else
            {

            }
            
        }

        private void GetLine()
        {
            if (line == 0)
            {
                if (sign == ClefSignMusicXML.G)
                {
                    line = 2;
                }
                if (sign == ClefSignMusicXML.C)
                {
                    line = 3;
                }
                if (sign == ClefSignMusicXML.F)
                {
                    line = 4;
                }
                if (sign == ClefSignMusicXML.TAB)
                {
                    line = 3;
                }
            }
            if (sign == ClefSignMusicXML.percussion)
            {
                line = 3;
            }
            if (sign == ClefSignMusicXML.TAB)
            {
                line = 3;
            }
            staffLine = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.AvaliableIndexLinePositions[10 - (line *2)];
        }

        private void GetSymbol()
        {
            switch (sign)
            {
                case ClefSignMusicXML.G:
                    switch (octaveChange)
                    {
                        case -2:
                            symbol = MusicSymbols.GClef15Down;
                            break;
                        case -1:
                            symbol = MusicSymbols.GClef8Down;
                            break;
                        case 1:
                            symbol = MusicSymbols.GClef8Up;
                            break;
                        case 2:
                            symbol = MusicSymbols.GClef15Up;
                            break;
                        default: symbol = MusicSymbols.GClef;
                            break;
                    }
                    break;
                case ClefSignMusicXML.F:
                    switch (octaveChange)
                    {
                        case -2:
                            symbol = MusicSymbols.FClef15Down;
                            break;
                        case -1:
                            symbol = MusicSymbols.FClef8Down;
                            break;
                        case 1:
                            symbol = MusicSymbols.FClef8Up;
                            break;
                        case 2:
                            symbol = MusicSymbols.FClef15Up;
                            break;
                        default:
                            symbol = MusicSymbols.FClef;
                            break;
                    }
                    break;
                case ClefSignMusicXML.C:
                    switch (octaveChange)
                    {
                        case -1:
                            symbol = MusicSymbols.CClef8Down;
                            break;
                        default: symbol = MusicSymbols.CClef;
                            break;
                    }
                    break;
                case ClefSignMusicXML.percussion:
                    symbol = MusicSymbols.Percussion;
                    break;
                case ClefSignMusicXML.TAB:
                    symbol = MusicSymbols.TAB;
                    break;
                case ClefSignMusicXML.jianpu:
                    symbol = string.Empty;
                    break;
                case ClefSignMusicXML.none:
                    symbol = string.Empty;
                    break;
            }
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
                return isEmpty;
            }

            set
            {
                isEmpty = value;
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
    }
}
