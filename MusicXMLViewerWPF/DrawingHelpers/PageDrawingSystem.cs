using MusicXMLScore.Converters;
using MusicXMLScore.ViewModel;
using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MusicXMLScore.LayoutControl;

namespace MusicXMLScore.DrawingHelpers
{
    class PageDrawingSystem
    {
        private List<string> _firstMeasureIdPerSystem;
        private List<int> _firstMeasureIndexPerSystem;
        private List<List<string>> _measuresIdRangePerSystem;
        private Canvas _pageCanvas;
        private Size _pageDimensions;
        private int _pageIndex;
        private LayoutGeneral _pageLayout;
        private List<string> _partIDsToDraw = new List<string>();

        private Dictionary<string, PartProperties> _partsProperties = new Dictionary<string, PartProperties>();

        //! PartsSystem<MeasuresIDsList<MeasureId>
        private List<PartsSystemDrawing> _partSystemsList;

        private readonly ScorePartwiseMusicXML _score;

        //! --------------------- test -------------------
        private LayoutPageContentInfo _pageContentLayout;

        private List<string> _partIDs;
        private MeasureSegmentContainer _measuresContainer;

        //! --------------------- !test -------------------

        public PageDrawingSystem(ScorePartwiseMusicXML score, int pageIndex)
        {
            _pageIndex = pageIndex;
            _score = score;
            _pageLayout = ViewModelLocator.Instance.Main.CurrentLayout;

            _pageDimensions = _pageLayout.PageProperties.PageDimensions.Dimensions;
            PageCanvas = new Canvas { Width = _pageDimensions.Width, Height = _pageDimensions.Height };
            GenerateAndDraw();
        }

        private void GenerateAndDraw()
        {
            GenerateMeasuresRangePerSystem();
            GetFirstMeasureDistancesPerSystem();
            AddAllPartsToDrawing();
            AddPartsSystem();
            ArrangeSystems();
        }

        public PageDrawingSystem(int pageIndex)
        {
            _pageIndex = pageIndex;
            _pageLayout = ViewModelLocator.Instance.Main.CurrentLayout;

            _pageDimensions = _pageLayout.PageProperties.PageDimensions.Dimensions;
            PageCanvas = new Canvas {Width = _pageDimensions.Width, Height = _pageDimensions.Height};
        }

        public PageDrawingSystem(LayoutPageContentInfo pageLayoutInfo) : this(pageLayoutInfo.PageIndex)
        {
            _pageContentLayout = pageLayoutInfo;
        }

        public Canvas PageCanvas
        {
            get { return _pageCanvas; }

            set { _pageCanvas = value; }
        }

        private void AddAllPartsToDrawing()
        {
            var parts = ViewModelLocator.Instance.Main.CurrentSelectedScore.Part;
            foreach (var part in parts)
            {
                _partIDsToDraw.Add(part.Id);
            }
        }

        private void AddPartsSystem()
        {
            _partSystemsList = new List<PartsSystemDrawing>();
            foreach (var measuresIDs in _measuresIdRangePerSystem)
            {
                int index = _measuresIdRangePerSystem.IndexOf(measuresIDs);
                PartsSystemDrawing partsSystem = new PartsSystemDrawing(index, measuresIDs, _partIDsToDraw, _partsProperties, _pageIndex);
                _partSystemsList.Add(partsSystem);
                if (partsSystem.PartSystemCanvas != null)
                {
                    _pageCanvas.Children.Add(partsSystem.PartSystemCanvas);
                }
            }
        }

        public void AddPartsSytem(List<PartsSystemDrawing> partSystemsCollection)
        {
            _partSystemsList = new List<PartsSystemDrawing>();
            foreach (var partSys in partSystemsCollection)
            {
                _partSystemsList.Add(partSys);
                if (partSys.PartSystemCanvas != null)
                {
                    PageCanvas.Children.Add(partSys.PartSystemCanvas);
                }
            }
        }

