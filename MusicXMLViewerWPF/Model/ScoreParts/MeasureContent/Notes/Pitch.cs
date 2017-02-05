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
        private int octave = -1;
        private StepType _step;
        private int calculated_step;
        private bool underNote;
        private bool addedLine;
        private float additionalLines;
        private int clefalter;

        public string Step { get { return step; } }
        public int StepId { get { return stepid; } }
        public int Alter { get { return alter; } }
        public int Octave { get { return octave; } }
        public StepType StepType { get { return _step; } private set { _step = value; } }
        public float AdditionalLines { get { return additionalLines; } }
        public int CalculatedStep { get { return calculated_step; } private set { calculated_step = value; } }
        public bool isLineUnderNote { get { return underNote; } }
        public bool HasAddedLine { get { return addedLine; } }
        public int ClefAlter { get { return clefalter; } set { clefalter = value; } }
        public Pitch()
        {

        }
        public Pitch(string step,int octave)
        {
            this.step = step;
            this.octave = octave;
            alter = 0;
            getStep(step);
            getStep(this.step);
            getPitch(_step);
            calculateStep();
            getAdditionalLines();
        }
        public Pitch(string step, int octave, int alter)
        {
            ClefAlter = alter;
            this.step = step;
            _step = getStep(step);
            getPitch(_step);
            this.alter = alter;
            this.octave = octave;
            StepType = getStep(Step);
            if (step != null && octave != -1)
            {
                CalculateStep();
            }
            //calculateStep();
            //getAdditionalLines();
        }
        public Pitch(XElement x, int clefalter)
        {
            ClefAlter = clefalter;
            foreach (var item in x.Elements())
            {
                switch (item.Name.LocalName)
                {
                    case "step":
                        step = item.Value;
                        _step = getStep(step);
                        getPitch(_step);
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
            StepType = getStep(Step);
            if (step != null && octave != -1)
            {
                CalculateStep();
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
        public StepType getStep(string s)
        {
            StepType _s = StepType.C;
            if (dict.ContainsKey(s))
            {
                _s = dict[s];
            }
            return _s;
        }
        private void CalculateStep()
        {
            CalculatedStep = ((octave - 4) * (-7) + StepId * -1) + ClefAlter;
        }
        private void calculateStep()
        {
            calculated_step = ((octave - 4) * (-7) + StepId * -1) + Clef.ClefAlterNote;
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
