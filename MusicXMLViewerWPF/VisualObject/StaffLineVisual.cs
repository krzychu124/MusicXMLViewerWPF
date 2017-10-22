using MusicXMLScore.Converters;
using MusicXMLScore.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicXMLScore.VisualObject
{
    internal class StaffLineVisual : VisualObjectWithOffset
    {
        private int _linesCount;
        private DrawingVisualHost _visual;
        private double _lineSpacing;
        private double _lineThickness;
        private Dictionary<int, double> _yOffsets;

        public int LinesCount
        {
            get { return _linesCount; }

            set
            {
                Set(ref _linesCount, value);
            }
        }

        public int StaffNumber { get; set; } = 1;

        public double LineThickness
        {
            get
            {
                return _lineThickness;
            }

            set
            {
                Set(ref _lineThickness, value);
            }
        }

        public StaffLineVisual(double width, int numberOfLines = 5, Brush color = null)
        {
            GetDefaults();
            Width = width;
            _linesCount = numberOfLines;
            Color = color ?? Brushes.Black;
            HorizontalOffset = 0.0;
            _yOffsets = new Dictionary<int, double>();
            InitCanvas();
            Draw();
            base.PropertyChanged += StaffLineVisual_PropertyChanged;
        }

        public void StaffLineVisual_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Color):
                    Draw();
                    break;
                case nameof(LinesCount):
                    if (LinesCount > 5)
                    {
                        _lineSpacing *= 1.5;
                    }
                    else //reset spacing
                    {
                        GetStaffLineSpacing();
                    }
                    //update visual;
                    Draw(true);
                    break;
                case nameof(LineThickness): //update visual
                    Draw(true);
                    break;
                case nameof(Width):
                    //update visual;
                    Draw(true);
                    break;
                case "CalculatedWidth":
                    var measure = sender as Model.ScorePartwisePartMeasureMusicXML;
                    if (measure != null) Width = measure.CalculatedWidth.TenthsToWPFUnit();
                    break;
                case nameof(HorizontalOffset):
                    UpdateCanvasPosition();
                    break;
                case nameof(DrawingHelpers.PartProperties.NumberOfLines):
                    var partProperties = sender as DrawingHelpers.PartProperties;
                    if (partProperties != null) LinesCount = partProperties.NumberOfLines;
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
            LineThickness = measureLayout.StaffLineThickness.TenthsToWPFUnit();
        }

        private void GetStaffLineSpacing()
        {
            var measureLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.MeasureStyle;
            _lineSpacing = measureLayout.StaffSpaceLegth.TenthsToWPFUnit();
        }

        private void GenerateStaffLinesPositions()
        {
            if (_yOffsets.Count != 0)
            {
                _yOffsets.Clear();
            }
            double offsetFromZero = (5 - _linesCount) / 2.0 * 10.0.TenthsToWPFUnit();
            double currentOffset = 0 + offsetFromZero;
            for (int i = 1; i <= _linesCount; i++)
            {
                _yOffsets.Add(i, currentOffset);
                currentOffset += _lineSpacing;
            }
        }

        private void Draw(bool updatePositions = false)
        {
            DrawingVisual staffLineVisual = new DrawingVisual();
            Pen pen = new Pen(Color, LineThickness);

            if (updatePositions || _yOffsets.Count == 0)
            {
                GenerateStaffLinesPositions();
            }
            using (DrawingContext dc = staffLineVisual.RenderOpen())
            {
                for (int i = 1; i <= _linesCount; i++)
                {
                    dc.DrawLine(pen, new Point(0.0, _yOffsets[i]), new Point(Width, _yOffsets[i]));
                }
            }
            _visual.ClearVisuals();
            _visual.AddVisual(staffLineVisual);
        }

        private void InitCanvas()
        {
            _visual = new DrawingVisualHost();
            CanvasVisual = new Canvas();
            CanvasVisual.Children.Add(_visual);
        }

        private void UpdateCanvasPosition()
        {
            Canvas.SetTop(CanvasVisual, HorizontalOffset);
        }
    }
}