using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
using MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses;
using System.Windows;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent
{
    class MeasureRest : AbstractContent
    {
        private const string restSymbol = MusicSymbols.WholeRest;
        public MeasureRest(double width, AbstractStaff staff) : base(staff)
        {
            Width = width;
        }

        public override void Update()
        {
            Draw();
        }

        //todo if width changed update visual
        private void Draw()
        {
            GetVisualControl().Children.Clear();
            var symbolWidth = DrawingHelpers.DrawingMethods.GetTextWidth(restSymbol, TypeFaces.GetMusicFont());
            var visualHost = new DrawingVisualHost();
            visualHost.AddCharacterGlyph(new Point(Width / 2 - (symbolWidth / 2), Staff[4, 1]), restSymbol);
            GetVisualControl().Children.Add(visualHost);
        }
    }
}
