using MusicXMLScore.Model.MeasureItems.Attributes;
using System.Linq;
using MusicXMLScore.Helpers;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses
{
    abstract class AbstractTime : IVisualHost
    {
        private string beatTime;
        private string beatType;
        private int[] compoundBeat;
        private int[] compoundTime;
        private readonly AbstractStaff staff;
        private TimeSymbolMusicXML timeSymbol;
        private readonly DrawingVisualHost visualHost;

        protected AbstractTime(string beatTime, string beatType, AbstractStaff staff)
        {
            visualHost = new DrawingVisualHost();
            SetTimeAndBeats(beatTime, beatType);
            this.staff = staff;
            this.timeSymbol = TimeSymbolMusicXML.normal;
        }
        protected AbstractTime(string beatTime, string beatType, TimeSymbolMusicXML timeSymbol, AbstractStaff staff)
        {
            visualHost = new DrawingVisualHost();
            this.timeSymbol = timeSymbol;
            SetTimeAndBeats(beatTime, beatType);
            this.staff = staff;
        }
        public string BeatTime { get => beatTime; }
        public string BeatType { get => beatType; }

        public TimeSymbolMusicXML TimeSymbol { get => timeSymbol; set => timeSymbol = value; }

        internal AbstractStaff Staff => staff;

        public DrawingVisualHost GetVisualsContainer()
        {
            return visualHost;
        }

        public abstract double GetVisualWidth();

        public void SetTimeAndBeats(string beatTime, string beatType)
        {
            this.beatTime = beatTime;
            this.beatType = beatType;
            if (beatTime.Contains("+"))
            {
                var compound = beatTime.Split('+').ToList();
                compoundTime = new int[compound.Count];
                for (int i = 0; i < compound.Count; i++)
                {
                    compoundTime[i] = int.Parse(compound[i]);
                }
                compoundBeat = new int[] { int.Parse(beatType) };
            }
            else
            {
                compoundBeat = new int[] { int.Parse(beatType) };
                compoundTime = new int[] { int.Parse(beatTime) };
            }
        }

        public abstract void Update();
    }
}
