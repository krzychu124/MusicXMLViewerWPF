using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLTestViewerWPF
{
    public class ClefType
    {
        private Clef sign;
        public Clef Sign { get { return sign; } }
        private string sign_s;
        public string Sign_s { get { return sign_s; } }
        private string symbol;
        public string Symbol { get { return symbol; } }

        public ClefType(string s)
        {
            setClef(s);
        }
        public ClefType(Clef c)
        {
            setSymbol(c);
        }
        

        private void setClef(string c)
        {
            sign_s = "Clef "+c;
            sign = c == "G" ? Clef.GClef : c == "F" ? Clef.FClef : Clef.CClef;
            setSymbol(sign);
        }

        private void setSymbol(Clef x)
        {
            
            switch (x)
            {
                case Clef.CClef:
                    symbol = MusChar.CClef;

                    break;
                case Clef.FClef:
                    symbol = MusChar.FClef;
                    break;
                case Clef.GClef:
                    symbol = MusChar.GClef;
                    break;
            }
        }
        public enum Clef
        {
            GClef = 0,
            CClef = 1,
            FClef = 2
        }
    }
}
