using System.Windows.Controls;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses
{
    abstract class AbstractContent : IVisualHostControl
    {
        private readonly AbstractStaff staff;

        //private readonly DrawingVisualHost visualHost;
        private readonly Canvas visualHost;
        private double width;

        protected AbstractContent(AbstractStaff staff)
        {
            visualHost = new Canvas();
            this.staff = staff;
        }

        public double Width { get => width; set => width = value; }

        internal AbstractStaff Staff => staff;

        public Canvas GetVisualControl()
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
