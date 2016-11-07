using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLViewerWPF;

namespace MusicXMLScore.Helpers
{
    class PreviewSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private Key key;
        private TimeSignature timesig;
        private Clef clef;

        public Key Key
        {
            get
            {
                return key;
            }

            set
            {
                if (value != key)
                {
                    key = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Key)));
                }
            }
        }

        public TimeSignature Timesig
        {
            get
            {
                return timesig;
            }

            set
            {
                if (value != timesig)
                {
                    timesig = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Timesig)));
                }
            }
        }

        public Clef Clef
        {
            get
            {
                return clef;
            }

            set
            {
                if (value != clef)
                {
                    clef = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Clef)));
                }
            }
        }

        public PreviewSettings()
        {
            PropertyChanged += PreviewSettings_PropertyChanged;
        }

        public PreviewSettings(Key k = null, TimeSignature t = null, Clef c = null)
        {
            PropertyChanged += PreviewSettings_PropertyChanged;
            if (k != null)
            {
                Key = k;
            }
            if (t != null)
            {
                Timesig = t;
            }
            if (c != null)
            {
                Clef = c;
            }
        }

        private void PreviewSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Key":
                    break;
                case "TimeSig":
                    break;
                case "Clef":
                    break;
                default: Log.LoggIt.Log($"Property {e.PropertyName} not implemented", Log.LogType.Warning);
                    break;
            }
        }
    }
}
