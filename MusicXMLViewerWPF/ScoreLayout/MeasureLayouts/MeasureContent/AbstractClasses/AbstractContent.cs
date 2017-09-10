using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLScore.Helpers;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses
{
    abstract class AbstractContent : IVisualHost
    {
        private readonly AbstractStaff staff;

        private readonly DrawingVisualHost visualHost;
        private double width;

        protected AbstractContent(AbstractStaff staff)
        {
            visualHost = new DrawingVisualHost();
            this.staff = staff;
        }

        public DrawingVisualHost VisualHost => visualHost;

        public double Width { get => width; set => width = value; }

        internal AbstractStaff Staff => staff;

        public DrawingVisualHost GetVisualsContainer()
        {
            return visualHost;
        }

        public double GetVisualWidth()
        {
            return width;
        }

        public abstract void Update();
    }
}
