using MusicXMLScore.ScoreLayout.PageLayouts.PageElements;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace MusicXMLScore.ScoreLayout.PageLayouts
{
    class WrappedLayout : AbstractLayout
    {
        public WrappedLayout(IList<AbstractPageElement> pageElements) : base(pageElements)
        {
        }

        public override void DoLayout(Canvas canvas)
        {
            canvas.Children.Clear();
            foreach(var item in PageElements)
            {
                canvas.Children.Add(item.VisualsContainer);
            }
        }

        public override void UpdateLayout()
        {
            throw new NotImplementedException();
        }
    }
}
