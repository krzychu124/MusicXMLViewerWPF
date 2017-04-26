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
            double result = 1;
            if (duration < shortestDuration)
            {
                Log.LoggIt.Log($"Given duration {duration} is lower than calculated shortest duration{shortestDuration}", Log.LogType.Exception);
                duration =  shortestDuration;
            }
            result = 1.5 + (alpha * (Math.Log(duration / shortestDuration, 2.0)));
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
    }
}
