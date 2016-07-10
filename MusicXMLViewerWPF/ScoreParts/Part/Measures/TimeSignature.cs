using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    public class TimeSignature : EmptyPrintStyle // MusicalChars // TODO_L test timesig class
    {
        private int beats;
        private int beats_type;
        private int measure_num;
        private int number; // staff number
        private SignatureType sigType;
        private string beats_str;
        private string beats_type_str;


        public int Beats { get { return beats; } }
        public int BeatsType { get { return beats_type; } }
        public int MeasureNum { get { return measure_num; } }
        public string BeatStr { get { return beats_str; } }
        public string BeatTypeStr { get { return beats_type_str; } }
        //TODO_L implement missing properties, separator,interchangeable
        public TimeSignature(XElement x) : base (x.Attributes())
        {
            var ele = x.Elements();
            foreach (var item in ele)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "beats":
                        beats = int.Parse(item.Value);
                        break;
                    case "beat-type":
                        beats_type = int.Parse(item.Value);
                        break;
                    default:
                        break;
                }
            }
            var attr = x.Attributes();
            foreach (var item in attr) // with switch,dictionary would look better, left it for now
            {
                if(item.Name.LocalName == "symbol")
                {
                    SetTimeType(item.Value);
                }
                if(item.Name.LocalName == "number")
                {
                    number = int.Parse(item.Value);
                }
            }
            SetBeatTime(beats_type);
            SetBeat(beats);
        }

        public TimeSignature(int beats, int beats_type,string symbol, int num)
        {

            SetTimeType(symbol);
            this.beats = beats;
            this.beats_type = beats_type;
           // this.type = MusSymbolType.TimeSignature; //changed derived class
            this.measure_num = num;
            SetBeatTime(beats_type);
            SetBeat(beats);
        }
        private void SetTimeType(string s)
        {
            switch (s)
            {
                case "common":
                    this.sigType = SignatureType.common;
                    break;
                case "cut":
                    this.sigType = SignatureType.cut;
                    break;
                default:
                    sigType = SignatureType.number;
                    break;
            }

        }
        private void SetBeatTime(int i)
        {
            if (beat_dic.ContainsKey(i))
            {
                this.beats_type_str = beat_dic[i];
            }
        }
        private void SetBeat(int i)
        {
            if (beat_d.ContainsKey(i))
            {
                this.beats_str = beat_d[i];
            }
        }
        public string GetNumber(int i)
        {
            string number = "??";
            if (beat_d.ContainsKey(i))
            {
                number = beat_d[i];
            }
            return number;
        }
        private Dictionary<int, string> beat_d = new Dictionary<int, string>()
        {
            {1,MusChar.one },
            {2,MusChar.two },
            {3,MusChar.three },
            {4,MusChar.four },
            {5,MusChar.five },
            {6,MusChar.six },
            {7,MusChar.seven },
            {8,MusChar.eight },
            {9,MusChar.nine },
        };
        private Dictionary<int, string> beat_dic = new Dictionary<int, string>()
        {
            {1,MusChar.oneT },
            {2,MusChar.twoT },
            {3,MusChar.threeT },
            {4,MusChar.fourT },
            {5,MusChar.fiveT },
            {6,MusChar.sixT },
            {7,MusChar.sevenT },
            {8,MusChar.eightT },
            {9,MusChar.nineT },
        };
    }
    enum SignatureType
    {
        common,
        cut,
        number
    }
}
