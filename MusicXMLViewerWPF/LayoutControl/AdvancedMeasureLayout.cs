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
            double currentPageContentHeight = 0.0;
            SystemsCollector(currentPageIndex);
        }
        public void SystemsCollector(int pageIndex)
        {
            List<Tuple<string, double>> allMeasuresWidths = new List<Tuple<string, double>>();
            foreach (var measure in measureSegmentsContainer.MeasureSegments.FirstOrDefault().Value)
            {
                allMeasuresWidths.Add(Tuple.Create(measure.MeasureID, measure.MinimalWidth));
            }

            double availableSystemWidth = ViewModelLocator.Instance.Main.CurrentPageLayout.GetContentWidth();
            double currentSystemWidth = 0.0;
            List<Tuple<string, double>> tempList = new List<Tuple<string, double>>();
            List<PartsSystemDrawing> partSystems = new List<PartsSystemDrawing>();
            int currentSystemIndex = 0;

            for (int i = 0; i < allMeasuresWidths.Count; i++)
            {
                currentSystemWidth += allMeasuresWidths[i].Item2;
                if (currentSystemWidth > availableSystemWidth)
                {
                    var measuresIDs = tempList.Select(x => x.Item1);
                    Dictionary<string, List<MeasureSegmentController>> measuresToAdd = new Dictionary<string, List<MeasureSegmentController>>();
                    var partIDs = measureSegmentsContainer.PartIDsList;
                    foreach (var id in partIDs)
                    {
                        List<MeasureSegmentController> measuresList = new List<MeasureSegmentController>();
                        measuresList = measureSegmentsContainer.MeasureSegments[id].Where(x => measuresIDs.Contains(x.MeasureID)).ToList();
                        measuresToAdd.Add(id, measuresList);
                    }
                    partSystems.Add(GeneratePartSystem(currentSystemIndex, pageIndex, measuresToAdd));
                    tempList.Clear();
                    currentSystemWidth = allMeasuresWidths[i].Item2;
                    tempList.Add(allMeasuresWidths[i]);
                    currentSystemIndex++;
                }
                else
                {
                    tempList.Add(allMeasuresWidths[i]);
                }
                if (i == allMeasuresWidths.Count -1 && tempList.Count != 0)
                {
                    var measuresIDs = tempList.Select(x => x.Item1);
                    Dictionary<string, List<MeasureSegmentController>> measuresToAdd = new Dictionary<string, List<MeasureSegmentController>>();
                    var partIDs = measureSegmentsContainer.PartIDsList;
                    foreach (var id in partIDs)
                    {
                        List<MeasureSegmentController> measuresList = new List<MeasureSegmentController>();
                        measuresList = measureSegmentsContainer.MeasureSegments[id].Where(x => measuresIDs.Contains(x.MeasureID)).ToList();
                        measuresToAdd.Add(id, measuresList);
                    }
                    partSystems.Add(GeneratePartSystem(currentSystemIndex, pageIndex, measuresToAdd));
                    tempList.Clear();
                }
            }
            PageDrawingSystem pds = new PageDrawingSystem(pageIndex);
            pds.AddPartsSytem(partSystems);
            pds.ArrangeSystems(true);
            AddPage(pds.PageCanvas);
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
    class LayoutDistances
    {
        private double defaultStaffDistance;
        private double defaultSystemDistance;
        private double defaultTopSystemDistance;
        Dictionary<int, double> systemDistances;
        Dictionary<int, double> systemHeights;
        public LayoutDistances()
        {
            SetDefaultDistances();
        }
        public LayoutDistances(double staffDistance, double systemDistance, double topSystemDistance)
        {
            defaultStaffDistance = staffDistance;
            defaultSystemDistance = systemDistance;
            defaultTopSystemDistance = topSystemDistance;
        }

        private void SetDefaultDistances()
        {
            var layout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout;
            defaultSystemDistance = 2.5 * layout.PageProperties.StaffHeight.MMToTenths();
            defaultTopSystemDistance = 3 * layout.PageProperties.StaffHeight.MMToTenths();
            defaultStaffDistance = 1.7 * layout.PageProperties.StaffHeight.MMToTenths();
        }
        public void GenerateDistances(List<string> partIds)
        {

        }
    }

    class LayoutSystemInfo
    {
        double systemHeight;
        double systemWidth;
        //! distances between parts inside this system
        Dictionary<string, double> partStaffDistances;
        //! calculated part heights 
        Dictionary<string, double> partHeights;
        //! collection of measure widths to generate measures row
        Dictionary<string, double> measureSharedWidths;
        public LayoutSystemInfo()
        {

        }
    }

}
