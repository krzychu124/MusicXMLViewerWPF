using MusicXMLScore.Converters;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.ViewModel;
using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.LayoutControl
{
    class AdvancedMeasureLayout
    {
        private ScorePartwiseMusicXML scoreFile;
        private ObservableCollection<UIElement> pagesCollection;
        private Dictionary<int, PageViewModel> pagesPerNumber;
        private MeasureSegmentContainer measureSegmentsContainer;
        private Dictionary<string, Dictionary<int, double>> fractionPositionsPerMeasure;

        List<SharedMeasureProperties> sharedMeasuresProps = new List<SharedMeasureProperties>();
        Dictionary<int, LayoutPageContentInfo> layoutPageInfo;
        public ObservableCollection<UIElement> PagesCollection
        {
            get
            {
                return pagesCollection;
            }

            set
            {
                pagesCollection = value;
            }
        }

        public AdvancedMeasureLayout(ScorePartwiseMusicXML score)
        {
            this.scoreFile = score;
        }
        public void AddBlankPage()
        {
            if (pagesCollection == null)
            {
                pagesCollection = new ObservableCollection<UIElement>();
            }
            if (pagesPerNumber == null)
            {
                pagesPerNumber = new Dictionary<int, PageViewModel>();
            }
            int index = pagesCollection.Count;
            PageViewModel pvm = new PageViewModel(index);
            pagesPerNumber.Add(index, pvm);
            pagesCollection.Add(new PageView() { DataContext = pvm });
        }
        public void AddPage(Canvas page)
        {
            if (pagesCollection == null)
            {
                pagesCollection = new ObservableCollection<UIElement>();
            }
            if (pagesPerNumber == null)
            {
                pagesPerNumber = new Dictionary<int, PageViewModel>();
            }
            int index = pagesCollection.Count;
            PageViewModel pvm = new PageViewModel(page);
            pagesPerNumber.Add(index, pvm);
            pagesCollection.Add(new PageView() { DataContext = pvm });
        }

        public void GenerateMeasureSegments()
        {
            measureSegmentsContainer = new MeasureSegmentContainer();
            var partIdsList = scoreFile.Part.Select(x => x.Id).ToList();
            measureSegmentsContainer.InitPartIDs(partIdsList);
            //! var test = measureSegmentsContainer["P1"]; //// indexer test
            foreach (var part in scoreFile.Part)
            {
                int stavesCount = part.GetStavesCount();
                foreach (var measure in part.Measure)
                {
                    MeasureSegmentController measureSegmentController = new MeasureSegmentController(measure, part.Id, stavesCount, 0, 0);
                    measureSegmentsContainer.AddMeasureSegmentController(measureSegmentController, part.Id);
                }
            }
        }

        public void FindOptimalMeasureWidths()
        {
            double pageContentWidth = ViewModelLocator.Instance.Main.CurrentPageLayout.GetContentWidth();
            List<MeasureSegmentController> testList = new List<MeasureSegmentController>();
            sharedMeasuresProps = new List<SharedMeasureProperties>();
            
            for (int i = 0; i < scoreFile.Part.FirstOrDefault().Measure.Count; i++)
            {
                //! all measures (from all parts) of current index i
                Dictionary<string, List<AntiCollisionHelper>> measureContentPropertiesList = new Dictionary<string, List<AntiCollisionHelper>>(); 
                List<MeasureSegmentController> measureOfAllParts = measureSegmentsContainer.MeasureSegments.Select(x => x.Value.ElementAt(i)).ToList();
                int shortestDuration = measureOfAllParts.Select(x => x.GetMinDuration()).Min();
                SharedMeasureProperties sharedMeasureProperties = new SharedMeasureProperties(measureOfAllParts.FirstOrDefault().MeasureID);
                //! collect collision helpers from all parts (current measureId only)
                foreach (var measure in measureOfAllParts)
                {
                    List<AntiCollisionHelper> acHelpers = measure.GetContentItemsProperties(shortestDuration);
                    sharedMeasureProperties.AddAntiCollisionHelper(measure.PartId, acHelpers);
                    measureContentPropertiesList.Add(measure.PartId, acHelpers);
                }
                //! calculate measure attributes widths (beginning = clef, key, timeSig)
                Tuple<double, double, double> attributesWidth = LayoutHelpers.GetAttributesWidth(measureOfAllParts);
                sharedMeasureProperties.AddMeasureAttributesWidths(attributesWidth);

                sharedMeasureProperties.GenerateFractionPositions();

                sharedMeasuresProps.Add(sharedMeasureProperties);
                //! Set calculated minimalWidth of current meeasureId in all parts
                double currentMeasureMinWidth = sharedMeasureProperties.SharedWidth;
                measureOfAllParts.ForEach((x) => { x.MinimalWidth = currentMeasureMinWidth; }); 
                //! each part measure => get width => get max => stretch to optimal => set optimal width each part measure
                
            }
            //! update measureWidth with new calculated minimalWidth
            measureSegmentsContainer.UpdateMeasureWidths();
            //! init measure content stretch info of all measures
            GetOptimalStretch();
            ArrangeAndStretchMeasuresContent();
            //! collect pages (calculate and arrange measureSystems into pages)
            PagesCollector();
        }

        private void ArrangeAndStretchMeasuresContent()
        {
            foreach (var item in measureSegmentsContainer.MeasureSegments)
            {
                item.Value.ForEach((x) => { x.ArrangeUsingDurationTable(fractionPositionsPerMeasure[x.MeasureID]); });
                foreach (var val in item.Value)
                {
                    val.ArrangeUsingDurationTable(fractionPositionsPerMeasure[val.MeasureID]);
                    if (val.BeamsController != null)
                    {
                        val.BeamsController.Draw(fractionPositionsPerMeasure[val.MeasureID]);
                        if (val.BeamsController.BeamsVisuals != null)
                        {
                            val.AddBeams(val.BeamsController.BeamsVisuals);
                        }
                    }
                }
            }
        }
        
        private PartsSystemDrawing GeneratePartSystem(int systemIndex, int pageIndex, Dictionary<string, List<MeasureSegmentController>> measuresToAdd)
        {
            List<string> partIDs = measureSegmentsContainer.PartIDsList;
            var partProperties = ViewModelLocator.Instance.Main.CurrentPartsProperties;
            return new PartsSystemDrawing(systemIndex, measuresToAdd, partIDs, partProperties, pageIndex);
        }

        public void PagesCollector()
        {
            int currentPageIndex = 0;
            List<Tuple<string, double>> allMeasuresWidths = new List<Tuple<string, double>>();
            foreach (var measure in measureSegmentsContainer.MeasureSegments.FirstOrDefault().Value)
            {
                allMeasuresWidths.Add(Tuple.Create(measure.MeasureID, measure.MinimalWidth));
            }
            List<string> unarrangedMeasures = new List<string>(allMeasuresWidths.Select(x=>x.Item1));

            SystemsCollector(currentPageIndex, allMeasuresWidths, unarrangedMeasures);
        }
        public void SystemsCollector(int pageIndex, List<Tuple<string, double>>allMeasures, List<string> unarrangedMeasures)
        {
            List<Tuple<string, double>> allMeasuresWidths = allMeasures;
            
            layoutPageInfo = new Dictionary<int, LayoutPageContentInfo>();
            LayoutPageContentInfo pageContent = new LayoutPageContentInfo(pageIndex);
            double currentWidth = 0.0;
            double currentHeight = 0.0;
            List<string> arrangedMeasures = new List<string>();
            List<SharedMeasureProperties> measuresToSystem = new List<SharedMeasureProperties>();
            for (int i = 0; i < unarrangedMeasures.Count; i++)
            {
                
                string measureId = unarrangedMeasures[i];
                SharedMeasureProperties currentMeasureProperties = sharedMeasuresProps.Where(x => x.MeasureId == measureId).FirstOrDefault();
                currentWidth += currentMeasureProperties.SharedWidth; //! allMeasuresWidths.Where(x => x.Item1 == measureId).FirstOrDefault().Item2;
                if (currentWidth > pageContent.PageContentWidth)
                {
                    List<SharedMeasureProperties> tempCollection = new List<SharedMeasureProperties>(measuresToSystem);
                    LayoutSystemInfo layoutSystem = new LayoutSystemInfo(tempCollection);
                    var partProperties = ViewModelLocator.Instance.Main.CurrentPartsProperties;
                    var partHeights = partProperties.Select((a) => new { key = a.Key, value = a.Value.PartHeight }).ToDictionary(item => item.key, item => item.value);
                    layoutSystem.AddPartHeights(partHeights);
                    layoutSystem.GeneratePartDistances(pageContent.DefaultStaffDistance);
                    layoutSystem.CalculateSystemDimensions();
                    layoutSystem.GenerateCoords();
                    bool fitsInPage = pageContent.AddSystemDimensionInfo(layoutSystem);
                    if (!fitsInPage)
                    {
                        pageContent.ArrangeSystems();
                        layoutPageInfo.Add(pageIndex, pageContent);
                        pageIndex++;
                        pageContent = new LayoutPageContentInfo(pageIndex);
                        pageContent.AddSystemDimensionInfo(layoutSystem);
                    }
                    measuresToSystem.Clear();
                    currentWidth = 0.0;
                }
                else
                {
                    if (currentMeasureProperties == null)
                    {
                        Log.LoggIt.Log($"Measure Id: {measureId} not found in SharedMeasureProperties");
                    }
                    else
                    {
                        measuresToSystem.Add(currentMeasureProperties);
                    }
                }
               
                if (i == (unarrangedMeasures.Count - 1) && measuresToSystem.Count != 0)
                {
                    List<SharedMeasureProperties> tempCollection = new List<SharedMeasureProperties>(measuresToSystem);
                    LayoutSystemInfo layoutSystem = new LayoutSystemInfo(tempCollection);
                    var partProperties = ViewModelLocator.Instance.Main.CurrentPartsProperties;
                    var partHeights = partProperties.Select((a) => new { key = a.Key, value = a.Value.PartHeight }).ToDictionary(item => item.key, item => item.value);
                    layoutSystem.AddPartHeights(partHeights);
                    layoutSystem.GeneratePartDistances(pageContent.DefaultStaffDistance);
                    layoutSystem.CalculateSystemDimensions();
                    layoutSystem.GenerateCoords();
                    bool fitsInPage = pageContent.AddSystemDimensionInfo(layoutSystem);
                    if (!fitsInPage)
                    {
                        pageContent.ArrangeSystems();
                        layoutPageInfo.Add(pageIndex, pageContent);
                        pageIndex++;
                        pageContent = new LayoutPageContentInfo(pageIndex);
                        pageContent.AddSystemDimensionInfo(layoutSystem);
                        pageContent.ArrangeSystems();
                        layoutPageInfo.Add(pageIndex, pageContent);
                    }
                    else
                    {
                        pageContent.ArrangeSystems();
                        layoutPageInfo.Add(pageIndex, pageContent);
                    }
                }
                if (i == (unarrangedMeasures.Count - 1) && layoutPageInfo.Count != pageIndex + 1)
                {
                    pageContent.ArrangeSystems();
                    layoutPageInfo.Add(pageIndex, pageContent);
                }
            }

            var partIDs = measureSegmentsContainer.PartIDsList;
            var partPropertiesTest = ViewModelLocator.Instance.Main.CurrentPartsProperties;
            var coords = layoutPageInfo.SelectMany(x => x.Value.AllMeasureCoords()).Distinct().ToDictionary(item =>item.Key,item=> item.Value);
            foreach (var p in partIDs)
            {
                partPropertiesTest[p].Coords = coords;
            }
            foreach (var page in layoutPageInfo)
            {
                List<PartsSystemDrawing> partSystemsTest = new List<PartsSystemDrawing>();
                foreach (var system in page.Value.SystemDimensionsInfo)
                {
                    Dictionary<string, List<MeasureSegmentController>> measuresToAdd = new Dictionary<string, List<MeasureSegmentController>>();
                    foreach (var partID in partIDs)
                    {
                        List<MeasureSegmentController> measures = new List<MeasureSegmentController>();
                        var measuresIDs = system.Measures.Select(x=>x.MeasureId);
                        measures = measureSegmentsContainer.MeasureSegments[partID].Where(x => measuresIDs.Contains(x.MeasureID)).ToList();
                        measuresToAdd.Add(partID, measures);
                    }
                    partSystemsTest.Add(GeneratePartSystem(0, page.Key, measuresToAdd));
                }
                PageDrawingSystem pdsTest = new PageDrawingSystem(page.Key);
                pdsTest.AddPartsSytem(partSystemsTest);
                pdsTest.ArrangeSystems(true, page.Value.AllSystemsPositions());
                AddPage(pdsTest.PageCanvas);
            }
        }
        private void GetOptimalStretch()
        {
            fractionPositionsPerMeasure = new Dictionary<string, Dictionary<int, double>>();
            foreach (var sharedInfo in sharedMeasuresProps)
            {
                string curMeasureId = sharedInfo.MeasureId;
                fractionPositionsPerMeasure.Add(curMeasureId, sharedMeasuresProps.Where(x=>x.MeasureId == curMeasureId).FirstOrDefault().SharedFractionPositions);
            }
        }
    }
    
}
