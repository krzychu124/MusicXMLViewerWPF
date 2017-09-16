using System;
using System.Collections.Generic;

namespace MusicXMLScore.ScoreLayout.PageLayouts
{
    class ContinuousLayout : AbstractLayout
    {
        protected ContinuousLayout(IList<AbstractPageElement> pageElements) : base(pageElements)
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
