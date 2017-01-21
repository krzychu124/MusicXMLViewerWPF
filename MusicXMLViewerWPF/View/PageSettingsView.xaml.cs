﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicXMLScore.Configuration
{
    /// <summary>
    /// Interaction logic for PageSettingsView.xaml
    /// </summary>
    public partial class PageSettingsView : UserControl
    {
        public PageSettingsView()
        {
            InitializeComponent();
            this.DataContext = new PageSettingsViewModel();
        }
    }
}