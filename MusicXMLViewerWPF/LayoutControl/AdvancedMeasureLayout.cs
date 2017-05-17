using MusicXMLScore.Converters;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
using MusicXMLScore.ViewModel;
using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.LayoutControl
{
    class AdvancedMeasureLayout : INotifyPropertyChanged
    {
        private ScorePartwiseMusicXML scoreFile;
        private ObservableCollection<UIElement> pagesCollection;
        private Dictionary<int, PageViewModel> pagesPerNumber;
        private MeasureSegmentContainer measureSegmentsContainer;
        private ObservableDictionary<string, ObservableDictionary<int, double>> fractionPositionsPerMeasure;
        //! ----
        private ObservableDictionary<string, ObservableDictionary<int, FractionHelper>> fractionPositionHelper;
        //! ---
        List<SharedMeasureProperties> sharedMeasuresProps = new List<SharedMeasureProperties>();
        Dictionary<int, LayoutPageContentInfo> layoutPageInfo;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private bool updateMeasures = false;
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

        public ObservableDictionary<string, ObservableDictionary<int, double>> FractionPositionsPerMeasure
        {
            get
            {
                return fractionPositionsPerMeasure;
            }

            private set
            {
                fractionPositionsPerMeasure = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(FractionPositionsPerMeasure)));
            }
        }

        public bool UpdateMeasures
        {
            get
            {
                return updateMeasures;
            }

            set
            {
                updateMeasures = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateMeasures)));
                updateMeasures = false;
            }
        }

        internal ObservableDictionary<string, ObservableDictionary<int, FractionHelper>> FractionPositionHelper
        {
            get
            {
                return fractionPositionHelper;
            }

            set
            {
                fractionPositionHelper = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(FractionPositionHelper)));
            }
        }

        public AdvancedMeasureLayout(ScorePartwiseMusicXML score)
        {
            this.scoreFile = score;
            PropertyChanged += OnPropertyChange;
            fractionPositionsPerMeasure = new ObservableDictionary<string, ObservableDictionary<int, double>>();
        }

        private void OnFractionPositionsCollectionChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
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

        /// <summary>
        /// Arranges measure content (spacing, beams and later- others)
        /// </summary>
        private void ArrangeAndStretchMeasuresContent(string measureNumber = null)
        {
            if (measureNumber != null)
            {
                var allParts = measureSegmentsContainer.PartIDsList;
                var allPartsMeasures = allParts.Select(x => x.Select(z => measureSegmentsContainer.MeasureSegments.Where(c => c.Key == x).Select(c => c.Value.Where(id => id.MeasureID == measureNumber).FirstOrDefault()).FirstOrDefault()).FirstOrDefault());
                if (allPartsMeasures != null)
                {
                    foreach (var item in allPartsMeasures)
                    {
                        item.MinimalWidth = fractionPositionHelper[measureNumber].LastOrDefault().Value.Position;
                        item.ArrangeUsingDurationTable(fractionPositionHelper[measureNumber].ToDictionary(x => x.Key, x => x.Value.Position), true);
                        if (item.BeamsController != null)
                        {
                            item.BeamsController.Draw(fractionPositionHelper[measureNumber].ToDictionary(x => x.Key, x => x.Value.Position));
                            if (item.BeamsController.BeamsVisuals != null)
                            {
                                //item.BeamsController.BeamsVisuals.Clear();
                                item.AddBeams(item.BeamsController.BeamsVisuals);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var measureSegmentItem in measureSegmentsContainer.MeasureSegments)
                {
                    //! arranges measure content (measureSegmentItem as measureSegment of Part)
                    foreach (var val in measureSegmentItem.Value)
                    {
                        //val.ArrangeUsingDurationTable(fractionPositionsPerMeasure[val.MeasureID].ToDictionary(item=>item.Key, item=>item.Value));
                        val.ArrangeUsingDurationTable(fractionPositionHelper[val.MeasureID].ToDictionary(item => item.Key, item => item.Value.Position));
                        //! updates beam of notes if any
                        if (val.BeamsController != null)
                        {
                            //val.BeamsController.Draw(fractionPositionsPerMeasure[val.MeasureID].ToDictionary(item => item.Key, item => item.Value));
                            val.BeamsController.Draw(fractionPositionHelper[val.MeasureID].ToDictionary(item => item.Key, item => item.Value.Position));
                            if (val.BeamsController.BeamsVisuals != null)
                            {
                                val.AddBeams(val.BeamsController.BeamsVisuals);
                            }
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
            int firstPageIndex = 0;
            List<Tuple<string, double>> allMeasuresWidths = new List<Tuple<string, double>>();
            foreach (var measure in measureSegmentsContainer.MeasureSegments.FirstOrDefault().Value)
            {
                allMeasuresWidths.Add(Tuple.Create(measure.MeasureID, measure.MinimalWidth));
            }

            List<string> unarrangedMeasures = new List<string>(allMeasuresWidths.Select(x=>x.Item1));

            SystemsCollector(firstPageIndex, unarrangedMeasures);
        }
        public void SystemsCollector(int pageIndex,  List<string> unarrangedMeasures)
        {

            //! collection of layout information about pages generated from score
            layoutPageInfo = new Dictionary<int, LayoutPageContentInfo>();

            LayoutPageContentInfo pageContent = new LayoutPageContentInfo(pageIndex);
            double currentWidth = 0.0;
            double currentHeight = 0.0; //! TODO horizontal layout spacing calculations

            List<SharedMeasureProperties> measuresToSystem = new List<SharedMeasureProperties>();
            for (int i = 0; i < unarrangedMeasures.Count; i++)
            {
                
                string measureId = unarrangedMeasures[i];
                SharedMeasureProperties currentMeasureProperties = sharedMeasuresProps.Where(x => x.MeasureId == measureId).FirstOrDefault();
                currentWidth += currentMeasureProperties.SharedWidth; 
                //! if calculated width would be to high, collected measures represents one system
                if (currentWidth > pageContent.PageContentWidth)
                {
                    List<SharedMeasureProperties> sharedMeasuresCollection = new List<SharedMeasureProperties>(measuresToSystem);
                    LayoutSystemInfo layoutSystem = new LayoutSystemInfo(sharedMeasuresCollection);

                    bool systemFitsOnPage = pageContent.AddSystemDimensionInfo(layoutSystem);
                    //! if this system won't fit inside page (too low heigh space available - add to new page and continue
                    if (!systemFitsOnPage)
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
                    //! add measure to collection
                    if (currentMeasureProperties != null)
                    {
                        measuresToSystem.Add(currentMeasureProperties);
                    }
                    else
                    {
                        Log.LoggIt.Log($"Measure Id: {measureId} not found in SharedMeasureProperties");
                    }
                }
                //! if any measure left unarranged
                if (i == (unarrangedMeasures.Count - 1) && measuresToSystem.Count != 0)
                {
                    List<SharedMeasureProperties> sharedMeasuresCollection = new List<SharedMeasureProperties>(measuresToSystem);
                    LayoutSystemInfo layoutSystem = new LayoutSystemInfo(sharedMeasuresCollection);

                    bool systemFitsOnPage = pageContent.AddSystemDimensionInfo(layoutSystem);
                    if (!systemFitsOnPage)
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
                //! if it is last measure and page not added to layoutPageInfo
                if (i == (unarrangedMeasures.Count - 1) && layoutPageInfo.Count != pageIndex + 1)
                {
                    pageContent.ArrangeSystems();
                    layoutPageInfo.Add(pageIndex, pageContent);
                }
            }
            //! -------------------------temp meaure coords update, todo refactor------------
            var partIDs = measureSegmentsContainer.PartIDsList;
            var partPropertiesTest = ViewModelLocator.Instance.Main.CurrentPartsProperties;
            var coords = layoutPageInfo.SelectMany(x => x.Value.AllMeasureCoords()).Distinct().ToDictionary(item =>item.Key,item=> item.Value);
            foreach (var p in partIDs)
            {
                partPropertiesTest[p].Coords = coords;
            }
            //!todo --------------------------------------------------------------------------

            //! sets collected pages into PageDrawingSystem which already has arrange (using updated coords dictionary in previous step) --looks awful now... toRefactor later
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
            fractionPositionHelper = new ObservableDictionary<string, ObservableDictionary<int, FractionHelper>>();
            (fractionPositionHelper as INotifyCollectionChanged).CollectionChanged += OnFractionPositionsCollectionChange;
            foreach (var sharedInfo in sharedMeasuresProps)
            {
                string curMeasureId = sharedInfo.MeasureId;
                sharedInfo.FractionPositionsChanged += OnFractionPositionChange;
                //FractionPositionsPerMeasure.Add(curMeasureId, sharedMeasuresProps.Where(x=>x.MeasureId == curMeasureId).FirstOrDefault().SharedFractionPositions);
                FractionPositionHelper.Add(curMeasureId, sharedMeasuresProps.Where(x => x.MeasureId == curMeasureId).FirstOrDefault().SharedFractions);
            }
        }
        private void OnPropertyChange(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(FractionPositionsPerMeasure):
                    break;
                case nameof(UpdateMeasures):
                    break;
                default:
                    break;
            }
        }
        private void OnFractionPositionChange(object sender, EventArgs e)
        {
            var measureProperties = sender as SharedMeasureProperties;
            FractionPositionHelper[measureProperties.MeasureId] = measureProperties.SharedFractions;
            ArrangeAndStretchMeasuresContent(measureProperties.MeasureId);
        }
    }
    
}
