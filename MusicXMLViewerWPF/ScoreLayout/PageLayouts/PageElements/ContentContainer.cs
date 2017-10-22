using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.ScoreLayout.PageLayouts.PageElements
{
    class ContentContainer : AbstractPageElement
    {
        private List<AbstractRowContainer> rows;
        public ContentContainer(double width, double height) : base(width, height)
        {
            VisualsContainer.Background = Brushes.SkyBlue;
            rows = new List<AbstractRowContainer>();
        }

        public void AddRowContainer(AbstractRowContainer row)
        {
            if(rows.Count == 0) {
            rows.Add(row);
            } else
            {
                //set row from param to current last row as next
                rows.LastOrDefault().NextContainer = row;
                //set previous
                row.PreviousContainer = rows.LastOrDefault();
                rows.Add(row);
            }
            VisualsContainer.Children.Add(row.Canvas);
        }

        public override void Update()
        {
        }

        public override void UpdateDimensions(double width, double height)
        {
            Width = width;
            foreach (var row in rows)
            {
                row.Width = width;//TODO... refactor to event
            }
        }
    }
}
