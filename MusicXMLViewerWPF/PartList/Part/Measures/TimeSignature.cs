using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    public class TimeSignature : MusicalChars //TODO implement Xelement ctor
    {
        private SignatureType sigType;
        private int beats;
        private int beats_type;
        private int measure_num;
        private string beats_str;
        private string beats_type_str;

        public TimeSignature(XElement x)
        {

        }

        public TimeSignature(int beats, int beats_type,string symbol, int num)
        {

            switch (symbol)
            {
                case "common": this.sigType = SignatureType.common;
                    break;
                case "cut": this.sigType = SignatureType.cut;
                    break;
                default: sigType = SignatureType.number;
                    break;
            }
            this.beats = beats;
            this.beats_type = beats_type;
            this.type = MusSymbolType.TimeSignature;
            this.measure_num = num;
            setBeatTime(beats_type);
            setBeat(beats);
        }
        
        public int Beats
        {
            get { return beats; }
        }
        public int BeatsType
        {
            get { return beats_type; }
        }
        public int MeasureNum
        {
            get { return measure_num; }
        }
        public string BeatStr
        {
            get { return beats_str; }
        }
        public string BeatTypeStr
        {
            get { return beats_type_str; }
        }
        private void setBeatTime(int i)
        {
            if (beat_dic.ContainsKey(i))
            {
                this.beats_type_str = beat_dic[i];
            }
        }
        private void setBeat(int i)
        {
            if (beat_d.ContainsKey(i))
            {
                this.beats_str = beat_d[i];
            }
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
