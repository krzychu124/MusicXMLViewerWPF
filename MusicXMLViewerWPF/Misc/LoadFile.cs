using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF.Misc
{
    public class LoadFile
    {
        private static XDocument doc;

        public static XDocument Document { get { return doc; } }

        public static void LoadDocument(XDocument x )
        {
            doc = x;
            Logger.Loaded("Document was loaded properly");
        }
        
    }
}
