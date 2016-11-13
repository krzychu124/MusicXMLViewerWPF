using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.Helpers
{
    class DrawingHelpers
    {
        private static Random rndom = new Random();
        public static void DrawString(DrawingContext dc, string text, Typeface t_f, Brush brush, float xPos, float yPos, float element_size)
        {
            dc.DrawText(new FormattedText(text, System.Threading.Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, t_f, element_size, brush), new Point(xPos, yPos));
        }
        /// <summary>
        /// Returns random Brush color
        /// </summary>
        /// <returns></returns>
        public static Brush PickBrush()
        {
            Brush result = Brushes.Transparent;

            Random rnd = new Random();

            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties();

            int random = rndom.Next(properties.Length);
            result = (Brush)properties[random].GetValue(null, null);

            return result;
        }
    }
}
