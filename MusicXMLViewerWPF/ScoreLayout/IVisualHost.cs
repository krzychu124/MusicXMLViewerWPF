using MusicXMLScore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.ScoreLayout
{
    interface IVisualHost
    {
        DrawingVisualHost GetVisualsContainer();
        double GetVisualWidth();
        
    }
}
