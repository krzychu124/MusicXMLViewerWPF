using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.ScoreLayout.PageLayouts.PageElements
{
    class FooterContainer : AbstractPageElement
    {

        public FooterContainer(double width, double height) : base(width, height)
        {
        }

        public void AddCopyRights(string text)
        {
            var simpleTextBox = new SimpleTextBox(text)
            {
                TextAlignment = System.Windows.TextAlignment.Center,
                Width = base.Width,
                Tag = "footer",
                Background = System.Windows.Media.Brushes.OliveDrab
            };
            items.Add(simpleTextBox);
            AddVisual(simpleTextBox);
        }
        public void AddCopyRights(IPageElementItem copyritghtsItem)
        {
            items.Add(copyritghtsItem);
            AddVisual(copyritghtsItem.GetUIElement());
        }

        public override void Update()
        {
            Console.WriteLine("FooterContainer updated");
        }

        public override void UpdateDimensions(double width, double height)
        {
            foreach (var item in items)
            {
                item.SetWidth(width);
            }
            Width = width;
        }
    }
}
