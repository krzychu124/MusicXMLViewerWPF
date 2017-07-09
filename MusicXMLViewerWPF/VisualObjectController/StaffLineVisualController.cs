using MusicXMLScore.Converters;
using MusicXMLScore.Helpers;
using MusicXMLScore.VisualObject;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.VisualObjectController
{
    class StaffLineVisualController : INotifyPropertyChanged
    {
        private int staffCount;
        private double width;
        private ObservableDictionary<int, StaffLineVisual> staffVisuals;
        private Dictionary<int, double> staffY;
        private Canvas staffLineCanvas;
        private double staffDistance;
        private int numberOfLines;

        public event PropertyChangedEventHandler PropertyChanged =delegate { };

        internal ObservableDictionary<int, StaffLineVisual> StaffVisuals
        {
            get
            {
                return staffVisuals;
            }

            set
            {
                staffVisuals = value;
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
                width = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
            }
        }

        public Canvas StaffLineCanvas
        {
            get
            {
                return staffLineCanvas;
            }

            set
            {
                staffLineCanvas = value;
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

        public StaffLineVisualController(int staffCount, double width, int numberOfLines, Model.ScorePartwisePartMeasureMusicXML measure = null, double staffDistance = 0.0)
        {
            this.staffCount = staffCount;
            this.width = width;
            this.staffDistance = staffDistance;
            this.numberOfLines = numberOfLines;

            staffVisuals = new ObservableDictionary<int, StaffLineVisual>();
            staffY = new Dictionary<int, double>();
            Draw();

            CollectStaffs();
            WeakEventManager<Model.ScorePartwisePartMeasureMusicXML, PropertyChangedEventArgs>.AddHandler(measure, "PropertyChanged", MeasureMusicXML_PropertyChanged);
            PropertyChanged += StaffLineVisualController_PropertyChanged;
        }
        
        public void AddStaff(StaffLineVisual staff, int staffNumber = 1)
        {
            if (staffVisuals.ContainsKey(staffNumber))
            {
                //! log staffLine is overwritten
                //! all references of previous staffLine has to be removes
                staffVisuals[staffNumber] = staff;
            }else
            {
                staffVisuals.Add(staffNumber, staff);
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
            staffY.Clear();
            for (int i = 1; i <= staffCount; i++)
            {
                staffY.Add(i, staffVisuals[i].YPosition);
            }
        }

        private void GenerateStaffLines()
        {
            double currentY =0.0;
            for (int i = 1; i <= staffCount; i++)
            {
                var staff = new StaffLineVisual(width, numberOfLines);
                staff.StaffNumber = i;
                if (i != 1)
                {
                    staff.YPosition = currentY + staffDistance;
                }
                currentY += staff.Height;
                AddStaff(staff, i);
            }
            UpdatePositions();
        }

        private void CollectStaffs()
        {
            staffLineCanvas = new Canvas();
            foreach (var staff in StaffVisuals.Values)
            {
                staffLineCanvas.Children.Add(staff.CanvasVisual);
            }
        }
    }
}
