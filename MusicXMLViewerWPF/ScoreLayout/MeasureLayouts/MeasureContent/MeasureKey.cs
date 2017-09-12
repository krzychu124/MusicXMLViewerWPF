using MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLScore.Model.MeasureItems.Attributes;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using MusicXMLScore.Converters;
using MusicXMLScore.DrawingHelpers;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent
{
    class MeasureKey : AbstractKey
    {
        private ClefSignMusicXML currentClefSign;
        private int currentClefLine;
        private bool currentIsFlatKey;
        private double visualWidth;
        public MeasureKey(ClefSignMusicXML clefSign, int clefLine, bool isFlat) : base(clefSign, clefLine, isFlat)
        {
            currentClefSign = clefSign;
            currentClefLine = clefLine;
            currentIsFlatKey = isFlat;
        }

        public override double GetVisualWidth()
        {
            return visualWidth;
        }

        public override void Update()
        {
            Draw();
        }

        private void Draw()
        {
            if (staff != null) {
                double separator = 1.5.TenthsToWPFUnit();
                double tempWidth = 0;
                drawingVisualHost.ClearVisuals();
                for (int i = 0; i < keyAccidentals.Length; i++)
                {
                    var symbol = keyAccidentals[i] == AccidentalValueMusicXML.flat ? MusicSymbols.Flat :
                        keyAccidentals[i] == AccidentalValueMusicXML.sharp ? MusicSymbols.Sharp : MusicSymbols.Natural;
                    tempWidth = DrawingMethods.GetTextWidth(symbol, Helpers.TypeFaces.GetMusicFont()) +1;
                    drawingVisualHost.AddCharacterGlyph(new System.Windows.Point(separator, staff.GetYStaffSpace(keyStaffSpaceIndex[i])), symbol);
                    separator += tempWidth;
                }
                visualWidth = separator;
            } else
            {
                visualWidth = 0;
                Console.WriteLine("Staff not set!");
            }
        }

        protected override void ChangeKeyAccidentals(AccidentalValueMusicXML[] newAccidentals, ClefSignMusicXML newClefSign, int newClefLine, bool newIsFlatKey)
        {
            //todo implementation...
        }
    }
}
