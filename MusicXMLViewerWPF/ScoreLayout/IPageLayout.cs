using System.Windows.Controls;

namespace MusicXMLScore.ScoreLayout
{
    interface IPageLayout
    {
        void UpdateLayout();
        void DoLayout(Canvas scorePage);
    }
}
