using System.Windows.Media;

namespace MusicXMLScore.LayoutStyle.Styles
{
    public class ItemsColorsStyle
    {
        public Brush SelectionColor { get; set; } = Brushes.DeepSkyBlue;
        public Brush ClefColor { get; set; } = Brushes.Black;
        public Brush DefaultColor { get; set; } = Brushes.Black;

        public ItemsColorsStyle()
        {
        }
    }
}
