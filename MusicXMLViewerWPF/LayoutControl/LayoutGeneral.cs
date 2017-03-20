using MusicXMLScore.Helpers;
using MusicXMLScore.Model.Defaults;
using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MusicXMLScore.LayoutStyle;

namespace MusicXMLScore.LayoutControl
{
    /// <summary>
    /// Properties for page
    /// </summary>
    public class LayoutGeneral
    {
        private PageProperties pageProperties;
        private Layout layoutStyle;
        public LayoutGeneral()
        {
            pageProperties = new PageProperties();
            layoutStyle = new LayoutStyle.Layout();
        }
        internal LayoutGeneral(MusicScore musicScore)
        {
            pageProperties = new PageProperties(musicScore);
        }
        public LayoutGeneral(ScorePartwiseMusicXML score)
        {
            pageProperties = score !=null?  new PageProperties(score.Defaults): null;
            layoutStyle = new LayoutStyle.Layout(score);
        }

        private void SaveAsDefaultStyle() //! not tested
        {
            XmlSerializer xml = new XmlSerializer(layoutStyle.GetType());
            TextWriter txtw = new StreamWriter(@".\DefaultLayoutStyle.xml");
            xml.Serialize(txtw, layoutStyle);
            txtw.Close();
        }
        private void LoadDefaultStyle() //! not tested
        {
            XmlSerializer xml = new XmlSerializer(layoutStyle.GetType());
            using (var stream = File.OpenRead(@".\DefaultLayoutStyle.xml"))
            {
                layoutStyle = (LayoutStyle.Layout) xml.Deserialize(stream);
            }
        }
        public PageProperties PageProperties { get { return pageProperties; } }
        public PageMarginsMusicXML PageMarginsEven { get { return PageProperties.PageMarginEven; } }
        public PageMarginsMusicXML PageMarginsOdd { get { return PageProperties.PageMarginOdd; } }
        public PageMarginsMusicXML PageMargins { get { return PageProperties.PageMarginBoth; } }
        public Layout LayoutStyle { get { return layoutStyle; }  set { layoutStyle = value; } }
    }
}
