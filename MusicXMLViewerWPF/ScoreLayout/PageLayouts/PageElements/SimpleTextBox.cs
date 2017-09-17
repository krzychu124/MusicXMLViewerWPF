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

        public UIElement GetUIElement()
        {
            return this;
        }

        public void SetWidth(double width)
        {
            Width = width;
        }
    }
}
