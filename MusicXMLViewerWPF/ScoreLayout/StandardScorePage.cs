using System;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.ScoreLayout
{
    class StandardScorePage : AbstractScorePage
    {
        private readonly IPageLayout pageLayout;
        private readonly string scoreId;
        private Canvas scorePage;

        public StandardScorePage(string scoreId, IPageLayout pageLayout)
        {
            this.scoreId = scoreId;
            this.pageLayout = pageLayout;
            scorePage = new Canvas();
        }

        public override UIElement GetContent()
        {
            return scorePage;
        }

        public override void UpdateContent()
        {
            Update();
        }

        private void Update()
        {
            throw new NotImplementedException();
        }
    }
}
