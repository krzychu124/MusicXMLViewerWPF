using MusicXMLScore.ScoreLayout.PageLayouts.PageElements;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace MusicXMLScore.ScoreLayout.PageLayouts
{
    class ContinuousLayout : AbstractLayout
    {
        protected ContinuousLayout(IList<AbstractPageElement> pageElements) : base(pageElements)
        {
        }

        public override void DoLayout(AbstractScorePage page, Canvas canvas)
        {
            throw new NotImplementedException();
        }

        public override void UpdateLayout()
        {
            throw new NotImplementedException();
        }
    }
}
