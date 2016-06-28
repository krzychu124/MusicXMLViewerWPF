using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicXMLViewerWPF
{
    public static class Logger
    {
        private static List<string> log = new List<string>();

        public static event EventHandler LogAdded;
        public static event EventHandler LogCleared;

        public static void Log(string message, [CallerMemberName] string memberName = "")
        {
            log.Add("["+memberName+"]"+": "+message);
            LogAdded?.Invoke(null, EventArgs.Empty);
        }

        public static string GetLastLog()
        {
            if (log.Count > 0)
                return log[log.Count - 1];
            else
                return null;
        }
        public static void EmptyXDocument(string s, [CallerMemberName] string memberName = "")
        {
            System.Windows.MessageBox.Show($"XDocument in: <{s}> is Empty, Load XML file again ");
            Log($"XDocument in: <{s}> is Empty, Load XML file again ");
        }
        public static void Loaded( string message,[CallerMemberName] string memberName="")
        {
            Log(memberName+": "+message);
        }
        public static string ClearLog()
        {
            log.Clear();
            LogCleared?.Invoke(null, EventArgs.Empty);
            System.Windows.MessageBox.Show("Log cleared.");
            return string.Empty;
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
