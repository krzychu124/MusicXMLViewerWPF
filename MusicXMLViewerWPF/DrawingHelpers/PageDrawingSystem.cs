using MusicXMLScore.Converters;
using MusicXMLScore.ViewModel;
using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.DrawingHelpers
{
    class PageDrawingSystem
    {
        #region Fields
        private List<string> firstMeasureIdPerSystem;
        private List<int> firstMeasureIndexPerSystem;
        private List<List<string>> measuresIdRangePerSystem;
        private Canvas pageCanvas;
        private Size pageDimensions;
        private int pageIndex = 0;
        private LayoutControl.LayoutGeneral pageLayout;
        private List<string> partIDsToDraw = new List<string>();
        private Dictionary<string, PartProperties> partsProperties = new Dictionary<string, PartProperties>();
        //! PartsSystem<MeasuresIDsList<MeasureId>
        private List<PartsSystemDrawing> partSystemsList;
        private ScorePartwiseMusicXML score;
        #endregion Fields

        #region Constructors
        public PageDrawingSystem(ScorePartwiseMusicXML score, int pageIndex)
        {
            this.pageIndex = pageIndex;
            this.score = score;
            pageLayout = ViewModelLocator.Instance.Main.CurrentLayout;

            pageDimensions = pageLayout.PageProperties.PageDimensions.Dimensions;
            PageCanvas = new Canvas() { Width = pageDimensions.Width, Height = pageDimensions.Height };

            GenerateMeasuresRangePerSystem();
            GetFirstMeasureDistancesPerSystem();
            AddAllPartsToDrawing();
            AddPartsSystem();
            ArrangeSystems();
        }
        public PageDrawingSystem(int pageIndex)
        {
            this.pageIndex = pageIndex;
            pageLayout = ViewModelLocator.Instance.Main.CurrentLayout;

            pageDimensions = pageLayout.PageProperties.PageDimensions.Dimensions;
            PageCanvas = new Canvas() { Width = pageDimensions.Width, Height = pageDimensions.Height };
        }

        #endregion Constructors

        #region Properties

        public Canvas PageCanvas
        {
            get
            {
                return pageCanvas;
            }

            set
            {
                pageCanvas = value;
            }
        }

        #endregion Properties

        #region Methods

        private void AddAllPartsToDrawing()
        {
            var parts = ViewModelLocator.Instance.Main.CurrentSelectedScore.Part;
            foreach (var part in parts)
            {
                partIDsToDraw.Add(part.Id);
            }
        }

        private void AddPartsSystem()
        {
            partSystemsList = new List<PartsSystemDrawing>();
            foreach (var measuresIDs in measuresIdRangePerSystem)
            {
                int index = measuresIdRangePerSystem.IndexOf(measuresIDs);
                PartsSystemDrawing partsSystem = new PartsSystemDrawing(index, measuresIDs, partIDsToDraw, partsProperties, pageIndex);
                partSystemsList.Add(partsSystem);
                if (partsSystem.PartSystemCanvas != null)
                {
                    pageCanvas.Children.Add(partsSystem.PartSystemCanvas);
                }
            }
        }
        public void AddPartsSytem(List<PartsSystemDrawing> partSystemsCollection)
        {
            partSystemsList = new List<PartsSystemDrawing>();
            foreach (var partSys in partSystemsCollection)
            {
                partSystemsList.Add(partSys);
                if (partSys.PartSystemCanvas != null)
                {
                    PageCanvas.Children.Add(partSys.PartSystemCanvas);
                }
            }
        }

        public void ArrangeSystems(bool advancedLayout = false, List<Point> precalculatedCoords = null)
        {
            double systemDistanceToPrevious = 0.0;
            double leftMargin = pageLayout.PageMargins.LeftMargin.TenthsToWPFUnit();
            if (!advancedLayout)
            {
                var firstSystemPartProperties = partsProperties.ElementAt(0).Value;
                systemDistanceToPrevious += pageLayout.PageMargins.TopMargin.TenthsToWPFUnit();
                PartProperties currentPartProperties = ViewModelLocator.Instance.Main.CurrentPartsProperties.Values.FirstOrDefault();
                foreach (var system in partSystemsList)
                {
                    double lMargin = 0.0;

                    int systemIndex = partSystemsList.IndexOf(system);
                    lMargin = leftMargin + currentPartProperties.SystemLayoutPerPage[pageIndex].ElementAt(system.SystemIndex).SystemMargins.LeftMargin.TenthsToWPFUnit();
                    if (systemIndex == 0)
                    {
                        systemDistanceToPrevious += firstSystemPartProperties.SystemLayoutPerPage.ElementAt(pageIndex).ElementAt(0).TopSystemDistance.TenthsToWPFUnit();
                    }
                    if (systemIndex != 0)
                    {
                        systemDistanceToPrevious += firstSystemPartProperties.SystemLayoutPerPage.ElementAt(pageIndex).ElementAt(systemIndex).SystemDistance.TenthsToWPFUnit();
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
                    if( partSystemsList.Count > precalculatedCoords.Count)
                    {
                        throw new NotImplementedException();
                    }
                    for (int i = 0; i < partSystemsList.Count; i++)
                    {
                        Canvas.SetTop(partSystemsList[i].PartSystemCanvas, precalculatedCoords[i].Y);
                        Canvas.SetLeft(partSystemsList[i].PartSystemCanvas, precalculatedCoords[i].X + leftMargin);
                    }
                }
            }
        }

        private void GenerateMeasuresRangePerSystem()
        {
            foreach (var part in score.Part)
            {
                PartProperties tempPartProperties = ViewModelLocator.Instance.Main.CurrentPartsProperties[part.Id];
                int partIndex = score.Part.IndexOf(part);
                if (partIndex != 0)
                {
                    partsProperties.Add(part.Id, tempPartProperties);
                }
                else
                {
                    partsProperties.Add(part.Id, tempPartProperties);
                }
            }
            measuresIdRangePerSystem = partsProperties.ElementAt(0).Value.MeasuresPerSystemPerPage.ElementAt(pageIndex);
        }

        private void GetFirstMeasureDistancesPerSystem()
        {
            firstMeasureIdPerSystem = new List<string>();
            firstMeasureIndexPerSystem = new List<int>();
            foreach (var measure in measuresIdRangePerSystem)
            {
                firstMeasureIdPerSystem.Add(measure.ElementAt(0));
                firstMeasureIndexPerSystem.Add(measure.ElementAt(0).GetMeasureIdIndex());
            }
        }

        #endregion Methods
    }
}
