using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.Helpers
{
    class DrawingHelpers
    {
        private static Random rndom = new Random();
        public static double PointsToPixels(double points)
        {
            return points * (96.0 / 72.0);
        }

        public static void DrawString(DrawingContext dc, string text, Typeface t_f, Brush brush, float xPos, float yPos, float element_size)
        {
            double p =PointsToPixels(element_size);
            dc.DrawText(new FormattedText(text, System.Threading.Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, t_f, p, brush), new Point(xPos, yPos));
        }
        /// <summary>
        /// Returns random Brush color
        /// </summary>
        /// <returns></returns>
        public static Brush PickRandomBrush()
        {
            Brush result = Brushes.Transparent;

            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties().Select(i => i).Where(i => i.Name.StartsWith("Dark") || i.Name.StartsWith("Medium")).ToArray();
            //! colors with first letter "D"
            int random = rndom.Next(properties.Length);
            result = (Brush)properties[random].GetValue(null, null);
            result.Freeze();
            return result;
        }
    }
}
