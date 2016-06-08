using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF
{
    class MusChar           ///add ornaments unicode symbols
    {
        private static string to_utf8(string s)
        {
            byte[] bytes = Encoding.Default.GetBytes(s);
            string myString = Encoding.UTF8.GetString(bytes);
            return myString;
        }
        //public static string Staff5L = to_utf8("\u1d110");
        //clefs
        public const string FClef = "\uE062";
        public static string GClef = "\uE050";//"\ue050";//"\U0001D11E";
        public const string CClef = "\uE05C";

        //staffs
        public const string Staff4L = "\uE019";
        public const string Staff5L = "\uE01A";//"\uE01A";// "\U0001D11A";   // x=8 size=10, 16-20,32-40
        public const string Staff5Ls = "\uE020";
        public const string Staff6L = "\uE01B";
        //bars
        public const string DashedBar = "\uE036";
        public const string DottedBar = "\uE037";
        public const string DoubleRepeatBar = "\uE042";
        public const string HeavyBar = "\uE034";
        public const string HeavyHeavyBar = "\uE035";
        public const string HeavyLightBar = "\uE033";
        public const string LeftRepeatBar = "\uE040";
        public const string LightHeavyBar = "\uE032";
        public const string LightLightBar = "\uE031";
        public const string RegularBar = "\uE030";
        public const string RightRepeatBar = "\uE041";
        public const string ShortBar = "\uE038";
        public const string TickBar = "\uE039";
        //repeats
        public const string RepeatFigure = "\U0001D10D";
        public const string RepeatMeasure = "\uE500";
        public const string Repeat2Measures = "\uE501";
        //gracenotes
        public const string GraceNote = "\uE562";
        public const string GraceNoteR = "\uE563";
        public const string GraceNoteSlash = "\uE560";
        public const string GraceNoteSlashR = "\uE561"; // R means upside-down
        public const string Dot = "\uE1E7";
        //notes
        public const string Whole = "\uE1D1";// "\U0001D117";
        public const string Half = "\uE1D3";//"\U0001D15E";
        public const string Quarter = "\uE1D5";//"\U0001D15F";
        public const string Eight = "\uE1D7";//"\uE1D7";"\uECA7"
        public const string Sixteen = "\uE1D9";//"\U0001D161";
        public const string ThirtyTwo = "\uE1DB";//"\U0001D162";
        public const string SixstyFour = "\uE1DD";// "\U0001D163";
        //notes upsidedown
        public const string WholeU = "\uE1D1";// "\U0001D117";
        public const string HalfU = "\uE1D4";//"\U0001D15E";
        public const string QuarterU = "\uE1D6";//"\U0001D15F";
        public const string EightU = "\uE1D8";//"\uE1D7";"\uECA7"
        public const string SixteenU = "\uE1DA";//"\U0001D161";
        public const string ThirtyTwoU = "\uE1DC";//"\U0001D162";
        public const string SixstyFourU = "\uE1DE";// "\U0001D163";
        //rests
        public const string WholeRest = "\uE4F4";
        public const string HalfRest = "\uE4F5";
        public const string QuarterRest = "\uE4E5";
        public const string EightRest = "\uE4E6";
        public const string SixteenRest = "\uE4E7";
        public const string ThirtyTwoRest = "\uE4E8";
        public const string SixstyFourRest = "\uE4E9";
        public const string MultiMeasureRest = "\uE4EE";
        //primitives
        public const string HalfDot = "\uE0A3";
        public const string QuarterDot = "\uE0A4";
        //public const string DurationDot = "\U0001D157";
        public const string NoteLine = "\uE016";

        public const string FlagEight = "\U0001D16E";
        public const string FlagSixteen = "\U0001D16f";
        public const string FlagThirtyTwo = "\U0001D170";
        public const string FlagSixstyFour = "\U0001D171";
        //articulations
        public const string Accent = "\uE4A0";
        public const string AccentBelow = "\uE4A1";
        public const string Staccato = "\uE4A2";
        public const string StaccatoBelow = "\uE4A3";
        public const string Tenuto = "\uE4A4";
        public const string TenutoBelow = "\uE4A5";
        public const string Marcato = "\uE4AC";
        public const string MarcatoBelow = "\uE4AD";
        //misc
        public const string Natural = "\uE261";
        public const string Sharp = "\uE262";
        public const string Flat = "\uE260";
        public const string DoubleSharp = "\uE263";
        public const string DoubleFlat = "\uE264";
        //ornaments
        public const string trillmark = "\uE566";
        public const string turn = "\uE567";
        public const string delayed_turn = "\uE569";
        public const string inverted_turn = "\uE568";
        public const string delayed_inverted_turn = "dit?";
        public const string vertical_turn = "\uE56A";
        public const string shake = "\uE56E";
        public const string wavy_line = "\uE5E4";
        public const string mordent = "\uE56D";
        public const string inverted_mordent = "\uE56C";
        public const string schleifer ="\uE587";
        public const string tremolo = "\uE220";
        
        public const string DalSegno = "\uE045";
        public const string DaCapo = "\uE046";
        public const string Segno = "\uE047";
        public const string Coda = "\uE048";
        public const string Breath = "\uE4CE";
        public const string Fermata = "\uE4C0";
        public const string FermataBelow = "\uE4C1";
        //dynamics
        public const string Rinforzando = "\uE523";
        public const string Subito ="\uE524";
        public const string Z_mus = "\uE525";
        public const string Piano = "\uE520";
        public const string Mezzo = "\uE521";
        public const string Forte = "\uE522";
        public const string Crescendo = "\uE53E";
        public const string Decrescendo = "\uE53F";
        //time
        public const string CommonTime = "\uE08A";
        public const string CutTime = "\uE08B";

        public const string one = "\uF55B";
        public const string two = "\uF55D";
        public const string three = "\uF55F";
        public const string four = "\uF561";
        public const string five = "\uF563";
        public const string six = "\uF565";
        public const string seven = "\uF567";
        public const string eight = "\uF569";
        public const string nine = "\uF56B";
        public const string zero = "\uF56D";
        //time beats
        public const string oneT = "\uF55A";
        public const string twoT = "\uF55C";
        public const string threeT = "\uF55E";
        public const string fourT = "\uF560";
        public const string fiveT = "\uF562";
        public const string sixT = "\uF564";
        public const string sevenT = "\uF566";
        public const string eightT = "\uF568";
        public const string nineT = "\uF56A";
        public const string zeroT = "\uF56C";

        public static string getNoteSymbol(MusSymbolDuration m)
        {
            string s = "??";
            
            if (DurationSymbol_Note.ContainsKey(m))
            {
                s = DurationSymbol_Note[m];
            }
            return s;
        }
        public static string getNoteSymbol(string s,bool d)
        {
            string x = "n?";
            if (d==false)
            {
                if (DurationSymbol_Note_S.ContainsKey(s))
                {
                    x = DurationSymbol_Note_S[s];
                }
            }
            else
            {
                if (DurationSymbol_Note_S_R.ContainsKey(s))
                {
                    x = DurationSymbol_Note_S_R[s];
                }
            }
            return x;
        }

        public static string getRestSymbol(string s)
        {
            MusSymbolDuration m = SymbolDuration.d_type(s);
            s = "??";
            if (DurationSymbol_Rest.ContainsKey(m))
            {
                s = DurationSymbol_Rest[m];
            }
            
            return s;
        }

        public static Dictionary<MusSymbolDuration, string> DurationSymbol_Note = new Dictionary<MusSymbolDuration, string>() {
            { MusSymbolDuration.Whole, Whole},
            { MusSymbolDuration.Half, Half},
            { MusSymbolDuration.Quarter, Quarter},
            { MusSymbolDuration.Eight, Eight},
            { MusSymbolDuration.Sixteen, Sixteen},
            { MusSymbolDuration.d32nd, ThirtyTwo},
            { MusSymbolDuration.d64th, SixstyFour},
            { MusSymbolDuration.Unknown, "n?" }
        };
        public static Dictionary<string, string> DurationSymbol_Note_S = new Dictionary<string, string>() {
            { "whole", Whole},
            { "half", Half},
            { "quarter", Quarter},
            { "eighth", Eight},
            { "16th", Sixteen},
            { "32nd", ThirtyTwo},
            { "64th", SixstyFour},
            { "", "n?" }
        };
        public static Dictionary<string, string> DurationSymbol_Note_S_R = new Dictionary<string, string>() {
            { "whole", Whole},
            { "half", HalfU},
            { "quarter", QuarterU},
            { "eighth", EightU},
            { "16th", SixteenU},
            { "32nd", ThirtyTwoU},
            { "64th", SixstyFourU},
            { "", "n?" }
        };
        public static Dictionary<MusSymbolDuration, string> DurationSymbol_Rest = new Dictionary<MusSymbolDuration, string>() {
            { MusSymbolDuration.Whole, WholeRest},
            { MusSymbolDuration.Half, HalfRest},
            { MusSymbolDuration.Quarter, QuarterRest},
            { MusSymbolDuration.Eight, EightRest},
            { MusSymbolDuration.Sixteen, SixteenRest},
            { MusSymbolDuration.d32nd, ThirtyTwoRest},
            { MusSymbolDuration.d64th, SixstyFourRest},
            { MusSymbolDuration.Unknown, "r?" }
        };
    }
}
