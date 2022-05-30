using GrafanaDashboardCreator.Model;
using GrafanaDashboardCreator.View.PopUps;
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

namespace GrafanaDashboardCreator.View.EditDashboardsView
{
    /// <summary>
    /// Interaktionslogik für EditDashboardsViewer.xaml
    /// </summary>
    public partial class EditDashboardsViewer : Window
    {
        private ModelService modelService;

        public EditDashboardsViewer(ModelService modelService)
        {
            InitializeComponent();
            this.modelService = modelService;
            SetListViewItemsSource();
        }

        private void SetListViewItemsSource()
        {
            //Add all relevant items to the view, so the user can see them
            //Items gets ordered for better view
            List<Row> list = new List<Row>();
            ObservableCollection<Row> rows = modelService.GetRows();

            foreach (Row row in rows)
            {
                if (!row.Name.Equals("FreeSpace"))
                {
                    list.Add(row);
                }
            }

            list = list.OrderBy(x => x.Name).ToList();
            list = list.OrderBy(x => x.Dashboard.Name).ToList();

            RowsListView.ItemsSource = list;
        }

        private void RenameRowButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (RowsListView.SelectedItem == null)
            {
                return;
            }

            Row row = RowsListView.SelectedItem as Row;

            RenamePopUp popUp = new RenamePopUp()
            {
                Owner = this
            };
            popUp.ShowDialog();

            if (popUp.ButtonPressed)
            {
                row.Name = popUp.EnteredText;
            }

            SetListViewItemsSource();
        }

        private void RenameDashboardButton_OnClick(object sender, RoutedEventArgs e)
        {
            SelectDashboardPopUp selectDashboardPopUp = new SelectDashboardPopUp(modelService.GetDashboards())
            {
                Owner = this
            };
            selectDashboardPopUp.ShowDialog();

            if (!selectDashboardPopUp.SingleSelectionButtonPressed)
            {
                if (selectDashboardPopUp.MultiSelectionButtonPressed)
                {
                    string title = "Error!";
                    string text = "Please select only one dashboard!";
                    MessageBox.Show(text, title);
                }
                return;
            }

            if (selectDashboardPopUp.SelectedDashboards == null || selectDashboardPopUp.SelectedDashboards.Count != 1)
            {
                return;
            }

            Dashboard dashboard = selectDashboardPopUp.SelectedDashboards.First();

            RenamePopUp renamePopUp = new RenamePopUp()
            {
                Owner = this
            };
            renamePopUp.ShowDialog();

            if (renamePopUp.ButtonPressed)
            {
                dashboard.Name = renamePopUp.EnteredText;
            }

            SetListViewItemsSource();
        }

        private void DeleteRowButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (RowsListView.SelectedItem == null)
            {
                return;
            }

            Row row = RowsListView.SelectedItem as Row;

            row.Dashboard.LinkedTabControl.Items.Remove(row.LinkedTabItem);
            modelService.RemoveRow(row);

            SetListViewItemsSource();
        }

        private void DeleteDashboardButton_OnClick(object sender, RoutedEventArgs e)
        {
            SelectDashboardPopUp popUp = new SelectDashboardPopUp(modelService.GetDashboards())
            {
                Owner= this
            };
            popUp.ShowDialog();

            if (!popUp.SingleSelectionButtonPressed)
            {
                if (popUp.MultiSelectionButtonPressed)
                {
                    string title = "Error!";
                    string text = "Please select only one dashboard!";
                    MessageBox.Show(text, title);
                }
                return;
            }

            if (popUp.SelectedDashboards == null || popUp.SelectedDashboards.Count != 1)
            {
                return;
            }

            Dashboard dashboard = popUp.SelectedDashboards.First();

            (Owner as MainWindow).MainTabControl.Items.Remove(dashboard.LinkedTabItem);
            modelService.RemoveDashboard(dashboard);

            SetListViewItemsSource();
        }
    }
}
