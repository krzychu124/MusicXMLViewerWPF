using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    public class TimeSignature : Segment, Misc.IDrawableMusicalChar // MusicalChars // TODO_L test timesig class
    {
        private EmptyPrintStyle additional_attributes;
        private int beats;
        private int beats_type;
        private int measure_num;
        private int number; // staff number
        private SignatureType sigType;
        private string beats_str;
        private string beats_type_str;

        public EmptyPrintStyle AdditionalAttributes { get { return additional_attributes; } }
        public int Beats { get { return beats; } }
        public int BeatsType { get { return beats_type; } }
        public int MeasureNum { get { return measure_num; } }
        public string BeatStr { get { return beats_str; } }
        public string BeatTypeStr { get { return beats_type_str; } }
        public SegmentType CharacterType { get { return SegmentType.TimeSig; } }
        //TODO_L implement missing properties, separator,interchangeable
        public TimeSignature(XElement x)
        {
            ID = Misc.RandomGenerator.GetRandomHexNumber();
            Segment_type = SegmentType.TimeSig;
            additional_attributes = new EmptyPrintStyle(x.Attributes());
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

        public void Draw(DrawingVisual visual)
        {
            using (DrawingContext dc = visual.RenderOpen())
            {
                if (sigType == SignatureType.number)
                {
                    Point p = new Point(Relative_x + Spacer_L, Relative_y);
                    FormattedText fbt = new FormattedText(BeatTypeStr, System.Threading.Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, TypeFaces.TimeNumbers, MusicScore.Defaults.Scale.Tenths, Brushes.Black);
                    fbt.TextAlignment = TextAlignment.Center;
                    FormattedText fb = new FormattedText(BeatStr, System.Threading.Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, TypeFaces.TimeNumbers, MusicScore.Defaults.Scale.Tenths, Brushes.Black);
                    fb.TextAlignment = TextAlignment.Center;
                    Misc.DrawingHelpers.DrawFormattedString(dc, fb, p);
                    Misc.DrawingHelpers.DrawFormattedString(dc, fbt, p);
                }
                if (sigType == SignatureType.common)
                {
                    Misc.DrawingHelpers.DrawString(dc, MusicalChars.CommonTime, TypeFaces.NotesFont, Brushes.Black, Relative_x + Spacer_L, Relative_y - 0.4f * MusicScore.Defaults.Scale.Tenths, MusicScore.Defaults.Scale.Tenths);
                }
                if (sigType == SignatureType.cut)
                {
                    Misc.DrawingHelpers.DrawString(dc, MusicalChars.CutTime, TypeFaces.NotesFont, Brushes.Black, Relative_x + Spacer_L, Relative_y - 0.4f * MusicScore.Defaults.Scale.Tenths, MusicScore.Defaults.Scale.Tenths);
                }
            }
        }

        public TimeSignature(int beats, int beats_type,string symbol = "", int num = 0)
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
                    sigType = SignatureType.common;
                    break;
                case "cut":
                    sigType = SignatureType.cut;
                    break;
                default:
                    sigType = SignatureType.number;
                    break;
            }

        }
        private void SetBeatTime(int i)
        {
            if (i < 10)
            {
                if (beat_dic.ContainsKey(i))
                {
                    this.beats_type_str = beat_dic[i];
                }
            }
            else
            {
                char[] chars = BeatsType.ToString().ToCharArray();
                foreach (var item in chars)
                {
                    beats_type_str += beat_dic[int.Parse(item.ToString())] +"   ";
                }
            }
        }
        private void SetBeat(int i)
        {
            if (i < 10)
            {
                if (beat_d.ContainsKey(i))
                {
                    this.beats_str = beat_d[i];
                }
            }
            else
            {
                char[] chars = Beats.ToString().ToCharArray();
                foreach (var item in chars)
                {
                    beats_str += beat_d[int.Parse(item.ToString())] + "   ";
                }
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
            {0,MusicalChars.zero },
            {1,MusicalChars.one },
            {2,MusicalChars.two },
            {3,MusicalChars.three },
            {4,MusicalChars.four },
            {5,MusicalChars.five },
            {6,MusicalChars.six },
            {7,MusicalChars.seven },
            {8,MusicalChars.eight },
            {9,MusicalChars.nine },
        };
        private Dictionary<int, string> beat_dic = new Dictionary<int, string>()
        {
            {0,MusicalChars.zeroT },
            {1,MusicalChars.oneT },
            {2,MusicalChars.twoT },
            {3,MusicalChars.threeT },
            {4,MusicalChars.fourT },
            {5,MusicalChars.fiveT },
            {6,MusicalChars.sixT },
            {7,MusicalChars.sevenT },
            {8,MusicalChars.eightT },
            {9,MusicalChars.nineT },
        };
    }
    enum SignatureType
    {
        number,
        common,
        cut,
    }
}
