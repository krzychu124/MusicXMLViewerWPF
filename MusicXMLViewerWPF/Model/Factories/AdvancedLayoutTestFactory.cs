using MusicXMLScore.ScoreLayout;
using MusicXMLScore.ScoreLayout.PageLayouts;
using MusicXMLScore.ScoreLayout.PageLayouts.PageElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.Model.Factories
{
    class AdvancedLayoutTestFactory
    {
        public static AbstractScorePage GetScorePage()
        {

            var header = new HeaderContainer(1200, 60);
            header.AddTitle("TEST TITLE");
            var footer = new FooterContainer(1200, 20);
            footer.AddCopyRights("COPYRIGHTS TEXT TEST");

            var contentContainer = new ContentContainer(1200,600);
            var rowContainer = new SimpleRowContainer(new System.Windows.Rect(0, 0, 1140, 100)) { Y = 40};
            var rowContainer2 = new SimpleRowContainer(new System.Windows.Rect(0, 0, 1140, 100)) { Y =180};
            var rowContainer3 = new SimpleRowContainer(new System.Windows.Rect(0, 0, 1140, 100)) { Y =320};
            rowContainer.AddItem(new MeasurePrototypeItem(100));
            rowContainer2.AddItem(new MeasurePrototypeItem(100));
            rowContainer3.AddItem(new MeasurePrototypeItem(100));

            contentContainer.AddRowContainer(rowContainer);
            contentContainer.AddRowContainer(rowContainer2);
            contentContainer.AddRowContainer(rowContainer3);
            var pageElements = new List<AbstractPageElement>{
                header,
                contentContainer,
                footer
            };
            var pageLayout = new WrappedLayout(pageElements);
            var scorePage = new StandardScorePage("ADVANCED_LAYOUT_TEST", pageLayout);
            scorePage.UpdateContent();
            return scorePage;
        }


        public static AbstractScorePage GetScorePage2()
        {
            var footer = new FooterContainer(1200, 20);
            footer.AddCopyRights("COPYRIGHTS TEXT TEST");

            var contentContainer = new ContentContainer(1200, 800);
            var rowContainer = new SimpleRowContainer(new System.Windows.Rect(0, 0, 1140, 100)) { Y = 40 };
            var rowContainer2 = new SimpleRowContainer(new System.Windows.Rect(0, 0, 1140, 100)) { Y = 180 };
            var rowContainer3 = new SimpleRowContainer(new System.Windows.Rect(0, 0, 1140, 100)) { Y = 320 };
            rowContainer.AddItem(new MeasurePrototypeItem(100));
            rowContainer2.AddItem(new MeasurePrototypeItem(100));
            rowContainer3.AddItem(new MeasurePrototypeItem(100));

            contentContainer.AddRowContainer(rowContainer);
            contentContainer.AddRowContainer(rowContainer2);
            contentContainer.AddRowContainer(rowContainer3);
            var pageElements = new List<AbstractPageElement>{
                contentContainer,
                footer
            };
            var pageLayout = new WrappedLayout(pageElements);
            var scorePage = new StandardScorePage("ADVANCED_LAYOUT_TEST2", pageLayout);
            scorePage.UpdateContent();
            return scorePage;
        }
    }
}
