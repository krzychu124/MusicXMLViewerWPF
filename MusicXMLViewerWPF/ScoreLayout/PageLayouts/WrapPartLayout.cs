using System.Collections.Generic;
using System.Windows.Controls;
using MusicXMLScore.ScoreLayout.PageLayouts.PageElements;
using System.Windows;

namespace MusicXMLScore.ScoreLayout.PageLayouts
{
    class WrapPartLayout : AbstractLayout
    {
        private Dictionary<string, ElementProperties> contentElements;
        private LinkedList<LinkedList<KeyValuePair<ContentElementTypes, ElementProperties>>> contentRows;

        public WrapPartLayout(IList<AbstractPageElement> pageElements) : base(pageElements)
        {
            contentRows = new LinkedList<LinkedList<KeyValuePair<ContentElementTypes, ElementProperties>>>();
        }

        public WrapPartLayout() : base(new List<AbstractPageElement>())
        {
            contentRows = new LinkedList<LinkedList<KeyValuePair<ContentElementTypes, ElementProperties>>>();
        }

        public override void DoLayout(AbstractScorePage page, Canvas canvas)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateLayout()
        {
            throw new System.NotImplementedException();
        }

        public void AddContentItem()
        {

        }
        

        private class ElementProperties
        {
            private double topMargin;
            private double bottomMargin;
            private double leftMargin;
            private double rightMargin;
            private double width;
            private double height;
            private List<double> distances;
            private List<double> heights;
            
            public ElementProperties()
            {
            }

        }

        private enum ContentElementTypes
        {
            Header,
            Footer,
            NewRow,
            NewPage,
            Measure,
            RectangleItem,
            Other
        }
    }
}
