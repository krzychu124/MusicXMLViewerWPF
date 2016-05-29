using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLTestViewerWPF
{ 
  


    public enum MusSymbolDuration
    {
        Whole = 1,
        Half = 2,
        Quarter = 4,
        Unknown = 6,
        Eight = 8,
        Sixteen = 16,
        d32nd = 32,
        d64th = 64

    }
    public static class SymbolDuration
    {
        public static MusSymbolDuration d_type(string t)
        {

            switch (t)
            {
                case "whole":
                    return MusSymbolDuration.Whole;
                    
                case "half":
                    return MusSymbolDuration.Half;
                    
                case "quarter":
                    return MusSymbolDuration.Quarter;
                    
                case "eighth":
                    return MusSymbolDuration.Eight;
                    
                case "16th":
                    return MusSymbolDuration.Sixteen;
                    
                case "32nd":
                    return MusSymbolDuration.d32nd;
                    
                case "64th":
                    return MusSymbolDuration.d64th;
                  
                default:
                    return MusSymbolDuration.Unknown;
                    
            }

        }

    }
}
