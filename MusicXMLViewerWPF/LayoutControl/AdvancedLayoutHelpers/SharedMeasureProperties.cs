using MusicXMLScore.Converters;
using MusicXMLScore.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutControl
{
    class SharedMeasureProperties :INotifyPropertyChanged
    {
        private string measureId;
        private double sharedWidth;
        private Dictionary<string, List<AntiCollisionHelper>> sharedACHelper;
        private ObservableDictionary<int, double> sharedFractionPositions;
        private ObservableDictionary<int, FractionHelper> sharedFractions;
        private Tuple<double, double, double> attributesWidths;
        public event PropertyChangedEventHandler PropertyChanged = delegate{ };
        public event EventHandler FractionPositionsChanged;
        public double SharedWidth
        {
            get
            {
                return sharedWidth;
            }

            set
            {
                if (value != sharedWidth)
                {
                    sharedWidth = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SharedWidth)));
                }
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SharedWidth):
                    UpdateSharedWidth(SharedWidth);
                    UpdateMeasureObjectWidth();
                    break;
                default:
                    Log.LoggIt.Log($"No action implemented for {e.PropertyName} property");
                    break;
            }
        }
        
        private void UpdateSharedWidth(double newWidth)
        {
            var list = sharedFractions.SkipWhile(x => x.Key < 0);
            double startingPosition = sharedFractions[0].Position;
            double currentWidth = sharedFractions.LastOrDefault().Value.Position;
            double difference = newWidth - currentWidth;
            double dif = difference / (double)list.Count();
            for (int i = 1; i < list.Count(); i++)
            {
                sharedFractions[list.ElementAt(i).Key].Position = sharedFractions[list.ElementAt(i).Key].Position + dif *i;
            }
            sharedFractions[sharedFractions.LastOrDefault().Key].Position = newWidth; //! update last position (used as measure width)
            //! notify about changes
            FractionPositionsChanged?.Invoke(this, EventArgs.Empty);
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

        internal ObservableDictionary<int, FractionHelper> SharedFractions
        {
            get
            {
                return sharedFractions;
            }

            set
            {
                sharedFractions = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SharedFractions)));
            }
        }

        public SharedMeasureProperties(string measureId)
        {
            this.measureId = measureId;
            sharedWidth = 0.0;
            sharedACHelper = new Dictionary<string, List<AntiCollisionHelper>>();
            sharedFractionPositions = new ObservableDictionary<int, double>();
            sharedFractions = new ObservableDictionary<int, FractionHelper>();//! test
            PropertyChanged += OnPropertyChanged;
        }

        private void CalculateWidth()
        {
            sharedWidth = sharedFractions.LastOrDefault().Value.Position;
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

            sharedFractions.Add(-3, new FractionHelper(-3,0));
            sharedFractions.Add(-2, new FractionHelper(-2, maxClef));
            sharedFractions.Add(-1, new FractionHelper(-1, maxKey + maxClef));
            if (sharedFractions.ContainsKey(0))
            {
                sharedFractions[0].Position = maxClef + maxKey + maxTime;
            }                 
            else              
            {                 
                sharedFractions.Add(0, new FractionHelper(0, maxClef + maxKey + maxTime));
            }
        }
        public void GenerateFractionPositions()
        {
            var grouppedFractions = sharedACHelper.SelectMany(x => x.Value).OrderBy(x => x.FactionPosition).GroupBy(x => x.FactionPosition).Select(x => x.ToList()).ToList(); 
            //! left margin of content 
            double minWidth = 10.0.TenthsToWPFUnit();

            if (sharedFractions.ContainsKey(0))
            {
                minWidth += sharedFractions[0].Position;
            }

            foreach (var item in grouppedFractions)
            {
                var min = item.Aggregate((c, d) => c.FractionDuration < d.FractionDuration ? c : d);
                minWidth += min.FractionStretch;
                if (item.Any(x => x.LeftMinWidth != 0))
                {
                    minWidth += item.Max(x => x.LeftMinWidth); //! temporary left margin
                }

                if (min.FactionPosition == 0 && sharedFractions.ContainsKey(0))
                {
                    sharedFractions[0].Position = minWidth - min.FractionStretch;
                }
                else
                {
                    sharedFractions.Add(min.FactionPosition, new FractionHelper(min.FactionPosition, minWidth - min.FractionStretch));
                }
                minWidth += item.Max(x => x.RightMinWidth); //! temporary right margin
            }
            sharedFractions.Add(sharedFractions.Max(x => x.Key) + 1, new FractionHelper(sharedFractions.Max(x => x.Key) + 1, minWidth));
            CalculateWidth();
        }

        private void CalculateFractionStretch(List<AntiCollisionHelper> acHelper)
        {
            double itemMinWidth = 21.0.TenthsToWPFUnit(); //! minimal item width
            foreach (var item in acHelper)
            {
                //! spacing factor never lower than 1, min stretch is equal to itemMinWidth
                item.FractionStretch = item.SpacingFactor * itemMinWidth;
            }
        }
        private void UpdateMeasureObjectWidth()
        {
            var keys = sharedACHelper.Select(x => x.Key);
            var test = keys.Select(x => ViewModel.ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.Where(k => k.Id == x).FirstOrDefault().MeasuresByNumber[MeasureId]);
            foreach (var item in test)
            {
                item.CalculatedWidth = SharedWidth.WPFUnitToTenths();
            }
        }
    }

    [DebuggerDisplay("Fraction: {Fraction} Position: {Position}")]
    class FractionHelper : INotifyPropertyChanged
    {
        private int fraction;
        private double position;

        public int Fraction
        {
            get
            {
                return fraction;
            }

            set
            {
                fraction = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Fraction)));
            }
        }

        public double Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public FractionHelper(int fraction, double position)
        {
            PropertyChanged += OnPropertyChanged;
            this.fraction = fraction;
            this.position = position;
           
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
    }
}
