using MusicXMLScore.Converters;
using MusicXMLScore.Helpers;
using MusicXMLScore.Model.MeasureItems.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicXMLScore.VisualObject
{
    class StaffLineVisual : INotifyPropertyChanged
    {
        private Brush _color;
        private int _linesCount;
        private double _yPosition;
        private double _width;
        private double _height;
        private DrawingVisualHost _visual;
        private Canvas _canvasVisual;
        private double _lineSpacing;
        private double _lineThickness;
        private Dictionary<int, double> _lineYOffset;
        private int _staffNumber = 1;
        public Brush Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
            }
        }

        public int LinesCount
        {
            get
            {
                return _linesCount;
            }

            set
            {
                if (value != _linesCount)
                {
                    _linesCount = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(LinesCount)));
                }
            }
        }

        public double YPosition
        {
            get
            {
                return _yPosition;
            }

            set
            {
                if (value != _yPosition)
                {
                    _yPosition = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(YPosition)));
                }
            }
        }

        public double Width
        {
            get
            {
                return _width;
            }

            set
            {
                if (value != _width)
                {
                    _width = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
                }
            }
        }

        public double Height
        {
            get
            {
                return _height;
            }

            set
            {
                _height = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Height)));
            }
        }

        public DrawingVisualHost Visual
        {
            get
            {
                return _visual;
            }

            set
            {
                _visual = value;
            }
        }

        public Canvas CanvasVisual
        {
            get
            {
                return _canvasVisual;
            }

            set
            {
                _canvasVisual = value;
            }
        }

        public int StaffNumber
        {
            get
            {
                return _staffNumber;
            }

            set
            {
                _staffNumber = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public StaffLineVisual(double width, int numberOfLines = 5, Brush color = null)
        {
            GetDefaults();
            this._width = width;
            this._linesCount = numberOfLines;
            this._color = color ?? Brushes.Black;
            this._yPosition = 0.0;
            _lineYOffset = new Dictionary<int, double>();
            InitCanvas();
            Draw();
            PropertyChanged += StaffLineVisual_PropertyChanged;
        }

        public StaffLineVisual(double width, StaffDetailsMusicXML staffDetails, Brush color = null):this(width)
        {
            //Todo
        }

        public void StaffLineVisual_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Color):
                    Draw();
                    //update visual;
                    break;
                case nameof(LinesCount):
                    if (LinesCount > 5)
                    {
                        _lineSpacing = _lineSpacing * 1.5;
                    }
                    else //reset spacing
                    {
                        GetStaffLineSpacing();
                    }
                    GenerateStaffLinesPositions();
                    Draw();
                    //update visual;
                    break;
                case nameof(Width):
                    Draw(true);
                    //update visual;
                    break;
                case "CalculatedWidth":
                    var measure = sender as Model.ScorePartwisePartMeasureMusicXML;
                    if (measure != null) Width = measure.CalculatedWidth.TenthsToWPFUnit();
                    break;
                case nameof(YPosition):
                    UpdateCanvasPosition();
                    break;
                case nameof(DrawingHelpers.PartProperties.NumberOfLines):
                    var partProperties = sender as DrawingHelpers.PartProperties;
                    if (partProperties != null) LinesCount = partProperties.NumberOfLines;
                    break;
                default:
                    //log warning no such property name / no action for current property
                    break;
            }
        }
        private void GetDefaults()
        {
            GetDefaultHeight();
            GetLineThickness();
            GetStaffLineSpacing();
        }

        private void GetDefaultHeight()
        {
            var pageLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout;
            Height = pageLayout.StaffHeight.MMToWPFUnit();
        }

        private void GetLineThickness()
        {
            var measureLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.MeasureStyle;
            _lineThickness = measureLayout.StaffLineThickness.TenthsToWPFUnit();
        }
        private void GetStaffLineSpacing()
        {
            var measureLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.MeasureStyle;
            _lineSpacing = measureLayout.StaffSpaceLegth.TenthsToWPFUnit();
        }
        private void GenerateStaffLinesPositions()
        {
            if (_lineYOffset.Count != 0)
            {
                _lineYOffset.Clear();
            }
            double offsetFromZero = (5 - _linesCount) / 2.0 * 10.0.TenthsToWPFUnit();
            double currentOffset = 0 + offsetFromZero;
            for (int i = 1; i <= _linesCount; i++)
            {
                _lineYOffset.Add(i, currentOffset);
                currentOffset += _lineSpacing;
            }
        }

        private void Draw(bool updatePositions = false)
        {
            DrawingVisual staffLineVisual = new DrawingVisual();
            Pen pen = new Pen(_color, _lineThickness);

            if (updatePositions || _lineYOffset.Count ==0)
            {
                GenerateStaffLinesPositions();
            }
            using (DrawingContext dc = staffLineVisual.RenderOpen())
            {
                for (int i = 1; i <= _linesCount; i++)
                {
                    dc.DrawLine(pen, new Point(0.0, _lineYOffset[i]), new Point(_width, _lineYOffset[i]));
                }
            }
            _visual.ClearVisuals();
            _visual.AddVisual(staffLineVisual);
        }
        private void InitCanvas()
        {
            _visual = new DrawingVisualHost();
            _canvasVisual = new Canvas();
            _canvasVisual.Children.Add(_visual);
        }
        private void UpdateCanvasPosition()
        {
            Canvas.SetTop(_canvasVisual, YPosition);
        }
    }
}
