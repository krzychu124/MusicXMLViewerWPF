using MusicXMLScore.Converters;
using MusicXMLScore.ViewModel;
using MusicXMLViewerWPF;
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
                PartsSystemDrawing partsSystem = new PartsSystemDrawing(index, measuresIDs, partIDsToDraw, partsProperties);
                partSystemsList.Add(partsSystem);
                pageCanvas.Children.Add(partsSystem.PartSystemCanvas);
            }
        }

        private void ArrangeSystems()
        {
            double systemDistanceToPrevious = 0.0;
            var firstSystemPartProperties = partsProperties.ElementAt(0).Value;
            double lmargin = pageLayout.PageMargins.LeftMargin.TenthsToWPFUnit();
            systemDistanceToPrevious += pageLayout.PageMargins.TopMargin.TenthsToWPFUnit();

            foreach (var system in partSystemsList)
            {
                int systemIndex = partSystemsList.IndexOf(system);
                if (systemIndex == 0)
                {
                    systemDistanceToPrevious += firstSystemPartProperties.SystemLayoutPerPage.ElementAt(pageIndex).ElementAt(0).TopSystemDistance.TenthsToWPFUnit();
                }
                if (systemIndex != 0)
                {
                    systemDistanceToPrevious += firstSystemPartProperties.SystemLayoutPerPage.ElementAt(pageIndex).ElementAt(systemIndex).SystemDistance.TenthsToWPFUnit();
                }
                Canvas.SetTop(system.PartSystemCanvas, systemDistanceToPrevious);
                Canvas.SetLeft(system.PartSystemCanvas, lmargin);
                systemDistanceToPrevious += system.Size.Height;//.TenthsToWPFUnit();
            }
        }

        private void GenerateMeasuresRangePerSystem()
        {
            foreach (var part in score.Part)
            {
                PartProperties tempPartProperties = new PartProperties(score, part.Id); 
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
