using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using MusicXMLScore.Converters;
using System.Runtime.CompilerServices;

namespace MusicXMLScore.VisualObject
{
    class VisualObjectBase : INotifyPropertyChanged
    {
        private Brush _color;
        private double _width;
        private double _height;
        public Canvas CanvasVisual { get; set; }
        public Brush Color
        {
            get { return _color; }
            set
            {
                Set(ref _color, value);
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                Set(ref _width, value);
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                Set(ref _height, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected bool Set<T>(ref T field, T value, [CallerMemberName]string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            field = value;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
