using MusicXMLScore.Helpers;
using MusicXMLScore.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MusicXMLScore.LayoutControl;
using MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes;
using MusicXMLScore.LayoutControl.SegmentPanelContainers;
using MusicXMLScore.LayoutStyle;

namespace MusicXMLScore.DrawingHelpers
{
    class PartsSystemDrawing
    {
        #region Fields

        private double leftMargin;
        private List<string> measuresList;
        private List<string> partIDsList;
        private Dictionary<string, Tuple<double, double, double>> partsPositions;
        //! <PartId, <MarginLeft, distance to previous, distance to topLine>>  
        //! First partID in first page system = TopDistance, next StaffDistance
        //! Other elements using StaffDistance
        private Dictionary<string, PartProperties> partsPropertiesList;
        private Dictionary<string, PartSegmentDrawing> partsSegments;
        private Canvas partSystemCanvas;
        private double partWidth;
        
        private Size size;
        private int systemIndex;
        private int pageIndex;
        private MeasureLayoutStyle attributesLayout;
        private LayoutSystemInfo systemLayout;
        #endregion Fields

        #region Constructors

        public PartsSystemDrawing(int systemIndex, List<string> measuresToDraw, List<string> partsIdList, Dictionary<string, PartProperties> partsProperties, int pageIndex)
        {
            this.systemIndex = systemIndex;
            measuresList = measuresToDraw;
            this.partIDsList = partsIdList;
            partWidth = measuresList.CalculateWidth(partIDsList.ElementAt(0));
            this.partsPropertiesList = partsProperties;
            this.pageIndex = pageIndex;
            attributesLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.MeasureStyle;
            GetSetSystemMargins();
            PartsSegmentsDraw();
        }

        public PartsSystemDrawing(Dictionary<string, List<MeasureSegmentController>> measuresToAdd, List<string> partIDs, LayoutSystemInfo layoutInfo)
        {
            attributesLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.MeasureStyle;
            systemLayout = layoutInfo;
            this.partIDsList = partIDs;
            partWidth = measuresToAdd.Where(x => x.Key == measuresToAdd.Keys.FirstOrDefault()).FirstOrDefault().Value.Sum(x => x.MinimalWidthWithAttributes);
            PartsSegmentsDraw(measuresToAdd, layoutInfo);
        }

        public double LeftMargin
        {
            get
            {
                return leftMargin;
            }

            set
            {
                leftMargin = value;
            }
        }

        #endregion Constructors

        #region Properties

        public Canvas PartSystemCanvas
        {
            get
            {
                return partSystemCanvas;
            }

            set
            {
                partSystemCanvas = value;
            }
        }

        public Size Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }

        public int SystemIndex
        {
            get
            {
                return systemIndex;
            }

            set
            {
                systemIndex = value;
            }
        }

        #endregion Properties

        #region Methods

        public void CalculatePositions(bool advanced = false)
        {
            if (advanced)
            {

            }
            else
            {
                partsPositions = new Dictionary<string, Tuple<double, double, double>>();
                double distanceToTop = 0.0;
                double distanceToPrevious = 0.0;
                foreach (var partSegment in partsSegments.Values)
                {
                    string partSegmentID = partSegment.PartId;
                    int partIndex = partSegmentID.GetPartIdIndex();
                    var currentPartProperties = partsPropertiesList[partSegmentID];
                    if (partIndex == 0)
                    {
                        distanceToPrevious = 0;
                        distanceToTop = partSegment.Size.Height;
                        partsPositions.Add(partSegmentID, new Tuple<double, double, double>(leftMargin, distanceToPrevious, distanceToPrevious));
                        //? distanceToPrevious instead of distanceToTop, no top offset while first part in system
                        //? distanceToTop set to partHeight to simulate bottom line of part ;)
                    }
                    else
                    {
                        distanceToPrevious = currentPartProperties.StaffLayout.ElementAt(systemIndex).StaffDistance.TenthsToWPFUnit();
                        distanceToTop += distanceToPrevious;
                        partsPositions.Add(partSegmentID, new Tuple<double, double, double>(leftMargin, distanceToPrevious, distanceToTop));
                        distanceToTop += partSegment.Size.Height;
                    }
                }
                this.size = new Size(partWidth, distanceToTop);
            }
        }

