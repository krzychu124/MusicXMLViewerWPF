using MusicXMLScore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicXMLScore.DrawingHelpers
{
    class DrawingMethods
    {
        public static double GetTextWidth(string text, Typeface typeFace)
        {
            GlyphTypeface glyphTypeface;
            if (!typeFace.TryGetGlyphTypeface(out glyphTypeface))
                throw new InvalidOperationException("No glyphtypeface found");
            double size = 40;

            ushort[] glyphIndexes = new ushort[text.Length];
            double[] advanceWidths = new double[text.Length];

            double totalWidth = 0;

            for (int n = 0; n < text.Length; n++)
            {
                ushort glyphIndex = glyphTypeface.CharacterToGlyphMap[text[n]];
                glyphIndexes[n] = glyphIndex;

                double width = glyphTypeface.AdvanceWidths[glyphIndex] * size;
                advanceWidths[n] = width;

                totalWidth += width;
            }
            return totalWidth;
        }
        public static void DrawCharacterGlyph(DrawingVisual visual, Point position, ushort glyphIndex)
        {
            PageProperties pageproperties = (PageProperties)ViewModel.ViewModelLocator.Instance.Main.CurrentPageProperties;
            double test = glyphIndex == 70? pageproperties.TenthToPx(3*pageproperties.StaffSpace) : pageproperties.TenthToPx(1*pageproperties.StaffSpace); //? temp
            //! ^^ measure lines - clef line property * staffspace(gets length from top line to choosen line) eg. clef line 4 == 5-4= 1*staffspace ==> 1staffspace from top
            GlyphTypeface gtf;
            Typeface typeFace = TypeFaces.BravuraMusicFont;
            typeFace.TryGetGlyphTypeface(out gtf);

            Point calculatedPosition = new Point(position.X, 0 + test * PageProperties.PxPerMM());
            double characterSize = 40;//todo refactor to scale

            using (DrawingContext dc = visual.RenderOpen())
            {
                GlyphRun gr = new GlyphRun(gtf,
                    0,       // Bi-directional nesting level
                    false,   // isSideways
                    characterSize,      // pt size
                    new ushort[] { glyphIndex },   // glyphIndices
                    calculatedPosition,          // baselineOrigin
                    new double[] { 0.0},  // advanceWidths
                    null,    // glyphOffsets
                    null,    // characters
                    null,    // deviceFontName
                    null,    // clusterMap
                    null,    // caretStops
                    null);   // xmlLanguage
                dc.DrawGlyphRun(Brushes.Black, gr);
            }
        }

        public static Size GetTextHeight(string text, double size, Typeface typeFace)
        {
            TextBlock txtblck = new TextBlock();
            txtblck.FontFamily = typeFace.FontFamily;
            txtblck.FontStyle = typeFace.Style;
            txtblck.FontWeight = typeFace.Weight;
            txtblck.FontSize = size;
            txtblck.Text = text;
            txtblck.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            txtblck.Arrange(new Rect(txtblck.DesiredSize));
            return new Size(txtblck.ActualWidth, txtblck.ActualHeight);
        }
    }
}
