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

namespace GrafanaDashboardCreator.View
{
    /// <summary>
    /// Interaktionslogik für DashboardPopUp.xaml
    /// </summary>
    public partial class DashboardPopUp : Window
    {
        public string EnteredText { get { return DashboardNameBox.Text; } }

        private bool buttonPressed = false;

        public bool ButtonPressed { get { return buttonPressed; } }

        public DashboardPopUp()
        {
            InitializeComponent();
            DashboardNameBox.Focus();
        }

        private void CreateDashboardButton_OnClick(object sender, RoutedEventArgs e)
        {
            buttonPressed = true;
            this.Close();
        }

        private void DashboardNameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CreateDashboardButton_OnClick(sender, e);
            }
        }
    }
}
