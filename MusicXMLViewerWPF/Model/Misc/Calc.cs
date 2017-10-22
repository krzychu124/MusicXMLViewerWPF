using System;
using System.Windows;

namespace MusicXMLViewerWPF
{
    /// <summary>
    /// Class Helper with methods for various calculations
    /// </summary>
    public static class Calc
    {
        public static Point MidPoint(Point p1, Point p2)
        {
            Point Mid;
            Mid = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
            return Mid;
        }

        public static float Slope(Point p1, Point p2)
        {
            float slope = (float)((p2.Y - p1.Y) / (p2.X - p1.X));
            return slope;
        }

        public static float Distance(Point p1, Point p2)
        {
            float dist = (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            return dist;
        }

        public static Point PerpendicularOffset(Point p1, Point p2, float distance)
        {
            Point M = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
            Point p = new Point(p1.X - p2.X, p1.Y - p2.Y);
            Point n = new Point(-p.Y, p.X);
            int norm_length = (int)Math.Sqrt((n.X * n.X) + (n.Y * n.Y));
            n.X /= norm_length;
            n.Y /= norm_length;
            return new Point(M.X + (distance * n.X), M.Y + (distance * n.Y));
        }
        /// <summary>
        /// Adds Point to Point (x+X, y+Y)
        /// </summary>
        /// <param name="one"></param>
        /// <param name="TWO"></param>
        /// <returns></returns>
        public static Point Add(Point one, Point TWO)
        {
            Point result = new Point();
            result.X = one.X + TWO.X;
            result.Y = one.Y + TWO.Y;
            return result;
        }
    }
}
