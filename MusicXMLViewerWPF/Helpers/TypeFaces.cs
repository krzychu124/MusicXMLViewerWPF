using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.Helpers
{
    class TypeFaces //TODO_LATER refactor to configurable from app-config window
    {
        public static Typeface BravuraTextFont = new Typeface(new FontFamily("Bravura Text"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        public static Typeface BravuraMusicFont = new Typeface(new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Bravura"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        public static Typeface TextFont = new Typeface(new FontFamily("Times New Roman"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

        public static Typeface GetMusicFont()
        {
            return BravuraMusicFont;
        }
        public static Typeface GetTextFont()
        {
            return TextFont;
        }
    }
}
