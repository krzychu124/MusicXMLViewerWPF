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

namespace MusicXMLScore.DrawingHelpers
{
    public class PartsSystemDrawing
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

        private double rightMargin;

        private Size size;
        private int systemIndex;
        private int pageIndex;
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
            GetSetSystemMargins();
            PartsSegmentsDraw();
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

        public void CalculatePositions()
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

        private void ArrangeContent()
        {
            partSystemCanvas.Width = size.Width;
            partSystemCanvas.Height = size.Height;
            LayoutStyle.MeasureLayoutStyle attributesLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.MeasureStyle;
            double staffSpace = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.StaffSpace.MMToWPFUnit();
            foreach (var partSegment in partsSegments.Values)
            {
                string partSegmentId = partSegment.PartId;
                Canvas.SetTop(partSegment.PartSegmentCanvas, partsPositions[partSegmentId].Item3); //TODO_H layout-problem with first part...
                Canvas.SetLeft(partSegment.PartSegmentCanvas, partsPositions[partSegmentId].Item1);
            }
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
            foreach (var m in measuresList)
            {
                List<Tuple<double, double, double>> attributesWidths = new List<Tuple<double, double, double>>();
                Dictionary<int, double> durationTable = new Dictionary<int, double>();
                List<List<int>> indexes = new List<List<int>>();
                double measureWidth = m.Select(x => x.Width).Max();
                foreach (var partMeasure in m)
                {
                    attributesWidths.Add(partMeasure.GetAttributesWidths());
                    indexes.Add(partMeasure.GetIndexes());
                }
                double maxClef = attributesWidths.Select(x => x.Item1).Max();
                if (maxClef != 0)
                {
                    maxClef += attributesLayout.ClefLeftOffset.TenthsToWPFUnit() + attributesLayout.ClefRightOffset.TenthsToWPFUnit();
                }
                double maxKey = attributesWidths.Select(x => x.Item2).Max();
                if (maxKey != 0)
                {
                    maxKey += attributesLayout.KeySigLeftOffset.TenthsToWPFUnit() + attributesLayout.KeySigRightOffset.TenthsToWPFUnit();
                }
                double maxTime = attributesWidths.Select(x => x.Item3).Max();
                if (maxTime != 0)
                {
                    maxTime += attributesLayout.TimeSigLeftOffset.TenthsToWPFUnit() + attributesLayout.TimeSigRightOffset.TenthsToWPFUnit();
                }
                durationTable.Add(-3, 0);
                durationTable.Add(-2, maxClef);
                durationTable.Add(-1, maxKey + maxClef);
                durationTable.Add(0, maxClef + maxKey + maxTime);
                double availableWidth = measureWidth - durationTable[0];
                List<int> possibleIndexes = indexes.SelectMany(x => x).Distinct().ToList();
                possibleIndexes.Sort();
                Dictionary<int, int> durationOfPosition = new Dictionary<int, int>();
                int measureDuration = m.Select(x => x.MaxDuration).Max();
                for (int i = 0; i < possibleIndexes.Count; i++)
                {
                    if (i < possibleIndexes.Count - 1)
                    {
                        durationOfPosition.Add(possibleIndexes[i], possibleIndexes[i + 1] - possibleIndexes[i]);
                    }
                    else
                    {
                        durationOfPosition.Add(possibleIndexes[possibleIndexes.Count - 1], measureDuration - possibleIndexes[possibleIndexes.Count - 1]);
                    }
                }
                int shortestDuration = durationOfPosition.Values.Where(x=>x > 0).Min();
                Dictionary<int, Tuple<double, double>> positions = new Dictionary<int, Tuple<double, double>>();
                double startingPosition = durationTable[0] + attributesLayout.AttributesRightOffset.TenthsToWPFUnit();
                for (int i = 0; i < durationOfPosition.Count; i++)
                {
                    if (i == 0)
                    {
                        int currentDuration = durationOfPosition[possibleIndexes[i]];
                        double previewSpacing = staffSpace * SpacingValue(currentDuration, shortestDuration, 0.6);
                        positions.Add(possibleIndexes[i], Tuple.Create(startingPosition, previewSpacing));
                    }
                    else
                    {
                        int currentDuration = durationOfPosition[possibleIndexes[i]];
                        double previewSpacing = staffSpace * SpacingValue(currentDuration, shortestDuration, 0.6);
                        startingPosition += previewSpacing;
                        positions.Add(possibleIndexes[i], Tuple.Create(startingPosition, previewSpacing));
                    }
                }
                CorrectStretch(availableWidth, positions, possibleIndexes);
                for (int i = 0; i < positions.Count; i++)
                {
                    if (i== 0)
                    {
                        durationTable[0]  = positions[possibleIndexes[i]].Item1;
                    }
                    if (i > 0)
                    {
                        durationTable.Add(possibleIndexes[i], positions[possibleIndexes[i]].Item1);
                    }
                }
                foreach (MeasureSegmentController measure in m)
                {
                    measure.ArrangeUsingDurationTable(durationTable);
                    RedrawBeams(measure, durationTable);
                }
                
            }
            //TODO_WIP arrange MeasureAttributesContainer items; If more than one part, rearange measureattributes placement of each part measure using largest width offset of every type 

        }

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

        private double SpacingValue(double duration, double shortest, double alpha = 0.6)
        {
            double result = 1;
            result = 1 + (alpha * (Math.Log(duration / shortest, 2.0)));
            return result;
        }

        private void CorrectStretch(double maxWidth, Dictionary<int, Tuple<double, double>> positions, List<int> indexes)
        {
            LayoutStyle.MeasureLayoutStyle attributesLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.MeasureStyle;
            double currentFullWidth = positions.Sum(x => x.Value.Item2);
            double difference = (maxWidth - attributesLayout.AttributesRightOffset.TenthsToWPFUnit()) - currentFullWidth;
            for (int i = 0; i < indexes.Count; i++)
            {
                Tuple<double, double> currentTuple = positions[indexes[i]];
                double currentPosition = currentTuple.Item1 ;
                double correctedSpacing = (currentTuple.Item2 / currentFullWidth) * difference;

                if (i == 0)
                {
                    Tuple<double, double> t = Tuple.Create(currentPosition, correctedSpacing + currentTuple.Item2);
                    positions[indexes[i]] = t;
                }
                else
                {
                    currentPosition = positions[indexes[i - 1]].Item1 + positions[indexes[i - 1]].Item2;
                    Tuple<double, double> t = Tuple.Create(currentPosition, correctedSpacing + currentTuple.Item2);
                    positions[indexes[i]] = t;
                }
            }
        }

        private void CalculateSize()
        {
            var lastSegmentPosition = partsPositions.LastOrDefault();
            this.size = new Size(partWidth + lastSegmentPosition.Value.Item1, lastSegmentPosition.Value.Item3);
        }

        private void GetSetSystemMargins() //TODO_WIP do more tests... //
        {
            var currentPartProperties = partsPropertiesList.ElementAt(0).Value;
            leftMargin = 0;
            rightMargin = 0;
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
            ArrangeContent();
        }

        #endregion Methods
    }
}
