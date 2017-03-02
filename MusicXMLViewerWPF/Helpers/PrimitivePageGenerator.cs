using MusicXMLScore.LayoutControl;
using MusicXMLScore.LayoutStyle;
using MusicXMLScore.Model.Defaults;
using MusicXMLScore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.Helpers
{
    public class PrimitivePageGenerator
    {
        private CanvasList page;
        private LayoutGeneral layout;
        public PrimitivePageGenerator(PageMarginType pageSide = PageMarginType.both)
        {
            layout = ViewModelLocator.Instance.Main.CurrentTabLayout;
            Size dimensions = layout.PageProperties.PageDimensions.Dimensions;
            page = new CanvasList(ScaleExtensions.TenthsToWPFUnit(dimensions.Width), ScaleExtensions.TenthsToWPFUnit(dimensions.Height));
            DrawMargins();
        }

        private void DrawMargins()
        {
            DrawingVisual margins = new DrawingVisual();
            using (DrawingContext dc= margins.RenderOpen())
            {
                PageMarginsMusicXML pm = layout.PageProperties.PageMargins.ElementAt(0);
                Size dimensions = layout.PageProperties.PageDimensions.Dimensions;
                Point lt = new Point(ScaleExtensions.TenthsToWPFUnit(pm.LeftMargin), ScaleExtensions.TenthsToWPFUnit(pm.TopMargin));
                Point rb = new Point(ScaleExtensions.TenthsToWPFUnit(dimensions.Width) - ScaleExtensions.TenthsToWPFUnit(pm.RightMargin),
                                    ScaleExtensions.TenthsToWPFUnit(dimensions.Height) - ScaleExtensions.TenthsToWPFUnit(pm.BottomMargin));
                Rect rectangle = new Rect(lt, rb);
                dc.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Blue, 1), rectangle);
            }
            page.AddVisual(margins);
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
