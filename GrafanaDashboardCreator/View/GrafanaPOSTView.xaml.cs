using GrafanaDashboardCreator.Model;
using GrafanaDashboardCreator.Net;
using GrafanaDashboardCreator.Parser;
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
    /// Interaktionslogik für GrafanaPOSTView.xaml
    /// </summary>
    public partial class GrafanaPOSTView : Window
    {
        private ModelService modelservice;

        //View for selecting a folder and then REST-POST the selected dashboards to this folder
        public GrafanaPOSTView(ModelService modelservice)
        {
            InitializeComponent();
            this.modelservice = modelservice;
            DashboardListView.ItemsSource = modelservice.GetDashboards();
        }

        private void SelectFolderButton_OnCLick(object sender, RoutedEventArgs e)
        {
            SelectGrafanaFolderPopUp folderPopUp = new SelectGrafanaFolderPopUp()
            {
                Owner = this
            };
            folderPopUp.ShowDialog();

            if (!folderPopUp.ButtonPressed)
            {
                return;
            }

            Folder folder = folderPopUp.SelectedFolder;

            foreach (Dashboard dashboard in DashboardListView.SelectedItems)
            {
                dashboard.Folder = folder;
            }

            DashboardListView.ItemsSource = modelservice.GetDashboards();
            DashboardListView.Items.Refresh();
        }

        private void UploadDashboardsButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                bool noTemplatefound = false;
                foreach (Dashboard dashboard in DashboardListView.SelectedItems)
                {
                    foreach (Row row in dashboard.GetRows())
                    {
                        foreach (Datasource datasource in row.Datasources)
                        {
                            if (datasource.Template == null)
                            {
                                noTemplatefound = true;
                            }
                        }
                    }
                }

                if (noTemplatefound)
                {
                    MessageBox.Show("Not all of your datasources does have a Template", "Error!");
                    return;
                }

                foreach (Dashboard dashboard in DashboardListView.SelectedItems)
                {
                    string uploadString = JSONParser.CreateFolderUploadJSON(dashboard).ToString();
                    RESTAPI.POSTJsonToGrafana(uploadString);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }
    }
}
