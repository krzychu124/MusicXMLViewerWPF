using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF.Misc
{
    class ScoreSystem
    {
        /// <summary>
        /// List of segments
        /// </summary>
        public static Dictionary<string, Segment> Segments = new Dictionary<string, Segment>();
        public static void Clear()
        {
            Segments.Clear();
        }
        public static Segment GetSegment(string id)
        {
            Segment segment = new Segment();
            if (Segments.ContainsKey(id))
            {
                segment = Segments[id];
            }
            else
            {
                Console.WriteLine($"Segment list NOT contain folowing key {id}");
            }
            return segment;
        }
        //static float test = 0;
        //public static void s (int i)
        //{
        //    test = i;
        //}
    }
}
