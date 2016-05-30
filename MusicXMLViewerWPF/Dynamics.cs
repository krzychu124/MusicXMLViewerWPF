using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF
{
    class Dynamics
    {
        public int posX;
        public int posY;
        public string placement;
        public int halign;
        public DynamicType type;
        public bool isRelative;
        public Dynamics()
        {

        }
        public Dynamics( int y, int align, string t, int x=0)
        {
            isRelative = x != 0 ? true : false;
            this.posX = x;
            this.posY = y;
            halign = align;
            setDynType(t);
        }
        public void setDynType(string t)
        {
            if (dynType_dict.ContainsKey(t))
            {
                type = dynType_dict[t];
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
