using MusicXMLScore.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutControl
{
    static class LayoutHelpers
    {
        /// <summary>
        /// Calculates minimal spacing according to shortest duration
        /// </summary>
        /// <param name="duration">Note/Rest duration</param>
        /// <param name="shortestDuration">Shortest Note/Rest duration inside measure for reference</param>
        /// <param name="alpha"></param>
        /// <returns>Minimal space for current note/rest</returns>
        public static double SpacingValue(double duration, double shortestDuration, double alpha = 0.6)
        {
            if (duration < shortestDuration)
            {
                Log.LoggIt.Log($"Given duration {duration} is lower than calculated shortest duration{shortestDuration}", Log.LogType.Exception);
                duration = shortestDuration;
            }
            var result = 1 + (alpha * (Math.Log(duration / shortestDuration, 2.0)));
            return result;
        }

        /// <summary>
        /// Stretch positions table to fill targetWidth.
        /// </summary>
        /// <param name="targetWidth"></param>
        /// <param name="positions">Measure segment items indexes positions</param>
        /// <param name="positionIndex">All unique indexes inside current measure</param>
        public static void StretchPositionsToWidth(double targetWidth, Dictionary<int, Tuple<double, double>> positions, List<int> positionIndex)
        {
            LayoutStyle.MeasureLayoutStyle attributesLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.MeasureStyle;
            double currentFullWidth = positions.Sum(x => x.Value.Item2);
            double difference = (targetWidth - attributesLayout.AttributesRightOffset.TenthsToWPFUnit()) - currentFullWidth;
            for (int i = 0; i < positionIndex.Count; i++)
            {
                Tuple<double, double> currentPositionPair = positions[positionIndex[i]];
                double currentPosition = currentPositionPair.Item1;
                double correctedSpacing = (currentPositionPair.Item2 / currentFullWidth) * difference;

                if (i == 0)
                {
                    Tuple<double, double> t = Tuple.Create(currentPosition, correctedSpacing + currentPositionPair.Item2);
                    positions[positionIndex[i]] = t;
                }
                else
                {
                    currentPosition = positions[positionIndex[i - 1]].Item1 + positions[positionIndex[i - 1]].Item2;
                    Tuple<double, double> t = Tuple.Create(currentPosition, correctedSpacing + currentPositionPair.Item2);
                    positions[positionIndex[i]] = t;
                }
            }
        }

        /// <summary>
        /// Gets each position duration.
        /// </summary>
        /// <param name="measureDurationValue">Duration of measure</param>
        /// <param name="positionIndexes">All unique position indexes</param>
        /// <returns>Collection of each position duration</returns>
        public static Dictionary<int, int> GetDurationOfPosition(int measureDurationValue, List<int> positionIndexes)
        {
            int measureDuration = measureDurationValue;
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
        /// Gets max width of all beginning attributes from measure segment of all parts
        /// </summary>
        /// <param name="measureSegments">List of measures with the same Number from all parts</param>
        /// <returns>Max width of each attribute(clef,key,time) of all parts</returns>
        public static Tuple<double, double, double> GetAttributesWidth(List<MeasureSegmentController> measureSegments)
        {
            List<Tuple<double, double, double>> attributesWidths = new List<Tuple<double, double, double>>();
            LayoutStyle.MeasureLayoutStyle attributesLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.MeasureStyle;
            foreach (var measureSegment in measureSegments)
            {
                attributesWidths.Add(measureSegment.GetAttributesWidths());
            }
            double maxClef = 0.0;
            double maxKey = 0.0;
            double maxTime = 0.0;
            if (attributesWidths.Count != 0)
            {
                maxClef = attributesWidths.Select(x => x.Item1).Max();
                maxKey = attributesWidths.Select(x => x.Item2).Max();
                maxTime = attributesWidths.Select(x => x.Item3).Max();
            }
            if (!maxClef.Equals4DigitPrecision(0.0))
            {
                maxClef += attributesLayout.ClefLeftOffset.TenthsToWPFUnit() + attributesLayout.ClefRightOffset.TenthsToWPFUnit();
            }
            if (!maxKey.Equals4DigitPrecision(0.0))
            {
                maxKey += attributesLayout.KeySigLeftOffset.TenthsToWPFUnit() + attributesLayout.KeySigRightOffset.TenthsToWPFUnit();
            }
            if (!maxTime.Equals4DigitPrecision(0.0))
            {
                maxTime += attributesLayout.TimeSigLeftOffset.TenthsToWPFUnit() + attributesLayout.TimeSigRightOffset.TenthsToWPFUnit();
            }

            return Tuple.Create(maxClef, maxKey, maxTime);
        }
    }
}