        private void ArrangeMeasureContent(bool advancedLayout) //TODO split on more methods
        {
            SetPartSystemDimensions(size.Width, size.Height);
            SetPartSegmentCanvasPositions(advancedLayout);
            List<List<MeasureSegmentController>> partMeasuresList = GetMeasuresList();

            if (!advancedLayout)
            {
                foreach (var partMeasureSegment in partMeasuresList)
                {
                    Dictionary<int, double> durationTable = new Dictionary<int, double>();
                    List<List<int>> indexes = GetAllMeasureIndexes(partMeasureSegment);
                    double measureWidth = partMeasureSegment.Select(x => x.Width).Max();
                    
                    Tuple<double, double, double> attributesWidth = LayoutHelpers.GetAttributesWidth(partMeasureSegment);
                    double maxClef = attributesWidth.Item1;
                    double maxKey = attributesWidth.Item2;
                    double maxTime = attributesWidth.Item3;
                    durationTable.Add(-3, 0);
                    durationTable.Add(-2, maxClef);
                    durationTable.Add(-1, maxKey + maxClef);
                    durationTable.Add(0, maxClef + maxKey + maxTime);

                    List<int> positionIndexes = indexes.SelectMany(x => x).Distinct().ToList();
                    positionIndexes.Sort();
                    double startingPosition = durationTable[0] + attributesLayout.AttributesRightOffset.TenthsToWPFUnit();
                    Dictionary<int, Tuple<double, double>> positions = GeneratePositionsTable(partMeasureSegment, positionIndexes, startingPosition);

                    double targetWidth = measureWidth - durationTable[0];
                    LayoutHelpers.StretchPositionsToWidth(targetWidth, positions, positionIndexes);

                    AddPositionsToDurationTable(durationTable, positionIndexes, positions);
                    int lastDuration = durationTable.Keys.Max() + 1;
                    //! adds one more which is measure duration (rigth barline position / calculating center position)
                    durationTable.Add(lastDuration, measureWidth);

                    //! Update measure segments content with calculated duration position table
                    foreach (MeasureSegmentController measureSegment in partMeasureSegment)
                    {
                        measureSegment.ArrangeUsingDurationTable(durationTable);
                        RedrawBeams(measureSegment, durationTable); 
                    }
                }
            }
        }

