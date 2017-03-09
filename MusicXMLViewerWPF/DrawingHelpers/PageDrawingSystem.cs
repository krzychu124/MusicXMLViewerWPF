using MusicXMLScore.Converters;
using MusicXMLScore.Helpers;
using MusicXMLScore.Model.Defaults;
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.Converters;
using MusicXMLScore.ViewModel;
using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace MusicXMLScore.DrawingHelpers
{
    class PageDrawingSystem
    {
        private Canvas pageCanvas;
        LayoutControl.LayoutGeneral pageLayout;
        List<string> partIDsToDraw = new List<string>();
        ScorePartwiseMusicXML score;
        List<string> firstMeasureIdPerSystem;
        List<int> firstMeasureIndexPerSystem;
        Dictionary<string, double> partStafDistances; //! <partId, distance from previous>
        private List<PartSegmentDrawing> listOfSegments;
        private List<string> measureIdList;
        Size pageDimensions;
        List<List<string>> measuresIdRangePerSystem; //! PartsSystem<MeasuresIDsList<MeasureId>
        Dictionary<string, PartProperties> partsProperties = new Dictionary<string, PartProperties>();
        List<PartsSystemDrawing> partSystemsList;
        int pageIndex = 0;
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

        public PageDrawingSystem(ScorePartwiseMusicXML score, int pageIndex)
        {
            this.pageIndex = pageIndex;
            pageLayout = ViewModelLocator.Instance.Main.CurrentTabLayout;
            this.score = score;
            pageDimensions = pageLayout.PageProperties.PageDimensions.Dimensions;
            PageCanvas = new Canvas() { Width = pageDimensions.Width, Height = pageDimensions.Height };
            GenerateMeasuresRangePerSystem();
            GetFirstMeasureDistancesPerSystem();
            AddAllPartsToDrawing();
            AddPartsSystem();
            ArrangeSystems();
        }

        private void GenerateMeasuresRangePerSystem()
        {
            foreach (var part in score.Part)
            {
                PartProperties pp = new PartProperties(score, part.Id); 
                //! TODO Correct margins.... left margin per system 
                //! seems done
                int partIndex = score.Part.IndexOf(part);
                if (partIndex != 0)
                {
                    partsProperties.Add(part.Id, pp);
                }
                else
                {
                    partsProperties.Add(part.Id, pp);
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
                    systemDistanceToPrevious += firstSystemPartProperties.SystemLayoutPerPage.ElementAt(pageIndex).ElementAt(0).TopSystemDistance.TenthsToWPFUnit(); //system.Size.Height.TenthsToWPFUnit(); //! 
                }
                if (systemIndex != 0)
                {
                    systemDistanceToPrevious += system.Size.Height.TenthsToWPFUnit() +  firstSystemPartProperties.SystemLayoutPerPage.ElementAt(pageIndex).ElementAt(systemIndex).SystemDistance.TenthsToWPFUnit();
                }
                Canvas.SetTop(system.PartSystemCanvas, systemDistanceToPrevious);
                Canvas.SetLeft(system.PartSystemCanvas, lmargin);
                
            }
        }

        public void AddPartsIdToDraw(List<string> partsId)
        {
            partIDsToDraw = partsId;
        }
        public void MeasureIdList(List<string> measureId)
        {
            firstMeasureIdPerSystem = measureId;
        }
    }
}
