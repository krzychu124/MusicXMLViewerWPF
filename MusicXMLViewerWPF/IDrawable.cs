﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    interface IDrawable
    {
        void Draw(CanvasList surface);
        //void DeleteDraw();
        //void ReDraw();
    }
}