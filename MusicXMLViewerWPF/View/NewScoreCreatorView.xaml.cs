using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
