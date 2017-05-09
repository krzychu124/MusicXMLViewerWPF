using MusicXMLScore.Converters;
using MusicXMLScore.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MusicXMLScore.LayoutControl;

namespace MusicXMLScore.DrawingHelpers
{
     class PartSegmentDrawing
    {
        #region Fields

        private List<string> measuresList;
        private string partId;
        private PartProperties partProperties;
        private List<MeasureSegmentController> partMeasures = new List<MeasureSegmentController>();
        private Canvas partSegmentCanvas;
        private Size size;
        private double staffDistance = 0.0;
        private int stavesCount = 1;
        private int systemIndex;
        private int pageIndex;
        #endregion Fields

        #region Constructors

        public PartSegmentDrawing(List<string> measuresList, string partId, PartProperties partProperites, int systemIndex, int pageIndex)
        {
            this.measuresList = measuresList;
            this.partId = partId;
            this.partProperties = partProperites;
            this.systemIndex = systemIndex;
            this.pageIndex = pageIndex;
            stavesCount = partProperties.NumberOfStaves;
            staffDistance = partProperites.StaffLayoutPerPage[pageIndex].ElementAt(systemIndex).StaffDistance;
            CalculateDimensions();
        }
        public PartSegmentDrawing(List<MeasureSegmentController> measureSegments, string partId, PartProperties partProperties, int systemIndex, int pageIndex)
        {
            this.measuresList = measureSegments.Select(x => x.MeasureID).ToList();
            this.partMeasures = measureSegments;
            this.partId = partId;
            this.partProperties = partProperties;
            this.systemIndex = systemIndex;
            this.pageIndex = pageIndex;
            stavesCount = partProperties.NumberOfStaves;
            staffDistance = partProperties.StaffLayoutPerPage[pageIndex].ElementAt(0).StaffDistance;
            CalculateDimensions();
        }

        #endregion Constructors

        #region Properties

        public string PartId
        {
            get
            {
                return partId;
            }

            set
            {
                partId = value;
            }
        }

        /// <summary>
        /// Part size in Tenths, convert to WPFUnit when using for drawing dimensions
        /// </summary>
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

        internal Canvas PartSegmentCanvas
        {
            get
            {
                return partSegmentCanvas;
            }

            set
            {
                partSegmentCanvas = value;
            }
        }

        internal List<MeasureSegmentController> PartMeasures
        {
            get
            {
                return partMeasures;
            }

            set
            {
                partMeasures = value;
            }
        }

        #endregion Properties

        #region Methods

        public Size GenerateContent()
        {
            foreach (var measureId in measuresList)
            {
                MeasureDrawing measureCanvas = new MeasureDrawing(measureId, partId, staffDistance, stavesCount);
                ScorePartwisePartMeasureMusicXML measureSerializable = ViewModel.ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.ElementAt(partId.GetPartIdIndex()).MeasuresByNumber[measureId];

                Canvas.SetTop(measureCanvas.BaseObjectVisual, 0);
                Canvas.SetLeft(measureCanvas.BaseObjectVisual, partProperties.Coords[measureId].X);
                PartSegmentCanvas.Children.Add(measureCanvas.BaseObjectVisual);


                MeasureSegmentController measureSegment = new LayoutControl.MeasureSegmentController(measureSerializable, partId, stavesCount, systemIndex, pageIndex);
                partMeasures.Add(measureSegment);

                //! workaroud to eliminate segment panel
                //Canvas measureObjectCanvas = measureSegment.GetMeasureCanvas();
                SegmentPanel spanel = measureSegment.GetContentPanel();

                Canvas.SetTop(spanel, 0);
                Canvas.SetLeft(spanel, partProperties.Coords[measureId].X);
                PartSegmentCanvas.Children.Add(spanel);

                //! workaroud to eliminate segment panel
                //Canvas.SetTop(measureObjectCanvas, 0);
                //Canvas.SetLeft(measureObjectCanvas, partProperties.Coords[measureId].X);
                //PartSegmentCanvas.Children.Add(measureObjectCanvas);
            }
            return size;
        }
        public Size GenerateContent(bool test, LayoutSystemInfo systemLayout =null)
        {
            foreach (var measureSegment in PartMeasures)
            {
                MeasureDrawing measureCanvas = new MeasureDrawing(measureSegment.MeasureID, partId, staffDistance, stavesCount);

                Canvas.SetTop(measureCanvas.BaseObjectVisual, 0);
                Canvas.SetLeft(measureCanvas.BaseObjectVisual, partProperties.Coords[measureSegment.MeasureID].X);
                PartSegmentCanvas.Children.Add(measureCanvas.BaseObjectVisual);

                SegmentPanel spanel = measureSegment.GetContentPanel();

                Canvas.SetTop(spanel, 0);
                Canvas.SetLeft(spanel, partProperties.Coords[measureSegment.MeasureID].X);
                PartSegmentCanvas.Children.Add(spanel);

            }
            return size;
        }

        private void CalculateDimensions()
        {
            double staffHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.StaffHeight.MMToWPFUnit();
            double segmentHeight = (stavesCount * staffHeight) + ((stavesCount - 1) * staffDistance);
            double segmentWidth = measuresList.CalculateWidth(partId);
            partSegmentCanvas = new Canvas() { Width = segmentWidth, Height = segmentHeight };
            size = new Size(segmentWidth, segmentHeight);
        }

        #endregion Methods
    }
}