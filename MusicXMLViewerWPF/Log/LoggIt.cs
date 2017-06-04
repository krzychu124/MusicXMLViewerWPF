//#define DEBUGLOG
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicXMLScore.Log
{
    class LoggIt : INotifyPropertyChanged
    {
        public class Logger
        {
            public LogType LogType { get; set; }
            public string MemberName { get; set; }
            public string Message { get; set; }
            public string Date { get; set; }
            public Brush MessageColor { get; set; }
            public Brush MessageBackgroundColor { get; set; } = Brushes.White;
            public Logger(LogType t, string memname, string message, string date)
            {
                LogType = t;
                MemberName = memname;
                Message = message;
                Date = date;
                SetMessageColor();
            }

            private void SetMessageColor()
            {
                switch (LogType)
                {
                    case LogType.Info:
                        MessageColor = Brushes.DarkBlue;
                        break;
                    case LogType.Warning:
                        MessageColor = Brushes.DarkOrange;
                        break;
                    case LogType.Error:
                        MessageColor = Brushes.Red;
                        break;
                    case LogType.Exception:
                        MessageColor = Brushes.White;
                        MessageBackgroundColor = Brushes.DarkRed;
                        break;
                    default:
                        break;
                }
            }
        }
        private static ObservableCollection<Logger> infolog;
        private static ObservableCollection<Logger> warninglog;
        private static ObservableCollection<Logger> errorlog;
        private static ObservableCollection<Logger> exceptionslog;
        private static ObservableCollection<Logger> alllog;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public static ObservableCollection<Logger> Infolog { get { return infolog; }  set { infolog = value; } }
        public static ObservableCollection<Logger> Warninglog { get { return warninglog; } set { warninglog = value; } }
        public static ObservableCollection<Logger> Errorlog { get { return errorlog; } set { errorlog = value; } }
        public static ObservableCollection<Logger> Exceptionslog { get { return exceptionslog; } set { exceptionslog = value; } }
        public static ObservableCollection<Logger> Alllog { get { return alllog; } set { alllog = value; } }

        public LoggIt()
        {
            infolog = new ObservableCollection<Logger>();
            warninglog = new ObservableCollection<Logger>();
            errorlog = new ObservableCollection<Logger>();
            exceptionslog = new ObservableCollection<Logger>();
            alllog = new ObservableCollection<Logger>();
        }

        public static void Log(string message, LogType logtype= LogType.Info, [CallerMemberName] string memberName = "")
        {
#if (DEBUGLOG)            
            string date = DateTime.Now.ToString();
            switch (logtype)
            {
                case LogType.Info:
                    Logger li = new Logger(logtype, memberName, message, date);
                    Alllog.Add(li);
                    Infolog.Add(li);
                    break;
                case LogType.Warning:
                    Logger lw = new Logger(logtype, memberName, message, date);
                    Alllog.Add(lw);
                    Warninglog.Add(lw);
                    break;
                case LogType.Error:
                    Logger ler = new Logger(logtype, memberName, message, date);
                    Alllog.Add(ler);
                    Errorlog.Add(ler);
                    break;
                case LogType.Exception:
                    Logger le = new Logger(logtype, memberName, message, date);
                    Alllog.Add(le);
                    Exceptionslog.Add(le);
                    ConsoleWrite(message, memberName);
                    break;
                default:
                    break;
            }
#endif
        }

        /// <summary>
        /// Log to console
        /// </summary>
        /// <param name="message"></param>
        /// <param name="memberName"></param>
        private static void ConsoleWrite(string message, string memberName)
        {
            Console.WriteLine($"{memberName}: {message}");
        }
    }

    enum LogType
    {
        Info,
        Warning,
        Error,
        Exception
    }
}
