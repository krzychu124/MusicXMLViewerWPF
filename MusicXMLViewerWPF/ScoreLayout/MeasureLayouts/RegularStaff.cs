using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MusicXMLScore.Helpers;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts
{
    class RegularStaff : AbstractStaff
    {
        //todo add customizable staff lines (differens line spacing, style, color etc.)
        private readonly Dictionary<int, double> _staffLinesCoords;

        public RegularStaff(int linesCount, double desiredHeight, double desiredWidth) : base(linesCount, desiredHeight, desiredWidth)
        {
            _staffLinesCoords = new Dictionary<int, double>();
        }

        public override IList<double> GetStaffLines()
        {
            return _staffLinesCoords.Values.ToList();
        }

        public override void Update()
        {
            UpdateVisual();
        }

        internal override double GetYOfLine(int index, int staffIndex)
        {
            return _staffLinesCoords[index];
        }

        private void Draw()
        {
            var visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                var pen = new Pen(Brushes.Black, 1);
                foreach (var staffLine in _staffLinesCoords.Values)
                {
                    dc.DrawLine(pen, new System.Windows.Point(0, staffLine), new System.Windows.Point(DesiredWidth, staffLine));
                }
            }
            GetVisualsContainer().AddVisual(visual);
        }

        private void GenerateStaff()
        {
            var tempGap = DesiredHeight / LinesCount;
            _staffLinesCoords.Clear();
            for (int i = 1; i <= LinesCount; i++)
            {
                _staffLinesCoords.Add(i, DesiredHeight -(i * tempGap));
            }
        }

        private void UpdateVisual()
        {
            GetVisualsContainer().ClearVisuals(); //? not tested, idea only....
            GenerateStaff();
            Draw();
        }
    }
}
