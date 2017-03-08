using MusicXMLScore.Helpers;
using MusicXMLScore.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.DrawingHelpers
{
    public class PartsSystemDrawing
    {
        private Canvas partSystemCanvas;
        private Dictionary<string,PartSegmentDrawing> partsSegments;
        private List<string> partIDsList;
        private List<string> measuresList;
        private Dictionary<string, PartProperties> partsPropertiesList;
        private Dictionary<string, Tuple<double, double, double>> partsPositions;
        //! <PartId, <MarginLeft, distance to previous, distance to topLine>>  
        //! First partID in first page system = TopDistance, next StaffDistance
        //! Other elements using StaffDistance
        private Point position;
        private Size size;
        private double partWidth;
        private int systemIndex;
        private double leftMargin;
        private double rightMargin;
        public PartsSystemDrawing(int systemIndex, List<string> measuresToDraw, List<string> partsIdList, Dictionary<string, PartProperties> partsProperties)
        {
            this.systemIndex = systemIndex;
            measuresList = measuresToDraw;
            this.partIDsList = partsIdList;
            partWidth = measuresList.CalculateWidth(partIDsList.ElementAt(0));
            this.partsPropertiesList = partsProperties;
            GetSetSystemMargins();
            PartsSegmentsDraw();
        }

        private void PartsSegmentsDraw()
        {
            partsSegments = new Dictionary<string, PartSegmentDrawing>();
            partSystemCanvas = new Canvas();
            foreach (var partId in partIDsList)
            {
                PartSegmentDrawing partSegment = new PartSegmentDrawing(measuresList, partId, partsPropertiesList[partId]);
                partsSegments.Add(partId, partSegment);
                partSegment.GenerateContent();
                PartSystemCanvas.Children.Add(partSegment.PartSegmentCanvas);
            }
            CalculatePositions();
            ArrangeContent();
        }

        private void ArrangeContent()
        {
            partSystemCanvas.Width = size.Width;
            partSystemCanvas.Height = size.Height;
            foreach (var partSegment in partsSegments.Values)
            {
                string partSegmentId = partSegment.PartId;
                Canvas.SetTop(partSegment.PartSegmentCanvas, partsPositions[partSegmentId].Item3);
                Canvas.SetLeft(partSegment.PartSegmentCanvas, partsPositions[partSegmentId].Item1);
            }
        }

        public void CalculatePositions()
        {
            partsPositions = new Dictionary<string, Tuple<double, double, double>>();
            double distanceToTop = 0.0;
            double distanceToPrevious = 0.0;
            //double leftMargin = 0.0;
            foreach (var partSegment in partsSegments.Values)
            {
                string partSegmentID = partSegment.PartId;
                int partIndex = partSegmentID.GetPartIdIndex();
                int firstMeasureIndex = systemIndex; //? measuresList.ElementAt(0).GetMeasureIdIndex();
                var currentPartProperties = partsPropertiesList[partSegmentID];
                //leftMargin = currentPartProperties.SystemLayout.ElementAt(firstMeasureIndex).SystemMargins.LeftMargin.TenthsToWPFUnit(); //? system index
                if (partIndex== 0)
                {
                    distanceToPrevious = currentPartProperties.SystemLayout.ElementAt(firstMeasureIndex).TopSystemDistance.TenthsToWPFUnit();
                    distanceToPrevious += partSegment.Size.Height;
                    distanceToTop = currentPartProperties.SystemLayout.ElementAt(firstMeasureIndex).TopSystemDistance.TenthsToWPFUnit();
                    distanceToTop += partSegment.Size.Height;
                    partsPositions.Add(partSegmentID, new Tuple<double, double, double>(leftMargin, distanceToPrevious, distanceToTop));
                }
                else
                {
                    distanceToPrevious = currentPartProperties.SystemLayout.ElementAt(firstMeasureIndex).SystemDistance.TenthsToWPFUnit();
                    distanceToPrevious += partSegment.Size.Height;
                    distanceToTop += currentPartProperties.SystemLayout.ElementAt(firstMeasureIndex).SystemDistance.TenthsToWPFUnit();
                    distanceToTop += partSegment.Size.Height;
                    partsPositions.Add(partSegmentID, new Tuple<double, double, double>(leftMargin, distanceToPrevious, distanceToTop));
                }
            }
            this.size = new Size(partWidth + leftMargin, distanceToTop);
        }
        private void CalculateSize()
        {
            var lastSegmentPosition = partsPositions.LastOrDefault();
            this.size = new Size(partWidth + lastSegmentPosition.Value.Item1, lastSegmentPosition.Value.Item3);
        }
        private void GetSetSystemMargins() //TODO_H do more tests...
        {
            var currentPartProperties = partsPropertiesList.ElementAt(0).Value;
            //var measureId
            leftMargin = currentPartProperties.SystemLayout.ElementAt(systemIndex).SystemMargins.LeftMargin.TenthsToWPFUnit();
            rightMargin = currentPartProperties.SystemLayout.ElementAt(systemIndex).SystemMargins.RightMargin.TenthsToWPFUnit();
        }
        public Canvas PartSystemCanvas
        {
            get
            {
                return partSystemCanvas;
            }

            set
            {
                partSystemCanvas = value;
            }
        }

        public Size Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }
    }
}
