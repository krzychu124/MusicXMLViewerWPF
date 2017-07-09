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
        private LayoutSystemInfo systemLayoutInfo;
        //! temp, test
        private List<Canvas> measuresSegments;
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

        public PartSegmentDrawing(List<MeasureSegmentController> measureSegments, string partID, LayoutSystemInfo layoutInfo)
        {
            this.partId = partID;
            this.measuresList = measureSegments.Select(x => x.MeasureID).ToList();
            this.partMeasures = measureSegments;
            this.partProperties = ViewModel.ViewModelLocator.Instance.Main.CurrentPartsProperties[partID];
            this.stavesCount = partProperties.NumberOfStaves;
            this.systemLayoutInfo = layoutInfo;
            systemLayoutInfo.PropertyChanged += SystemLayoutInfo_PropertyChanged;
            staffDistance = partProperties.StaffLayoutPerPage[pageIndex].ElementAt(0).StaffDistance;
            CalculateDimensions();
        }

        private void SystemLayoutInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(systemLayoutInfo.UpdateLayout))
            {
                UpdateContent();
            }
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

        public void GenerateContent()
        {
            foreach (var measureId in measuresList)
            {
                MeasureDrawing measureCanvas = new MeasureDrawing(measureId, partId, staffDistance, stavesCount);
                ScorePartwisePartMeasureMusicXML measureSerializable = ViewModel.ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.ElementAt(partId.GetPartIdIndex()).MeasuresByNumber[measureId];

                Canvas.SetTop(measureCanvas.BaseObjectVisual, 0);
                Canvas.SetLeft(measureCanvas.BaseObjectVisual, partProperties.Coords[measureId].X);
                PartSegmentCanvas.Children.Add(measureCanvas.BaseObjectVisual);


                MeasureSegmentController measureSegment = new LayoutControl.MeasureSegmentController(measureSerializable, partId, stavesCount);
                partMeasures.Add(measureSegment);


                Canvas.SetTop(measureSegment.GetMeasureCanvas(), 0);
                Canvas.SetLeft(measureSegment.GetMeasureCanvas(), partProperties.Coords[measureId].X);
                PartSegmentCanvas.Children.Add(measureSegment.GetMeasureCanvas());
            }
        }
        public void GenerateContent(bool test, LayoutSystemInfo systemLayout)
        {
            if (systemLayout != null)
            {
                measuresSegments = new List<Canvas>(); //! holds reference for future position update
                //! use system layout info
                foreach (var measureSegment in PartMeasures)
                {
                    //! -------test
                    measuresSegments.Add(measureSegment.GetMeasureCanvas());
                    //! -------
                    Canvas.SetTop(measureSegment.GetMeasureCanvas(), 0);
                    Canvas.SetLeft(measureSegment.GetMeasureCanvas(), systemLayout.WhicheverPartMeasureCoords(measureSegment.MeasureID, partId).X);
                    PartSegmentCanvas.Children.Add(measureSegment.GetMeasureCanvas());
                }
            }
        }

        private void UpdateContent()
        {
            for (int i = 0; i < partMeasures.Count; i++)
            {
                Canvas.SetTop(measuresSegments[i], 0);
                Canvas.SetLeft(measuresSegments[i], systemLayoutInfo.WhicheverPartMeasureCoords(partMeasures[i].MeasureID, partId).X);
            }
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