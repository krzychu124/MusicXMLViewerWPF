using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    public class Dynamics : EmptyPrintStyle// need rework
    {
        //public int posX;
       // public int posY;
        private string other;
        private string placement;
        //public int halign;
        private DynamicType type;
        private string symbol;

        public string Other { get { return other; } }
        public string Placement { get { return placement; } }
        public DynamicType Type { get { return type; } }
        public string Symbol { get { return symbol; } }
        public bool isRelative;

        public Dynamics(XElement x) : base (x.Attributes())
        {
            placement = x.Attribute("placement") != null ?  x.Attribute("placement").Value : "below";
            SetDynType(x.Elements().ToList());
            SetStringSymbol(Type);
        }

        public Dynamics( int y, string align, string t, int x=0)//TODO_L improve, currently temp, only for compatibility while reworking
        {
            isRelative = x != 0 ? true : false;
            def_x = x;
            def_y = y;
            h_align = align == "left" ? Halign.left : align == "center" ? Halign.center : Halign.right; 
            SetDynType(t);
            SetStringSymbol(Type);
        }

        public void SetDynType(List<XElement> x)
        {
           
            foreach (var item in x.Elements())
            {
                string t = item.Name.LocalName;
                symbol = t;
                if (stringToDynType_dict.ContainsKey(t))
                {
                    other = null;
                    type = stringToDynType_dict[t];
                }
                else
                {
                    type = DynamicType.other;
                    other = t;
                }
            }
        }

        public void SetDynType(string t)
        {
            symbol = t;
            if (stringToDynType_dict.ContainsKey(t))
            {
                other = null;
                type = stringToDynType_dict[t];
            }
            else
            {
                type = DynamicType.other;
                other = t;
            }
        }

        public void SetStringSymbol(DynamicType t)
        {
            if (dynTypeToSymbol_dict.ContainsKey(t))
            {
                other = null;
                symbol = dynTypeToSymbol_dict[t];
            }
        }

        public void Draw(DrawingVisual visual, System.Windows.Point p)
        {
            DrawingVisual dynamic_visual = new DrawingVisual();
            using (DrawingContext dc = dynamic_visual.RenderOpen())
            {
                float posY = (float)p.Y + 50f;
                Misc.DrawingHelpers.DrawString(dc, Symbol, TypeFaces.MeasuresFont, Brushes.Black, (float)p.X, posY, MusicScore.Defaults.Scale.Tenths/2.2f);
            }
            visual.Children.Add(dynamic_visual);
        }

        public Dictionary<string, DynamicType> stringToDynType_dict = new Dictionary<string, DynamicType> {
            {"other", DynamicType.other },
            {"f", DynamicType.f },
            {"ff", DynamicType.ff },
            {"fff", DynamicType.fff },
            {"ffff", DynamicType.ffff },
            {"fp", DynamicType.fp },
            {"fz", DynamicType.fz },
            {"m", DynamicType.m },
            {"mf", DynamicType.mf },
            {"mp", DynamicType.mp },
            {"n", DynamicType.n },
            {"p", DynamicType.p },
            {"pp", DynamicType.pp },
            {"ppp", DynamicType.ppp },
            {"pppp", DynamicType.pppp },
            {"r", DynamicType.r },
            {"rf", DynamicType.rf },
            {"rfz", DynamicType.rfz },
            {"s", DynamicType.s },
            {"sf", DynamicType.sf },
            {"sffz", DynamicType.sffz },
            {"sfp", DynamicType.sfp },
            {"sfpp", DynamicType.sfpp },
            {"sfz", DynamicType.sfz },
            {"z", DynamicType.z },
        };

        public Dictionary<DynamicType, string> dynTypeToSymbol_dict = new Dictionary<DynamicType, string>
        {
            {DynamicType.other, "?Dyn?" },
            {DynamicType.f, MusChar.f },
            {DynamicType.ff, MusChar.ff },
            {DynamicType.fff, MusChar.fff },
            {DynamicType.ffff, MusChar.ffff},
            {DynamicType.fp, MusChar.fp },
            {DynamicType.fz, MusChar.fz },
            {DynamicType.m, MusChar.m  },
            {DynamicType.mf, MusChar.mf },
            {DynamicType.mp, MusChar.mp },
            {DynamicType.n, MusChar.n },
            {DynamicType.p, MusChar.p },
            {DynamicType.pf, MusChar.pf },
            {DynamicType.pp, MusChar.pp },
            {DynamicType.ppp, MusChar.ppp },
            {DynamicType.pppp, MusChar.pppp },
            {DynamicType.r, MusChar.r },
            {DynamicType.rf, MusChar.rf },
            {DynamicType.rfz, MusChar.rfz },
            {DynamicType.s, MusChar.s },
            {DynamicType.sf, MusChar.sf },
            {DynamicType.sffz, MusChar.sffz },
            {DynamicType.sfp, MusChar.sfp },
            {DynamicType.sfpp, MusChar.sfpp },
            {DynamicType.sfz, MusChar.sfz },
            {DynamicType.sfzp, MusChar.sfzp },
            {DynamicType.z, MusChar.z }
        };
    }

    public enum DynamicType
    {
        other,
        f,
        ff,
        fff,
        ffff,
        fp,
        fz,
        m,
        mf,
        mp,
        n,
        p,
        pf,
        pp,
        ppp,
        pppp,
        r,
        rf,
        rfz,
        s,
        sf,
        sffz,
        sfp,
        sfpp,
        sfz,
        sfzp,
        z
    }
}
