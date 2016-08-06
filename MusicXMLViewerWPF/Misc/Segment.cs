using MusicXMLViewerWPF.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF
{
    public class Segment 
    {
        #region protected fields
        private float width;
        private float height;
        
        private float relative_x;
        private float relative_y;
        
        private float calculated_x;
        private float calculated_y;
        
        private float offset_x;
        private float offset_y;
        
        private SegmentType segment_type;
        #endregion
        
        #region public properties
        public float Calculated_x { get { return calculated_x; } set { calculated_x = value; } }
        public float Calculated_y { get { return calculated_y; } set { calculated_y = value; } }
        public float Height { get { return height; } set { height = value; } }  
        public float Offset_x { get { return offset_x; } set { offset_x = value; } }
        public float Offset_y { get { return offset_y; } set { offset_y = value; } }
        public float Relative_x { get { return relative_x; } set { relative_x = value; } }
        public float Relative_y { get { return relative_y; } set { relative_y = value; } }
        public SegmentType Segment_type { get { return segment_type; } set { segment_type = value; } }
        public float Width { get { return width; } set { width = value; } }
        #endregion

        public void SetRelativePos(float x, float y)
        {
            Relative_x = x;
            Relative_y = y;
        }

        public void SetCalculatedPos(float x, float y)
        {
            Calculated_x = x;
            Calculated_y = y;
        }

        public void SetOffset(float x, float y)
        {
            Offset_x = x;
            Offset_y = y;
        }

        public void SetDimensions( float w, float h)
        {
            Width = w;
            Height = h;
        }
    }
}
