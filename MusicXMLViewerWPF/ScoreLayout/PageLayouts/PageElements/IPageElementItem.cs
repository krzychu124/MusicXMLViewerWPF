﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.ScoreLayout.PageLayouts.PageElements
{
    interface IPageElementItem
    {
        UIElement GetUIElement();
    }
}
