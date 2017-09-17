using MusicXMLScore.ScoreLayout.PageLayouts.PageElements;
using System.Collections.Generic;
using System.Windows.Controls;

namespace MusicXMLScore.ScoreLayout.PageLayouts
{
    abstract class AbstractLayout : IPageLayout
    {
        private readonly IList<AbstractPageElement> pageElements;

        protected AbstractLayout(IList<AbstractPageElement> pageElements)
        {
            this.pageElements = pageElements;
        }

        internal IList<AbstractPageElement> PageElements => pageElements;

        public abstract void DoLayout(Canvas canvas);
        public abstract void UpdateLayout();
    }
}
