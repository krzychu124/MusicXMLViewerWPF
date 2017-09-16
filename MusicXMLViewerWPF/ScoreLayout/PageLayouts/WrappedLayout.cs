using System;
using System.Collections.Generic;

namespace MusicXMLScore.ScoreLayout.PageLayouts
{
    class WrappedLayout : AbstractLayout
    {
        protected WrappedLayout(IList<AbstractPageElement> pageElements) : base(pageElements)
        {
        }

        public override void DoLayout()
        {
            throw new NotImplementedException();
        }

        public override void UpdateLayout()
        {
            throw new NotImplementedException();
        }
    }
}
