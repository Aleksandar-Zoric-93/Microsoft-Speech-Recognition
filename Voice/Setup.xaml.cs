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

namespace Voice
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        MainWindow mainWindow = null;

        public Window1(MainWindow mw)
        {
            InitializeComponent();
            this.mainWindow = mw;

        }

        private void submitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.mainWindow.assignNames(yourName.Text, aiNameTxt.Text);
            this.Close();
        }

        public void closeSetupWindow()
        {
            this.Close();
        }

    }
}
