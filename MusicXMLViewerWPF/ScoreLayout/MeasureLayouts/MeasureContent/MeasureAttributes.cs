using MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent
{
    class MeasureAttributes : AbstractAttributes
    {
        public MeasureAttributes(bool isVisible, AbstractClef clef, AbstractKey key, AbstractTime time) : base(isVisible, clef, key, time)
        {
        }

        //todo get width including element margins
        public override double GetVisualWidth()
        {
            return Clef.GetVisualWidth() + Key.GetVisualWidth() + Time.GetVisualWidth();
        }
    }
}
