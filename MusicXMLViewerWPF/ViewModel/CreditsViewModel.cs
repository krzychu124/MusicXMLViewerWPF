﻿using MusicXMLScore.Helpers;
using MusicXMLViewerWPF.Credit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace MusicXMLScore.ViewModel
{
    class CreditsViewModel
    {
        private ObservableCollection<UIElement> creditslist = new ObservableCollection<UIElement>();

        public ObservableCollection<UIElement> CreditsList { get { return creditslist; } set { creditslist = value; } }

        public RelayCommand AddTitleCommand { get; set; }
        public RelayCommand AddSubTitleCommand { get; set; }
        public RelayCommand AddComposerCommand { get; set; }
        public RelayCommand AddArrangerCommand { get; set; }
        public RelayCommand AddLyricistCommand { get; set; }
        public RelayCommand AddInstrumentNameCommand { get; set; }

        public CreditsViewModel()
        {
            AddTitleCommand = new RelayCommand(OnAddTitleCommand);
            AddSubTitleCommand = new RelayCommand(OnAddSubTitleCommand);
            AddComposerCommand = new RelayCommand(OnAddComposerCommand);
            AddArrangerCommand = new RelayCommand(OnAddArrangerCommand);
            AddLyricistCommand = new RelayCommand(OnAddLyricistCommand);
            AddInstrumentNameCommand = new RelayCommand(OnAddInstrumentNameCommand);

            CreditsList.Add(GenerateLayout(Dock.Top, "titles_space"));
            CreditsList.Add(GenerateLayout(Dock.Left, "lyricist_instr_nm_space"));
            CreditsList.Add(GenerateLayout(Dock.Right, "compos_arr_space"));
        }

        private void OnAddInstrumentNameCommand()
        {
            var stck = CreditsList.ElementAt(1) as StackPanel;
            stck.Children.Add(GenerateCreditTextBox("testing credit instrument generator", CreditType.intrumentname));
           // MessageBox.Show("Added intrument name");
        }

        private void OnAddLyricistCommand()
        {
            var stck = CreditsList.ElementAt(1) as StackPanel;
            stck.Children.Insert(0,GenerateCreditTextBox("testing credit lyricist generator", CreditType.lyricist));
           // MessageBox.Show("Added lyricist");
        }

        private void OnAddArrangerCommand()
        {
            var stck = CreditsList.ElementAt(2) as StackPanel;
            stck.Children.Add(GenerateCreditTextBox("testing credit arranger generator", CreditType.arranger));
           // MessageBox.Show("Added arranger");
        }

        private void OnAddComposerCommand()
        {
            var stck = CreditsList.ElementAt(2) as StackPanel;
            stck.Children.Insert(0, GenerateCreditTextBox("testing credit composer generator", CreditType.composer));
           // MessageBox.Show("Added composer");
        }

        private void OnAddSubTitleCommand()
        {
            var stck = CreditsList.ElementAt(0) as StackPanel;
            stck.Children.Add(GenerateCreditTextBox("testing credit subtitle generator", CreditType.subtitle));
           // MessageBox.Show("Added subtitle");
        }

        private void OnAddTitleCommand()
        {
            var stck = CreditsList.ElementAt(0) as StackPanel;
            stck.Children.Insert(0, GenerateCreditTextBox("testing credit title generator", CreditType.subtitle));
            //MessageBox.Show("Added title");
        }

        private StackPanel GenerateLayout(Dock dock, string name)
        {
            StackPanel stck = new StackPanel();
            stck.Name = name;
            stck.SetValue(DockPanel.DockProperty, dock);
            stck.Children.Add(new DockPanel());
            return stck;
        }

        private TextBox GenerateCreditTextBox(string value, CreditType credittype)
        {
            Dock dock = Dock.Top;
            TextBox tb = new TextBox();
            tb.Text = value;
            tb.Background = Brushes.Transparent;
            tb.BorderThickness = new Thickness(0.0);
            tb.SetValue(Credit.CreditTypeProperty, credittype);
            switch (credittype)
            {
                case CreditType.page_number:
                    break;
                case CreditType.title:
                    tb.SetValue(DockPanel.DockProperty, Dock.Top);
                    tb.HorizontalAlignment = HorizontalAlignment.Center;
                    break;
                case CreditType.subtitle:
                    tb.SetValue(DockPanel.DockProperty, Dock.Top);
                    tb.HorizontalAlignment = HorizontalAlignment.Center;
                    break;
                case CreditType.composer:
                    tb.SetValue(DockPanel.DockProperty, Dock.Right);
                    tb.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case CreditType.lyricist:
                    tb.SetValue(DockPanel.DockProperty, Dock.Left);
                    tb.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case CreditType.arranger:
                    tb.SetValue(DockPanel.DockProperty, Dock.Right);
                    tb.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case CreditType.intrumentname:
                    tb.SetValue(DockPanel.DockProperty, Dock.Top);
                    tb.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case CreditType.separator:
                    tb.SetValue(DockPanel.DockProperty, Dock.Top);
                    tb.HorizontalAlignment = HorizontalAlignment.Stretch;
                    tb.Height = 1;
                    break;
                case CreditType.rights:
                    break;
                case CreditType.other:
                    break;
                case CreditType.none:
                    break;
                default:
                    break;
            }
            //DockPanel.SetDock(tb, dock);
            return tb;
        }

    }
}
