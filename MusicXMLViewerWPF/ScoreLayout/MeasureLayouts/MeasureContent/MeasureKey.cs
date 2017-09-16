using MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses;
using System;
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
        public MeasureKey(ClefSignMusicXML clefSign, int clefLine, int keyFifths) : base(clefSign, clefLine, keyFifths)
        {
            currentClefSign = clefSign;
            currentClefLine = clefLine;
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
            visualWidth = 0;
            if (staff != null) {
                double separator = 1.5.TenthsToWPFUnit();
                double tempWidth = 0;
                drawingVisualHost.ClearVisuals();
                if (keyAccidentals.Length ==1 && keyAccidentals[0] == AccidentalValueMusicXML.none)
                {
                    //skip drawing...
                    return;
                }
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
                Console.WriteLine("Staff not set!");
            }
        }

        protected override void ChangeKeyAccidentals(AccidentalValueMusicXML[] newAccidentals, ClefSignMusicXML newClefSign, int newClefLine, bool newIsFlatKey)
        {
            //todo implementation...
        }
    }
}
