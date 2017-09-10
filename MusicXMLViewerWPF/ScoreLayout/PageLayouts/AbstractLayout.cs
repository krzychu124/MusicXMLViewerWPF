using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