        public void ArrangeSystems(bool advancedLayout = false, List<Point> precalculatedCoords = null)
        {
            double systemDistanceToPrevious = 0.0;
            double leftMarginScore = _pageLayout.PageMargins.LeftMargin.TenthsToWPFUnit();
            if (!advancedLayout)
            {
                var firstSystemPartProperties = _partsProperties.ElementAt(0).Value;
                systemDistanceToPrevious += _pageLayout.PageMargins.TopMargin.TenthsToWPFUnit();
                PartProperties currentPartProperties = ViewModelLocator.Instance.Main.PartsProperties.Values.FirstOrDefault();
                foreach (var system in _partSystemsList)
                {
                    double lMargin = 0.0;

                    int systemIndex = _partSystemsList.IndexOf(system);
                    lMargin = leftMarginScore + currentPartProperties.SystemLayoutPerPage[_pageIndex][system.SystemIndex].SystemMargins
                                  .LeftMargin.TenthsToWPFUnit();
                    if (systemIndex == 0)
                    {
                        systemDistanceToPrevious += firstSystemPartProperties.SystemLayoutPerPage[_pageIndex][0].TopSystemDistance
                            .TenthsToWPFUnit();
                    }
                    if (systemIndex != 0)
                    {
                        systemDistanceToPrevious += firstSystemPartProperties.SystemLayoutPerPage[_pageIndex][systemIndex]
                            .SystemDistance.TenthsToWPFUnit();
                    }

                    Canvas.SetTop(system.PartSystemCanvas, systemDistanceToPrevious);
                    Canvas.SetLeft(system.PartSystemCanvas, lMargin);
                    systemDistanceToPrevious += system.Size.Height;
                }
            }
            else
            {
                if (precalculatedCoords != null)
                {
                    if (_partSystemsList.Count > precalculatedCoords.Count)
                    {
                        throw new NotImplementedException();
                    }
                    for (int i = 0; i < _partSystemsList.Count; i++)
                    {
                        Canvas.SetTop(_partSystemsList[i].PartSystemCanvas, precalculatedCoords[i].Y);
                        Canvas.SetLeft(_partSystemsList[i].PartSystemCanvas, precalculatedCoords[i].X + leftMarginScore);
                    }
                }
            }
        }

        private void GenerateMeasuresRangePerSystem()
        {
            foreach (var part in _score.Part)
            {
                PartProperties tempPartProperties = ViewModelLocator.Instance.Main.PartsProperties[part.Id];
                int partIndex = _score.Part.IndexOf(part);
                if (partIndex != 0)
                {
                    _partsProperties.Add(part.Id, tempPartProperties);
                }
                else
                {
                    _partsProperties.Add(part.Id, tempPartProperties);
                }
            }
            _measuresIdRangePerSystem = _partsProperties.ElementAt(0).Value.MeasuresPerSystemPerPage[_pageIndex];
        }

        private void GetFirstMeasureDistancesPerSystem()
        {
            _firstMeasureIdPerSystem = new List<string>();
            _firstMeasureIndexPerSystem = new List<int>();
            foreach (var measure in _measuresIdRangePerSystem)
            {
                _firstMeasureIdPerSystem.Add(measure[0]);
                _firstMeasureIndexPerSystem.Add(measure[0].GetMeasureIdIndex());
            }
        }

        internal void AssignMeasureSegmentContainer(MeasureSegmentContainer measureSegmentsContainer, List<string> partIDsToAdd)
        {
            _measuresContainer = measureSegmentsContainer;

            if (_pageContentLayout != null)
            {
                if (partIDsToAdd != null && partIDsToAdd.Count != 0)
                {
                    _partIDs = partIDsToAdd;
                }
                else
                {
                    Log.LoggIt.Log("List of part ID's is empty! No further actions performed");
                }
            }
            else
            {
                Log.LoggIt.Log("Page layout not set! Page not drawn/arranged", Log.LogType.Warning);
            }
        }

        internal void ArrangeSystemsAdvanced()
        {
            if (_pageContentLayout != null)
            {
                List<PartsSystemDrawing> partSystemsTest = new List<PartsSystemDrawing>();
                foreach (var systemLayout in _pageContentLayout.SystemDimensionsInfo)
                {
                    Dictionary<string, List<MeasureSegmentController>> measuresToAdd = new Dictionary<string, List<MeasureSegmentController>>();
                    foreach (var partId in _partIDs)
                    {
                        List<MeasureSegmentController>
                            measures = new List<MeasureSegmentController>(); //! collection of measures with the same ID/Number form all parts
                        var measuresIDs = systemLayout.Measures.Select(x => x.MeasureId);
                        measures = _measuresContainer.MeasureSegments[partId].Where(x => measuresIDs.Contains(x.MeasureId)).ToList();
                        measuresToAdd.Add(partId, measures);
                    }
                    partSystemsTest.Add(new PartsSystemDrawing(measuresToAdd, _partIDs, systemLayout));
                }
                AddPartsSytem(partSystemsTest);
                ArrangeUsingLayoutInfo();
            }
        }

        public void ArrangeUsingLayoutInfo(LayoutPageContentInfo pageLayout = null)
        {
            if (pageLayout == null)
            {
                if (_pageContentLayout != null)
                {
                    //! do layout
                    List<Point> precalculatedCoords = new List<Point>();
                    for (int i = 0; i < _partSystemsList.Count; i++)
                    {
                        precalculatedCoords.Add(_pageContentLayout.SystemPosition(i));
                    }
                    ArrangeSystems(true, precalculatedCoords);
                }
            }
            else
            {
                _pageContentLayout = pageLayout;
                //! rearrange if necessary
                //! do layout
            }
        }
    }
}