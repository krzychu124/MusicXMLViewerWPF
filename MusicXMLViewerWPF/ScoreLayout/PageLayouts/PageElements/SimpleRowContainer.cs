using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.ScoreLayout.PageLayouts.PageElements
{
    class SimpleRowContainer : AbstractRowContainer
    {
        public SimpleRowContainer(Rect bounds) : base(bounds)
        {
            X = 30;
        }
    }
}
