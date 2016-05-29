using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLTestViewerWPF
{
    public class Clef : MusicalChars
    {
        private ClefType sign;
        private int line;
        private int measure_num;
        private static int clef_alter;
        public ClefType Sign { get { return sign; } }
        public int Line { get { return line; } }
        public int MeasureId { get { return measure_num; } }
        public static int ClefAlter { get { return clef_alter; } }

        public Clef(string c, int line, int num)
        {
            
            base.type = MusSymbolType.Clef;
            this.line = line;
            this.measure_num = num;
            this.sign = new ClefType(c);

            clef_alter = sign.Sign == ClefType.Clef.GClef ? 0 : sign.Sign == ClefType.Clef.FClef? -12: -6;
        }
    }
}
