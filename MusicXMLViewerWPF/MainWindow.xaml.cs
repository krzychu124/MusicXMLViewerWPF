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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicXMLScore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
        public void checkdpi()
        {
            double dpiX =0;
            double dpiY =0;
            PresentationSource presentationsource = PresentationSource.FromVisual(this);

            if (presentationsource != null) // make sure it's connected
            {
                dpiX = 96.0 * presentationsource.CompositionTarget.TransformToDevice.M11;
                dpiY = 96.0 * presentationsource.CompositionTarget.TransformToDevice.M22;
            }
            Console.WriteLine($"Current dpi: {dpiX}, {dpiY}");
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            checkdpi();
        }
    }
}
