using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.Prototypes
{
    class RowTopMargin :DependencyObject
    {

        // Using a DependencyProperty as the backing store for TopMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopMarginProperty =
            DependencyProperty.Register(nameof(TopMargin), typeof(double), typeof(RowTopMargin), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnPropertyChangedCallBack)));

       

        public double TopMargin
        {
            get { return (double)GetValue(TopMarginProperty); }
            set { SetValue(TopMarginProperty, value); }
        }

        public FrameworkElement Parent { get; internal set; }

        private static void OnPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue != e.NewValue)
            {
                var row = d as RowTopMargin;
                if(row.Parent != null)
                {
                    row.Parent.InvalidateArrange();
                }
            }
        }
    }
}
