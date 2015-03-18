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
using System.Windows.Shapes;

namespace Tuto.Publishing.Views
{
    /// <summary>
    /// Interaction logic for ManualMatch.xaml
    /// </summary>
    public partial class ManualMatch : Window
    {
        public ManualMatch()
        {
            InitializeComponent();
			ok.Click+=(s,a)=>{ DialogResult= true; Close(); };
			cancel.Click += (s, a) => { DialogResult = false; Close(); };
        }
    }
}