        /// <summary>
        /// Adds position coords(X) of each index to current durationTable.
        /// </summary>
        /// <param name="durationTable"></param>
        /// <param name="positionIndexes"></param>
        /// <param name="positions"></param>
        private static void AddPositionsToDurationTable(Dictionary<int, double> durationTable, List<int> positionIndexes, Dictionary<int, Tuple<double, double>> positions)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                if (i == 0)
                {
                    durationTable[0] = positions[positionIndexes[i]].Item1;
                }
                if (i > 0)
                {
                    durationTable.Add(positionIndexes[i], positions[positionIndexes[i]].Item1);
                }
            }
        }

        private Dictionary<int, Tuple<double, double>> GeneratePositionsTable(List<MeasureSegmentController> partMeasureSegment, List<int> positionIndexes, double startingPosition)
        {
            double staffSpace = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.StaffSpace.MMToWPFUnit();
            Dictionary<int, int> durationOfPosition = GetDurationOfPosition(partMeasureSegment, positionIndexes);
            int shortestDuration = durationOfPosition.Values.Where(x => x > 0).Min();
            Dictionary<int, Tuple<double, double>> positions = new Dictionary<int, Tuple<double, double>>();
            double currentStartPosition = startingPosition;
            for (int i = 0; i < durationOfPosition.Count; i++)
            {
                if (i == 0)
                {
                    int currentDuration = durationOfPosition[positionIndexes[i]];
                    double previewSpacing = staffSpace * LayoutHelpers.SpacingValue(currentDuration, shortestDuration, 0.6);
                    positions.Add(positionIndexes[i], Tuple.Create(currentStartPosition, previewSpacing));
                }
                else 
                {
                    int currentDuration = durationOfPosition[positionIndexes[i]];
                    double previewSpacing = staffSpace * LayoutHelpers.SpacingValue(currentDuration, shortestDuration, 0.6);
                    currentStartPosition += previewSpacing;
                    positions.Add(positionIndexes[i], Tuple.Create(currentStartPosition, previewSpacing));
                }
            }
            return positions;
        }

        /// <summary>
        /// Gets List of all measureSemgent content(notes, rests) indexes from all parts.
        /// </summary>
        /// <param name="partMeasureSegment"></param>
        /// <returns></returns>
        private List<List<int>> GetAllMeasureIndexes(List<MeasureSegmentController> partMeasureSegment)
        {
            List<List<int>> indexes = new List<List<int>>();
            foreach (var measureSegment in partMeasureSegment)
            {
                indexes.Add(measureSegment.GetIndexes());
            }
            return indexes;
        }

        /// <summary>
        /// Sets positions of all partSegments Canvas.
        /// </summary>
        private void SetPartSegmentCanvasPositions(bool advanced = false)
        {
            if (advanced)
            {
                foreach (var partSegment in partsSegments.Values)
                {
                    string partSegmentId = partSegment.PartId;
                    Canvas.SetTop(partSegment.PartSegmentCanvas, systemLayout.PartPositionY(partSegmentId));
                    Canvas.SetLeft(partSegment.PartSegmentCanvas, 0.0); //! todo margin
                }
            }
            else
            {
                foreach (var partSegment in partsSegments.Values)
                {
                    string partSegmentId = partSegment.PartId;
                    Canvas.SetTop(partSegment.PartSegmentCanvas, partsPositions[partSegmentId].Item3); //TODO_H layout-problem with first part...
                    Canvas.SetLeft(partSegment.PartSegmentCanvas, partsPositions[partSegmentId].Item1);
                }
            }
        }

        /// <summary>
        /// Sets Canvas dimensions
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void SetPartSystemDimensions(double width, double height)
        {
            partSystemCanvas.Width = width;
            partSystemCanvas.Height = height;
        }

        /// <summary>
        /// Returns List of all measures from all PartSegment
        /// </summary>
        /// <returns></returns>
        private List<List<MeasureSegmentController>> GetMeasuresList()
        {
            int measuresCount = partsSegments.ElementAt(0).Value.PartMeasures.Count;
            List<List<MeasureSegmentController>> measuresList = new List<List<MeasureSegmentController>>();
            for (int i = 0; i < measuresCount; i++)
            {
                List<MeasureSegmentController> measures = new List<MeasureSegmentController>();
                foreach (var partSegment in partsSegments.Values)
                {
                    measures.Add(partSegment.PartMeasures.ElementAt(i));
                }
                measuresList.Add(measures);
            }
            return measuresList;
        }

        /// <summary>
        /// Gets each position duration.
        /// </summary>
        /// <param name="partMeasureSegment"></param>
        /// <param name="positionIndexes">All unique position indexes</param>
        /// <returns>Collection of each position duration</returns>
        private Dictionary<int,int> GetDurationOfPosition(List<MeasureSegmentController> partMeasureSegment, List<int> positionIndexes)
        {
            int measureDuration = partMeasureSegment.Select(x => x.MaxDuration).Max();
            Dictionary<int, int> durationOfPosition = new Dictionary<int, int>();
            for (int i = 0; i < positionIndexes.Count; i++)
            {
                if (i < positionIndexes.Count - 1)
                {
                    durationOfPosition.Add(positionIndexes[i], positionIndexes[i + 1] - positionIndexes[i]);
                }
                else
                {
                    durationOfPosition.Add(positionIndexes[positionIndexes.Count - 1], measureDuration - positionIndexes[positionIndexes.Count - 1]);
                }
            }
            return durationOfPosition;
        }
        
        /// <summary>
        /// Upades/draws beams between notes if any.
        /// </summary>
        /// <param name="measureSegment">Selected measureSegment</param>
        /// <param name="durationTable">Reference durationTable</param>
        private void RedrawBeams(MeasureSegmentController measureSegment, Dictionary<int, double> durationTable)
        {
            if (measureSegment.BeamsController != null)
            {
                measureSegment.BeamsController.Draw(durationTable);
                if (measureSegment.BeamsController.BeamsVisuals != null)
                {
                    measureSegment.AddBeams(measureSegment.BeamsController.BeamsVisuals);
                }
            }
        }

        /// <summary>
        /// Sets horizontal offset(margin) of current system to 0
        /// </summary>
        private void GetSetSystemMargins() //TODO_WIP do more tests... //
        {
            var currentPartProperties = partsPropertiesList.ElementAt(0).Value;
            leftMargin = 0;
            //rightMargin = 0;
        }

        private void PartsSegmentsDraw()
        {
            partsSegments = new Dictionary<string, PartSegmentDrawing>();
            partSystemCanvas = new Canvas();
            foreach (var partId in partIDsList)
            {
                PartSegmentDrawing partSegment = new PartSegmentDrawing(measuresList, partId, partsPropertiesList[partId], systemIndex, pageIndex);
                partsSegments.Add(partId, partSegment);
                partSegment.GenerateContent();
                PartSystemCanvas.Children.Add(partSegment.PartSegmentCanvas);
            }
            CalculatePositions();
            ArrangeMeasureContent(false);
        }
        
        private void PartsSegmentsDraw(Dictionary<string, List<MeasureSegmentController>> measuresList, LayoutSystemInfo layoutInfo)
        {
            partsSegments = new Dictionary<string, PartSegmentDrawing>();
            partSystemCanvas = new Canvas();
            foreach (var partId in partIDsList)
            {
                PartSegmentDrawing partSegment = new PartSegmentDrawing(measuresList[partId], partId, layoutInfo);
                partsSegments.Add(partId, partSegment);
                partSegment.GenerateContent(true, layoutInfo);
                PartSystemCanvas.Children.Add(partSegment.PartSegmentCanvas);
            }
            ArrangeMeasureContent(true);
        }

        #endregion Methods
    }
}
