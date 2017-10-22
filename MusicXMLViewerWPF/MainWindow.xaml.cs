using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

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
            //! test // ScorePartwiseMusicXML xml = MusicXMLScore.ViewModel.MainWindowViewModel.TempMethod<ScorePartwiseMusicXML>("C:\\Users\\Krzychu124\\Desktop\\xml_test_samples\\Echigo-Jishi.xml");
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        public void checkdpi()
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = (int)dpiXProperty.GetValue(null, null);
            var dpiY = (int)dpiYProperty.GetValue(null, null);

            IntPtr hdc = GetDC(IntPtr.Zero);
            Console.WriteLine(GetDeviceCaps(hdc, LOGPIXELSX));
            Console.WriteLine(GetDeviceCaps(hdc, LOGPIXELSY));
            Console.WriteLine($"Current dpi: {dpiX}, {dpiY}");
            MessageBox.Show($"Current dpi: {dpiX}, {dpiY}", "DPI", MessageBoxButton.OK);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            checkdpi();
            GC.Collect();
        }

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// Logical pixels inch in X
        /// </summary>
        const int LOGPIXELSX = 88;
        /// <summary>
        /// Logical pixels inch in Y
        /// </summary>
        const int LOGPIXELSY = 90;
    }
}
