using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicXMLScore.ScoreLayout
{
    class StandardScorePage : AbstractScorePage
    {
        private readonly IPageLayout pageLayout;
        private readonly string scoreId;
        private Canvas scorePage;


        public StandardScorePage(string scoreId, IPageLayout pageLayout) : base(scoreId)
        {
            PropertyChanged += StandardScorePage_PropertyChanged;
            this.scoreId = scoreId;
            this.pageLayout = pageLayout;
            scorePage = new Canvas
            {
                ClipToBounds = true
            };
            pageLayout.SetRootPage(this);
        }

        public override UIElement GetContent()
        {
            return scorePage;
        }
        public override void SetDimensions(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public override void UpdateContent()
        {
            Update();
        }

        private void StandardScorePage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Width):
                    scorePage.Width = Width;
                    pageLayout.UpdateLayout();
                    break;
                case nameof(Height):
                    scorePage.Height = Height;
                    pageLayout.UpdateLayout();
                    break;
                default:
                    break;
            }
        }

        private void Update()
        {
            pageLayout.DoLayout(this, scorePage);
        }
    }
}
