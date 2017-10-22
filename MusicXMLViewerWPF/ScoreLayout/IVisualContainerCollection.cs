using System.Collections.Generic;

namespace MusicXMLScore.ScoreLayout
{
    interface IVisualContainerCollection
    {
        IList<IVisualHostControl> GetVisualHostContainers();
    }
}
