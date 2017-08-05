using MusicXMLScore.Converters;
using MusicXMLScore.Helpers;
using MusicXMLScore.VisualObject;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.VisualObjectController
{
    internal class StaffLineVisualController : INotifyPropertyChanged
    {
        private int _numberOfLines;
        private int _staffCount;
        private double _staffDistance;
        private Dictionary<int, double> _staffsVerticalPositions;
        private double _width;

        public StaffLineVisualController(int staffCount, double width, int numberOfLines, Model.ScorePartwisePartMeasureMusicXML measure, double staffDistance = 0.0)
        {
            _staffCount = staffCount;
            _width = width;
            _staffDistance = staffDistance;
            _numberOfLines = numberOfLines;

            StaffVisuals = new ObservableDictionary<int, StaffLineVisual>();
            _staffsVerticalPositions = new Dictionary<int, double>();
            GenerateStaffLines();

            CollectStaffs();
            WeakEventManager<Model.ScorePartwisePartMeasureMusicXML, PropertyChangedEventArgs>.AddHandler(measure, "PropertyChanged", MeasureMusicXML_PropertyChanged);
            PropertyChanged += StaffLineVisualController_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public Canvas StaffLineCanvas { get; set; }

        public double Width
        {
            get { return _width; }

            set
            {
                if (!_width.Equals4DigitPrecision(value)) {
                    _width = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
                }
            }
        }

        internal ObservableDictionary<int, StaffLineVisual> StaffVisuals { get; }

        private void AddStaff(StaffLineVisual staff, int staffNumber = 1)
        {
            if (StaffVisuals.ContainsKey(staffNumber))
            {
                //! log staffLine is overwritten
                //! all references of previous staffLine has to be removes
                StaffVisuals[staffNumber] = staff;
            }
            else
            {
                StaffVisuals.Add(staffNumber, staff);
            }
        }

        private void CollectStaffs()
        {
            StaffLineCanvas = new Canvas();
            foreach (var staff in StaffVisuals.Values)
            {
                StaffLineCanvas.Children.Add(staff.CanvasVisual);
            }
        }

        private void GenerateStaffLines()
        {
            double currentY = 0.0;
            for (int i = 1; i <= _staffCount; i++)
            {
                var staff = new StaffLineVisual(_width, _numberOfLines) { StaffNumber = i };
                if (i != 1)
                {
                    staff.HorizontalOffset = currentY + _staffDistance;
                }
                currentY += staff.Height;
                AddStaff(staff, i);
            }
            UpdatePositions();
        }

        private void MeasureMusicXML_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var measure = sender as Model.ScorePartwisePartMeasureMusicXML;
            if (measure != null)
            {
                if (e.PropertyName == nameof(measure.CalculatedWidth))
                {
                    Width = measure.CalculatedWidth.TenthsToWPFUnit();
                }
            }
        }

        private void StaffLineVisualController_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Width))
            {
                foreach (var staff in StaffVisuals.Values)
                {
                    staff.Width = Width;
                }
            }
        }

        private void UpdatePositions()
        {
            _staffsVerticalPositions.Clear();
            for (int i = 1; i <= _staffCount; i++)
            {
                _staffsVerticalPositions.Add(i, StaffVisuals[i].HorizontalOffset);
            }
        }
    }
}