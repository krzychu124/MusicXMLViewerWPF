using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MusicXMLScore.Helpers;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts
{
    abstract class AbstractStaff : IStaff, IVisualHost
    {
        private double desiredHeight;
        private double desiredWidth;
        private readonly int linesCount;
        private DrawingVisualHost visualHost;
        protected AbstractStaff(int linesCount, double desiredHeight, double desiredWidth)
        {
            this.linesCount = linesCount;
            this.desiredWidth = desiredWidth;
            this.desiredHeight = desiredHeight;
            visualHost = new DrawingVisualHost();
        }

        public double DesiredHeight { get => desiredHeight; set => desiredHeight = value; }
        public double DesiredWidth { get => desiredWidth; set => desiredWidth = value; }
        public double Heigth { get; protected set; }

        public int LinesCount => linesCount;
        public double TopMargin { get; protected set; }
        public double Width { get; protected set; }
        public double this[int lineIndex, int staffIndex] => GetYOfLine(lineIndex, staffIndex);

        public abstract IList<double> GetStaffLines();
        public DrawingVisualHost GetVisualsContainer()
        {
            return visualHost;
        }

        public double GetVisualWidth()
        {
            return DesiredWidth;
        }
        public abstract void Update();

        /// <summary>
        /// Y position of line. Line indexing from bottom to top
        /// </summary>
        /// <param name="index">Index of line(counting from 1, from bottom to top)</param>
        /// <param name="staffIndex">Staff line of search index line(when multiStaff)</param>
        /// <returns></returns>
        internal abstract double GetYOfLine(int index, int staffIndex = 1);
    }
}
