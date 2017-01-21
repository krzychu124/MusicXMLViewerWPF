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
    public class Dynamics : EmptyPrintStyle //? Feature - check if not coliding with other object when drawing  //TODO 
    {
        //public int posX;
       // public int posY;
        private string other;
        private string placement;
        //public int halign;
        private DynamicType type;
        private string symbol;
        private string measureid;

        public string Other { get { return other; } }
        public string Placement { get { return placement; } }
        public DynamicType Type { get { return type; } }
        public string Symbol { get { return symbol; } }
        public string MeasureID { get { return measureid; } set { measureid = value; } }
        public bool isRelative;

        public Dynamics(XElement x) : base (x.Element("dynamics").Attributes())
        {
            placement = x.Attribute("placement") != null ?  x.Attribute("placement").Value : "below";
            SetDynType(x.Elements().ToList());
            SetStringSymbol(Type);
        }

        public Dynamics( int y, string align, string t, int x=0)
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
        #region void Draw(DV, Point) not usage for now
        public void Draw(DrawingVisual visual, System.Windows.Point p)
        {
            DrawingVisual dynamic_visual = new DrawingVisual();
            using (DrawingContext dc = dynamic_visual.RenderOpen())
            {
                float posY = (float)p.Y + 50f;
                Misc.DrawingHelpers.DrawString(dc, Symbol, TypeFaces.MeasuresFont, Brushes.Black, (float)p.X, posY, 40/2.2f);
            }
            visual.Children.Add(dynamic_visual);
        }
        #endregion

        public void Draw(DrawingVisual visual)
        {
            Point position = new Point();
            if (DefX != 0)
            {
                position.X += DefX;
            }
            ScoreParts.Part.Measures.Measure measure = (ScoreParts.Part.Measures.Measure)Misc.ScoreSystem.GetMeasureSegment(MeasureID);
            position = new Point(position.X + measure.Relative.X, position.Y + measure.Relative.Y);
            DrawingVisual dynamic_visual = new DrawingVisual();
            using (DrawingContext dc = dynamic_visual.RenderOpen())
            {
                if (Placement == "below")
                {
                    position.Y += measure.Height * 0.7;
                }
                Misc.DrawingHelpers.DrawString(dc, Symbol, TypeFaces.MeasuresFont, Brushes.Black, (float)position.X, (float)position.Y, 40 * 0.45f);
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
            {DynamicType.f, MusicalChars.f },
            {DynamicType.ff, MusicalChars.ff },
            {DynamicType.fff, MusicalChars.fff },
            {DynamicType.ffff, MusicalChars.ffff},
            {DynamicType.fp, MusicalChars.fp },
            {DynamicType.fz, MusicalChars.fz },
            {DynamicType.m, MusicalChars.m  },
            {DynamicType.mf, MusicalChars.mf },
            {DynamicType.mp, MusicalChars.mp },
            {DynamicType.n, MusicalChars.n },
            {DynamicType.p, MusicalChars.p },
            {DynamicType.pf, MusicalChars.pf },
            {DynamicType.pp, MusicalChars.pp },
            {DynamicType.ppp, MusicalChars.ppp },
            {DynamicType.pppp, MusicalChars.pppp },
            {DynamicType.r, MusicalChars.r },
            {DynamicType.rf, MusicalChars.rf },
            {DynamicType.rfz, MusicalChars.rfz },
            {DynamicType.s, MusicalChars.s },
            {DynamicType.sf, MusicalChars.sf },
            {DynamicType.sffz, MusicalChars.sffz },
            {DynamicType.sfp, MusicalChars.sfp },
            {DynamicType.sfpp, MusicalChars.sfpp },
            {DynamicType.sfz, MusicalChars.sfz },
            {DynamicType.sfzp, MusicalChars.sfzp },
            {DynamicType.z, MusicalChars.z }
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
