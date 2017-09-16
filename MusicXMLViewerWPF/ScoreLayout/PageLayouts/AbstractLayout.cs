using System.Collections.Generic;

namespace MusicXMLScore.ScoreLayout.PageLayouts
{
    abstract class AbstractLayout : IPageLayout
    {
        private readonly IList<AbstractPageElement> pageElements;

        protected AbstractLayout(IList<AbstractPageElement> pageElements)
        {
            this.pageElements = pageElements;
        }

        public abstract void DoLayout();
        public abstract void UpdateLayout();
    }
}
