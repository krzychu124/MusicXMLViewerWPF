using MusicXMLViewerWPF.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Rest : Note, IAutoPosition
    {
        // private int dots; inherited
        // private int multimeasure;
        //private bool hasDot; inherited
        //private float posX; inherited
        //private int id; inherited
        private bool ismeasurerest;
        private int measure_num;
        private string duration_symbol;
        public static List<Rest> RestList = new List<Rest>();

        //public bool HasDot { get { return hasDot; } } inherited
        //public int Duration { get { return duration; } } inherited
        //public int MeasureId { get { return measure_num; } } inherited
        //public int Voice { get { return voice; } } inherited
        //public string Symbol { get { return duration_symbol; } } inherited
        public bool IsMeasureRest { get { return ismeasurerest; } }
        public float X { get { return posX; } }
        public int CharId { get { return id; } }

        public Rest(XElement x)
        {
            Segment_type = SegmentType.Rest;
            duration = int.Parse(x.Element("duration").Value);
            voice = int.Parse(x.Element("voice").Value);
            Width = 10f;
            ismeasurerest = false;
        }

        public Rest()
        {
            Segment_type = SegmentType.Rest;
            Width = 10f;
        }
        public Rest(int m_num,int id,int d, int v, string t, float x, bool restType, bool dot = false)
        {
            this.id = id;
            base.Segment_type = SegmentType.Rest;
            measure_num = m_num;
            this.posX = x;
            duration = d;
            voice = v;
            ismeasurerest = restType;
            duration_symbol = MusChar.getRestSymbol(t);
            hasDot = dot;
        }
        public Rest(int m_num,int id,int d, int v, bool restType)
        {
            this.id = id;
            base.Segment_type = SegmentType.Rest;
            measure_num = m_num;
            duration = d;
            voice = v;
            ismeasurerest = restType;
        }

        public void MeasureRestDuration(int duration)
        {


        }
        public static void ExtractRests()
        {
              RestList = LoadDocToClasses.list.OfType<Rest>().ToList();
        }
    }
}
