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
            //! Loop through measures
            double currentWidth = 0.0;
            List<MeasureSegmentController> testList = new List<MeasureSegmentController>();
            calculatedOptimalWidths = new List<Tuple<string, double>>();
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
                SharedMeasureProperties s = new SharedMeasureProperties(measureOfAllParts.FirstOrDefault().MeasureID);
                foreach (var measure in measureOfAllParts)
                {
                    List<AntiCollisionHelper> acHelpers = measure.GetContentItemsProperties(shortestDuration);
                    s.AddAntiCollisionHelper(measure.PartId, acHelpers);
                    measureContentPropertiesList.Add(measure.PartId, acHelpers);
                }
                s.GenerateFractionPositions();
                sharedMeasuresProps.Add(s);
                double tempMinWidth = s.SharedWidth;
                var measuresMaxWidth = measureOfAllParts.Select(x => x.MinimalContentWidth()).Max();
                string measureId = measureOfAllParts.FirstOrDefault().MeasureID;
                calculatedOptimalWidths.Add(Tuple.Create(measureId, measuresMaxWidth));
                measureColHelpers.Add(measureOfAllParts.FirstOrDefault().MeasureID, new Dictionary<string, List<AntiCollisionHelper>>(measureContentPropertiesList));
                measureOfAllParts.ForEach((x) => { x.MinimalWidth = tempMinWidth /*measuresMaxWidth*/; }); //! temp
                //! each part measure => get width => get max => stretch to optimal => set optimal width each part measure

                if (i < 4)//! test, temp
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
            //double tempItemMinWidth = 17.0.TenthsToWPFUnit();
            fractionPositionsPerMeasure = new Dictionary<string, Dictionary<int, double>>();
            foreach (var perMeasureCollHelpers in measureColHelpers)
            {
                //Dictionary<int, double> fractionsPostitions = new Dictionary<int, double>();
                string curMeasureId = perMeasureCollHelpers.Key;
                //foreach (var perPartCollHelpers in perMeasureCollHelpers.Value)
                //{
                //    string curPartId = perPartCollHelpers.Key;
                //    double minimalWidth = 0.0;
                //    foreach (var item in perPartCollHelpers.Value)
                //    {
                //        item.FractionStretch = tempItemMinWidth * item.SpacingFactor;
                //    }
                //    var test = perPartCollHelpers.Value.GroupBy(x => x.FactionPosition).Select(x => x.ToList()).ToList();
                //    foreach (var item in test)
                //    {
                //        //? check for 0 bug
                //        //! Get shortest duration of durations inside current fraction
                //        var min = item.Aggregate((c, d) => c.FractionDuration < d.FractionDuration ? c : d); 
                //        minimalWidth += min.FractionStretch;
                //        if (fractionsPostitions.ContainsKey(min.FactionPosition))
                //        {
                //            continue;
                //        }
                //        fractionsPostitions.Add(min.FactionPosition, minimalWidth - min.FractionStretch);
                //    }
                //    fractionsPostitions.Add(fractionsPostitions.Max(x=>x.Key) + 100, minimalWidth);
                //    minimalWidth = 0.0;
                //    for (int i = 1; i < perPartCollHelpers.Value.Count; i++)
                //    {
                //        AntiCollisionHelper currentCollHelper = perPartCollHelpers.Value[i - 1];
                //        AntiCollisionHelper nextCollHelper = perPartCollHelpers.Value[i];
                //    }
                //}
                fractionPositionsPerMeasure.Add(curMeasureId, sharedMeasuresProps.Where(x=>x.MeasureId == curMeasureId).FirstOrDefault().SharedFractionPositions);

                //fractionPositionsPerMeasure.Add(curMeasureId, fractionsPostitions);
            }
        }
    }

    [DebuggerDisplay("{DebugDisplay, nq}")]
    class AntiCollisionHelper //temporary name
    {
        private int factionPosition;
        private double fractionDuration;
        private double spacingFactor;
        private double elementWidth;
        private double leftMinWidth;
        private double rightMinWidth;
        private double fractionPosX;
        private double fractionStretch;
        public int FactionPosition
        {
            get
            {
                return factionPosition;
            }

            set
            {
                factionPosition = value;
            }
        }

        public double FractionDuration
        {
            get
            {
                return fractionDuration;
            }

            set
            {
                fractionDuration = value;
            }
        }

        public double ElementWidth
        {
            get
            {
                return elementWidth;
            }

            set
            {
                elementWidth = value;
            }
        }

        public double LeftMinWidth
        {
            get
            {
                return leftMinWidth;
            }

            set
            {
                leftMinWidth = value;
            }
        }

        public double RightMinWidth
        {
            get
            {
                return rightMinWidth;
            }

            set
            {
                rightMinWidth = value;
            }
        }

        private string DebugDisplay
        {
            get { return string.Format("{0}, {1:N2}, {2:N2}, {3:N2}, {4:N2} {5:N2} FS:{6:N2}", FactionPosition, FractionDuration, SpacingFactor, ElementWidth, LeftMinWidth, RightMinWidth, FractionStretch); }
        }

        public double SpacingFactor
        {
            get
            {
                return spacingFactor;
            }

            set
            {
                spacingFactor = value;
            }
        }

        public double FractionPosX
        {
            get
            {
                return fractionPosX;
            }

            set
            {
                fractionPosX = value;
            }
        }

        public double FractionStretch
        {
            get
            {
                return fractionStretch;
            }

            set
            {
                fractionStretch = value;
            }
        }

        public AntiCollisionHelper(int fractionPosition, double fractionDuration, double spacingFactor, double elementWidth, double leftMin, double rightMin)
        {
            this.FactionPosition = fractionPosition;
            this.FractionDuration = fractionDuration;
            this.spacingFactor = spacingFactor;
            this.ElementWidth = elementWidth;
            this.LeftMinWidth = leftMin;
            this.RightMinWidth = rightMin;
        }
    }

    class SharedMeasureProperties
    {
        private string measureId;
        private double sharedWidth;
        private Dictionary<string, List<AntiCollisionHelper>> sharedACHelper;
        private Dictionary<int, double> sharedFractionPositions;

        public double SharedWidth
        {
            get
            {
                return sharedWidth;
            }

            set
            {
                sharedWidth = value;
            }
        }

        public string MeasureId
        {
            get
            {
                return measureId;
            }

            set
            {
                measureId = value;
            }
        }

        public Dictionary<int, double> SharedFractionPositions
        {
            get
            {
                return sharedFractionPositions;
            }

            set
            {
                sharedFractionPositions = value;
            }
        }

        public SharedMeasureProperties(string measureId)
        {
            this.measureId = measureId;
            sharedWidth = 0.0;
            sharedACHelper = new Dictionary<string, List<AntiCollisionHelper>>();
            sharedFractionPositions = new Dictionary<int, double>();
        }

        private void CalculateWidth()
        {
            sharedWidth = sharedFractionPositions.LastOrDefault().Value;
        }

        public void AddAntiCollisionHelper(string partId, AntiCollisionHelper acHelper)
        {
            if (sharedACHelper.ContainsKey(partId))
            {
                sharedACHelper[partId].Add(acHelper);
            }
            else
            {
                sharedACHelper.Add(partId, new List<AntiCollisionHelper>() { acHelper });
            }
        }
        public void AddAntiCollisionHelper(string partId, List<AntiCollisionHelper> acHelper)
        {
            CalculateFractionStretch(acHelper);
            if (sharedACHelper.ContainsKey(partId))
            {
                sharedACHelper[partId].AddRange(acHelper);
            }
            else
            {
                sharedACHelper.Add(partId, acHelper);

            }
        }
        public void GenerateFractionPositions()
        {
            var grouppedFractions = sharedACHelper.SelectMany(x=>x.Value).OrderBy(x=>x.FactionPosition).GroupBy(x => x.FactionPosition).Select(x => x.ToList()).ToList(); //! not tested
            double minWidth = 10.0.TenthsToWPFUnit();
            foreach (var item in grouppedFractions)
            {
                var min = item.Aggregate((c, d) => c.FractionDuration < d.FractionDuration ? c : d);
                minWidth += min.FractionStretch;
                if (item.Any(x=>x.LeftMinWidth != 0))
                {
                    minWidth += item.Max(x=>x.LeftMinWidth); //! temp
                }
                sharedFractionPositions.Add(min.FactionPosition, minWidth - min.FractionStretch);
                minWidth += item.Max(x=> x.RightMinWidth); //! temp
            }
            sharedFractionPositions.Add(sharedFractionPositions.Max(x=>x.Key) + 1, minWidth);
            CalculateWidth();
        }

        private void CalculateFractionStretch(List<AntiCollisionHelper> acHelper)
        {
            double tempItemMinWidth = 20.0.TenthsToWPFUnit(); //! temp
            foreach (var item in acHelper)
            {
                item.FractionStretch = item.SpacingFactor * tempItemMinWidth;
            }
        }
    }

}
