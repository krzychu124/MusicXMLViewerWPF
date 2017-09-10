using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.ScoreLayout
{
    interface IScorePage
    {
        UIElement GetContent();
        void UpdateContent();
    }
}
