﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.ScoreLayout
{
    interface IVisualContainerCollection
    {
        IList<IVisualHost> GetVisualContainers();
    }
}
