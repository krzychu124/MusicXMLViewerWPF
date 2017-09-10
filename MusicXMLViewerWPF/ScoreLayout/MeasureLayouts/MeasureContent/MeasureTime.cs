using MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent
{
    class MeasureTime : AbstractTime
    {
        public MeasureTime()
        {
        }

        public override double GetVisualWidth()
        {
            return 0;
        }
    }
}
