using MusicXMLScore.Converters;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace MusicXMLScore.LayoutControl
{
    class LayoutSystemInfo : INotifyPropertyChanged
    {
        private double _systemHeight;
        private double _systemWidth;
        private double _defaultStaffDistance;

        private bool _updateLayout;

        //! distances between parts inside this system
        private Dictionary<string, double> _partStaffDistances;

        //! calculated part heights 
        private Dictionary<string, double> _partHeights;

        //! collection of measure widths to generate measures row
        private Dictionary<string, double> _measureSharedWidths;

        //! collection of measures inside this system
        private List<SharedMeasureProperties> _measures;

        //! measures X Coordinates inside system
        private Dictionary<string, double> _measureCoordsX;

        //! measures Y Coordinates inside system
        private Dictionary<string, double> _measureCoordsY;

        private Dictionary<string, double> _measureMinSharedWidth;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public bool UpdateLayout
        {
            get { return _updateLayout; }
            set
            {
                if (_updateLayout != value)
                {
                    _updateLayout = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateLayout)));
                    _updateLayout = false;
                }
            }
        }

        public double SystemHeight
        {
            get { return _systemHeight; }
        }

        public double SystemWidth
        {
            get { return _systemWidth; }
        }

        internal List<SharedMeasureProperties> Measures
        {
            get { return _measures; }

            set { _measures = value; }
        }

        public Dictionary<string, double> PartHeights
        {
            get { return _partHeights; }

            set { _partHeights = value; }
        }

        public double PartPositionY(string partId) => _measureCoordsY.ContainsKey(partId) ? _measureCoordsY[partId] : 0.0;

        public Point FirstPartMeasureCoords(string measureId) => new Point(_measureCoordsX[measureId], 0.0);

        /// <summary>
        /// Measure coordinates with valid measureID and partID
        /// </summary>
        /// <param name="measureId">ID of this measure</param>
        /// <param name="partId">Valid Part ID of this measure</param>
        /// <returns></returns>
        public Point WhicheverPartMeasureCoords(string measureId, string partId) => new Point(_measureCoordsX[measureId], _measureCoordsY[partId]);

        public LayoutSystemInfo(List<SharedMeasureProperties> measuresOfSystem)
        {
            _measures = measuresOfSystem;
            var layout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout;
            _defaultStaffDistance = 1.5 * layout.PageProperties.StaffHeight.MMToTenths();
            var partProperties = ViewModel.ViewModelLocator.Instance.Main.PartsProperties;
            var partHeights = partProperties.Select(a => new {key = a.Key, value = a.Value.PartHeight})
                .ToDictionary(item => item.key, item => item.value);
            AddPartHeights(partHeights);
            GeneratePartDistances();
        }

        public void AddPartHeights(Dictionary<string, double> partHeights)
        {
            _partHeights = partHeights;
        }

        public void AddPartDistances(Dictionary<string, double> partDistances)
        {
            _partStaffDistances = partDistances;
        }

        public void ArrangeSystem(bool stretchMeasures, double desiredWidth)
        {
            CalculateSystemDimensions(stretchMeasures, desiredWidth);
            GenerateCoords();
        }

        public void GeneratePartDistances()
        {
            _partStaffDistances = new Dictionary<string, double>();
            if (_partHeights != null)
            {
                foreach (var item in _partHeights)
                {
                    _partStaffDistances.Add(item.Key, _defaultStaffDistance);
                }
            }
            else
            {
                Log.LoggIt.Log(
                    $"Part Heights did not initialized! Part Distances does not calculated properly using distance: {_defaultStaffDistance}",
                    Log.LogType.Exception);
                //! init/generate part heights dictionary
            }
        }

        public void CalculateSystemDimensions(bool stretchMeasuresToWidth = false, double desiredWidth = 0.0)
        {
            double heightParts = _partHeights.Sum(x => x.Value);
            double distances = _partStaffDistances.Skip(1).Sum(x => x.Value);
            _systemHeight = heightParts + distances;
            if (_measureSharedWidths == null)
            {
                GetSharedWidths();
                GetMinSharedWidths();
            }
            _systemWidth = _measureSharedWidths.Sum(x => x.Value);

            if (stretchMeasuresToWidth && _systemWidth < desiredWidth)
            {
                UpdateSystemWidth(desiredWidth);
            }
            //! test ------------
            if (!stretchMeasuresToWidth)
            {
                _systemWidth = _measureMinSharedWidth.Sum(x => x.Value); //! update anyway
                UpdateSystemWidth(_systemWidth);
            }
        }

        private void GetSharedWidths()
        {
            _measureSharedWidths = new Dictionary<string, double>();
            foreach (var measure in _measures)
            {
                _measureSharedWidths.Add(measure.MeasureId, measure.SharedWidth);
            }
        }

        private void GetMinSharedWidths()
        {
            _measureMinSharedWidth = new Dictionary<string, double>();
            foreach (var measure in _measures)
            {
                _measureMinSharedWidth.Add(measure.MeasureId, measure.MinimalSharedWidth);
            }
        }

        private void GenerateMeasureXCoordinates()
        {
            double currentX = 0.0;
            if (_measureSharedWidths == null || _measureSharedWidths.Count == 0)
            {
                Log.LoggIt.Log("no measures inside collection: X coordinates did not generated", Log.LogType.Exception);
            }
            else
            {
                _measureCoordsX = new Dictionary<string, double>();
                foreach (var item in _measureSharedWidths)
                {
                    _measureCoordsX.Add(item.Key, currentX);
                    currentX += item.Value;
                }
            }
            if (currentX > _systemWidth && !currentX.Equals4DigitPrecision(_systemWidth))
            {
                Log.LoggIt.Log($"System width too small!!! Now is: {_systemWidth}, should be {currentX}", Log.LogType.Exception);
            }
        }

        public void UpdateSystemWidth(double desiredWidth)
        {
            double currentWidth = _measureSharedWidths.Sum(x => x.Value);
            double difference = desiredWidth - currentWidth;
            double itemsCount = _measureSharedWidths.Count;
            double offset = difference / itemsCount;
            foreach (var item in _measures)
            {
                item.SharedWidth += offset;
            }
            GetSharedWidths();
            _systemWidth = desiredWidth;
        }

        public void UpdateSystemHeight(double height)
        {
            _systemHeight = height;
        }

        private void GenerateMeasureYCoordinates()
        {
            double currentY = 0.0;
            if (_partStaffDistances == null || _partStaffDistances.Count == 0)
            {
                Log.LoggIt.Log("no part staff distances specified inside collection: Y coordinates did not generated", Log.LogType.Exception);
            }
            else
            {
                _measureCoordsY = new Dictionary<string, double>();
                foreach (var item in _partStaffDistances)
                {
                    _measureCoordsY.Add(item.Key, currentY);
                    currentY += item.Value + _partHeights[item.Key];
                }
            }
            if (currentY > _systemHeight && !currentY.Equals4DigitPrecision(_systemHeight))
            {
                Log.LoggIt.Log($"System height too small!!! Now is: {_systemHeight}, should be {currentY}", Log.LogType.Exception);
            }
        }

        private void GenerateCoords()
        {
            GenerateMeasureXCoordinates();
            GenerateMeasureYCoordinates();
        }
    }
}