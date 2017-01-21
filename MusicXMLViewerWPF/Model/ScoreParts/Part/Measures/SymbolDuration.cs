using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF
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
        public static MusSymbolDuration DurStrToMusSymbol(string t)
        {
            //t = t.ToLower();
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
        public static string MusSymbolToDurStr(MusSymbolDuration m)
        {
            switch (m)
            {
                case MusSymbolDuration.Whole:
                    return "whole";
                case MusSymbolDuration.Half:
                    return "half";
                case MusSymbolDuration.Quarter:
                    return "quarter";
                case MusSymbolDuration.Unknown:
                    return "";
                case MusSymbolDuration.Eight:
                    return "eighth";
                case MusSymbolDuration.Sixteen:
                    return "16th";
                case MusSymbolDuration.d32nd:
                    return "32nd";
                case MusSymbolDuration.d64th:
                    return "64th";
                default:
                    return "";
            }
        }

    }
}
