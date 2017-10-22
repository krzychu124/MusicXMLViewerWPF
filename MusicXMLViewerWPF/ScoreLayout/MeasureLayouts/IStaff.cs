using System.Collections.Generic;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts
{
    interface IStaff
    {
        IList<double> GetStaffLines();
    }
}