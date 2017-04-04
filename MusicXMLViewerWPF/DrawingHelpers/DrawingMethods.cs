using MusicXMLScore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MusicXMLScore.Converters;

namespace MusicXMLScore.DrawingHelpers
{
    static class DrawingMethods
    {
        public static double GetTextWidth(string text, Typeface typeFace, bool isAdditional = false)
        {
            double sizeFactor = 1.0;
            GlyphTypeface glyphTypeface;
            if (!typeFace.TryGetGlyphTypeface(out glyphTypeface))
                throw new InvalidOperationException("No glyphtypeface found");
            if (isAdditional)
            {
                sizeFactor = 0.8;
            }
            double size = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.StaffHeight.MMToWPFUnit() * sizeFactor;

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
        public static void DrawCharacterGlyph(DrawingVisual visual, Point position, ushort glyphIndex, bool isSmall = false)
        {
            PageProperties pageProperties = (PageProperties)ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout;
            double test = glyphIndex == 70? pageProperties.TenthToPx(3*pageProperties.StaffSpace) : pageProperties.TenthToPx(1*pageProperties.StaffSpace); //? temp
            //! ^^ measure lines - clef line property * staffspace(gets length from top line to choosen line) eg. clef line 4 == 5-4= 1*staffspace ==> 1staffspace from top
            GlyphTypeface gtf;
            Typeface typeFace = TypeFaces.BravuraMusicFont;
            typeFace.TryGetGlyphTypeface(out gtf);
            double smallFactor = isSmall ? 0.7 : 1;
            Point calculatedPosition = new Point(position.X, position.Y /** PageProperties.PxPerMM()*/);

            double characterSize = pageProperties.StaffHeight.MMToWPFUnit() * smallFactor; //40;//todo refactor to scale

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

        public static void AddCharacterGlyph(this CanvasList canvas, Point position, string character, bool isSmall = false, Brush color = null)
        {
            Brush characterColor = color ?? Brushes.Black;
            DrawingVisual visual = new DrawingVisual();
            ushort glyphIndex = character.GetGlyphIndexOfCharacter();
            PageProperties pageProperties = (PageProperties)ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout;
            GlyphTypeface gtf;
            Typeface typeFace = TypeFaces.BravuraMusicFont;
            typeFace.TryGetGlyphTypeface(out gtf);
            double smallFactor = isSmall ? 0.7 : 1;
            Point calculatedPosition = new Point(position.X, position.Y/* + pageProperties.IndexStaffLinePositions[3]*/);

            double characterSize = pageProperties.StaffHeight.MMToWPFUnit() * smallFactor; //40;//todo refactor to scale

            using (DrawingContext dc = visual.RenderOpen())
            {
                GlyphRun gr = new GlyphRun(gtf,
                    0,       // Bi-directional nesting level
                    false,   // isSideways
                    characterSize,      // pt size
                    new ushort[] { glyphIndex },   // glyphIndices
                    calculatedPosition,          // baselineOrigin
                    new double[] { 0.0 },  // advanceWidths
                    null,    // glyphOffsets
                    null,    // characters
                    null,    // deviceFontName
                    null,    // clusterMap
                    null,    // caretStops
                    null);   // xmlLanguage
                dc.DrawGlyphRun(characterColor, gr);
            }
            canvas.AddVisual(visual);
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

        public static void AddLedgerLine(this CanvasList canvas, Point position, double noteHeadWidth)
        {
            DrawingVisual ledgerLine = new DrawingVisual();
            Pen pen = new Pen(Brushes.Black, (0.9).TenthsToWPFUnit());
            double offset = noteHeadWidth/4;
            Point p1 = new Point(position.X -offset, position.Y);
            Point p2 = new Point(position.X + noteHeadWidth + offset, position.Y);
            using (DrawingContext dc = ledgerLine.RenderOpen())
            {
                dc.DrawLine(pen, p1, p2);
            }
            canvas.AddVisual(ledgerLine);
        }
    }
}
