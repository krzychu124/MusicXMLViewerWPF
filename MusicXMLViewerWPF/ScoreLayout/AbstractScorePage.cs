using System.ComponentModel;
using System.Windows;

namespace MusicXMLScore.ScoreLayout
{
    abstract class AbstractScorePage : IScorePage, INotifyPropertyChanged
    {
        private double height;
        private readonly string id;
        private double width;

        private AbstractScorePage previousPage;
        private AbstractScorePage nextPage;

        protected AbstractScorePage(string id)
        {
            this.id = id;

        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public double Height
        {
            get => height;
            set
            {
                height = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Height)));
            }
        }


        public string Id => id;

        public double Width
        {
            get => width;
            set
            {
                width = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
            }
        }

        internal AbstractScorePage PreviousPage { get => previousPage; set => previousPage = value; }
        internal AbstractScorePage NextPage { get => nextPage; set => nextPage = value; }

        public void AddListener(PropertyChangedEventHandler handler)
        {
            PropertyChanged += handler;
        }
        public abstract UIElement GetContent();

        public void RemoveListener(PropertyChangedEventHandler handler)
        {
            PropertyChanged -= handler;
        }

        public abstract void SetDimensions(double width, double height);

        public abstract void UpdateContent();
    }
}
