using MusicXMLScore.Converters;
using MusicXMLScore.Helpers;
using MusicXMLScore.VisualObject;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.VisualObjectController
{
    class StaffLineVisualController : INotifyPropertyChanged
    {
        private int _staffCount;
        private double _width;
        private Dictionary<int, double> _staffY;
        private Canvas _staffLineCanvas;
        private double _staffDistance;
        private int _numberOfLines;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        internal ObservableDictionary<int, StaffLineVisual> StaffVisuals { get; }

        public double Width
        {
            get { return _width; }

            set
            {
                _width = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
            }
        }

        public Canvas StaffLineCanvas
        {
            get { return _staffLineCanvas; }

            set { _staffLineCanvas = value; }
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

        public StaffLineVisualController(int staffCount, double width, int numberOfLines, Model.ScorePartwisePartMeasureMusicXML measure = null,
            double staffDistance = 0.0)
        {
            _staffCount = staffCount;
            _width = width;
            _staffDistance = staffDistance;
            _numberOfLines = numberOfLines;

            StaffVisuals = new ObservableDictionary<int, StaffLineVisual>();
            _staffY = new Dictionary<int, double>();
            Draw();

            CollectStaffs();
            WeakEventManager<Model.ScorePartwisePartMeasureMusicXML, PropertyChangedEventArgs>.AddHandler(measure, "PropertyChanged",
                MeasureMusicXML_PropertyChanged);
            PropertyChanged += StaffLineVisualController_PropertyChanged;
        }

        public void AddStaff(StaffLineVisual staff, int staffNumber = 1)
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

        public void Draw(bool update = false)
        {
            if (update)
            {
                //todo
            }
            else
            {
                GenerateStaffLines();
            }
        }

        private void UpdatePositions()
        {
            _staffY.Clear();
            for (int i = 1; i <= _staffCount; i++)
            {
                _staffY.Add(i, StaffVisuals[i].YPosition);
            }
        }

        private void GenerateStaffLines()
        {
            double currentY = 0.0;
            for (int i = 1; i <= _staffCount; i++)
            {
                var staff = new StaffLineVisual(_width, _numberOfLines) {StaffNumber = i};
                if (i != 1)
                {
                    staff.YPosition = currentY + _staffDistance;
                }
                currentY += staff.Height;
                AddStaff(staff, i);
            }
            UpdatePositions();
        }

        private void CollectStaffs()
        {
            _staffLineCanvas = new Canvas();
            foreach (var staff in StaffVisuals.Values)
            {
                _staffLineCanvas.Children.Add(staff.CanvasVisual);
            }
        }
    }
}