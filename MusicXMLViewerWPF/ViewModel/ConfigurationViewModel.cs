using System;
using GalaSoft.MvvmLight;
using MusicXMLScore.Helpers;
using System.Windows;

namespace MusicXMLScore.ViewModel
{
    /// <summary>
    /// Interaction logic for ConfigurationView.xaml
    /// </summary>
    class ConfigurationViewModel : ViewModelBase
    {

        public RelayCommand<Object> CancelCommand { get; set; }
        public RelayCommand<Object> OKCommand { get; set; }

        public ConfigurationViewModel()
        {
            OKCommand = new RelayCommand<object>(OnOkClick);
            CancelCommand = new RelayCommand<Object>(OnCancelClick);
        }

        private void OnOkClick(object obj)
        {
            var w = obj as Window;
            w.Close();
        }

        private void OnCancelClick(object window)
        {
            var w = window as Window;
            w.Close();
        }
    }
}
