using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Key : EmptyPrintStyle//  MusicalChars //TODO_L implement missing properties 
    {
        private int measure_num;
        private bool isSharp;
        private bool isNatural = false;
        private Fifths fifths;
        private Mode mode;
        public Key( int fifths, string mode, int num)
        {
            //this.musicalcharacter = fifths < 0 ? "b" : fifths > 0 ? "#" : " ";
            isSharp = false;
            isSharp = fifths > 0 ? true : fifths < 0 ? false : isNatural = true;
            SetFifths(fifths);
            SetMode(mode);
            //this.type = MusSymbolType.Key;
            this.measure_num = num;
        }

        public Key(XElement x):base(x.Attributes())
        {
            var ele = x.Elements();
            foreach (var item in ele)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "fifths":
                        SetFifths(int.Parse(item.Value));
                        break;
                    case "mode":
                        SetMode(item.Value);
                        break;
                    default:
                        break;
                }
            }
            isNatural = false;
            isSharp = false;
            isSharp = fifths > 0 ? true : fifths < 0 ? false : isNatural = true;
        }

        private void SetMode(string s)
        {
            switch (s)
            {
                case "minor":
                    this.mode = Mode.minor;
                    break;
                case "major":
                    this.mode = Mode.major;
                    break;
                default:
                    this.mode = Mode.unknown;
                    break;
            }
        }

        private void SetFifths(int i)
        {
            if(FifthDic.ContainsKey(i))
            {
                this.fifths = FifthDic[i];
            }
        }
        private static Dictionary<int, Fifths> FifthDic=new Dictionary<int, Fifths> {
            { -1, Fifths.F },
            { -2, Fifths.Bb },
            { -3, Fifths.Eb },
            { -4, Fifths.Ab },
            { -5, Fifths.Db },
            { 0,Fifths.C },
            { 1,Fifths.G },
            { 2,Fifths.D },
            { 3,Fifths.A },
            { 4,Fifths.E },
            { 5,Fifths.B },
            { -6,Fifths.Gb },
        };

        public Fifths Fifths
        {
            get
            {
                return fifths;
            }
        }

        public Mode Mode
        {
            get
            {
                return mode;
            }
        }
        public int MeasureId
        {
            get
            {
                return measure_num;
            }
        }
    }
    enum Fifths
    {
        C,
        G =1,
        D =2,
        A =3,
        E =4,
        B =5,
        Gb =-6,
        Db =-5,
        Ab =-4,
        Eb =-3,
        Bb =-2,
        F = -1

    
    }
    enum Mode
    {
        major,
        minor,
        unknown
    }
}
