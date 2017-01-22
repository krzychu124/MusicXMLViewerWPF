using MusicXMLViewerWPF.ScoreParts.Part.Measures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.Helpers
{
    class CustomPartSystem : Grid
    {
        private Dictionary<string, List<Measure>> partslist;
        event PropertyChangedEventHandler PropertyChanged = delegate { };
        bool ok = false;
        public bool Loaded { get { return ok; } set { if (ok != value) { ok = value; PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Loaded))); } } }
        public CustomPartSystem()
        {
            PropertyChanged += CustomPartSystem_PropertyChanged;
        }

        private void CustomPartSystem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Loaded")
            {
                if (Loaded)
                {
                    AddParts();
                    GenerateRowDefinitions();
                    GetPartSystem();
                }
            }
        }

        private void IFLoaded(object ready)
        {
            Loaded = (bool)ready;
        }

        private void AddParts()
        {
            if (Loaded)
            {//TODO_L Refactor needed
               /*if (MusicXMLViewerWPF.MusicScore.isLoaded) 
                {
                    //throw new NotImplementedException();
                    //var list = MusicXMLViewerWPF.MusicScore.GetParts();
                    //int count = list.ElementAt(0).Value.MeasureSegmentList.Count;
                    //partslist = new Dictionary<string, List<Measure>>();
                    //foreach (var item in list)
                    //{
                    //    string id = item.Key;
                    //    List<Measure> temp = new List<Measure>();
                    //    for (int i = 0; i < 7; i++)
                    //    {
                    //        temp.Add(item.Value.MeasureSegmentList.ElementAt(i));
                    //    }
                    //    partslist.Add(id, temp);
                    //}
                }*/
            }
        }

        private void GenerateRowDefinitions()
        {
            if (partslist != null)
            {
                for (int i = 0; i < partslist.Count; i++)
                {
                    RowDefinition rd = new RowDefinition();
                    rd.MinHeight = 60;
                    rd.Height = new GridLength(60);
                    RowDefinitions.Add(rd);
                    RowDefinition rdSpace = new RowDefinition();
                    rd.Height = new GridLength(40);
                    RowDefinitions.Add(rdSpace);
                }
            }
        }

        private void GetPartSystem()
        {
            if (partslist != null)
            {
                for (int i = 0; i < partslist.Count; i++)
                {
                    StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
                    foreach (var item in partslist.ElementAt(i).Value)
                    {
                        sp.Children.Add(item.DrawableMeasure);
                    }
                    SetRow(sp, i *2);
                    Children.Add(sp);
                }
            }
        }
    }
}
