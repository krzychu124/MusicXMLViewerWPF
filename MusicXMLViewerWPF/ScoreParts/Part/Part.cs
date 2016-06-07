using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MusicXMLViewerWPF.ScoreParts.Part
{
    class Part
    {
        private string part_id;
        private List<Measures.Measure> measure_list = new List<Measures.Measure>();
        private Dictionary<int, Point> measure_margin_helper = new Dictionary<int, Point>() { }; /// <summary> helper list to store coordinates of starting point for every line </summary>
                                                      
        public string Id { get { return part_id; } }
        public List<Measures.Measure> MeasureList { get { return measure_list; } }
        public Dictionary<int,Point> MarginHelper { get { return measure_margin_helper; } }

        public Part(XElement x)
        {
            part_id = x.Attribute("id").Value;
            Point tempPoint = new Point();
            var measures = x.Elements();
            for (int i = 0; i < measures.Count(); i++)
            {
                XElement item = measures.ElementAt(i);
                measure_list.Add(new Measures.Measure(item)); // 

                if (i == 0)
                {
                    measure_margin_helper.Add(i, new Point(MusicScore.Defaults.Page.Margins.Left, MusicScore.Defaults.Page.Margins.Top));
                    Logger.Log($"helper_dict measure-page margins {MusicScore.Defaults.Page.Margins.Left}  {MusicScore.Defaults.Page.Margins.Top}");
                }
                else
                {
                    if (measure_list.ElementAt(i).PrintProperties != null)
                    {
                        if (measure_list.ElementAt(i).PrintProperties.NewSystem)
                        {
                            if (measure_list.ElementAt(i).PrintProperties.SystemLayout != null)
                            {
                                tempPoint.X += measure_list.ElementAt(i).PrintProperties.SystemLayout.LeftRelative;
                                tempPoint.Y += measure_list.ElementAt(i).PrintProperties.SystemLayout.RightRelative;
                                measure_margin_helper.Add(i, tempPoint);
                                Logger.Log($"helper_dict measure nr. {i} added point {tempPoint.X} {tempPoint.Y} ");
                            }

                            //measure_margin_helper.Add(i, new Point(measure_margin_helper.ElementAt(measure_margin_helper.Count).Value.X, measure_margin_helper.ElementAt(measure_margin_helper.Count).Value.Y));
                        }
                        else
                        {
                            Logger.Log($"helper_dict measure nr. {i + 1}");
                        }
                        measure_margin_helper.Add(measure_list.ElementAt(i).Number, measure_margin_helper.ElementAt(measure_margin_helper.Count - 1).Value);
                        Logger.Log($"helper_dict measure nr. {measure_margin_helper.ElementAt(measure_margin_helper.Count -1).Value.X} {measure_margin_helper.ElementAt(measure_margin_helper.Count -1).Value.Y}");

                    }
                }
            }
            foreach (var item in measure_margin_helper)
            {
                Logger.Log($"helper_dict measure nr. {item.Key} X: {item.Value.X} Y: {item.Value.Y} "); 
            }
            //foreach (var item in measures)
            //{
            //    measure_list.Add(new Measures.Measure(item));
            //}

        }
        
        public void DrawMeasures(CanvasList surface)
        {
            Point start = new Point();
            start.X = MusicScore.Defaults.Page.Margins.Left;
            start.Y = MusicScore.Defaults.Page.Margins.Top;
            start.X += MeasureList[0].PrintProperties.SystemLayout.LeftRelative != 0f ? MeasureList[0].PrintProperties.SystemLayout.LeftRelative : 0f ;
            start.Y += MeasureList[0].PrintProperties.SystemLayout.LeftRelative != 0f ? MeasureList[0].PrintProperties.SystemLayout.SystemDistance : 0f;
            Point current = new Point();
            foreach ( var measure in measure_list)
            {
                current.X = measure.PrintProperties != null ? measure.PrintProperties.SystemLayout != null? measure.PrintProperties.SystemLayout.LeftRelative : current.X : current.X;
                current.Y = measure.PrintProperties != null ? measure.PrintProperties.SystemLayout != null ? measure.PrintProperties.SystemLayout.SystemDistance : current.Y : current.Y;
                if (measure.PrintProperties != null)
                {
                    if (measure.PrintProperties.NewPage)
                    {
                        start.X = current.X;
                        start.Y += current.Y;
                    }
                }
                measure.Draw(surface, start);
                start.X += measure.Width;
            }
        }
    }
}
