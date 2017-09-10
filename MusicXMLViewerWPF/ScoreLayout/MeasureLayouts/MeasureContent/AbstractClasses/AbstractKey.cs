using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses
{
    class AbstractKey : IVisual
    {
        public DrawingVisual GetVisual()
        {
            throw new NotImplementedException();
        }

        public double GetVisualWidth()
        {
            return 0;
        }

        public void Update()
        {
            Console.WriteLine("Key Updated");
        }
    }
}
