using MusicXMLScore.Converters;
using MusicXMLScore.LayoutControl;
using MusicXMLScore.LayoutStyle;
using MusicXMLScore.Model.Defaults;
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.ViewModel;
using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MusicXMLScore.Helpers
{
    public class PrimitivePageGenerator
    {
        private CanvasList page;
        private LayoutGeneral layout;
        private ScorePartwiseMusicXML score;
        private Size dimensions;
        private List<double> topLines;
        public PrimitivePageGenerator(ScorePartwiseMusicXML score, PageMarginType pageSide = PageMarginType.both)
        {
            layout = ViewModelLocator.Instance.Main.CurrentTabLayout;
            dimensions = layout.PageProperties.PageDimensions.Dimensions;
            page = new CanvasList(dimensions.Width.TenthsToWPFUnit(), dimensions.Height.TenthsToWPFUnit());
            this.score = score;
            var partId =  this.score.TryGetEveryPartId();
            DrawMargins();
            DrawCreditsSpace();
            //DrawMeasuresTopLine();
            CalculateMeasureTopLines();
            MusicXMLScore.DrawingHelpers.PartProperties pp = new MusicXMLScore.DrawingHelpers.PartProperties(score, "P1");
        }

        private void DrawMeasuresTopLine()
        {
            double topLineDistance = 0.0;
            double topSystemDistance = 0.0;
            if (score.Defaults.SystemLayout != null)
            {
                if (score.Defaults.SystemLayout.TopSystemDistanceSpecified)
                {
                    topLineDistance = score.Defaults.SystemLayout.TopSystemDistance;
                    if (score.Defaults.SystemLayout.SystemDistanceSpecified)
                    {
                        topSystemDistance = score.Defaults.SystemLayout.SystemDistance.TenthsToWPFUnit();
                    }
                }
            }
            double topmargin = layout.PageMargins.TopMargin.TenthsToWPFUnit();
            Line defaultTopDistance = new Line();
            defaultTopDistance.X1 = 0;
            defaultTopDistance.Y1 = topLineDistance + topmargin;
            defaultTopDistance.X2 = dimensions.Width.TenthsToWPFUnit();
            defaultTopDistance.Y2 = topLineDistance + topmargin;
            defaultTopDistance.Stroke = Brushes.Green;
            defaultTopDistance.StrokeThickness = 1;
            ToolTip t1 = new ToolTip();
            t1.Content = "Default Top Distance";
            defaultTopDistance.ToolTip = t1;
            CanvasList c1 = new CanvasList(dimensions.Width.TenthsToWPFUnit(), 5);
            DrawingVisual toplineVisual = new DrawingVisual();
            using (DrawingContext dc = toplineVisual.RenderOpen())
            {
                dc.DrawLine(new Pen(Brushes.DarkOliveGreen, 1), new Point(defaultTopDistance.X1, defaultTopDistance.Y1), new Point(defaultTopDistance.X2, defaultTopDistance.Y2));
            }
            c1.SetToolTipText("Default top line");
            c1.AddVisual(toplineVisual);
            double measureTopDistance = 0.0;
            var printList = score.Part.ElementAt(0).Measure.ElementAt(0).Items.OfType<PrintMusicXML>();
            if (printList != null)
            {
                if (printList.ElementAt(0).SystemLayout != null)
                {
                    measureTopDistance = printList.ElementAt(0).SystemLayout.TopSystemDistanceSpecified? printList.ElementAt(0).SystemLayout.TopSystemDistance: 0.0;
                }
            }
            CanvasList c2 = new CanvasList(dimensions.Width.TenthsToWPFUnit(), 5);
            DrawingVisual toplineVisual2 = new DrawingVisual();
            using (DrawingContext dc = toplineVisual2.RenderOpen())
            {
                dc.DrawLine(new Pen(Brushes.MediumSeaGreen, 1), new Point(0, topmargin+ measureTopDistance), new Point(defaultTopDistance.X2, topmargin + measureTopDistance));
            }
            c2.SetToolTipText("Measure top line");
            c2.AddVisual(toplineVisual2);
            page.AddVisual(c1);
            page.AddVisual(c2);
        }

        private void DrawCreditsSpace()
        {
            if (score.Credits.Count != 0)
            {
                foreach (var credit in score.Credits)
                {
                    Size dimensionsInWPF = new Size(dimensions.Width.TenthsToWPFUnit(), dimensions.Height.TenthsToWPFUnit());
                    double X = double.Parse(credit.CreditW.DefaultX).TenthsToWPFUnit();
                    double Y = double.Parse(credit.CreditW.DefaultY).TenthsToWPFUnit();
                    Y = dimensionsInWPF.Height - Y;
                    Point p1 = new Point(X-5, Y-5);
                    Point p2 = new Point(X+5, Y+5);
                    Point p3 = new Point(X, Y);
                    DrawingVisual creditSpace = new DrawingVisual();
                    using (DrawingContext dc = creditSpace.RenderOpen())
                    {
                        dc.DrawRectangle(Brushes.Red, new Pen(Brushes.Red, 1), new Rect(p1, p2));
                        //dc.DrawLine(new Pen(Brushes.Black, 1), new Point(0, p3.Y), new Point(dimensionsInWPF.Width, p3.Y));
                    }
                    CanvasList c = new CanvasList(10, 10);
                    c.SetToolTipText(credit.CreditW.Value + "\n" + p3.X.ToString("X: 0.##") + " " + p3.Y.ToString("Y: 0.##"));
                    c.AddVisual(creditSpace);
                    page.AddVisual(c);
                }

            }
            
        }

        private void DrawMargins()
        {
            DrawingVisual marginsVisual = new DrawingVisual();
            PageMarginsMusicXML pageMargins = layout.PageMargins;
            Size dimensions = layout.PageProperties.PageDimensions.Dimensions;
            Point lt = new Point(pageMargins.LeftMargin.TenthsToWPFUnit(), pageMargins.TopMargin.TenthsToWPFUnit());
            Point rb = new Point(dimensions.Width.TenthsToWPFUnit() - pageMargins.RightMargin.TenthsToWPFUnit(),
                                 dimensions.Height.TenthsToWPFUnit() - pageMargins.BottomMargin.TenthsToWPFUnit());
            Rect rectangle = new Rect(lt, rb);
            using (DrawingContext dc= marginsVisual.RenderOpen())
            {
                dc.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Blue, 1), rectangle);
            }
            page.AddVisual(marginsVisual);
        }

        private void CalculateMeasureTopLines()
        {
            double defaultTopSystemDistance = 0.0;
            double defaultSystemDistance = 0.0;
            if (score?.Defaults?.SystemLayout != null)
            {
                SystemLayoutMusicXML systemLayout = score.Defaults.SystemLayout;
                defaultTopSystemDistance = systemLayout.TopSystemDistanceSpecified ? score.Defaults.SystemLayout.TopSystemDistance.TenthsToWPFUnit() : 0.0;
                defaultSystemDistance = systemLayout.SystemDistanceSpecified ? score.Defaults.SystemLayout.SystemDistance.TenthsToWPFUnit() : 0.0;
            }
            double partsCount = score.Part.Count;
            var numberOfLinesPerPage = score.Part.ElementAt(0).TryGetLinesPerPage();
            double currentY = layout.PageMargins.TopMargin.TenthsToWPFUnit();
            double defaultMarginLeft = layout.PageMargins.LeftMargin.TenthsToWPFUnit();
            double currentMarginLeft =defaultMarginLeft;
            double currentSystemDistance = defaultSystemDistance;
            var firstPage = numberOfLinesPerPage.ElementAt(0);
            double sysDistAcc = currentY;
            double staffHeight = layout.PageProperties.StaffHeight.MMToWPFUnit();
            foreach (var item in firstPage)
            {
                string numberToken = item;
                var measure = score.Part.ElementAt(0).Measure.Where(i => i.Number == numberToken).FirstOrDefault();
                if (firstPage.IndexOf(item) == 0)
                {
                    currentSystemDistance = defaultTopSystemDistance;
                    var part = measure.Items.OfType<PrintMusicXML>().FirstOrDefault();
                    if (part != null)
                    {
                        if (part.SystemLayout != null)
                        {
                            var topSystemSpecified = part?.SystemLayout?.TopSystemDistanceSpecified ?? part.SystemLayout.TopSystemDistanceSpecified;
                            if (topSystemSpecified)
                            {
                                currentSystemDistance = part.SystemLayout.TopSystemDistance.TenthsToWPFUnit();
                            }
                            if (part.SystemLayout.SystemMargins != null)
                            {
                                double systemMarginLeft = part.SystemLayout.SystemMargins != null ? part.SystemLayout.SystemMargins.LeftMargin : 0.0;
                                currentMarginLeft = defaultMarginLeft + systemMarginLeft;
                            }
                        }
                    }
                    sysDistAcc += currentSystemDistance; //! todo more tests
                    DrawMeasureLine(currentMarginLeft, sysDistAcc, numberToken);
                    currentSystemDistance = defaultSystemDistance;
                    //sysDistAcc += staffHeight;
                }
                else
                {
                    var part = measure.Items.OfType<PrintMusicXML>().FirstOrDefault();
                    if (part != null)
                    {
                        if (part.SystemLayout != null)
                        {
                            var systemSpecified = part?.SystemLayout?.SystemDistanceSpecified ?? part.SystemLayout.SystemDistanceSpecified;
                            if (systemSpecified)
                            {
                                currentSystemDistance = part.SystemLayout.SystemDistance.TenthsToWPFUnit();
                            }
                            if (part.SystemLayout.SystemMargins != null)
                            {
                                double systemMarginLeft = part.SystemLayout.SystemMargins != null ? part.SystemLayout.SystemMargins.LeftMargin : 0.0;
                                currentMarginLeft = defaultMarginLeft + systemMarginLeft;
                            }
                            
                        }
                        sysDistAcc += currentSystemDistance;
                        sysDistAcc += staffHeight *2;
                        DrawMeasureLine(currentMarginLeft, sysDistAcc, numberToken);
                        
                    }
                }
            }
        }
        
        private void DrawMeasureLine(double margin, double Y, string tooltip)
        {
            double staffHeight = layout.PageProperties.StaffHeight.MMToWPFUnit();
            Point p1 = new Point(0, Y);
            Point p2 = new Point(dimensions.Width.TenthsToWPFUnit(), Y);
            Point mp1 = new Point(margin, Y);
            Point mp2 = new Point(margin, Y - staffHeight);
            DrawingVisual line = new DrawingVisual();
            using (DrawingContext dc = line.RenderOpen())
            {
                dc.DrawLine(new Pen(Brushes.ForestGreen, 2), p1, p2);
                dc.DrawLine(new Pen(Brushes.Red, 3), mp1, mp2);
            }
            CanvasList measureLine = new CanvasList(dimensions.Width.TenthsToWPFUnit(), 5);
            measureLine.SetToolTipText("Measure: " + tooltip + "\n" + mp1.X.ToString("X: 0.##") + " " + mp1.Y.ToString("Y: 0.##") + "\n" + staffHeight.ToString("Staff Height: 0.###"));
            measureLine.AddVisual(line);
            page.AddVisual(measureLine);
        }

        private void DrawPrimitiveMeasure()
        {

        }
        internal CanvasList Page
        {
            get
            {
                return page;
            }

            set
            {
                page = value;
            }
        }
    }
}
