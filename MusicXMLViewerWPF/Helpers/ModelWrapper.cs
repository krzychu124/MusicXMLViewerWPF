using MusicXMLViewerWPF.ScoreParts.Part;
using MusicXMLViewerWPF.ScoreParts.Part.Measures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.Helpers
{
    /// <summary>
    /// Helper methods for calculating layout properties drawable objects
    /// </summary>
    class ModelWrapper
    {
        static Dictionary<int,List<Measure>> SortMeasuresToSystems(List<Measure> measureslist, bool AutoSystems = false)
        {
            
            if (measureslist == null) return null;
            Dictionary<int, List<Measure>> result = new Dictionary<int, List<Measure>>();
            List<Measure> temp = new List<Measure>();
            int systemindex = 1;
            for (int i = 0; i < measureslist.Count; i++)
            {
                
                var measure = measureslist.ElementAt(i);
                if (measure.IsFirstInLine)
                {
                    temp.Add(measure);
                    result.Add(systemindex, temp);
                    if (i == 0) continue;

                    temp.Clear();
                }
                else
                {
                    temp.Add(measure);
                }

            }
            return result;
        }

        static Dictionary<int, List<CustomPartSystem>> GetSystemPerPage()
        {
            return null;
        }
        static List<double> GetMaxMeasureWidths(List<Part> partlist)
        {
            if (partlist == null) return null;
            List<double> result = new List<double>();
            int measuresCount = partlist.ElementAt(0).MeasureSegmentList.Count;
            for (int j = 0; j < measuresCount; j++)
            {
                double max = 0;
                for (int i = 0; i < partlist.Count; i++)
                {
                    Measure m = partlist.ElementAt(i).MeasureSegmentList.ElementAt(j);
                    double temp = m.Width;
                    if (temp >= max)
                    {
                        max = temp;
                    }
                }
                result.Add(max);
            }

            return result;
        }

        static double GetMaxSystemWidth(CustomPartSystem cps)
        {
            return 0.0; //TODO_I
        }
    }
}
