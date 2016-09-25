using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Pitch
    {
        private string step;
        private int stepid;
        private int alter;
        private int octave;
        private StepType step_;
        private int calculated_step;
        private bool underNote;
        private bool addedLine;
        private float additionalLines;

        public string Step { get { return step; } }
        public int StepId { get { return stepid; } }
        public int Alter { get { return alter; } }
        public int Octave { get { return octave; } }
        public StepType StepType { get { return step_; } }
        public float AdditionalLines { get { return additionalLines; } }
        public int CalculatedStep { get { return calculated_step; } }
        public bool isLineUnderNote { get { return underNote; } }
        public bool HasAddedLine { get { return addedLine; } }

        public Pitch()
        {

        }
        public Pitch(string s,int o)
        {
            step = s;
            octave = o;
            alter = 0;
            getStep(s);
            getStep(step);
            getPitch(step_);
            calculateStep();
            getAdditionalLines();
        }
        public Pitch(string s, int o, int alter)
        {
            step = s;
            octave = o;
            this.alter = alter;
            getStep(s);
            getStep(step);
            getPitch(step_);
            calculateStep();
            getAdditionalLines();
        }
        public Pitch(XElement x)
        {
            foreach (var item in x.Elements())
            {
                switch (item.Name.LocalName)
                {
                    case "step":
                        step = item.Value;
                        break;
                    case "alter":
                        alter = int.Parse(item.Value);
                        break;
                    case "octave":
                        octave = int.Parse(item.Value);
                        break;
                    default:
                        Logger.Log($"{item.Name.LocalName} not implemented");
                        break;
                }

            }
        }
        public void getPitch(StepType s )
        {
            stepid = (int)s;
        }
        public void getStep(StepType s)
        {
            if (dict.ContainsValue(s))
            {
                step = dict.First(x=>x.Value == s).Key;
            }
        }
        public void getStep(string s)
        {
            if (dict.ContainsKey(s))
            {
                step_ = dict[s];
            }
        }
        private void calculateStep()
        {
            calculated_step = ((octave - 4) * (-7) + StepId * -1) + Clef.ClefAlter;
        }
        private void getAdditionalLines()
        {
            int s = Math.Abs(calculated_step % 2);
            underNote = s == 1 ? true : false;
            addedLine = false;
            float num = calculated_step / 2;
            if (calculated_step >= 0)
            {
                addedLine = true;
                additionalLines = underNote == true ? num + s : num + s + 1;
            }
            if (calculated_step <= -12)
            {
                addedLine = true;
                num = (calculated_step + 12) / 2;
                additionalLines = underNote == true ? Math.Abs(num - s) : Math.Abs(num - s - 1);
            }
        }
        public static Dictionary<string, StepType> dict = new Dictionary<string, StepType>()
        {
            {"C",StepType.C },
            {"D",StepType.D },
            {"E",StepType.E },
            {"F",StepType.F },
            {"G",StepType.G },
            {"A",StepType.A },
            {"B",StepType.B },
        };
    }
    enum StepType
    {
        C,
        D,
        E,
        F,
        G,
        A,
        B
    }
}
