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
                Canvas.SetTop(partSegment.PartSegmentCanvas, partsPositions[partSegmentId].Item3); //TODO_H layout-problem with first part...
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
                var currentPartProperties = partsPropertiesList[partSegmentID];
                if (partIndex== 0)
                {
                    distanceToPrevious = 0;
                    distanceToTop = partSegment.Size.Height;
                    partsPositions.Add(partSegmentID, new Tuple<double, double, double>(leftMargin, distanceToPrevious, distanceToPrevious)); 
                    //? distanceToPrevious instead of distanceToTop, no top offset while first part in system
                    //? distanceToTop set to partHeight to simulate bottom line of part ;)
                }
                else
                {
                    distanceToPrevious = currentPartProperties.StaffLayout.ElementAt(systemIndex).StaffDistance.TenthsToWPFUnit();
                    distanceToTop += distanceToPrevious; //? + partSegment.Size.Height;
                    partsPositions.Add(partSegmentID, new Tuple<double, double, double>(leftMargin, distanceToPrevious, distanceToTop));
                    distanceToTop += partSegment.Size.Height; //? test
                }
            }
            this.size = new Size(partWidth + leftMargin, distanceToTop);
        }
        private void CalculateSize()
        {
            var lastSegmentPosition = partsPositions.LastOrDefault();
            this.size = new Size(partWidth + lastSegmentPosition.Value.Item1, lastSegmentPosition.Value.Item3);
        }
        private void GetSetSystemMargins() //TODO_H do more tests... //
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
