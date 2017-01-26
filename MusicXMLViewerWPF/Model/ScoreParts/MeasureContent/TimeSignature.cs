using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using MusicXMLScore.Helpers;
using MusicXMLViewerWPF.Misc;
using System.ComponentModel;

namespace MusicXMLViewerWPF
{
    class TimeSignature : Segment, IDrawableMusicalObject// MusicalChars // TODO_I Scale Refactor needed // test timesig class
    {
        private EmptyPrintStyle additional_attributes;
        private int beats;
        private int beats_type;
        private int measure_num;
        private int number; // staff number
        private SignatureType sigType;
        private string beats_str;
        private string beats_type_str;
        private bool loadstatus;
        private CanvasList drawablemusicalobject;
        private DrawableMusicalObjectStatus dmusicalobjstatus;
        

        public EmptyPrintStyle AdditionalAttributes { get { return additional_attributes; } }
        public int Beats { get { return beats; } }
        public int BeatsType { get { return beats_type; } }
        public int MeasureNum { get { return measure_num; } }
        public string BeatStr { get { return beats_str; } }
        public string BeatTypeStr { get { return beats_type_str; } }
        public SegmentType CharacterType { get { return SegmentType.TimeSig; } }

        public CanvasList DrawableMusicalObject { get { return drawablemusicalobject; }  set { drawablemusicalobject = value; } }
        public DrawableMusicalObjectStatus DrawableObjectStatus { get { return dmusicalobjstatus; } private set { if (dmusicalobjstatus != value) dmusicalobjstatus = value; } } 
        public bool Loaded { get { return loadstatus; } private set { if (loadstatus != value) loadstatus = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Loaded))); } }
        public new PropertyChangedEventHandler PropertyChanged = delegate { };

        //TODO_L implement missing properties, separator,interchangeable
        public TimeSignature(XElement x)
        {
            this.PropertyChanged += TimeSigPropertyChanged;
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
            Loaded = true;
        }

        private void TimeSigPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Loaded":
                    if (Loaded)
                    {
                        InitDrawableObject();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Set beat and beat_time. If time signature is common / cut  -   beat / beattime doesn't matter
        /// </summary>
        /// <param name="beats"></param>
        /// <param name="beat_type"></param>
        /// <param name="stype"></param>
        public TimeSignature(int beats, int beat_type, SignatureType stype = SignatureType.number)
        {
            PropertyChanged += TimeSigPropertyChanged;
            ID = Misc.RandomGenerator.GetRandomHexNumber();
            sigType = stype;
            if (stype == SignatureType.number)
            {
                this.beats = beats;
                this.beats_type = beat_type;
                SetBeatTime(beats_type);
                SetBeat(beats);
            }
            Segment_type = SegmentType.TimeSig;
            Loaded = true;
        }

        public void Draw(DrawingVisual visual)
        {
            using (DrawingContext dc = visual.RenderOpen())
            {
                if (sigType == SignatureType.number)
                {
                    Point p = new Point(Relative_x , Relative_y);
                    FormattedText fbt = new FormattedText(BeatTypeStr, System.Threading.Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, TypeFaces.TimeNumbers, 40, Brushes.Black);
                    fbt.TextAlignment = TextAlignment.Center;
                    FormattedText fb = new FormattedText(BeatStr, System.Threading.Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, TypeFaces.TimeNumbers, 40, Brushes.Black);
                    fb.TextAlignment = TextAlignment.Center;
                    Misc.DrawingHelpers.DrawFormattedString(dc, fb, p);
                    Misc.DrawingHelpers.DrawFormattedString(dc, fbt, p);
                }
                if (sigType == SignatureType.common)
                {
                    Misc.DrawingHelpers.DrawString(dc, MusicalChars.CommonTime, TypeFaces.NotesFont, Brushes.Black, Relative_x , Relative_y - 0.4f * 40, 40);
                }
                if (sigType == SignatureType.cut)
                {
                    Misc.DrawingHelpers.DrawString(dc, MusicalChars.CutTime, TypeFaces.NotesFont, Brushes.Black, Relative_x , Relative_y - 0.4f * 40, 40);
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

        public void InitDrawableObject()
        {
            if (Loaded)
            {
                DrawableMusicalObject = new CanvasList(this.Width, this.Height);
                DrawingVisual visual = new DrawingVisual();
                Draw(visual);
                DrawableMusicalObject.AddVisual(visual);
                DrawableMusicalObject.SetValue(CustomMeasurePanel.StaticPositionProperty, true);
                DrawableObjectStatus = DrawableMusicalObjectStatus.ready;
            }
        }

        public void ReloadDrawableObject()
        {
            DrawableMusicalObject.ClearVisuals();
            DrawableObjectStatus = DrawableMusicalObjectStatus.notready;
            InitDrawableObject();
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
