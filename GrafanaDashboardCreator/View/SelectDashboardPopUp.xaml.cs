using GrafanaDashboardCreator.Model;
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
    /// Interaktionslogik für SelectDashboardPopUp.xaml
    /// </summary>
    public partial class SelectDashboardPopUp : Window
    {
        private bool buttonPressed = false;

        public bool ButtonPressed { get { return buttonPressed; } }

        public Dashboard SelectedDashboard { get { return DashboardSelectBox.SelectedItem as Dashboard; } }

        public SelectDashboardPopUp(ObservableList<Dashboard> dashboards)
        {
            InitializeComponent();
            DashboardSelectBox.ItemsSource = dashboards;
        }

        private void RemoveDashboard_Click(object sender, RoutedEventArgs e)
        {
            buttonPressed = true;
            this.Close();
        }
    }
}
