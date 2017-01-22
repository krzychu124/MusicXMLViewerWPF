using MusicXMLScore.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MusicXMLScore.Log
{
    class LogViewModel : INotifyPropertyChanged
    {
        private static LoggIt log;
        private ObservableCollection<LoggIt.Logger> infolog;
        private ObservableCollection<LoggIt.Logger> warninglog;
        private ObservableCollection<LoggIt.Logger> errorlog;
        private ObservableCollection<LoggIt.Logger> exceptionslog;
        private ObservableCollection<LoggIt.Logger> alllog;
        private XmlDataProvider xmlfile;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ObservableCollection<LoggIt.Logger> InfoLog { get { return infolog; }  set { infolog = value; } }
        public ObservableCollection<LoggIt.Logger> WarningLog { get { return warninglog; } set { warninglog = value; } }
        public ObservableCollection<LoggIt.Logger> ErrorLog { get { return errorlog; } set { errorlog = value; } }
        public ObservableCollection<LoggIt.Logger> ExceptionsLog { get { return exceptionslog; } set { exceptionslog = value; } }
        public ObservableCollection<LoggIt.Logger> AllLog { get { return alllog; } set { alllog = value; } }
        public XmlDataProvider XMLLoadedFile { get { return xmlfile; } set { xmlfile = value; PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(XMLLoadedFile))); } }

        public LogViewModel()
        {
            log = new LoggIt();
            infolog = LoggIt.Infolog;
            warninglog = LoggIt.Warninglog;
            errorlog = LoggIt.Errorlog;
            exceptionslog = LoggIt.Exceptionslog;
            alllog = LoggIt.Alllog;
            LoggIt.Log("test1");
            LoggIt.Log("test2", LogType.Warning);
        }

        private void SetXmlFile(object xml)
        {
            XMLLoadedFile = xml as XmlDataProvider;
        }
    }
}
