using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MusicXMLScore.Converters;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
using MusicXMLScore.LayoutStyle;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using MusicXMLScore.Model.MeasureItems.Attributes;
using MusicXMLScore.ViewModel;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes
{
    class ClefContainerItem : IAttributeItemVisual
    {
        private string _itemStaff;
        private Canvas _itemCanvas;
        private readonly int _attributeIndex = 0;
        private double _itemWidth;
        private double _itemLeftMargin;
        private double _itemRightMargin;
        private Rect _itemRectBounds;
        private Visual _visual;
        private ClefSignMusicXML _sign;
        private int _line;
        private int _octaveChange;
        private bool _isAdditional;
        private string _symbol;
        private double _staffLine;
        private bool _isEmpty;
        private bool _visible = true;
        private int _fractionPosition;

        public ClefContainerItem(string staff, int fractionPosition, ClefMusicXML clef)
        {
            _fractionPosition = fractionPosition;
            _sign = clef.Sign;
            _line = clef.Line != null ? int.Parse(clef.Line) : 0;
            _octaveChange = clef.ClefOctaveChange != null ? int.Parse(clef.ClefOctaveChange) : 0;
            _itemCanvas = new Canvas();
            if (clef.AdditionalSpecified)
            {
                _isAdditional = clef.Additional == YesNoMusicXML.yes;
            }
            else
            {
                _isAdditional = false;
            }
            if (fractionPosition != 0)
            {
                _isAdditional = true;
            }
            SetStandardClefMargins();
            GetSymbol();
            GetLine();
            Draw();
        }

        public ClefContainerItem(ClefSignMusicXML sign, int line, int octaveChange = 0, bool additional = false)
        {
            _sign = sign;
            _line = line;
            _octaveChange = octaveChange;
            _isAdditional = additional;

            GetSymbol();
            GetLine();

            Draw();
        }

        public ClefContainerItem(ClefMusicXML clef)
        {
            _sign = clef.Sign;
            _line = clef.Line != null ? int.Parse(clef.Line) : 0;
            _octaveChange = clef.ClefOctaveChange != null ? int.Parse(clef.ClefOctaveChange) : 0;
            if (clef.AdditionalSpecified)
            {
                _isAdditional = clef.Additional == YesNoMusicXML.yes ? true : false;
            }
            else
            {
                _isAdditional = false;
            }
            if (_fractionPosition != 0)
            {
                _isAdditional = true;
            }
            GetSymbol();
            GetLine();
            Draw();
        }

        private void Draw(bool visible = true, bool empty = false)
        {
            if (visible)
            {
                double tempLine = _staffLine;
                string tempSymbol = _symbol;
                if (empty) // used for proper layout spacing, invisible but taking space; //TODO_LATER more test/refactor
                {
                    _itemWidth = DrawingMethods.GetTextWidth(MusicSymbols.GClef, TypeFaces.GetMusicFont(), _isAdditional);
                    tempSymbol = "";
                    tempLine = 0.0;
                }
                else
                {
                    _itemWidth = DrawingMethods.GetTextWidth(_symbol, TypeFaces.GetMusicFont(), _isAdditional);
                }
                DrawingVisualHost cc = new DrawingVisualHost();
                //cc.Width = 10;
                //cc.Height = 10;
                cc.AddCharacterGlyph(new Point(0, tempLine), tempSymbol, _isAdditional);
                ItemCanvas.Children.Add(cc);
            }
        }

        private void GetLine()
        {
            if (_line == 0)
            {
                if (_sign == ClefSignMusicXML.G)
                {
                    _line = 2;
                }
                if (_sign == ClefSignMusicXML.C)
                {
                    _line = 3;
                }
                if (_sign == ClefSignMusicXML.F)
                {
                    _line = 4;
                }
                if (_sign == ClefSignMusicXML.TAB)
                {
                    _line = 3;
                }
            }
            if (_sign == ClefSignMusicXML.percussion)
            {
                _line = 3;
            }
            if (_sign == ClefSignMusicXML.TAB)
            {
                _line = 3;
            }
            _staffLine = ViewModelLocator.Instance.Main.CurrentPageLayout.AvaliableIndexLinePositions[10 - (_line * 2)];
        }

        private void GetSymbol()
        {
            switch (_sign)
            {
                case ClefSignMusicXML.G:
                    switch (_octaveChange)
                    {
                        case -2:
                            _symbol = MusicSymbols.GClef15Down;
                            break;
                        case -1:
                            _symbol = MusicSymbols.GClef8Down;
                            break;
                        case 1:
                            _symbol = MusicSymbols.GClef8Up;
                            break;
                        case 2:
                            _symbol = MusicSymbols.GClef15Up;
                            break;
                        default:
                            _symbol = MusicSymbols.GClef;
                            break;
                    }
                    break;
                case ClefSignMusicXML.F:
                    switch (_octaveChange)
                    {
                        case -2:
                            _symbol = MusicSymbols.FClef15Down;
                            break;
                        case -1:
                            _symbol = MusicSymbols.FClef8Down;
                            break;
                        case 1:
                            _symbol = MusicSymbols.FClef8Up;
                            break;
                        case 2:
                            _symbol = MusicSymbols.FClef15Up;
                            break;
                        default:
                            _symbol = MusicSymbols.FClef;
                            break;
                    }
                    break;
                case ClefSignMusicXML.C:
                    switch (_octaveChange)
                    {
                        case -1:
                            _symbol = MusicSymbols.CClef8Down;
                            break;
                        default:
                            _symbol = MusicSymbols.CClef;
                            break;
                    }
                    break;
                case ClefSignMusicXML.percussion:
                    _symbol = MusicSymbols.Percussion;
                    break;
                case ClefSignMusicXML.TAB:
                    _symbol = MusicSymbols.TAB;
                    break;
                case ClefSignMusicXML.jianpu:
                    _symbol = string.Empty;
                    break;
                case ClefSignMusicXML.none:
                    _symbol = string.Empty;
                    break;
            }
        }

        private void SetStandardClefMargins()
        {
            Layout layout = ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle;
            SetItemMargins(layout.MeasureStyle.ClefLeftOffset.TenthsToWPFUnit(), layout.MeasureStyle.ClefRightOffset.TenthsToWPFUnit());
        }

        public void SetItemMargins(double left, double right)
        {
            ItemLeftMargin = left;
            ItemRightMargin = right;
        }

        public Rect ItemRectBounds
        {
            get { return _itemRectBounds; }

            set { _itemRectBounds = value; }
        }

        public double ItemWidth
        {
            get { return _itemWidth; }
        }

        public bool Empty
        {
            get { return _isEmpty; }

            set { _isEmpty = value; }
        }

        public bool Visible
        {
            get { return _visible; }

            set { _visible = value; }
        }

        public double ItemWidthMin
        {
            get { return 0; }

            set { }
        }

        public int AttributeIndex
        {
            get { return _attributeIndex; }
        }

        public Canvas ItemCanvas
        {
            get { return _itemCanvas; }

            set { _itemCanvas = value; }
        }

        public string ItemStaff
        {
            get { return _itemStaff; }

            set { _itemStaff = value; }
        }

        public double ItemLeftMargin
        {
            get { return _itemLeftMargin; }

            private set { _itemLeftMargin = value; }
        }

        public double ItemRightMargin
        {
            get { return _itemRightMargin; }

            private set { _itemRightMargin = value; }
        }
    }
}