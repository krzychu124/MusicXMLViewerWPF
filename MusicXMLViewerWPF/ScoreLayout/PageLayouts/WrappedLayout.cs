using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
