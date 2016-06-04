using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    public class Clef : EmptyPrintStyle//MusicalChars //TODO_H implement XElement ctor
    {
        private ClefType sign;
        private int line;
        private int measure_num;
        private static int clef_alter;

        public ClefType Sign { get { return sign; } }
        public int Line { get { return line; } }
        public int MeasureId { get { return measure_num; } }
        public static int ClefAlter { get { return clef_alter; } }

        public Clef(XElement x):base(x.Attributes())
        {
            //TODO_H check if possible to go to parenet of parent to get measure number and other info...
            var ele = x.Elements();
            foreach (var item in ele)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "sign":
                        sign = new ClefType(item.Value);
                        clef_alter = sign.Sign == ClefType.Clef.GClef ? 0 : sign.Sign == ClefType.Clef.FClef ? -12 : -6;
                        break;
                    case "line":
                        line = int.Parse(item.Value);
                        break;
                    case "clef-octave-change":
                        Logger.Log("Clef-octave-change not implemented");
                        break;
                    default:
                        break;
                }
            }
        }
        
        public Clef(string c, int line, int num)
        {
            
          //  base.type = MusSymbolType.Clef;
            this.line = line;
            this.measure_num = num;
            this.sign = new ClefType(c);

            clef_alter = sign.Sign == ClefType.Clef.GClef ? 0 : sign.Sign == ClefType.Clef.FClef? -12: -6;
        }
    }
}
