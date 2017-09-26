using MusicXMLScore.ScoreLayout.PageLayouts.PageElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace MusicXMLScore.ScoreLayout.PageLayouts
{
    class WrappedLayout : AbstractLayout
    {

        public WrappedLayout(IList<AbstractPageElement> pageElements) : base(pageElements)
        {
        }

        public override void DoLayout(AbstractScorePage page, Canvas canvas)
        {
           
            
            base.Width = PageElements.Max(element => element.Width);
            base.Height = PageElements.Sum(element => element.Height);
            page.SetDimensions(Width, Height);
            double cursorY = 0;
            canvas.Children.Clear();
            foreach (var item in PageElements)
            {
                item.Y =  cursorY;
                cursorY += item.Height;
                canvas.Children.Add(item.VisualsContainer);
            }
        }

        public override void UpdateLayout()
        {
            foreach (var item in PageElements)
            {
                item.UpdateDimensions(Root.Width, Root.Height);
            }
            double cursorY = 0;

            var canvas = Root.GetContent() as Canvas;
            canvas.Children.Clear();
            foreach (var item in PageElements)
            {
                item.Y = cursorY;
                cursorY += item.Height;
                canvas.Children.Add(item.VisualsContainer);
            }
        }
    }
}
