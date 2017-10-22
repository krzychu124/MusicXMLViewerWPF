using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicXMLScore.ScoreLayout.PageLayouts.PageElements
{
    class SimpleTextBox : TextBlock, IPageElementItem
    {

        public SimpleTextBox(string text) : base()
        {
            Text = text;
        }

        public void DrawNumber(int count)
        {
            //todo
        }

        public double GetMinWidth()
        {
            return 150;
        }

        public FrameworkElement GetUIElement()
        {
            return this;
        }

        public double GetWidth()
        {
            return Width;
        }

        public void SetWidth(double width)
        {
            Width = width;
        }
    }
}
