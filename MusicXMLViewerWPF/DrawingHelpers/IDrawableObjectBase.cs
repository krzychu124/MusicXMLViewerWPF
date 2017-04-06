using MusicXMLScore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.DrawingHelpers
{
    interface IDrawableObjectBase
    {
        DrawingVisualHost BaseObjectVisual { get; }
        void InvalidateVisualObject();
    }
}
