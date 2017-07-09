using MusicXMLScore.Converters;
using MusicXMLScore.Helpers;
using MusicXMLScore.Model.MeasureItems.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicXMLScore.VisualObject
{
    class StaffLineVisual : INotifyPropertyChanged
    {
        private Brush color;
        private int linesCount;
        private double yPosition;
        private double width;
        private double height;
        private DrawingVisualHost visual;
        private Canvas canvasVisual;
        private double lineSpacing;
        private double lineThickness;
        private Dictionary<int, double> lineYOffset;
        private int staffNumber = 1;
        public Brush Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
            }
        }

        public int LinesCount
        {
            get
            {
                return linesCount;
            }

            set
            {
                if (value != linesCount)
                {
                    linesCount = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(LinesCount)));
                }
            }
        }

        public double YPosition
        {
            get
            {
                return yPosition;
            }

            set
            {
                if (value != yPosition)
                {
                    yPosition = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(YPosition)));
                }
            }
        }

        public double Width
        {
            get
            {
                return width;
            }

            set
            {
                if (value != width)
                {
                    width = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
                }
            }
        }

        public double Height
        {
            get
            {
                return height;
            }

            set
            {
                height = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Height)));
            }
        }

        public DrawingVisualHost Visual
        {
            get
            {
                return visual;
            }

            set
            {
                visual = value;
            }
        }

        public Canvas CanvasVisual
        {
            get
            {
                return canvasVisual;
            }

            set
            {
                canvasVisual = value;
            }
        }

        public int StaffNumber
        {
            get
            {
                return staffNumber;
            }

            set
            {
                staffNumber = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public StaffLineVisual(double width, int numberOfLines = 5, Brush color = null)
        {
            GetDefaults();
            this.width = width;
            this.linesCount = numberOfLines;
            this.color = color ?? Brushes.Black;
            this.yPosition = 0.0;
            lineYOffset = new Dictionary<int, double>();
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
                        lineSpacing = lineSpacing * 1.5;
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
                    Width = measure.CalculatedWidth.TenthsToWPFUnit();
                    break;
                case nameof(YPosition):
                    UpdateCanvasPosition();
                    break;
                case nameof(DrawingHelpers.PartProperties.NumberOfLines):
                    var partProperties = sender as DrawingHelpers.PartProperties;
                    LinesCount = partProperties.NumberOfLines;
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
            lineThickness = measureLayout.StaffLineThickness.TenthsToWPFUnit();
        }
        private void GetStaffLineSpacing()
        {
            var measureLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.MeasureStyle;
            lineSpacing = measureLayout.StaffSpaceLegth.TenthsToWPFUnit();
        }
        private void GenerateStaffLinesPositions()
        {
            if (lineYOffset.Count != 0)
            {
                lineYOffset.Clear();
            }
            double offsetFromZero = (5 - linesCount) / 2.0 * 10.0.TenthsToWPFUnit();
            double currentOffset = 0 + offsetFromZero;
            for (int i = 1; i <= linesCount; i++)
            {
                lineYOffset.Add(i, currentOffset);
                currentOffset += lineSpacing;
            }
        }

        private void Draw(bool updatePositions = false)
        {
            DrawingVisual staffLineVisual = new DrawingVisual();
            Pen pen = new Pen(color, lineThickness);

            if (updatePositions || lineYOffset.Count ==0)
            {
                GenerateStaffLinesPositions();
            }
            using (DrawingContext dc = staffLineVisual.RenderOpen())
            {
                for (int i = 1; i <= linesCount; i++)
                {
                    dc.DrawLine(pen, new Point(0.0, lineYOffset[i]), new Point(width, lineYOffset[i]));
                }
            }
            visual.ClearVisuals();
            visual.AddVisual(staffLineVisual);
        }
        private void InitCanvas()
        {
            visual = new DrawingVisualHost();
            canvasVisual = new Canvas();
            canvasVisual.Children.Add(visual);
        }
        private void UpdateCanvasPosition()
        {
            Canvas.SetTop(canvasVisual, YPosition);
        }
    }
}
