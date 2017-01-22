using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF
{
    public class ClefType
    {
        private Clef sign;
        private string sign_s;
        private string symbol;

        public Clef Sign { get { return sign; } }
        public string Sign_s { get { return sign_s; } }
        public string Symbol { get { return symbol; } }

        public ClefType(string s)
        {
            setClef(s);
        }
        public ClefType(Clef c)
        {
            sign_s = "Clef " + c;
            sign = c;
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
                    symbol = MusicalChars.CClef;

                    break;
                case Clef.FClef:
                    symbol = MusicalChars.FClef;
                    break;
                case Clef.GClef:
                    symbol = MusicalChars.GClef;
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
