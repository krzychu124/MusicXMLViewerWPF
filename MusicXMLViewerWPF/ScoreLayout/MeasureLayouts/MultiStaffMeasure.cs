using MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses;
using System;
using System.Collections.Generic;
using System.Windows;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts
{

    //todo work in progress
    class MultiStaffMeasure : AbstractMeasure
    {
        private IList<AbstractStaff> staffs;

        protected MultiStaffMeasure(string id, string scoreId, IList<AbstractStaff> staffs, double width, AbstractMeasureContent content) 
            : base(id, scoreId, staffs?.Count?? 0, width, content)
        {
            this.staffs = staffs;
        }

        protected MultiStaffMeasure(string id, string scoreId, int staffCount, double width, AbstractMeasureContent content) : base(id, scoreId, staffCount, width, content)
        {
        }

        public override Rect GetBounds()
        {
            var tempHeight=0.0;
            var tempWidth = 0.0;
            if (staffs == null)
            {
                Console.WriteLine($"MultiStaff with id: {Id} staffs field not set properly. Field set to empty staff collection");
                staffs = new List<AbstractStaff>();
            }
            foreach (var staff in staffs)
            {
               if (staff.Width > tempWidth)
                {
                    tempWidth = staff.Width;
                }
                tempHeight += staff.Heigth + staff.TopMargin;
            }
            return new Rect(0,0, tempWidth, tempHeight);
        }
    }
}
