using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses
{
    abstract class AbstractMeasureContent : IVisualContainerCollection
    {
        private AbstractAttributes attributes;
        private AbstractContent content;

        protected AbstractMeasureContent(AbstractAttributes attributes, AbstractContent content)
        {
            this.attributes = attributes;
            this.content = content;
            System.Windows.Controls.Canvas.SetLeft(content.GetVisualsContainer(), attributes.GetVisualWidth());
        }

        public IList<IVisualHost> GetVisualContainers()
        {
            return new List<IVisualHost> { attributes, content };
        }

        public void SetAttributes(AbstractAttributes attributes)
        {
            if (attributes != null)
            {
                this.attributes = attributes;
            }
        }

        public void SetContent(AbstractContent content)
        {
            if (content != null)
            {
                this.content = content;
            }
        }
        public void Update()
        {
            attributes.Update();
            content.Update();
        }
    }
}
