using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class XmlRead
    {
        private static string file_path;
        private static XDocument Doc;
        public string File_path
        {
            get { return file_path; }

            set { file_path = value; }
        }

        public static XDocument Document
        {
            get
            { if(Doc!=null)
                {
                    return Doc;
                }
                else
                {
                    XDocument d = new XDocument();
                    return d;
                }
                
            }

            set { Doc = value; }
        }
        public static XDocument GetXmlInventory()
        {
            try
            {
                XDocument inventoryDoc
                = XDocument.Load(file_path);

                return inventoryDoc;
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine("File not found !",ex.Message);   
                return null;
            }
            catch ( ArgumentNullException)
            {
                Console.WriteLine("invalid path to file");
                return null;
            }
        }

        internal static XDocument GetXmlInventory(string fileName)
        {
            try
            {
                XDocument inventoryDoc
                = XDocument.Load(fileName);
                
                Document = inventoryDoc;
               
                return inventoryDoc;
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine("File not found !", ex.Message);
                return null;
            }
        }
    }
}
