using System.Windows.Controls;

namespace MusicXMLScore.ScoreLayout
{
    interface IPageLayout
    {
        void SetRootPage(AbstractScorePage scorePage);
        void UpdateLayout();
        void DoLayout(AbstractScorePage page, Canvas scorePage);
    }
}
