using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts
{
    interface IMeasureElement
    {
        Rect GetBounds();
    }
}
