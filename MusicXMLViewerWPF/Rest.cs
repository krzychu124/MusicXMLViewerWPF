using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLTestViewerWPF
{
    class Rest : MusicalChars
    {
        private int measure_num;
        private int id;
        private float posX;
        private int duration;
        private int voice;
        private string duration_symbol;
        private bool ismeasurerest;
        public static List<Rest> RestList = new List<Rest>();
        // private int multimeasure;
        private bool hasDot;
        // private int dots;
        
        public int MeasureId { get { return measure_num; } }
        public int CharId { get { return id; } }
        public float X { get { return posX; } }
        public int Duration { get { return duration; } }
        public int Voice { get { return voice; } }
        public string Symbol { get { return duration_symbol; } }
        public bool IsMeasureRest { get { return ismeasurerest; } }
        public bool HasDot { get { return hasDot; } }

        public Rest()
        {

        }
        public Rest(int m_num,int id,int d, int v, string t, float x, bool restType, bool dot = false)
        {
            this.id = id;
            base.type = MusSymbolType.Rest;
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
            base.type = MusSymbolType.Rest;
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
