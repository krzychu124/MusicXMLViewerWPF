using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MusicXMLScore.Helpers;
using MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses;
using System.Windows.Controls;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts
{
    abstract class AbstractMeasure : IVisualHostControl
    {
        private readonly AbstractMeasureContent content;
        private readonly string id;
        private readonly string scoreId;
        private readonly int staffCount;
        //private readonly DrawingVisualHost visualsHost;
        private readonly Canvas visualsHost;
        private double width;

        protected AbstractMeasure(string id, string scoreId, int staffCount, double width, AbstractMeasureContent content)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Id cannot be null or empty");
            }
            if (staffCount < 1)
            {
                throw new ArgumentException("Staff count cannot be lower than 1");
            }
            this.id = id;
            this.staffCount = staffCount;
            this.scoreId = scoreId;
            visualsHost = new Canvas();
            this.width = width;
            this.content = content;
        }

        public string Id => id;

        public int StaffCount => staffCount;

        public double Width { get => width; set => width = value; }

        public abstract Rect GetBounds();

        public Canvas GetVisualControl()
        {
            return visualsHost;
        }

        public double GetVisualWidth()
        {
            return Width;
        }
    }
}
