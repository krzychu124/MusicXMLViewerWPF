using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicXMLScore.ScoreLayout
{
    interface IVisual
    {
        DrawingVisual GetVisual();
        void Update();
        double GetVisualWidth();
    }
}
