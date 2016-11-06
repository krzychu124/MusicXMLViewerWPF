using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF.Misc
{
    class Segments
    {
        private List<Segment> list; 
        public List<Segment> SegmentsList { get { return list; } }

        public Segments()
        {
            list = new List<Segment>();
        }

        public Segment GetElement(int number)
        {
            Segment segment = null;
            if (number < list.Count - 1)
            {
                segment = list.ElementAt(number - 1);
            }
            else
            {
                Logger.Log($"Segment number exceeded count of list{number}");
            }
            return segment;
        }

        public Segment GetLast()
        {
            return list.Last();
        }

        public void Add(Segment s)
        {
            list.Add(s);
        }

        public void RemoveWithID(string id)
        {
            var index = list.FindIndex(i => i.ID == id);
            list.RemoveAt(index);
        }

        public Segment GetSegmentWithID(string id)
        {
            var segment = list.Select(i => i).Where(i => i.ID == id).First();
            return segment;
        }
    }
}
