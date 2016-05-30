using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLTestViewerWPF
{
    public static class Logger
    {
        private static List<string> log = new List<string>();

        public static event EventHandler LogAdded;

        public static void Log(string message)
        {
            log.Add(message);

            if (LogAdded != null)
                LogAdded(null, EventArgs.Empty);
        }

        public static string GetLastLog()
        {
            if (log.Count > 0)
                return log[log.Count - 1];
            else
                return null;
        }
    }
}
//{
//    public static class Logger
//    {
//        public static string text;

//        public static string Text
//        {
//            get
//            {
//                return text;
//            }

//            set
//            {
//                text = value;

//            }
//        }


//        public static void writeLineToTextBlock(string text)
//        {
//            Text += " \n"+ text;
//        }
//        public static void writeToTextBlock(string t)
//        {
//            Text += t+ " ";
//        }
//    }
//}
