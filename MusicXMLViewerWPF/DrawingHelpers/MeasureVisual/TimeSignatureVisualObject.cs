using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLScore.Helpers;
using MusicXMLScore.Model.MeasureItems.Attributes;
using MusicXMLScore.Converters;
using System.Windows.Media;
using System.Windows;

namespace MusicXMLScore.DrawingHelpers.MeasureVisual
{
    class TimeSignatureVisualObject : IDrawableObjectBase
    {
        private CanvasList visualObject;
        private bool invalidated;
        private TimeMusicXML timeSig;
        private string beatSymbol;
        private string beatTimeSymbol;
        private string symbol;
        private bool isSymbol = false;
        private double[] staffLinesY;
        public TimeSignatureVisualObject(TimeMusicXML timeSig, double[] staffLinesY)
        {
            this.timeSig = timeSig;
            this.staffLinesY = staffLinesY;
            isSymbol = timeSig.TimeSymbolSpecified;
            GetTime();
            InitObjectVisual();
        }

        private void InitObjectVisual()
        {
            visualObject = new CanvasList(10, 10);
            if (isSymbol)
            {
                DrawingVisual visual = new DrawingVisual();
                DrawingHelpers.DrawingMethods.DrawCharacterGlyph(visual, new Point(0, staffLinesY[2]), symbol.GetIndexFromGlyph());
                visualObject.AddVisual(visual);
            }
            else
            {
                //beats
                double beatY = staffLinesY[3];
                var numberWidth = DrawingHelpers.DrawingMethods.GetTextWidth(MusicSymbols.TZero, TypeFaces.GetMusicFont());
                char[] beatChars = beatSymbol.ToCharArray();
                double width = numberWidth * (beatChars.Length -1);
                double offset = width / beatChars.Length;
                foreach (var item in beatChars)
                {
                    var shift = offset - width;
                    DrawingVisual visual = new DrawingVisual();
                    DrawingMethods.DrawCharacterGlyph(visual, new Point(shift +30, beatY), item.ToString().GetIndexFromGlyph());
                    visualObject.AddVisual(visual);
                    offset += width;
                }
                //beatTime
                double beatTimeY = staffLinesY[1];
                char[] beatTimeChars = beatTimeSymbol.ToCharArray();
                width = numberWidth * (beatTimeChars.Length - 1);
                offset = width / beatTimeChars.Length;
                foreach (var item in beatTimeChars)
                {
                    var shift = offset - width;
                    DrawingVisual visual = new DrawingVisual();
                    DrawingMethods.DrawCharacterGlyph(visual, new Point(shift + 30, beatTimeY), item.ToString().GetIndexFromGlyph());
                    visualObject.AddVisual(visual);
                    offset += width;
                }
            }
        }

        private void GetTime()
        {
            if (isSymbol)
            {
                symbol = timeSig.TimeSymbol == TimeSymbolMusicXML.cut ? MusicSymbols.CutTime :
                    timeSig.TimeSymbol == TimeSymbolMusicXML.common ? MusicSymbols.CommonTime :
                    "";
            }
            else
            {
                var beatsValue = timeSig.Items[timeSig.ItemsElementName.GetIndexFromObjectArray(TimeChoiceTypeMusicXML.beats)].ToString();
                var beatTimeValue = timeSig.Items[timeSig.ItemsElementName.GetIndexFromObjectArray(TimeChoiceTypeMusicXML.beattype)].ToString();
                beatSymbol = MusicSymbols.GetCustomTimeNumber(beatsValue);
                beatTimeSymbol = MusicSymbols.GetCustomTimeNumber(beatTimeValue);
            }
        }

        public CanvasList BaseObjectVisual
        {
            get
            {
                return visualObject;
            }
        }

        public bool Invalidated
        {
            get
            {
                return invalidated;
            }

            set
            {
                invalidated = value;
            }
        }

        public void InvalidateVisualObject()
        {
            Invalidated = true;
        }
    }
}
