using GrafanaDashboardCreator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaktionslogik für RowPopUp.xaml
    /// </summary>
    public partial class RowPopUp : Window
    {
        public string EnteredText { get { return RowNameBox.Text; } }

        public Dashboard SelectedDashboard { get { return SelectedDashboardBox.SelectedItem as Dashboard; } }

        //Button pressed is for checking if the window was closed without pressing the "Confirm" button
        private bool buttonPressed = false;

        public bool ButtonPressed { get { return buttonPressed; } }

        public RowPopUp(ObservableCollection<Dashboard> dashboards, Dashboard dashboard)
        {
            InitializeComponent();
            SelectedDashboardBox.ItemsSource = dashboards;
            RowNameBox.Focus();

            if (dashboards.Contains(dashboard))
            {
                SelectedDashboardBox.SelectedItem = dashboard;
            }
        }

        private void CreateRowButton_OnClick(object sender, RoutedEventArgs e)
        {
            buttonPressed = true;
            this.Close();
        }

        private void RowNameBox_KeyDown(object sender, KeyEventArgs e)
        {
            //For accepting "Return" as "confirmation"
            if (e.Key == Key.Return)
            {
                CreateRowButton_OnClick(sender, e);
            }
        }
    }
}
