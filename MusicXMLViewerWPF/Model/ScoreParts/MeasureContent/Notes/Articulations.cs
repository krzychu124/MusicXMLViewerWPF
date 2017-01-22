using System.Collections.Generic;

namespace MusicXMLViewerWPF
{
    class Articulations : Notations
    {
        private new ArticulationType type;
        private float x;
        private float y;
        private bool placement;
        private string symbol;
        //private DynamicType dyn_type;
        
        public ArticulationType Type { get { return type; } }
        public float X { get { return x; } }
        public float Y { get { return y; } }
        public bool Placement { get { return placement; } }
        public string Symbol { get { return symbol; } }


        public Articulations(string t, int x, int y, string p)
        {
            setNotationType("articulation");
            setType(t);
            this.x = x;
            this.y = y;
            placement = p =="below"? false: true;
            setSymbol(type);
        }

        public void setType(string t)
        {
            if (articulation_dict.ContainsKey(t))
            {
                type = articulation_dict[t];
            } 
        }

        public void setSymbol(ArticulationType type)
        {
            if (articulation_symbols_dict.ContainsKey(type))
            {

                if (placement)
                {
                    symbol = articulation_symbols_dict[type+1];
                }
                else
                {
                    symbol = articulation_symbols_dict[type];
                }
            }
           
        }

        public Dictionary<string, ArticulationType> articulation_dict = new Dictionary<string, ArticulationType> {
           {"other", ArticulationType.other },
           {"accent", ArticulationType.accent },
           {"strong_accent", ArticulationType.strong_accent},
           {"staccato", ArticulationType.staccato },
           {"staccatissimo", ArticulationType.staccatissimo },
           {"tenuto", ArticulationType.tenuto },
           {"breath", ArticulationType.breath },
        };

        public Dictionary<ArticulationType, string> articulation_symbols_dict = new Dictionary<ArticulationType, string>{
            {ArticulationType.other, "?" },
            {ArticulationType.accent, articulationSymbols.Accent },
            {ArticulationType.accent_below, articulationSymbols.AccentBelow },
            {ArticulationType.staccato, articulationSymbols.Staccato },
            {ArticulationType.staccato_below, articulationSymbols.StaccatoBelow },
            {ArticulationType.tenuto, articulationSymbols.Tenuto },
            {ArticulationType.tenuto_below, articulationSymbols.TenutoBelow },
            {ArticulationType.breath, articulationSymbols.Breath },
            {ArticulationType.staccatissimo, articulationSymbols.Staccatissimo },
            {ArticulationType.staccatissimo_below, articulationSymbols.Staccatissimo },
            {ArticulationType.strong_accent, articulationSymbols.Marcato },
            {ArticulationType.strong_accent_below, articulationSymbols.MarcatoBelow },
        };

        


        static class articulationSymbols
        {
            public const string Accent = "\uE4A0";
            public const string AccentBelow = "\uE4A1";
            public const string Staccato = "\uE4A2";
            public const string StaccatoBelow = "\uE4A3";
            public const string Staccatissimo = "\uE4A6";
            public const string StaccatissimoBelow = "\uE4A7";
            public const string Tenuto = "\uE4A4";
            public const string TenutoBelow = "\uE4A5";
            public const string Marcato = "\uE4AC";
            public const string MarcatoBelow = "\uE4AD";


            public const string Breath = "\uE4CE";
            
        }
    }
    enum ArticulationType
    {
        other =0,
        accent,
        accent_below,
        strong_accent,
        strong_accent_below,
        staccato,
        staccato_below,
        staccatissimo,
        staccatissimo_below,
        tenuto,
        tenuto_below,
        breath
    }
}