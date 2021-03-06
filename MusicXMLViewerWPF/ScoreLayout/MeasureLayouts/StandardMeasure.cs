﻿using MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses;
using System.Windows;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts
{
    class StandardMeasure : AbstractMeasure
    {
        private double height;
        private readonly AbstractStaff staff;

        public StandardMeasure(string id, string scoreId, AbstractStaff staff, double width, AbstractMeasureContent content) : base(id, scoreId, 1, width, content)
        {
            this.staff = staff;
            width = staff.DesiredWidth;
            height = staff.DesiredHeight;
            GetVisualControl().Children.Add(staff.GetVisualsContainer());
            foreach (var item in content.GetVisualHostContainers())
            {
                GetVisualControl().Children.Add(item.GetVisualControl());
            }
        }

        public override Rect GetBounds()
        {
            return new Rect(0, 0, Width, height);
        }
    }
}
