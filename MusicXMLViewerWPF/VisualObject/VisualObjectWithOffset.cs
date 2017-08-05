using MusicXMLScore.Converters;

namespace MusicXMLScore.VisualObject
{
    internal class VisualObjectWithOffset : VisualObjectBase
    {
        private double _horizontalOffset;
        private double _verticalOffset;

        public double HorizontalOffset
        {
            get { return _horizontalOffset; }
            set
            {
                Set(ref _horizontalOffset, value);
            }
        }

        public double VerticalOffset
        {
            get { return _verticalOffset; }
            set {
                Set(ref _verticalOffset, value);
            }
        }
    }
}