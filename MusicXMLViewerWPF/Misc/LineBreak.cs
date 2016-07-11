using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLViewerWPF.Misc
{
    class LineBreak
    {
        float _x;
        float _y;
        float _height;
        BreakType break_type;

        public float PosX { get { return _x; } }
        public float PosY { get { return _y; } }
        public BreakType Type { get { return break_type; } }
        public Point Position { get { return new Point(_x, _y); } }

        public LineBreak(float x, float y)
        {
            _x = MusicScore.Defaults.Page.Width - MusicScore.Defaults.Page.Margins.Right + 20f;
            //_x = x;
            _y = y;
            break_type = BreakType.line;
        }

        public LineBreak(float x, float y, string type, float value = 0f): this(x,y)
        {
            SetBreak(type, value);
        }

        private void SetBreak(string type, float height = 0f)
        {
            _height = height;
            switch(type)
            {
                case "title":
                    break_type = BreakType.score_info;
                    break;
                case "line":
                    break_type = BreakType.line;
                    break;
                case "page":
                    break_type = BreakType.page;
                    break;
                case "section":
                    break_type = BreakType.section;
                    break;
                case "up":
                    break_type = BreakType.staff_spacer_up;
                    break;
                case "down":
                    break_type = BreakType.staff_spacer_down;
                    break;
            }
        }
        public void DrawBreak(DrawingVisual visual)
        {
            string text = "";
            if (Type == BreakType.score_info)
            {
                text = "T";
            }
            else
            {
                if (Type == BreakType.line)
                {
                    text = "L";
                }
                else
                {
                    if (Type == BreakType.page)
                    {
                        text = "P";
                    }
                    else
                    {
                        if (Type == BreakType.section)
                        {
                            text = "S";
                        }
                        else
                        {
                            if (Type == BreakType.staff_spacer_up)
                            {
                                text = "^S^";
                            }
                            else
                            {
                                text = "vSv";
                            }
                        }
                    }
                }
            }
            float spacer = Type == BreakType.staff_spacer_down || Type == BreakType.staff_spacer_up ? 15f : 10f;
            Point l_u = new Point(_x - spacer, _y - spacer);
            Point r_d = new Point(_x + spacer, _y + spacer);
            Rect rectangle = new Rect(l_u, r_d);
            Pen pen = new Pen(Brushes.LightBlue, 1);
            DrawingVisual visual_break = new DrawingVisual();
            using (DrawingContext dc = visual_break.RenderOpen())
            {
                dc.DrawRectangle(Brushes.LightBlue, pen, rectangle);
                DrawingHelpers.DrawText(dc, text, Position, 12f, Halign.center, Valign.top, font_weight:"bold", withsub:false, color:Brushes.DarkBlue);
            }
            visual.Children.Add(visual_break);
        }
    }
    enum BreakType
    {
        score_info,
        line,
        page,
        section,
        staff_spacer_down,
        staff_spacer_up
    }
}
