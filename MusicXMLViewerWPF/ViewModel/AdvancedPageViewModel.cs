using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.ViewModel
{
    class AdvancedPageViewModel : ViewModelBase
    {
        public Thickness Margin { get; set; }
        public ObservableCollection<UIElement> PageContent { get; set; }

        public AdvancedPageViewModel()
        {
            Margin = new Thickness(20, 20, 20, 30);
            PageContent = new ObservableCollection<UIElement>();
            var block = new TextBlock
            {
                Text = "test",
                Width = 50
            };
            DockPanel.SetDock(block, Dock.Bottom);
            var block2 = new TextBlock
            {
                Text = "test2"
            };
            DockPanel.SetDock(block2, Dock.Right);
            DockPanel.SetDock(block2, Dock.Top);
            PageContent.Add(block);
            PageContent.Add(block2);
        }
    }
}
