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

        protected FooterContainer(double width, double height) : base(width, height)
        {
        }

        public void AddCopyRights(string text)
        {
            var simpleTextBox = new SimpleTextBox(text);
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
    }
}
