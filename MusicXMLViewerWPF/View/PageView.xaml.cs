﻿using System.Windows.Controls;

namespace MusicXMLScore.ViewModel
{
    /// <summary>
    /// Interaction logic for PageView.xaml
    /// </summary>
    public partial class PageView : UserControl
    {
        public PageView()
        {
            InitializeComponent();
            //this.DataContext = new PageViewModel();
        }

        //private void StaffLineCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    Point ptCurrent = e.GetPosition(gridPage);//cwp
        //    var list = HitSprite(ptCurrent, sender as UIElement);
        //    //var list2 = list as IEnumerable<FrameworkElement>;
        //    string message = ""; 
        //    foreach (var item in list)
        //    {
        //        message += item.GetType() + Environment.NewLine;
        //    }
        //    {
        //        MessageBox.Show("You hit pos: " + ptCurrent.X + " " + ptCurrent.Y + Environment.NewLine + message);
        //    }
        //}
        //private List<DependencyObject> HitSprite(Point p, UIElement myUiElement)
        //{
        //    var result = new List<DependencyObject>();
        //    VisualTreeHelper.HitTest(
        //        myUiElement,
        //        null,
        //        new HitTestResultCallback(
        //            (HitTestResult hit) => {
        //                result.Add(hit.VisualHit);
        //                return HitTestResultBehavior.Continue;
        //            }),
        //        new PointHitTestParameters(p));
        //    return result;
        //}
    }
}
