using System.Windows.Controls;

namespace MusicXMLScore.ViewModel
{
    /// <summary>
    /// Interaction logic for PageSettingsView.xaml
    /// </summary>
    public partial class PageSettingsView : UserControl
    {
        public PageSettingsView()
        {
            InitializeComponent();
            this.DataContext = new PageSettingsViewModel();
        }
    }
}
