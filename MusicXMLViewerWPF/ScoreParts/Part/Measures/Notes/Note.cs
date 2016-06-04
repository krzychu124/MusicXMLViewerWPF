﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace MusicXMLViewerWPF
{
    class Note : MusicalChars
    { //Need to be reworked !! 1)little improvements done
        private Beam beam;
        private bool hasBeams;
        private bool hasDot;
        private bool hasNotations;
        private bool isDefaultStem;
        private bool isRest;
        private bool stem_dir;
        private float defaultStem;
        private float posX;
        private float posY;
        private float stem;
        private int dot;
        private int duration;
        private int id;
        private int measure_id;
        private int voice;
        private MusSymbolDuration symbol_type;
        private Pitch pitch;
        private string symbol;
        private List<Notations> notationsList;

        public Beam Beam { get { return beam; } }
        public bool HasBeams { get { return hasBeams; } }
        public bool HasDot { get { return hasDot; } }
        public bool HasNotations { get { return hasNotations; } }
        public bool isDefault_Stem { get { return isDefaultStem; } }
        public bool IsRest { get { return isRest; } }
        public bool Stem_dir { get { return stem_dir; } }
        public float DefaultStem { get { return defaultStem; } }
        public float PosX { get { return posX; } }
        public float PosY  { get { return posY; } }
        public float Stem { get { return stem; } }
        public int Dot { get { return dot; } }
        public int Duration { get { return duration; } }
        public int Id { get { return id; } }
        public int MeasureId { get { return measure_id; } }
        public int Voice { get { return voice; } }
        public MusSymbolDuration SymbolType { get { return symbol_type; } }
        public Pitch Pitch { get { return pitch; } }
        public string Symbol { get { return symbol; } }
        public List<Notations> NotationsList { get { return notationsList; } }
         /*
        public Note( int measure_id, int id,float pos, Pitch p, int dur,int v, string t, float s, string dir,bool r, int dot, bool notations)
        {
            this.measure_id = measure_id;
            this.type = MusSymbolType.Note;
            this.id = id;
            posX = pos;
            CalculatePitch(p);
            pitch = p;
            duration = dur;
            voice = v;
            stem_dir = dir == "up" ? false : true;
            symbol_type =SymbolDuration.d_type(t);
            symbol = MusChar.getNoteSymbol(t,stem_dir);
            isRest = r;
            this.dot = dot;
            hasDot = dot != 0 ? true : false;
            stem = s;
            hasNotations = notations;
            
        } */
        //public Notations notations;
        public Note(int measure_id, int id,float pos, Pitch p, int dur,int v, string t, float s, string dir, bool hasStemVal, bool r, int num, string bm,Dictionary<int, string> beamList, int dot, bool notations, List<Notations> n_list)
        {
            isDefaultStem = hasStemVal ? false : true;
            this.measure_id = measure_id;
            this.type = MusSymbolType.Note;
            this.id = id;
            posX = pos;
            CalculatePitch(p);
            pitch = p;
            duration = dur;
            voice = v;
            stem_dir = dir == "up" ? false : true;
            symbol_type =SymbolDuration.d_type(t);
            symbol = MusChar.getNoteSymbol(t,stem_dir);
            isRest = r;
            stem = s;
            this.dot = dot;
            hasDot = dot != 0 ? true : false;
            beam = new Beam(bm,num,pos,beamList);
            hasBeams = true;
            //beam.Add(b);
            defaultStem = CalculateStem();
            hasNotations = notations;
            notationsList = n_list;
        }
        private void CalculatePitch(Pitch p)
        {

            int o = p.Octava;
            float scale = Measures.Scale;
            this.posY = (p.CalculatedStep * 3.95f) + scale * 0.6f;
        }
            
        
        private float CalculateStem()
        {
            float c;
            c = (Measures.Scale * 0.75f );


            return c;
        }
    }

    public class Beam
    {
        
        private Beam_type typ;
        private Dictionary<int,int> beamlist = new Dictionary<int, int>();
        private float pos;
        private int number;

        public Beam_type BeamType { get { return typ; } }
        public Dictionary<int,int> NoteBeamList { get { return beamlist; } }
        public float Position { get { return pos; } }
        public int BeamNumber { get { return number; } }
        public int NoteBeamsCount { get { return beamlist.Count; } }
        
        public Beam( string type, int n, float p, Dictionary<int, string> l)
        {
            number = n;
            pos = p;
            typ = getBeamType(type);
            foreach (var item in l)
            {
                Beam_type b = getBeamType(item.Value);
                beamlist.Add(item.Key,Convert.ToInt32(b));
            }
            
        }
        private Beam_type getBeamType(string type)
        {
            Beam_type t = Beam_type.stop;
            switch (type)
            {
                case "begin":
                    t = Beam_type.start;
                    break;
                case "continue":
                    t = Beam_type.next;
                    break;
                case "end":
                    t = Beam_type.stop;
                    break;
                case "forward hook":
                    t = Beam_type.forward;
                    break;
                case "backward hook":
                    t = Beam_type.backward;
                    break;

            }
            return t;
        }

        public enum Beam_type
        {
            start,
            next,
            stop,
            forward,
            backward
         }
    }

    public enum DurationType
    {
        unknown,
        whole = 1,
        half = 2,
        quarter = 4,
        eight = 8,
        d16th = 16,
        d32nd = 32,
        d64th = 64
           
    }

    enum NoteStem
    {
        up = 0,
        down = 1
    }
  
}