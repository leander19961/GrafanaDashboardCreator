﻿using GrafanaDashboardCreator.Model;
using HandlebarsDotNet.Collections;
using System;
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
    /// Interaktionslogik für RemoveRowPopUp.xaml
    /// </summary>
    public partial class RemoveRowPopUp : Window
    {
        private bool buttonPressed = false;

        public bool ButtonPressed { get { return buttonPressed; } }

        public Row SelectedRow { get { return RowSelectBox.SelectedItem as Row; } }
        public Dashboard SelectedDashboard { get { return DashboardSelectBox.SelectedItem as Dashboard; } }

        public RemoveRowPopUp(ObservableList<Dashboard> dashboards)
        {
            InitializeComponent();
            DashboardSelectBox.ItemsSource = dashboards;
        }

        private void RemoveRow_Click(object sender, RoutedEventArgs e)
        {
            buttonPressed = true;
            this.Close();
        }

        private void DashboardSelectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RowSelectBox.SelectedItem = null;
            RowSelectBox.ItemsSource = null;
            RowSelectBox.ItemsSource = (DashboardSelectBox.SelectedItem as Dashboard).GetRows();
        }
    }
}