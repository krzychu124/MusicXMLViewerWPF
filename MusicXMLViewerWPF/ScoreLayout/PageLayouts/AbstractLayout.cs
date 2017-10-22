using MusicXMLScore.ScoreLayout.PageLayouts.PageElements;
using System.Collections.Generic;
using System.Windows.Controls;
using System;
using System.ComponentModel;
using System.Linq;

namespace MusicXMLScore.ScoreLayout.PageLayouts
{
    abstract class AbstractLayout : IPageLayout
    {
        private double width;
        private double height;

        private readonly IList<AbstractPageElement> pageElements;

        protected AbstractLayout(IList<AbstractPageElement> pageElements)
        {
            this.pageElements = pageElements;
        }

        public AbstractScorePage Root { get; private set; }
        public double Width
        {
            get => width; set
            {
                width = value;
                if (Root != null)
                {
                    Root.Width = value;
                }
            }
        }
        public double Height
        {
            get => height; set
            {
                height = value;
                if (Root != null)
                {
                    Root.Height = value;
                }
            }
        }

        internal IList<AbstractPageElement> PageElements => pageElements;

        public abstract void DoLayout(AbstractScorePage page,Canvas canvas);

        public void SetRootPage(AbstractScorePage scorePage)
        {
            if (Root != null)
            {
                Root.RemoveListener(OnRootPropertyChanged);
            }
            Root = scorePage;
            Root.AddListener(OnRootPropertyChanged);
        }

        private void OnRootPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Height):
                    var item = pageElements.FirstOrDefault(x=> x is ContentContainer);
                    if (item != null)
                    item.Height = item.Height - 30;
                    break;
                default:
                    break;
            }
        }

        public abstract void UpdateLayout();
    }
}
