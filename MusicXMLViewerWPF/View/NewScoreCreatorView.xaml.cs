using System.Windows;

namespace MusicXMLScore.ViewModel
{
    /// <summary>
    /// Interaction logic for NewScoreCreator.xaml
    /// </summary>
    public partial class NewScoreCreatorView : Window
    {
        public NewScoreCreatorView()
        {
            InitializeComponent();
            //NewScoreCreatorViewModel vm = new NewScoreCreatorViewModel();
            this.DataContext = new NewScoreCreatorViewModel();
        }
    }
}
