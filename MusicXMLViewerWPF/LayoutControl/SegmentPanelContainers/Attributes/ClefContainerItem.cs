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
using MusicXMLScore.LayoutStyle.Styles;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes
{
    class ClefContainerItem : MeasureAttributeBase, IAttributeItemVisual, ISelectable
    {
        private string _itemStaff;
        private double _itemWidth;
        private Rect _itemRectBounds;
        private ClefSignMusicXML _sign;
        private int _line;
        private int _octaveChange;
        private bool _isAdditional;
        private string _symbol;
        private double _staffLine;
        private bool _isEmpty;
        private bool _isSelected;

        public ClefContainerItem(string staff, int fractionPosition, ClefMusicXML clef) :
            base(AttributeType.clef, int.Parse(staff), fractionPosition)
        {
            _sign = clef.Sign;
            _line = clef.Line != null ? int.Parse(clef.Line) : 0;
            _octaveChange = clef.ClefOctaveChange != null ? int.Parse(clef.ClefOctaveChange) : 0;
            _isAdditional = clef.AdditionalSpecified ? clef.Additional == YesNoMusicXML.yes : false;
            if (fractionPosition != 0)
            {
                _isAdditional = true;
            }
            Update();
            //---------temp--test-----------------
            ItemCanvas.MouseDown += _itemCanvas_MouseDown;
            var context = new ContextMenu();
            var menuItem = new MenuItem
            {
                Header = "Change Clef"
            };
            menuItem.Click += MenuItem_Click;
            context.Items.Add(menuItem);
            ItemCanvas.ContextMenu = context;
            //-------------------------------------
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_sign == ClefSignMusicXML.G)
            {
                ChangeSign(ClefSignMusicXML.F);
            }
            else
            {
                ChangeSign(ClefSignMusicXML.G);
            }
        }

        private void _itemCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Select();
        }

        protected override void Update()
        {

            GetSymbol();
            GetLine();
            if (IsVisible)
            {
                double tempLine = _staffLine;
                string tempSymbol = _symbol;
                if (_isEmpty) // used for proper layout spacing, invisible but taking space; //TODO_LATER more test/refactor
                {
                    _itemWidth = DrawingMethods.GetTextWidth(MusicSymbols.GClef, TypeFaces.GetMusicFont(), _isAdditional);
                    tempSymbol = "";
                    tempLine = 0.0;
                }
                else
                {
                    _itemWidth = DrawingMethods.GetTextWidth(_symbol, TypeFaces.GetMusicFont(), _isAdditional);
                }
                DrawingVisualHost clefVisualsHost = new DrawingVisualHost();
                clefVisualsHost.AddCharacterGlyph(new Point(0, tempLine), tempSymbol, _isAdditional, Color);
                ItemCanvas.Children.Clear();
                ItemCanvas.Children.Add(clefVisualsHost);
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
            this.SetStaffLine();
        }

        private void SetStaffLine()
        {
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

        public void Select()
        {
            System.Console.WriteLine("Clef Clicked");
            _isSelected = !_isSelected;
            Color = _isSelected ? ColorStyle.SelectionColor : ColorStyle.ClefColor;
            Update();
        }

        public void ChangeSign(ClefSignMusicXML sign)
        {
            _sign = sign;
            _line = 0;
            Update();
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

        public double ItemWidthMin
        {
            get { return 0; }

            set { }
        }

        public string ItemStaff
        {
            get { return _itemStaff; }

            set { _itemStaff = value; }
        }
        
    }
}