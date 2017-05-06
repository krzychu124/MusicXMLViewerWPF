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
        private List<Tuple<string, double>> calculatedOptimalWidths;
        private Dictionary<string, Dictionary<string, List<AntiCollisionHelper>>> measureColHelpers;
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
        //! Test
        public void FindOptimalMeasureWidths()
        {
            List<string> partIDs = measureSegmentsContainer.PartIDsList;
            double pageContentWidth = ViewModelLocator.Instance.Main.CurrentPageLayout.GetContentWidth();
            double currentWidth = 0.0;
            List<MeasureSegmentController> testList = new List<MeasureSegmentController>();
            var partProperties = ViewModelLocator.Instance.Main.CurrentPartsProperties;
            Dictionary<string, List<MeasureSegmentController>> testDictionary = new Dictionary<string, List<MeasureSegmentController>>();
            measureColHelpers = new Dictionary<string, Dictionary<string, List<AntiCollisionHelper>>>();
            sharedMeasuresProps = new List<SharedMeasureProperties>();
            foreach (var item in partIDs)
            {
                testDictionary.Add(item, new List<MeasureSegmentController>()); //! temp, dictonary of measures inside one system
            }
            for (int i = 0; i < scoreFile.Part.FirstOrDefault().Measure.Count; i++)
            {
                //! all measures (from all parts) of current index i
                Dictionary<string, List<AntiCollisionHelper>> measureContentPropertiesList = new Dictionary<string, List<AntiCollisionHelper>>(); 
                var measureOfAllParts = measureSegmentsContainer.MeasureSegments.Select(x => x.Value.ElementAt(i)).ToList();
                var testDurations = measureOfAllParts.SelectMany(x => x.GetIndexes()).Distinct().ToList();
                int shortestDuration = measureOfAllParts.Select(x => x.GetMinDuration()).Min();
                SharedMeasureProperties sharedMeasureProperties = new SharedMeasureProperties(measureOfAllParts.FirstOrDefault().MeasureID);
                foreach (var measure in measureOfAllParts)
                {
                    List<AntiCollisionHelper> acHelpers = measure.GetContentItemsProperties(shortestDuration);
                    sharedMeasureProperties.AddAntiCollisionHelper(measure.PartId, acHelpers);
                    measureContentPropertiesList.Add(measure.PartId, acHelpers);
                }
                Tuple<double, double, double> attributesWidth = LayoutHelpers.GetAttributesWidth(measureOfAllParts);
                sharedMeasureProperties.AddMeasureAttributesWidths(attributesWidth);
                sharedMeasureProperties.GenerateFractionPositions();
                sharedMeasuresProps.Add(sharedMeasureProperties);
                double tempMinWidth = sharedMeasureProperties.SharedWidth;
                var measuresMaxWidth = measureOfAllParts.Select(x => x.MinimalContentWidth()).Max();
                string measureId = measureOfAllParts.FirstOrDefault().MeasureID;
                measureColHelpers.Add(measureOfAllParts.FirstOrDefault().MeasureID, new Dictionary<string, List<AntiCollisionHelper>>(measureContentPropertiesList));
                measureOfAllParts.ForEach((x) => { x.MinimalWidth = tempMinWidth /*measuresMaxWidth*/; }); //! temp
                //! each part measure => get width => get max => stretch to optimal => set optimal width each part measure

                if (i < 7)//! test, temp
                {
                    if (currentWidth + measuresMaxWidth < pageContentWidth)
                    {
                        currentWidth += measuresMaxWidth;
                        foreach (var measure in measureOfAllParts)
                        {
                            testDictionary[measure.PartId].Add(measure);
                        }
                    }
                }
            }
            foreach (var item in partIDs)//! temp test, updates measures widths
            {
                ViewModelLocator.Instance.Main.CurrentPartsProperties[item].SetSystemMeasureRanges();
            }
            GetOptimalStretch();
            TestStretch();
            PartsSystemDrawing psd = new PartsSystemDrawing(0, testDictionary, partIDs, partProperties, 0);
            Canvas.SetTop(psd.PartSystemCanvas, 100);
            Canvas.SetLeft(psd.PartSystemCanvas, 30);
            AddPage(psd.PartSystemCanvas);
        }

        private void TestStretch() //! test 
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

        public List<Tuple<string, double>> FillPartSystem(List<Tuple<string, double>> measuresIdWidths, int systemIndex, int currentPageIndex)
        {
            //Adds measures to part system untile cumulated width not exceed max width of current system
            //if any measure left unadded, return new list of unassigned measures for new part system/new page if no space for partSystem

            Dictionary<string, List<MeasureSegmentController>> testDictionary = new Dictionary<string, List<MeasureSegmentController>>();
            AddPage(GeneratePartSystem(systemIndex, currentPageIndex, testDictionary).PartSystemCanvas);
            return new List<Tuple<string, double>>(); //returns empty if no more measures left unasigned 
        }
        
        private PartsSystemDrawing GeneratePartSystem(int systemIndex, int pageIndex, Dictionary<string, List<MeasureSegmentController>> measuresToAdd)
        {
            List<string> partIDs = measureSegmentsContainer.PartIDsList;
            var partProperties = ViewModelLocator.Instance.Main.CurrentPartsProperties;
            return new PartsSystemDrawing(systemIndex, measuresToAdd, partIDs, partProperties, pageIndex);
        }

        public bool TryArrangeSystem()
        {
            return false;
        }

        public void PagesCollector()
        {

        }
        public void SystemsCollector()
        {

        }
        private void GetOptimalStretch()
        {
            fractionPositionsPerMeasure = new Dictionary<string, Dictionary<int, double>>();
            foreach (var perMeasureCollHelpers in measureColHelpers)
            {
                string curMeasureId = perMeasureCollHelpers.Key;
                fractionPositionsPerMeasure.Add(curMeasureId, sharedMeasuresProps.Where(x=>x.MeasureId == curMeasureId).FirstOrDefault().SharedFractionPositions);
            }
        }
    }
}
