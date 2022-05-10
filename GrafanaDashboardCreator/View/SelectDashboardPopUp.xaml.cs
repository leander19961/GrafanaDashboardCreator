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
        private bool multiSelectionButtonPressed = false;
        private bool singleSelectionButtonPressed = false;

        public bool MultiSelectionButtonPressed { get { return multiSelectionButtonPressed; } }
        public bool SingleSelectionButtonPressed { get { return singleSelectionButtonPressed; } }


        public List<Dashboard> SelectedDashboard
        { 
            get 
            {
                if (singleSelectionButtonPressed)
                {
                    return new List<Dashboard>() { DashboardSelectBox.SelectedItem as Dashboard };
                }
                else
                {
                    List<Dashboard> result = new List<Dashboard>();
                    foreach (Dashboard dashboard in DashboardSelectBox.ItemsSource)
                    {
                        result.Add(dashboard);
                    }

                    return result;
                }
            } 
        }

        public SelectDashboardPopUp(ObservableList<Dashboard> dashboards)
        {
            InitializeComponent();
            DashboardSelectBox.ItemsSource = dashboards;
        }

        private void SelectDashboardButton_Click(object sender, RoutedEventArgs e)
        {
            singleSelectionButtonPressed = true;
            this.Close();
        }

        private void SelectAllDashboardsButton_OnClick(object sender, RoutedEventArgs e)
        {
            multiSelectionButtonPressed = true;
            this.Close();
        }
    }
}
