using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.ScoreLayout.PageLayouts.PageElements
{
    class ContentContainer : AbstractPageElement
    {
        protected ContentContainer(double width, double height) : base(width, height)
        {
        }

        public void SetSize(Size size)
        {
            VisualsContainer.Width = size.Width;
            VisualsContainer.Height = size.Height;
        }

        public override void Update()
        {
        }
    }
}
