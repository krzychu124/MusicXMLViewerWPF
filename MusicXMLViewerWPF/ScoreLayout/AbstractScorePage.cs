using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.ScoreLayout
{
    abstract class AbstractScorePage : IScorePage
    {
        public abstract UIElement GetContent();

        public abstract void UpdateContent();
    }
}
