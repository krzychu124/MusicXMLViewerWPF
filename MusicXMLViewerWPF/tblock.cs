using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLTestViewerWPF
{
    public class tblock 
    {
        public static string text;
        
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;

            }
        }

        
        public  void writeLineToTextBlock(string text)
        {

           Text += " \n"+ text;
        }
        public  void writeToTextBlock(string t)
        {
            this.Text += t+ " ";
            
        }
    }
}
