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

        private List<string> _measuresList;
        private string _partId;
        private PartProperties _partProperties;
        private List<MeasureSegmentController> _partMeasures = new List<MeasureSegmentController>();
        private Canvas _partSegmentCanvas;
        private Size _size;
        private double _staffDistance;
        private int _stavesCount = 1;
        private int _systemIndex;
        private int _pageIndex;
        private LayoutSystemInfo _systemLayoutInfo;
        //! temp, test
        private List<Canvas> _measuresSegments;
        #endregion Fields

        #region Constructors

        public PartSegmentDrawing(List<string> measuresList, string partId, PartProperties partProperites, int systemIndex, int pageIndex)
        {
            _measuresList = measuresList;
            _partId = partId;
            _partProperties = partProperites;
            _systemIndex = systemIndex;
            _pageIndex = pageIndex;
            _stavesCount = _partProperties.NumberOfStaves;
            _staffDistance = partProperites.StaffLayoutPerPage[pageIndex].ElementAt(systemIndex).StaffDistance;
            CalculateDimensions();
        }

        public PartSegmentDrawing(List<MeasureSegmentController> measureSegments, string partId, LayoutSystemInfo layoutInfo)
        {
            _partId = partId;
            _measuresList = measureSegments.Select(x => x.MeasureId).ToList();
            _partMeasures = measureSegments;
            _partProperties = ViewModel.ViewModelLocator.Instance.Main.PartsProperties[partId];
            _stavesCount = _partProperties.NumberOfStaves;
            _systemLayoutInfo = layoutInfo;
            _systemLayoutInfo.PropertyChanged += SystemLayoutInfo_PropertyChanged;
            _staffDistance = _partProperties.StaffLayoutPerPage[_pageIndex].ElementAt(0).StaffDistance;
            CalculateDimensions();
        }

        private void SystemLayoutInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(_systemLayoutInfo.UpdateLayout))
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
                return _partId;
            }

            set
            {
                _partId = value;
            }
        }

        /// <summary>
        /// Part size in Tenths, convert to WPFUnit when using for drawing dimensions
        /// </summary>
        public Size Size
        {
            get
            {
                return _size;
            }

            set
            {
                _size = value;
            }
        }

        internal Canvas PartSegmentCanvas
        {
            get
            {
                return _partSegmentCanvas;
            }

            set
            {
                _partSegmentCanvas = value;
            }
        }

        internal List<MeasureSegmentController> PartMeasures
        {
            get
            {
                return _partMeasures;
            }

            set
            {
                _partMeasures = value;
            }
        }

        #endregion Properties

        #region Methods

        public void GenerateContent()
        {
            foreach (var measureId in _measuresList)
            {
                MeasureDrawing measureCanvas = new MeasureDrawing(measureId, _partId, _staffDistance, _stavesCount);
                ScorePartwisePartMeasureMusicXML measureSerializable = ViewModel.ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.ElementAt(_partId.GetPartIdIndex()).MeasuresByNumber[measureId];

                Canvas.SetTop(measureCanvas.BaseObjectVisual, 0);
                Canvas.SetLeft(measureCanvas.BaseObjectVisual, _partProperties.Coords[measureId].X);
                PartSegmentCanvas.Children.Add(measureCanvas.BaseObjectVisual);


                MeasureSegmentController measureSegment = new MeasureSegmentController(measureSerializable, _partId, _stavesCount);
                _partMeasures.Add(measureSegment);


                Canvas.SetTop(measureSegment.GetMeasureCanvas(), 0);
                Canvas.SetLeft(measureSegment.GetMeasureCanvas(), _partProperties.Coords[measureId].X);
                PartSegmentCanvas.Children.Add(measureSegment.GetMeasureCanvas());
            }
        }
        public void GenerateContent(bool test, LayoutSystemInfo systemLayout)
        {
            if (systemLayout != null)
            {
                _measuresSegments = new List<Canvas>(); //! holds reference for future position update
                //! use system layout info
                foreach (var measureSegment in PartMeasures)
                {
                    //! -------test
                    _measuresSegments.Add(measureSegment.GetMeasureCanvas());
                    //! -------
                    Canvas.SetTop(measureSegment.GetMeasureCanvas(), 0);
                    Canvas.SetLeft(measureSegment.GetMeasureCanvas(), systemLayout.WhicheverPartMeasureCoords(measureSegment.MeasureId, _partId).X);
                    PartSegmentCanvas.Children.Add(measureSegment.GetMeasureCanvas());
                }
            }
        }

        private void UpdateContent()
        {
            for (int i = 0; i < _partMeasures.Count; i++)
            {
                Canvas.SetTop(_measuresSegments[i], 0);
                Canvas.SetLeft(_measuresSegments[i], _systemLayoutInfo.WhicheverPartMeasureCoords(_partMeasures[i].MeasureId, _partId).X);
            }
        }

        private void CalculateDimensions()
        {
            double staffHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.StaffHeight.MMToWPFUnit();
            double segmentHeight = (_stavesCount * staffHeight) + ((_stavesCount - 1) * _staffDistance);
            double segmentWidth = _measuresList.CalculateWidth(_partId);
            _partSegmentCanvas = new Canvas { Width = segmentWidth, Height = segmentHeight };
            _size = new Size(segmentWidth, segmentHeight);
        }

        #endregion Methods
    }
}