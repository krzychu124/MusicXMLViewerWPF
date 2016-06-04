using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
        public bool isRelative;
        public Dynamics(XElement x) : base (x.Attributes())
        {
            placement = x.Attribute("placement") != null ?  x.Attribute("placement").Value : "below";
            SetDynType(x.Value);
        }
        public Dynamics( int y, string align, string t, int x=0)//TODO_L improve, currently temp, only for compatibility while reworking
        {
            isRelative = x != 0 ? true : false;
            def_x = x;
            def_y = y;
            h_align = align == "left" ? Halign.left : align == "center" ? Halign.center : Halign.right; 
            SetDynType(t);
        }
        public void SetDynType(string t)
        {
            if (dynType_dict.ContainsKey(t))
            {
                other = null;
                type = dynType_dict[t];
            }
            else
            {
                type = DynamicType.other;
                other = t;
            }
        }
        public Dictionary<string, DynamicType> dynType_dict = new Dictionary<string, DynamicType> {
            {"other",DynamicType.other },
            {"p", DynamicType.p },
            {"pp", DynamicType.pp },
            {"ppp", DynamicType.ppp },
            {"f", DynamicType.f },
            {"ff", DynamicType.ff },
            {"fff", DynamicType.fff },
            {"mp", DynamicType.mp },
            {"mf", DynamicType.mf },
            {"sf", DynamicType.sf },
            {"sfp", DynamicType.sfp },
            {"sfz", DynamicType.sfz }
            
        };
    }
    public enum DynamicType
    {
        other,
        p,
        pp,
        ppp,
        f,
        ff,
        fff,
        mp,
        mf,
        sf,
        sfp,
        sfz,
    }
}
