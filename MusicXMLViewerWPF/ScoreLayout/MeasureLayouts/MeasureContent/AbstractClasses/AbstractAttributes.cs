using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLScore.Helpers;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses
{
    abstract class AbstractAttributes : IVisualHost
    {
        private AbstractClef clef;
        private bool isVisible;
        private AbstractKey key;
        private AbstractTime time;

        protected AbstractAttributes(bool isVisible, AbstractClef clef, AbstractKey key, AbstractTime time)
        {
            this.isVisible = isVisible;
            this.clef = clef;
            this.key = key;
            this.time = time;
        }

        public bool IsVisible { get => isVisible; set => isVisible = value; }
        internal AbstractClef Clef { get => clef; set => clef = value; }
        internal AbstractKey Key { get => key; set => key = value; }
        internal AbstractTime Time { get => time; set => time = value; }

        public DrawingVisualHost GetVisualsContainer()
        {
            return clef.GetVisualsContainer();
        }

        public abstract double GetVisualWidth();
        public void Update()
        {
            clef.Update();
            key.Update();
            time.Update();
        }
    }
}
