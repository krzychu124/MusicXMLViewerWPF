using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
using MusicXMLScore.Model.MeasureItems.Attributes;
using MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent
{
    class MeasureClef : AbstractClef
    {
        private readonly ClefSignMusicXML _clefSign;
        private readonly int _lineOfStaff;
        private readonly string _symbol;
        private readonly DrawingVisualHost _visualsHost;
        private bool isCurtesy;
        private readonly AbstractStaff staff;

        public MeasureClef(ClefSignMusicXML clefSign, int lineOfStaff, int octaveChange, AbstractStaff staff)
        {
            _visualsHost = new DrawingVisualHost();
            _lineOfStaff = lineOfStaff;
            _clefSign = clefSign;
            MusicSymbols.TryGetClefSymbol(clefSign, octaveChange, out _symbol);
            this.staff = staff;
        }

        public MeasureClef(ClefMusicXML clef, AbstractStaff staff)
        {
            var octaveChange = 0;
            if (!String.IsNullOrEmpty(clef.ClefOctaveChange))
            {
                octaveChange = int.Parse(clef.ClefOctaveChange);
            }
            if (MusicSymbols.TryGetClefSymbol(clef.Sign, octaveChange, out string symbol))
            {
                _clefSign = clef.Sign;
                _symbol = symbol;
            }
            else
            {
                throw new ArgumentException("An error occured while parsing clef symol");
            }
            if (int.TryParse(clef.Line, out int line))
            {
                _lineOfStaff = line;
            }

            this.staff = staff;
        }

        public bool IsCurtesy { get => isCurtesy; set => isCurtesy = value; }

        public override DrawingVisualHost GetVisualsContainer()
        {
            return _visualsHost;
        }

        public override double GetVisualWidth()
        {
            return DrawingMethods.GetTextWidth(_symbol, TypeFaces.GetMusicFont(), IsCurtesy);
        }

        public override void Update()
        {
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            _visualsHost.ClearVisuals();
            _visualsHost.AddCharacterGlyph(new System.Windows.Point(0, staff[_lineOfStaff, 1]), _symbol, IsCurtesy);
        }
    }
}
