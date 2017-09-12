using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MusicXMLScore.Helpers;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses
{
    abstract class AbstractAttributes : IVisualHostControl
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

        public Canvas GetVisualControl()
        {
            var canvas = new Canvas();
            canvas.Children.Add(Clef.GetVisualsContainer());
            Canvas.SetLeft(Key.GetVisualsContainer(), Clef.GetVisualWidth());
            canvas.Children.Add(Key.GetVisualsContainer());
            Canvas.SetLeft(Time.GetVisualsContainer(), Clef.GetVisualWidth() + Key.GetVisualWidth());
            canvas.Children.Add(Time.GetVisualsContainer());
            return canvas;

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
