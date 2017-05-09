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
        }
        public void AddPartHeights(Dictionary<string, double> partHeights)
        {
            this.partHeights = partHeights;
        }

        public void AddPartDistances(Dictionary<string, double> partDistances)
        {
            partStaffDistances = partDistances;
        }
        public void GeneratePartDistances(double defaultStaffDistance)
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
                Log.LoggIt.Log($"Part Heights did not initialized! Part Distances does not calculated properly using distance: {defaultStaffDistance}");
                //! init/generate part heights dictionary
            }
        }

        public void CalculateSystemDimensions()
        {
            double heightParts = partHeights.Sum(x => x.Value);
            double distances = partStaffDistances.Skip(1).Sum(x => x.Value);
            systemHeight = heightParts + distances;
            if (measureSharedWidths == null)
            {
                GetSharedWidths();
            }
            systemWidth = measureSharedWidths.Sum(x => x.Value);
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
                Log.LoggIt.Log("no measures inside collection: X coordinates did not generated");
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
                Log.LoggIt.Log($"System width too small!!! Now is: {systemWidth}, should be {currentX}");
            }
        }

        public void UpdateSystemWidth(double width)
        {
            systemWidth = width;
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
                Log.LoggIt.Log("no part staff distances specified inside collection: Y coordinates did not generated");
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
                Log.LoggIt.Log($"System height too small!!! Now is: {systemHeight}, should be {currentY}");
            }
        }
        public void GenerateCoords()
        {
            GenerateMeasureXCoordinates();
            GenerateMeasureYCoordinates();
        }
    }
}
