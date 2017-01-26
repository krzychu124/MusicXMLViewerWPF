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
    /// Contains collection of instrument-part groups
    /// </summary>
    class PartsSegmentViewModel //TODO_I collection of parts (parts - contains collection of measures) Each part represent one instrument
    {
        private ObservableCollection<UIElement> partscollection; //? maight be changed to collection of Part-type later
        public ObservableCollection<UIElement> PartsCollection { get { return partscollection; } set { partscollection = value; } }
        public PartsSegmentViewModel()
        { 
            PartsCollection = new ObservableCollection<UIElement>();
            AddPartToCollection(); // temp prototype
        }

        private void AddPartToCollection()
        {
            PartsCollection.Add(new View.PartView());
            View.PartView pw = new View.PartView();
            pw.SetValue(Helpers.CustomPartsSegmentPanel.TopMarginProperty, 60.0);
            PartsCollection.Add(pw);
            PartsCollection.Add(new View.PartView());
            View.PartView pw2 = new View.PartView();
            PartsCollection.Add(pw2);
        }
    }
}
