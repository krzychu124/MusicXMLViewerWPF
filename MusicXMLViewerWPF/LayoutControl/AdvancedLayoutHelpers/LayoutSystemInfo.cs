using MusicXMLScore.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.LayoutControl
{ 
    class LayoutSystemInfo
    {
        private double systemHeight;
        private double systemWidth;
        double defaultStaffDistance;

        //! distances between parts inside this system
        private Dictionary<string, double> partStaffDistances;
        //! calculated part heights 
        private Dictionary<string, double> partHeights;
        //! collection of measure widths to generate measures row
        private Dictionary<string, double> measureSharedWidths;
        //! collection of measures inside this system
        private List<SharedMeasureProperties> measures;
        //! measures X Coordinates inside system
        private Dictionary<string, double> measureCoordsX;
        //! measures Y Coordinates inside system
        private Dictionary<string, double> measureCoordsY;
        public double SystemHeight
        {
            get
            {
                return systemHeight;
            }
        }

        public double SystemWidth
        {
            get
            {
                return systemWidth;
            }
        }

        internal List<SharedMeasureProperties> Measures
        {
            get
            {
                return measures;
            }

            set
            {
                measures = value;
            }
        }

        public KeyValuePair<string, Point> MeasureCoords(string key) => new KeyValuePair<string, Point>(key, new Point(measureCoordsX[key], 0.0)); 

        public LayoutSystemInfo(List<SharedMeasureProperties> measuresOfSystem)
        {
            measures = measuresOfSystem;
            var layout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout;
            defaultStaffDistance = 1.7 * layout.PageProperties.StaffHeight.MMToTenths();
            var partProperties = ViewModel.ViewModelLocator.Instance.Main.CurrentPartsProperties;
            var partHeights = partProperties.Select((a) => new { key = a.Key, value = a.Value.PartHeight }).ToDictionary(item => item.key, item => item.value);
            AddPartHeights(partHeights);
            GeneratePartDistances();
        }
        public void AddPartHeights(Dictionary<string, double> partHeights)
        {
            this.partHeights = partHeights;
        }

        public void AddPartDistances(Dictionary<string, double> partDistances)
        {
            partStaffDistances = partDistances;
        }

        public void ArrangeSystem(bool stretchLayout, double desiredWidth)
        {
            CalculateSystemDimensions(stretchLayout, desiredWidth);
            GenerateCoords();
        }
        public void GeneratePartDistances()
        {
            partStaffDistances = new Dictionary<string, double>();
            if (partHeights != null)
            {
                foreach (var item in partHeights)
                {
                    partStaffDistances.Add(item.Key, defaultStaffDistance);
                }
            }
            else
            {
                Log.LoggIt.Log($"Part Heights did not initialized! Part Distances does not calculated properly using distance: {defaultStaffDistance}", Log.LogType.Exception);
                //! init/generate part heights dictionary
            }
        }

        public void CalculateSystemDimensions(bool stretchToWidth = false, double desiredWidth = 0.0)
        {
            double heightParts = partHeights.Sum(x => x.Value);
            double distances = partStaffDistances.Skip(1).Sum(x => x.Value);
            systemHeight = heightParts + distances;
            if (measureSharedWidths == null)
            {
                GetSharedWidths();
            }
            systemWidth = measureSharedWidths.Sum(x => x.Value);

            if (stretchToWidth && systemWidth < desiredWidth)
            {
               UpdateSystemWidth(desiredWidth);
            }
           
        }
        private void GetSharedWidths()
        {
            measureSharedWidths = new Dictionary<string, double>();
            foreach (var measure in measures)
            {
                measureSharedWidths.Add(measure.MeasureId, measure.SharedWidth);
            }
        }
        private void GenerateMeasureXCoordinates()
        {
            double currentX = 0.0;
            if (measureSharedWidths == null || measureSharedWidths.Count == 0)
            {
                Log.LoggIt.Log("no measures inside collection: X coordinates did not generated", Log.LogType.Exception);
            }
            else
            {
                measureCoordsX = new Dictionary<string, double>();
                foreach (var item in measureSharedWidths)
                {
                    measureCoordsX.Add(item.Key, currentX);
                    currentX += item.Value;
                }
            }
            if (currentX > systemWidth)
            {
                Log.LoggIt.Log($"System width too small!!! Now is: {systemWidth}, should be {currentX}", Log.LogType.Exception);
            }
        }

        public void UpdateSystemWidth(double desiredWidth)
        {
            double currentWidth = systemWidth;
            double difference = desiredWidth - currentWidth;
            double itemsCount = measureSharedWidths.Count;
            double offset = difference / itemsCount;
           // var keys = measureSharedWidths.Select(x=>x.Key);
            foreach (var item in measures)
            {
                double correctedValue = (item.SharedWidth / desiredWidth) *difference;
                item.SharedWidth = item.SharedWidth + offset;
            }
            GetSharedWidths();
            systemWidth = desiredWidth;
        }

        public void UpdateSystemHeight(double height)
        {
            systemHeight = height;
        }

        private void GenerateMeasureYCoordinates()
        {
            double currentY = 0.0;
            if (partStaffDistances == null || partStaffDistances.Count == 0)
            {
                Log.LoggIt.Log("no part staff distances specified inside collection: Y coordinates did not generated", Log.LogType.Exception);
            }
            else
            {
                measureCoordsY = new Dictionary<string, double>();
                foreach (var item in partStaffDistances)
                {
                    measureCoordsY.Add(item.Key, currentY);
                    currentY += item.Value + partHeights[item.Key];
                }
            }
            if (currentY > systemHeight)
            {
                Log.LoggIt.Log($"System height too small!!! Now is: {systemHeight}, should be {currentY}", Log.LogType.Exception);
            }
        }
        public void GenerateCoords()
        {
            GenerateMeasureXCoordinates();
            GenerateMeasureYCoordinates();
        }
    }
}
