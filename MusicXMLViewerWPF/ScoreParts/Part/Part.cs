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
        private Dictionary<int, Point> measure_margin_helper = new Dictionary<int, Point>() { }; 
        /// <summary> helper list to store coordinates of starting point for every line </summary>
        private List<MeasureCoordinates> measure_pos = new List<MeasureCoordinates>();
        private List<Measures.Measure> measure_segment_list = new List<Measures.Measure>(); //! segmemt test

        public string Id { get { return part_id; } }
        public List<Measures.Measure> MeasureList { get { return measure_list; } }
        public Dictionary<int,Point> MarginHelper { get { return measure_margin_helper; } }
        public List<MeasureCoordinates> PositionHelper { get { return measure_pos; } }
        public List<Measures.Measure> MeasureSegmentList { get { return measure_segment_list; } } //! segment test

        public Part(XElement x)
        {
            part_id = x.Attribute("id").Value;
            Point tempPoint = new Point();
            Point tempMargins = new Point(MusicScore.Defaults.Page.Margins.Left, MusicScore.Defaults.Page.Margins.Top + MusicScore.Defaults.SystemLayout.TopSystemDistance);
            float sysdist = MusicScore.Defaults.SystemLayout.SystemDistance;
            var measures = x.Elements();
            Point measure_location = MusicScore.Defaults.Page.MeasuresContentSpace.TopLeft;
            float measure_location_left = (float)MusicScore.Defaults.Page.MeasuresContentSpace.X;
            float measure_system = (float)MusicScore.Defaults.SystemLayout.TopSystemDistance;
            measure_location.Y += measure_system;
            //! Main loop for searching measures in current part
            bool test = true;
            bool test2 = false;
            for (int i = 0; i < measures.Count(); i++)
            {
                XElement item = measures.ElementAt(i);
                //measure_list.Add(new Measures.Measure(item));
                
                if (test)
                {
                    Measures.Measure measure = new Measures.Measure(item);
                    if (measure.PrintProperties != null)
                    {
                        if (measure.PrintProperties.SystemLayout != null)
                        {
                            if (measure.PrintProperties.SystemLayout.SystemDistance != measure_system)
                            {
                                measure_system = measure.PrintProperties.SystemLayout.SystemDistance;
                            }
                            if (measure.PrintProperties.SystemLayout.LeftRelative != measure_location_left)
                            {
                                measure_location_left = measure.PrintProperties.SystemLayout.LeftRelative + (float)MusicScore.Defaults.Page.MeasuresContentSpace.Left;
                            }
                        }
                        if (measure.PrintProperties.StaffLayoutList.Count != 0)
                        {
                            float staffspacer = measure.PrintProperties.StaffLayoutList.ElementAt(0).Distance;
                            measure_system += staffspacer;
                        }
                    }
                    if (i == 0) measure.IsFirstInLine = true;
                    //measure.Relative = new Point(measure_location.X, measure_location.Y - 10f);
                    measure.SetSpacers(0, measure.Width);
                    measure.Height = 80f;
                    if (measure.IsFirstInLine)
                    {
                        measure_location.X = measure_location_left;
                        measure_location.Y += MusicScore.Defaults.SystemLayout.SystemDistance;
                        if (measure.PrintProperties?.StaffLayoutList.Count != 0 && measure.PrintProperties?.StaffLayoutList.Count != null) //! no working properly
                        {
                            float staffspacer = measure.PrintProperties.StaffLayoutList.ElementAt(0).Distance;
                            measure_location.Y += staffspacer;
                        }
                        measure.Relative = new Point( measure_location.X, measure_location.Y - 10f); //! Set relative position of measure
                        measure.Calculated = new Point(measure_location.X, measure_location.Y); //! Set calculated relative position of measure
                        measure_location.X += measure.Width;
                    }
                    else
                    {
                        measure.Relative = new Point(measure_location.X, measure_location.Y - 10f);
                        measure.Calculated = new Point(measure_location.X, measure_location.Y);
                        measure_location.X += measure.Width;
                    }
                    measure_segment_list.Add(measure);
                    //test = false;
                }
                if (test2)
                {
                    measure_list.Add(new Measures.Measure(item)); // 

                    if (i == 0)
                    {
                        if (measure_list.ElementAt(i).PrintProperties != null)
                        {
                            if (measure_list.ElementAt(i).PrintProperties.SystemLayout != null)
                            {
                                tempPoint = new Point(tempMargins.X + measure_list.ElementAt(i).PrintProperties.SystemLayout.LeftRelative, tempMargins.Y + measure_list.ElementAt(i).PrintProperties.SystemLayout.SystemDistance + MusicScore.Defaults.SystemLayout.TopSystemDistance);
                            }
                            else
                            {
                                tempPoint = new Point(tempMargins.X, tempMargins.Y + MusicScore.Defaults.SystemLayout.TopSystemDistance);
                            }
                            //todo Logger.Log($"Margins {tempMargins.X} {tempMargins.Y}");
                            //todo Logger.Log($"First point {tempPoint.X} {tempPoint.Y}");
                            measure_margin_helper.Add(measure_list.ElementAt(i).Number, tempPoint);
                            //Logger.Log($"helper_dict measure-page margins {MusicScore.Defaults.Page.Margins.Left}  {MusicScore.Defaults.Page.Margins.Top}");
                        }
                    }
                    else
                    {
                        if (measure_list.ElementAt(i).PrintProperties != null)
                        {
                            if (measure_list.ElementAt(i).PrintProperties.NewSystem)
                            {
                                //measure_list.ElementAt(i).IsFirstInLine = true;
                                if (measure_list.ElementAt(i).PrintProperties.SystemLayout != null)
                                {
                                    tempPoint.X = measure_list.ElementAt(i).PrintProperties.SystemLayout.LeftRelative + tempMargins.X;
                                    tempPoint.Y += measure_list.ElementAt(i).PrintProperties.SystemLayout.SystemDistance;
                                    sysdist = measure_list.ElementAt(i).PrintProperties.SystemLayout.SystemDistance;
                                    measure_margin_helper.Add(measure_list.ElementAt(i).Number, tempPoint);
                                    //todo Logger.Log($"helper_dict measure nr. {i+1}  changed system layout");
                                }
                                else
                                {
                                    //todo Logger.Log($"helper_dict NewSystem in measure nr. {i + 1}");
                                    measure_margin_helper.Add(measure_list.ElementAt(i).Number, new Point(measure_margin_helper.ElementAt(measure_margin_helper.Count - 1).Value.X, measure_margin_helper.ElementAt(measure_margin_helper.Count - 1).Value.Y + sysdist));
                                }
                                //measure_margin_helper.Add(i, new Point(measure_margin_helper.ElementAt(measure_margin_helper.Count).Value.X, measure_margin_helper.ElementAt(measure_margin_helper.Count).Value.Y));
                            }
                            else
                            {
                                //measure_margin_helper.Add(measure_list.ElementAt(i).Number, new Point(measure_margin_helper.ElementAt(measure_margin_helper.Count - 1).Value.X, measure_margin_helper.ElementAt(measure_margin_helper.Count - 1).Value.Y));
                                //todo Logger.Log($"helper_dict NewSystem value: no in measure nr. {i + 1}");
                            }
                            //measure_margin_helper.Add(measure_list.ElementAt(i).Number, measure_margin_helper.ElementAt(measure_margin_helper.Count - 1).Value);
                            //Logger.Log($"helper_dict measure nr. {measure_margin_helper.ElementAt(measure_margin_helper.Count -1).Value.X} {measure_margin_helper.ElementAt(measure_margin_helper.Count -1).Value.Y}");

                        }
                    }
                }
            }

            foreach (var item in measure_margin_helper) // Only to check if list was filled correctly
            {
                //todo Logger.Log($"helper_dict measure nr. {item.Key} X: {item.Value.X} Y: {item.Value.Y} "); 
            }
            //foreach (var item in measures)
            //{
            //    measure_list.Add(new Measures.Measure(item));
            //}
            if (measure_margin_helper.Count != 0)
            {
                FillPositionHelper();
            }
                
        }
        private void FillPositionHelper()
        {
            Point start = new Point();
            start.X = measure_margin_helper.ElementAt(0).Value.X;
            start.Y = measure_margin_helper.ElementAt(0).Value.Y;
            foreach (var measure in measure_list)
            {
                if (measure_margin_helper.ContainsKey(measure.Number))
                {
                    start = measure_margin_helper[measure.Number];
                    MusicScore.AddBreak(MusicScore.Defaults.Page.Width - (float)start.X, (float)start.Y, "line");
                }
                measure_pos.Add(new MeasureCoordinates(measure.Number, start, new Point(start.X + measure.Width, start.Y)));
                start.X += measure.Width;
            }
        }
        
        public void DrawMeasures(CanvasList surface)
        {
            Point start = new Point();
            if (measure_margin_helper.Count != 0)
            {
                start.X = measure_margin_helper.ElementAt(0).Value.X;
                start.Y = measure_margin_helper.ElementAt(0).Value.Y;
            }
            
            //Point current = new Point();
            foreach ( var measure in measure_list)
            {
                if (measure_margin_helper.ContainsKey(measure.Number))
                {
                    start = measure_margin_helper[measure.Number];
                   // MusicScore.AddBreak( MusicScore.Defaults.Page.Width - (float)start.X, (float)start.Y, "line");
                }
                //if (measure.PrintProperties != null)
                //{
                //    if (measure.PrintProperties.NewPage)
                //    {
                //        start.X = current.X;
                //        start.Y = current.Y;
                //    }
                //}
                measure.Draw(surface, start);
                start.X += measure.Width;
                
            }
        }
    }

    public class MeasureCoordinates
    {
        private int num;
        private Point begin;
        private Point end;

        public int Number { get { return num; } }
        public Point Start { get { return begin; } }
        public Point End { get { return end; } }

        public MeasureCoordinates(int measure_number, Point start_point, Point end_point)
        {
            num = measure_number;
            begin = start_point;
            end = end_point;
        }
    }
}
