using MusicXMLScore.View;
using MusicXMLViewerWPF;
using MusicXMLViewerWPF.ScoreParts.MeasureContent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.ViewModel
{
    /// <summary>
    /// Displays PartMeasures loaded or passed by parameter to ctor.
    /// </summary>
    class PartViewModel
    {
        private ObservableCollection<UIElement> measuresCollection;
        
        public PartViewModel()
        {
            //TODO basic collection of few measuresVM for further tests
            MeasuresCollection = new ObservableCollection<UIElement>();
            AddTestMeasures();
        }

        public PartViewModel(Part part)
        {
            MeasuresCollection = new ObservableCollection<UIElement>();
            Part = part;
            AddMeasures();
        }

        public ObservableCollection<UIElement> MeasuresCollection { get { return measuresCollection; } set { measuresCollection = value; } }

        public Part Part { get; set; }

        private void AddMeasure(Measure measureToAdd)
        {
            MeasureViewModel measure = new MeasureViewModel(measureToAdd);
            MeasuresCollection.Add(new MeasureView() { DataContext = measure });
        }

        private void AddMeasures()
        {
            foreach (var measure in Part.MeasureList)
            {
            AddMeasure(measure);
            }
        }
        private void AddTestMeasures()
        {
            for (int i = 0; i < 5; i++)
            {
                var mv = new MeasureView();
                MeasuresCollection.Add(mv);//temp prototype
            }
        }
    }
}
