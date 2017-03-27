using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.Model.MeasureItems.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.ScoreProperties
{
    public class TimeSignatures
    {
        Dictionary<string, TimeMusicXML> timeSignatures = new Dictionary<string, TimeMusicXML>();
        MusicXMLViewerWPF.ScorePartwiseMusicXML score;

        /// <summary>
        /// Dictionary of time signature changes measureNumber as Key
        /// </summary>
        public Dictionary<string, TimeMusicXML> TimeSignaturesDictionary
        {
            get
            {
                return timeSignatures;
            }

            set
            {
                timeSignatures = value;
            }
        }

        public TimeSignatures(MusicXMLViewerWPF.ScorePartwiseMusicXML score)
        {
            this.score = score;
            if (score.Part != null)
            {
                GenerateTimeSignaturesPerMeasure();
            }
        }
        private void GenerateTimeSignaturesPerMeasure() //TODO_Later refactor to no cloning, but finding first going back from measure.Number, which provide only clef changes in dictionary
        {
            var firstPart = score.Part.FirstOrDefault();
            TimeMusicXML currentTimeSignature = new TimeMusicXML();
            foreach (var measure in firstPart.Measure)
            {
                if (measure.Items.OfType<AttributesMusicXML>().FirstOrDefault() != null)
                {
                    var attributes = measure.Items.OfType<AttributesMusicXML>().FirstOrDefault();
                    if (attributes.Time.Count != 0)
                    {
                        var timeSig = attributes.Time.ElementAt(0);
                        currentTimeSignature = timeSig;
                        currentTimeSignature.PrintObject = Model.Helpers.SimpleTypes.YesNoMusicXML.yes;
                        timeSignatures.Add(measure.Number, currentTimeSignature);
                    }
                    else
                    {
                        TimeMusicXML time = currentTimeSignature.Clone();
                        time.PrintObject = Model.Helpers.SimpleTypes.YesNoMusicXML.no;
                        timeSignatures.Add(measure.Number, time);
                    }
                }
                else
                {
                    TimeMusicXML time = currentTimeSignature.Clone();
                    time.PrintObject = Model.Helpers.SimpleTypes.YesNoMusicXML.no;
                    timeSignatures.Add(measure.Number, time);
                }
              
            }
            //var test = timeSignatures.Select(i => i).Where(i => i.Value.PrintObject == Model.Helpers.SimpleTypes.YesNoMusicXML.yes);
        }
        
        /// <summary>
        /// Gets Time Signature of selected measureId
        /// </summary>
        /// <param name="measureId">MeasureId token, which is Measure.Number</param>
        /// <returns>Time attributes attached to measureId, PartId is not necessary due to all parts shares the same time signature</returns>
        public TimeMusicXML GetTimeSignature(string measureId)
        {
            TimeMusicXML time = timeSignatures[measureId];
            return time; //? new Model.MeasureItems.Attributes.TimeMusicXML();
        }
    }
}
