using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.DrawingHelpers
{
    class MusicSymbols          ///add ornaments unicode symbols
    {
        private static string to_utf8(string s)
        {
            byte[] bytes = Encoding.Default.GetBytes(s);
            string myString = Encoding.UTF8.GetString(bytes);
            return myString;
        }
        //public static string Staff5L = to_utf8("\u1d110");
        #region clefs
        public const string FClef = "\uE062";
        public const string FClef8Up = "\uE065";
        public const string FClef15Up = "\uE066";
        public const string FClef8Down = "\uE064";
        public const string FClef15Down = "\uE063";
        public const string GClef = "\uE050";//"\ue050";//"\U0001D11E";
        public const string GClef8Up = "\uE053";
        public const string GClef8Down = "\uE052";
        public const string GClef15Up = "\uE054";
        public const string GClef15Down = "\uE051";
        public const string CClef = "\uE05C";
        public const string CClef8Down = "\uE05D";
        public const string Percussion = "\uE069";
        public const string TAB = "\uE06D";
        #endregion

        //static class StaffFiveLines
        //{
        //    private static string shortLineSymbol = "\uE020";
        //    private static string longLineSymbol = "\uE01A";
        //    private static string mediumLineSymbol = "uE014";

        //    public static string ShortLine { get { return shortLineSymbol; } }
        //    public static string MediumLine { get { return mediumLineSymbol; } }
        //    public static string LongLine { get { return longLineSymbol; } }
        //}
        //public class StaffLine
        //{
        //    private string shortLineSymbol = "\uE020";
        //    private string longLineSymbol = "\uE01A";
        //    private string mediumLineSymbol = "uE014";
        //    protected StaffLine(string s, string m, string l)
        //    {

        //    }
        //    public string ShortLine { get { return shortLineSymbol; } }
        //    public string MediumLine { get { return mediumLineSymbol; } }
        //    public string LongLine { get { return longLineSymbol; } }
        //}
        //public class StaffOneLine : StaffLine
        //{
        //    public StaffOneLine():base("\uE01C","\uE010","\uE022")
        //    {
        //    }
        //}
        //TODO_I Implementation missing
        #region staffs 
        #region oneLine
        public readonly string shortStaffOneSymbol = "\uE01C";
        public readonly string mediumStaffOneSymbol = "\uE010";
        public static readonly string longStaffOneSymbol = "\uE016";
        #endregion
        #region twoLine
        public readonly string shortStaffTwoSymbol;
        public readonly string mediumStaffTwoSymbol;
        public readonly string longStaffTwoSymbol;
        #endregion
        #region threeLine
        public readonly string shortStaffTreeSymbol;
        public readonly string mediumStaffThreeSymbol;
        public readonly string longStaffThreeSymbol;
        #endregion
        #region fourLine
        public readonly string shortStaffFourSymbol;
        public readonly string mediumStaffFourSymbol;
        public readonly string longStaffFourSymbol;
        #endregion
        #region fiveLine
        public readonly string shortStaffFiveSymbol = "\uE020";
        public static readonly string mediumStaffFiveSymbol = "\uE01A";
        public readonly string longStaffFiveSymbol = "uE014";
        #endregion
        #region sixLine
        public readonly string shortStaffSixSymbol;
        public readonly string mediumStaffSixSymbol;
        public readonly string longStaffSixSymbol;
        #endregion
        public const string Staff4L = "\uE019";
        public const string Staff5L = "\uE01A";//"\uE01A";// "\U0001D11A";   // x=8 size=10, 16-20,32-40
        public const string Staff5Ls = "\uE020";
        public const string Staff6L = "\uE01B";
        #endregion

        #region bars
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
        #endregion

        #region repeats
        public const string RepeatFigure = "\U0001D10D";
        public const string RepeatMeasure = "\uE500";
        public const string Repeat2Measures = "\uE501";
        #endregion

        #region gracenotes
        public const string GraceNote = "\uE562";
        public const string GraceNoteR = "\uE563";
        public const string GraceNoteSlash = "\uE560";
        public const string GraceNoteSlashR = "\uE561"; // R means upside-down
        public const string Dot = "\uE1E7";
        #endregion

        #region notes
        public const string Whole = "\uE1D2";// "\U0001D117";
        public const string Half = "\uE1D3";//"\U0001D15E";
        public const string Quarter = "\uE1D5";//"\U0001D15F";
        public const string Eight = "\uE1D7";//"\uE1D7";"\uECA7"
        public const string Sixteen = "\uE1D9";//"\U0001D161";
        public const string ThirtyTwo = "\uE1DB";//"\U0001D162";
        public const string SixstyFour = "\uE1DD";// "\U0001D163";
        #endregion

        #region notes upsidedown
        public const string WholeU = "\uE1D2";// "\U0001D117";
        public const string HalfU = "\uE1D4";//"\U0001D15E";
        public const string QuarterU = "\uE1D6";//"\U0001D15F";
        public const string EightU = "\uE1D8";//"\uE1D7";"\uECA7"
        public const string SixteenU = "\uE1DA";//"\U0001D161";
        public const string ThirtyTwoU = "\uE1DC";//"\U0001D162";
        public const string SixstyFourU = "\uE1DE";// "\U0001D163";
        #endregion

        #region rests
        public const string WholeRest = "\uE4F4";
        public const string HalfRest = "\uE4F5";
        public const string QuarterRest = "\uE4E5";
        public const string EightRest = "\uE4E6";
        public const string SixteenRest = "\uE4E7";
        public const string ThirtyTwoRest = "\uE4E8";
        public const string SixstyFourRest = "\uE4E9";
        public const string MultiMeasureRest = "\uE4EE";
        #endregion

        #region primitives
        public const string WholeDot = "\uE0A2";
        public const string HalfDot = "\uE0A3";
        public const string QuarterDot = "\uE0A4";
        //public const string DurationDot = "\U0001D157";
        public const string NoteLine = "\uE010";

        public const string FlagEight = "\U0001D16E";
        public const string FlagSixteen = "\U0001D16f";
        public const string FlagThirtyTwo = "\U0001D170";
        public const string FlagSixstyFour = "\U0001D171";
        #endregion

        #region articulations
        public const string Accent = "\uE4A0";
        public const string AccentBelow = "\uE4A1";
        public const string Staccato = "\uE4A2";
        public const string StaccatoBelow = "\uE4A3";
        public const string Tenuto = "\uE4A4";
        public const string TenutoBelow = "\uE4A5";
        public const string Marcato = "\uE4AC";
        public const string MarcatoBelow = "\uE4AD";
        #endregion

        #region misc
        public const string Natural = "\uE261";
        public const string Sharp = "\uE262";
        public const string Flat = "\uE260";
        public const string DoubleSharp = "\uE263";
        public const string DoubleFlat = "\uE264";
        public const string RepeatDots = "\uE043";
        #endregion

        #region ornaments
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
        #endregion

        #region repeat signs
        public const string DalSegno = "\uE045";
        public const string DaCapo = "\uE046";
        public const string Segno = "\uE047";
        public const string Coda = "\uE048";
        public const string Breath = "\uE4CE";
        public const string Fermata = "\uE4C0";
        public const string FermataBelow = "\uE4C1";
        #endregion

        #region dynamics_by_name
        public const string Rinforzando = "\uE523";
        public const string Subito ="\uE524";
        public const string Z_mus = "\uE525";
        public const string Piano = "\uE520";
        public const string Mezzo = "\uE521";
        public const string Forte = "\uE522";
        public const string Crescendo = "\uE53E";
        public const string Decrescendo = "\uE53F";
        #endregion

        #region dynamics
        public static string f = "\uE522";
        public static string ff = "\uE52F";
        public static string fff = "\uE530";
        public static string ffff = "\uE531";
        public static string fp = "\uE534";
        public static string fz = "\uE535";
        public static string m = "\uE521";
        public static string mf = "\uE52D";
        public static string mp = "\uE52C";
        public static string n = "\uE526";
        public static string p = "\uE520";
        public static string pf = "\uE52E";
        public static string pp = "\uE52B";
        public static string ppp = "\uE52A";
        public static string pppp = "\uE529";
        public static string r = "\uE523";
        public static string rf = "\uE53C";
        public static string rfz = "\uE53D";
        public static string s = "\uE524";
        public static string sf = "\uE536";
        public static string sffz = "\uE53B";
        public static string sfp = "\uE537";
        public static string sfpp = "\uE538";
        public static string sfz = "\uE539";
        public static string sfzp = "\uE53A";
        public static string z = "\uE525";
        #endregion

        #region time signs
        public const string CommonTime = "\uE08A";
        public const string CutTime = "\uE08B";
        #region time numbers
        public const string one = "\uF55B";
        public const string two = "\uF55D";
        public const string three = "\uF55F";
        public const string four = "\uF561";
        public const string five = "\uF563";
        public const string six = "\uF565";
        public const string seven = "\uF567";
        public const string eight = "\uF569";
        public const string nine = "\uf466"; //! e089"; //! "\uF56B"; 
        public const string zero = "\uF559";
        #endregion
        #region time beats
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
        #endregion
        #region Custom Time
        public const string TZero ="\uE080";
        public const string TOne = "\uE081";
        public const string TTwo = "\uE082";
        public const string TThree = "\uE083";
        public const string TFour = "\uE084";
        public const string TFive = "\uE085";
        public const string TSix = "\uE086";
        public const string TSeven = "\uE087";
        public const string TEight = "\uE088";
        public const string TNine = "\uE089";
        #endregion
        #endregion

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
            if (d==true)
            {
                if (DurationSymbol_Note_S.ContainsKey(s))
                {
                    x = DurationSymbol_Note_S[s];
                }
            }
            else
            {
                if (s == null) return x;
                if (DurationSymbol_Note_S_R.ContainsKey(s))
                {
                    x = DurationSymbol_Note_S_R[s];
                }
            }
            return x;
        }

        public static string getRestSymbol(string s)
        {
            MusSymbolDuration m = SymbolDuration.DurStrToMusSymbol(s);
            s = "??";
            if (DurationSymbol_Rest.ContainsKey(m))
            {
                s = DurationSymbol_Rest[m];
            }
            
            return s;
        }

        public static Dictionary<MusSymbolDuration, string> DurationSymbol_Note = new Dictionary<MusSymbolDuration, string>() {
            { MusSymbolDuration.Whole, WholeDot}, //! temp //
            { MusSymbolDuration.Half, Half},
            { MusSymbolDuration.Quarter, Quarter},
            { MusSymbolDuration.Eight, Eight},
            { MusSymbolDuration.Sixteen, Sixteen},
            { MusSymbolDuration.d32nd, ThirtyTwo},
            { MusSymbolDuration.d64th, SixstyFour},
            { MusSymbolDuration.Unknown, "n?" }
        };
        public static Dictionary<string, string> DurationSymbol_Note_S = new Dictionary<string, string>() {
            { "whole", WholeDot},
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
        public static string GetCustomTimeNumber(string number)
        {
            string result = string.Empty;
            var array = number.ToCharArray();
            foreach (var item in array)
            {
                result += customTimeNumbers[int.Parse(item.ToString())];
            }
            return result;
        }
        private static Dictionary<int, string> customTimeNumbers = new Dictionary<int, string>()
        {
            {0,TZero },
            {1,TOne },
            {2,TTwo },
            {3,TThree },
            {4,TFour },
            {5,TFive },
            {6,TSix },
            {7,TSeven },
            {8,TEight },
            {9,TNine },
        };
    }
}
