using MusicXMLScore.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutControl
{
    class SharedMeasureProperties
    {
        private string measureId;
        private double sharedWidth;
        private Dictionary<string, List<AntiCollisionHelper>> sharedACHelper;
        private Dictionary<int, double> sharedFractionPositions;
        private Tuple<double, double, double> attributesWidths;
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

        public void AddMeasureAttributesWidths(Tuple<double, double, double> attributesWidths)
        {
            this.attributesWidths = attributesWidths;
            double maxClef = attributesWidths.Item1;
            double maxKey = attributesWidths.Item2;
            double maxTime = attributesWidths.Item3;
            sharedFractionPositions.Add(-3, 0);
            sharedFractionPositions.Add(-2, maxClef);
            sharedFractionPositions.Add(-1, maxKey + maxClef);
            if (sharedFractionPositions.ContainsKey(0))
            {
                sharedFractionPositions[0] = maxClef + maxKey + maxTime;
            }
            else
            {
                sharedFractionPositions.Add(0, maxClef + maxKey + maxTime);
            }
        }
        public void GenerateFractionPositions()
        {
            var grouppedFractions = sharedACHelper.SelectMany(x => x.Value).OrderBy(x => x.FactionPosition).GroupBy(x => x.FactionPosition).Select(x => x.ToList()).ToList(); //! not tested
            double minWidth = 10.0.TenthsToWPFUnit();
            if (sharedFractionPositions.ContainsKey(0))
            {
                minWidth += sharedFractionPositions[0];
            }
            foreach (var item in grouppedFractions)
            {
                var min = item.Aggregate((c, d) => c.FractionDuration < d.FractionDuration ? c : d);
                minWidth += min.FractionStretch;
                if (item.Any(x => x.LeftMinWidth != 0))
                {
                    minWidth += item.Max(x => x.LeftMinWidth); //! temp
                }
                if (min.FactionPosition == 0 && sharedFractionPositions.ContainsKey(0))
                {
                    sharedFractionPositions[0] = minWidth - min.FractionStretch;
                }
                else
                {
                    sharedFractionPositions.Add(min.FactionPosition, minWidth - min.FractionStretch);
                }
                minWidth += item.Max(x => x.RightMinWidth); //! temp
            }
            sharedFractionPositions.Add(sharedFractionPositions.Max(x => x.Key) + 1, minWidth);
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
