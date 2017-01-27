using MusicXMLScore.View;
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

        public PartViewModel(MusicXMLViewerWPF.Part part)
        {

        }
        public ObservableCollection<UIElement> MeasuresCollection { get { return measuresCollection; } set { measuresCollection = value; } }

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
