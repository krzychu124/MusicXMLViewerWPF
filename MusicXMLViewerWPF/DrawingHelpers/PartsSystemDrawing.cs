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
        #region Fields

        private double leftMargin;
        private List<string> measuresList;
        private List<string> partIDsList;
        private Dictionary<string, Tuple<double, double, double>> partsPositions;
        //! <PartId, <MarginLeft, distance to previous, distance to topLine>>  
        //! First partID in first page system = TopDistance, next StaffDistance
        //! Other elements using StaffDistance
        private Dictionary<string, PartProperties> partsPropertiesList;
        private Dictionary<string, PartSegmentDrawing> partsSegments;
        private Canvas partSystemCanvas;
        private double partWidth;

        private double rightMargin;

        private Size size;
        private int systemIndex;
        private int pageIndex;
        #endregion Fields

        #region Constructors

        public PartsSystemDrawing(int systemIndex, List<string> measuresToDraw, List<string> partsIdList, Dictionary<string, PartProperties> partsProperties, int pageIndex)
        {
            this.systemIndex = systemIndex;
            measuresList = measuresToDraw;
            this.partIDsList = partsIdList;
            partWidth = measuresList.CalculateWidth(partIDsList.ElementAt(0));
            this.partsPropertiesList = partsProperties;
            this.pageIndex = pageIndex;
            GetSetSystemMargins();
            PartsSegmentsDraw();
        }

        public double LeftMargin
        {
            get
            {
                return leftMargin;
            }

            set
            {
                leftMargin = value;
            }
        }

        #endregion Constructors

        #region Properties

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

        public int SystemIndex
        {
            get
            {
                return systemIndex;
            }

            set
            {
                systemIndex = value;
            }
        }

        #endregion Properties

        #region Methods

        public void CalculatePositions()
        {
            partsPositions = new Dictionary<string, Tuple<double, double, double>>();
            double distanceToTop = 0.0;
            double distanceToPrevious = 0.0;
            foreach (var partSegment in partsSegments.Values)
            {
                string partSegmentID = partSegment.PartId;
                int partIndex = partSegmentID.GetPartIdIndex();
                var currentPartProperties = partsPropertiesList[partSegmentID];
                if (partIndex == 0)
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
                    distanceToTop += distanceToPrevious;
                    partsPositions.Add(partSegmentID, new Tuple<double, double, double>(leftMargin, distanceToPrevious, distanceToTop));
                    distanceToTop += partSegment.Size.Height;
                }
            }
            this.size = new Size(partWidth, distanceToTop);
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
            //TODO_WIP arrange MeasureAttributesContainer items; If more than one part, rearange measureattributes placement of each part measure using largest width offset of every type 

        }

        private void CalculateSize()
        {
            var lastSegmentPosition = partsPositions.LastOrDefault();
            this.size = new Size(partWidth + lastSegmentPosition.Value.Item1, lastSegmentPosition.Value.Item3);
        }

        private void GetSetSystemMargins() //TODO_WIP do more tests... //
        {
            var currentPartProperties = partsPropertiesList.ElementAt(0).Value;
            leftMargin = 0;// currentPartProperties.SystemLayout.ElementAt(systemIndex).SystemMargins.LeftMargin.TenthsToWPFUnit();
            rightMargin = 0;//currentPartProperties.SystemLayout.ElementAt(systemIndex).SystemMargins.RightMargin.TenthsToWPFUnit();
        }

        private void PartsSegmentsDraw()
        {
            partsSegments = new Dictionary<string, PartSegmentDrawing>();
            partSystemCanvas = new Canvas();
            foreach (var partId in partIDsList)
            {
                PartSegmentDrawing partSegment = new PartSegmentDrawing(measuresList, partId, partsPropertiesList[partId], systemIndex, pageIndex);
                partsSegments.Add(partId, partSegment);
                partSegment.GenerateContent();
                PartSystemCanvas.Children.Add(partSegment.PartSegmentCanvas);
            }
            CalculatePositions();
            ArrangeContent();
        }

        #endregion Methods
    }
}
