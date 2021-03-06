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

namespace GrafanaDashboardCreator.View.PopUps
{
    /// <summary>
    /// Interaktionslogik für AddToDashboardPopUp.xaml
    /// </summary>
    public partial class AddToDashboardPopUp : Window
    {
        //Button pressed is for checking if the window was closed without pressing the "Confirm" button
        private bool buttonPressed = false;

        public bool ButtonPressed { get { return buttonPressed; } }

        public Dashboard SelectedDashboard { get { return DashboardSelectBox.SelectedItem as Dashboard; } }
        public Row SelectedRow { get { return RowSelectBox.SelectedItem as Row; } }

        public AddToDashboardPopUp(ObservableCollection<Dashboard> dashboards)
        {
            InitializeComponent();
            DashboardSelectBox.ItemsSource = dashboards;
        }

        private void AddToDashboard_Click(object sender, RoutedEventArgs e)
        {
            buttonPressed = true;
            this.Close();
        }

        private void DashboardSelectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Loads the rows of the current selected dashboard
            RowSelectBox.SelectedItem = null;
            RowSelectBox.ItemsSource = null;
            RowSelectBox.ItemsSource = (DashboardSelectBox.SelectedItem as Dashboard).GetRows();
            RowSelectBox.SelectedItem = (RowSelectBox.ItemsSource as List<Row>).First();
        }
    }
}
