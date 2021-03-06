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
using GrafanaDashboardCreator.Model;

namespace GrafanaDashboardCreator.View.PopUps
{
    /// <summary>
    /// Interaktionslogik für SelectDashboardPopUp.xaml
    /// </summary>
    public partial class SelectDashboardPopUp : Window
    {
        //Button pressed is for checking if the window was closed without pressing the "Confirm" button
        private bool multiSelectionButtonPressed = false;
        private bool singleSelectionButtonPressed = false;

        public bool MultiSelectionButtonPressed { get { return multiSelectionButtonPressed; } }
        public bool SingleSelectionButtonPressed { get { return singleSelectionButtonPressed; } }


        public List<Dashboard> SelectedDashboards
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

        public SelectDashboardPopUp(ObservableCollection<Dashboard> dashboards)
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
