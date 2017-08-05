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
        private ScorePartwiseMusicXML _scoreFile;
        private ObservableCollection<UIElement> _pagesCollection;
        private Dictionary<int, PageViewModel> _pagesPerNumber;
        private MeasureSegmentContainer _measureSegmentsContainer;
        private ObservableDictionary<string, ObservableDictionary<int, FractionHelper>> _fractionPositionHelper;
        List<SharedMeasureProperties> _sharedMeasuresProps = new List<SharedMeasureProperties>();
        Dictionary<int, LayoutPageContentInfo> _layoutPageInfo;
        Dictionary<int, PageDrawingSystem> _pages;

        private bool _ignoreLayoutLoadedFromFile; //! ignores layout elements loaded from file (new-page, new-system) //WiP
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ObservableCollection<UIElement> PagesCollection
        {
            get
            {
                return _pagesCollection;
            }

            set
            {
                _pagesCollection = value;
            }
        }

        internal ObservableDictionary<string, ObservableDictionary<int, FractionHelper>> FractionPositionHelper
        {
            get
            {
                return _fractionPositionHelper;
            }

            set
            {
                _fractionPositionHelper = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(FractionPositionHelper)));
            }
        }

        public bool IgnoreLayoutLoadedFromFile
        {
            get
            {
                return _ignoreLayoutLoadedFromFile;
            }

            set
            {
                _ignoreLayoutLoadedFromFile = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IgnoreLayoutLoadedFromFile)));
            }
        }

        public AdvancedMeasureLayout(ScorePartwiseMusicXML score)
        {
            _scoreFile = score;
            GenerateMeasureSegments();
            FindOptimalMeasureWidths();
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
            }
        }

        /// <summary>
        /// Adds canvas (if parameterless, adds blank page) to pages collection
        /// </summary>
        /// <param name="page"></param>
        public void AddPage(Canvas page = null)
        {
            if (_pagesCollection == null)
            {
                _pagesCollection = new ObservableCollection<UIElement>();
            }
            if (_pagesPerNumber == null)
            {
                _pagesPerNumber = new Dictionary<int, PageViewModel>();
            }
            int index = _pagesCollection.Count;
            PageViewModel pvm;
            if (page != null)
            {
                pvm = new PageViewModel(page);
            }
            else
            {
                pvm = new PageViewModel(index);
            }
            _pagesPerNumber.Add(index, pvm);
            _pagesCollection.Add(new PageView { DataContext = pvm });
        }

        /// <summary>
        /// Generate measure segments of all measures from music score
        /// </summary>
        private void GenerateMeasureSegments()
        {
            _measureSegmentsContainer = new MeasureSegmentContainer();
            _measureSegmentsContainer.GenerateMeasureSegments(_scoreFile);
        }

        private void FindOptimalMeasureWidths()
        {
            double pageContentWidth = ViewModelLocator.Instance.Main.CurrentPageLayout.GetContentWidth();
            List<MeasureSegmentController> testList = new List<MeasureSegmentController>();
            _sharedMeasuresProps = new List<SharedMeasureProperties>();

            for (int i = 0; i < _scoreFile.Part.FirstOrDefault().Measure.Count; i++)
            {
                //! all measures (from all parts) of current index i
                Dictionary<string, List<AntiCollisionHelper>> measureContentPropertiesList = new Dictionary<string, List<AntiCollisionHelper>>();
                List<MeasureSegmentController> measureOfAllParts = _measureSegmentsContainer.MeasureSegments.Select(x => x.Value[i]).ToList();
                int shortestDuration = measureOfAllParts.Select(x => x.GetMinDuration()).Min();
                SharedMeasureProperties sharedMeasureProperties = new SharedMeasureProperties(measureOfAllParts.FirstOrDefault().MeasureId);
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

                _sharedMeasuresProps.Add(sharedMeasureProperties);
                //! Set calculated minimalWidth of current meeasureId in all parts
                double currentMeasureMinWidth = sharedMeasureProperties.SharedWidth;
                measureOfAllParts.ForEach(x => x.MinimalWidth = currentMeasureMinWidth);
                //! each part measure => get width => get max => stretch to optimal => set optimal width each part measure

            }
            //! update measureWidth with new calculated minimalWidth
            _measureSegmentsContainer.UpdateMeasureWidths();
            //! init measure content stretch info of all measures
            GetOptimalStretch();

            ArrangeAndStretchMeasuresContent();
            //! collect measures into systems
            SystemsCollector(_measureSegmentsContainer.MeasureSegments.FirstOrDefault().Value.Select(measure => measure.MeasureId).ToList());
            //! collect pages (using layoutPageInfo)
            PagesCollector();
        }

        /// <summary>
        /// Arranges measure content (spacing, beams and later, others) - if parameterless, updates all measures
        /// </summary>
        /// <param name="measureNumber">Updates only measures with selected number</param>
        private void ArrangeAndStretchMeasuresContent(string measureNumber = null)
        {
            if (measureNumber != null) //! update only measures with selected measureNumber
            {
                //! get part Id's
                var allParts = _measureSegmentsContainer.PartIDsList;
                //! select measures with measureNumber from all parts
                var allPartsMeasures = allParts.Select(x => x.Select(z => _measureSegmentsContainer.MeasureSegments.Where(c => c.Key == x).Select(c => c.Value.FirstOrDefault(id => id.MeasureId == measureNumber)).FirstOrDefault()).FirstOrDefault());
                if (allPartsMeasures != null)
                {
                    foreach (var item in allPartsMeasures)
                    {
                        //! set new width of measure which invoke event to update staffline width
                        item.MinimalWidth = _fractionPositionHelper[measureNumber].LastOrDefault().Value.Position;
                        //! rearrange measure content, pass true for content stretch update
                        item.ArrangeUsingDurationTable(_fractionPositionHelper[measureNumber].ToDictionary(x => x.Key, x => x.Value.Position), true);
                        //! if measure has any note beams to draw, calculate,draw and add.
                        if (item.BeamsController != null)
                        {
                            item.BeamsController.Draw(_fractionPositionHelper[measureNumber].ToDictionary(x => x.Key, x => x.Value.Position));
                            if (item.BeamsController.BeamsVisuals != null)
                            {
                                item.AddBeams(item.BeamsController.BeamsVisuals);
                            }
                        }
                    }
                }
            }
            else //! update all measures
            {
                foreach (var measureSegmentItem in _measureSegmentsContainer.MeasureSegments)
                {
                    //! arrange measure content (measureSegmentItem as measureSegment of Part)
                    foreach (var val in measureSegmentItem.Value)
                    {
                        val.ArrangeUsingDurationTable(_fractionPositionHelper[val.MeasureId].ToDictionary(item => item.Key, item => item.Value.Position));
                        //! draw and add note beams if any
                        if (val.BeamsController != null)
                        {
                            val.BeamsController.Draw(_fractionPositionHelper[val.MeasureId].ToDictionary(item => item.Key, item => item.Value.Position));
                            if (val.BeamsController.BeamsVisuals != null)
                            {
                                val.AddBeams(val.BeamsController.BeamsVisuals);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Collect and add pages using calculated layoutPageInfo
        /// </summary>
        private void PagesCollector()
        {
            if (_layoutPageInfo != null && _layoutPageInfo.Count != 0)
            {
                var partIDs = _measureSegmentsContainer.PartIDsList;

                _pages = new Dictionary<int, PageDrawingSystem>();
                foreach (var pageInfo in _layoutPageInfo)
                {
                    PageDrawingSystem page = new PageDrawingSystem(pageInfo.Value);
                    page.AssignMeasureSegmentContainer(_measureSegmentsContainer, partIDs);
                    page.ArrangeSystemsAdvanced();
                    _pages.Add(pageInfo.Key, page);
                    AddPage(page.PageCanvas);
                }
            }
        }

        /// <summary>
        /// Collect measures into systems
        /// </summary>
        /// <param name="unarrangedMeasures"></param>
        public void SystemsCollector(List<string> unarrangedMeasures)
        {
            //! collection of layout information about pages generated from score
            _layoutPageInfo = new Dictionary<int, LayoutPageContentInfo>();
            int pageIndex = 0;
            LayoutPageContentInfo pageContent = new LayoutPageContentInfo(pageIndex);
            double currentWidth = 0.0;
            //double currentHeight = 0.0; //! TODO horizontal layout spacing calculations

            List<SharedMeasureProperties> measuresToSystem = new List<SharedMeasureProperties>();
            for (int i = 0; i < unarrangedMeasures.Count; i++)
            {
                string measureId = unarrangedMeasures[i];
                SharedMeasureProperties currentMeasureProperties = _sharedMeasuresProps.FirstOrDefault(x => x.MeasureId == measureId);
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
                        _layoutPageInfo.Add(pageIndex, pageContent);
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
                if (i == unarrangedMeasures.Count - 1 && measuresToSystem.Count != 0)
                {
                    List<SharedMeasureProperties> sharedMeasuresCollection = new List<SharedMeasureProperties>(measuresToSystem);
                    LayoutSystemInfo layoutSystem = new LayoutSystemInfo(sharedMeasuresCollection);

                    bool systemFitsOnPage = pageContent.AddSystemDimensionInfo(layoutSystem);
                    if (!systemFitsOnPage)
                    {
                        pageContent.ArrangeSystems();
                        _layoutPageInfo.Add(pageIndex, pageContent);
                        pageIndex++;
                        pageContent = new LayoutPageContentInfo(pageIndex);
                        pageContent.AddSystemDimensionInfo(layoutSystem);
                        pageContent.ArrangeSystems();
                        _layoutPageInfo.Add(pageIndex, pageContent);
                    }
                    else
                    {
                        pageContent.ArrangeSystems();
                        _layoutPageInfo.Add(pageIndex, pageContent);
                    }
                }
                //! if it is last measure and page not added to layoutPageInfo
                if (i == unarrangedMeasures.Count - 1 && _layoutPageInfo.Count != pageIndex + 1)
                {
                    pageContent.ArrangeSystems();
                    _layoutPageInfo.Add(pageIndex, pageContent);
                }
            }
        }
        /// <summary>
        /// Inits collection of fraction position table from each SharedMeasureProperties
        /// </summary>
        private void GetOptimalStretch()
        {
            _fractionPositionHelper = new ObservableDictionary<string, ObservableDictionary<int, FractionHelper>>();
            (_fractionPositionHelper as INotifyCollectionChanged).CollectionChanged += OnFractionPositionsCollectionChange;
            foreach (var sharedMeasure in _sharedMeasuresProps)
            {
                string currentMeasureId = sharedMeasure.MeasureId;
                sharedMeasure.FractionPositionsChanged += OnFractionPositionChange; //! listen to shared fractions collection change
                FractionPositionHelper.Add(currentMeasureId, _sharedMeasuresProps.FirstOrDefault(x => x.MeasureId == currentMeasureId).SharedFractions);
            }
        }

        private void OnFractionPositionChange(object sender, EventArgs e)
        {
            var measureProperties = sender as SharedMeasureProperties;
            //! update fraction table
            if (measureProperties != null)
            {
                FractionPositionHelper[measureProperties.MeasureId] = measureProperties.SharedFractions;
                //! update visuals
                ArrangeAndStretchMeasuresContent(measureProperties.MeasureId);
            }
        }
    }
}
