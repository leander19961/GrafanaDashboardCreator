using GrafanaDashboardCreator.Model;
using GrafanaDashboardCreator.Net;
using GrafanaDashboardCreator.Parser;
using GrafanaDashboardCreator.Resource;
using GrafanaDashboardCreator.View;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;

using static GrafanaDashboardCreator.Resource.Constants;

namespace GrafanaDashboardCreator
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ModelService modelService;

        public MainWindow()
        {
            InitializeComponent();
            SettingsIO.CheckForFileSystem();

            modelService = new ModelService();
        }

        private void GetDatasourcesButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Starts a GET-REQUEST to OpenNMS for geting datasources from a node
            try
            {
                //First GET-Request all nodes, view them and wait for the user to select the node for GET-REQUEST their datasources
                string nodesXML = RESTAPI.GETNodesFromOpenNMS();

                if (nodesXML == null || nodesXML == "")
                {
                    MessageBox.Show("GET request to OpenNMS failed!", "Error!");
                    return;
                }

                //Parse the nodes from the request result
                XMLParser.GetNodesFromXML(modelService, nodesXML);

                SelectNodePopUp popUp = new SelectNodePopUp(modelService.GetNodes())
                {
                    Owner = this
                };

                popUp.ShowDialog();

                if (!popUp.ButtonPressed || popUp.SelectedNode == null)
                {
                    return;
                }

                //If a node got selected GET-REQUEST the datasources
                string resourcesXML = RESTAPI.GETResourcesFromOpenNMS(popUp.SelectedNode.NodeForeignSource + ":" + popUp.SelectedNode.NodeForeignID);

                if (resourcesXML == null || resourcesXML == "")
                {
                    MessageBox.Show("GET request to OpenNMS failed!", "Error!");
                    return;
                }

                //Parse the datasoruces from the equest result
                XMLParser.GetResourcesFromXML(modelService, resourcesXML);
                //View the datasources
                DatasourceListView.ItemsSource = null;
                DatasourceListView.ItemsSource = modelService.GetDatasources();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        private void CreateNewDashboardButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Creates a new dashboard

            //Open a window and wait for userinput
            DashboardPopUp dashboardPopUp = new DashboardPopUp
            {
                Owner = this
            };
            dashboardPopUp.ShowDialog();

            //Check if the user confirmed the input and check the input for valid format
            if (!dashboardPopUp.ButtonPressed)
            {
                return;
            }

            string name = dashboardPopUp.EnteredText;

            if (name == "")
            {
                string title = "No name entered!";
                string text = "You need to enter a name for your new dashboard!";
                MessageBox.Show(text, title);
                return;
            }

            //Create the new dashboard with the entered input and connect it with its relevant view elements
            TabItem newDashboardTab = new TabItem();
            DashboardView dashboardView = new DashboardView();
            Dashboard newDashboard = modelService.CreateDashboard(name, newDashboardTab, dashboardView.FindName("DashboardTabControl") as TabControl);

            Binding newBinding = new Binding("Name");
            newBinding.Source = newDashboard;

            newDashboardTab.Name = "Dashboard";
            newDashboardTab.SetBinding(TabItem.HeaderProperty, newBinding);

            Frame frame = new Frame();
            dashboardView.SetBinding(TabControl.NameProperty, newBinding);

            dashboardView.DataContext = newDashboard;
            frame.Content = dashboardView;

            newDashboardTab.Content = frame;
            MainTabControl.Items.Add(newDashboardTab);
            MainTabControl.SelectedItem = newDashboardTab;

            TabItem freeSpaceTabItem = dashboardView.FindName("FreeSpaceRow") as TabItem;
            ListView freeSpaceListView = dashboardView.FindName("FreeSpaceRowListView") as ListView;
            Row freeSpaceRow = modelService.CreateRow("FreeSpace", freeSpaceTabItem, freeSpaceListView);
            freeSpaceListView.ItemsSource = freeSpaceRow.Datasources;
            newDashboard.WithRows(freeSpaceRow);
        }

        private void CreateNewRowButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Creates a new row

            //Open a window and wait for userinput
            TabItem selectedTab = MainTabControl.SelectedItem as TabItem;

            RowPopUp rowPopUp = new RowPopUp(modelService.GetDashboards(), modelService.GetDashboardByTabItem(selectedTab))
            {
                Owner = this
            };
            rowPopUp.ShowDialog();

            //Check if the user confirmed the input and check the input for valid format
            if (!rowPopUp.ButtonPressed)
            {
                return;
            }

            string name = rowPopUp.EnteredText;

            if (name == "")
            {
                string title = "No name entered!";
                string text = "You need to enter a name for your new row!";
                MessageBox.Show(text, title);
                return;
            }

            Dashboard dashboard = rowPopUp.SelectedDashboard;

            if (dashboard == null)
            {
                string title = "No dashboard selected!";
                string text = "You need to select a dashboard for your new row!";
                MessageBox.Show(text, title);
                return;
            }

            //Create the new row with the entered input and connect it with its relevant view elements
            TabControl dashboardTabControl = dashboard.LinkedTabControl;

            TabItem newRowTab = new TabItem();
            Frame frame = new Frame();
            RowView rowView = new RowView();
            Row newRow = modelService.CreateRow(name, newRowTab, rowView.RowListView);

            Binding newBinding = new Binding("Name");
            newBinding.Source = newRow;

            newRowTab.SetBinding(TabItem.HeaderProperty, newBinding);
            rowView.SetBinding(TabControl.NameProperty, newBinding);

            rowView.DataContext = newRow;
            frame.Content = rowView;

            newRowTab.Content = frame;
            dashboardTabControl.Items.Add(newRowTab);
            dashboardTabControl.SelectedItem = newRowTab;
            rowView.RowListView.ItemsSource = newRow.Datasources;

            dashboard.WithRows(newRow);
        }

        private void AddDatasourceToDashboardButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Adds the selected datasources to a dashboard

            //Opens a window and wait for the user to select the target dashboard and row
            AddToDashboardPopUp addToDashboardPopUp = new AddToDashboardPopUp(modelService.GetDashboards())
            {
                Owner = this
            };
            addToDashboardPopUp.ShowDialog();

            //Check if the user confirmed the input and check the input
            if (!addToDashboardPopUp.ButtonPressed)
            {
                return;
            }

            Row row = addToDashboardPopUp.SelectedRow;

            if (row == null)
            {
                return; //TODO
            }

            //Add the selected datasources to the selcted row of the selected dashboard
            foreach (Datasource dataSource in DatasourceListView.SelectedItems)
            {
                modelService.AddDataSourceToRow(dataSource, row);
            }

            row.LinkedListView.Items.Refresh();
        }

        private void RemoveDatasourceFromDashboardButton_OnCLick(object sender, RoutedEventArgs e)
        {
            //Removes datasources from the selected dashboard or the main sourceview

            //Opens a window to check if the user just missclicked
            AreYouShurePopUp popUp = new AreYouShurePopUp("Delete these Datasources?")
            {
                Owner = this
            };
            popUp.ShowDialog();

            //Check if the user confirmed the input and check the input
            if (!popUp.ButtonPressed)
            {
                return;
            }

            //Check if the "Datasources"-Tab is selcted
            if ((MainTabControl.SelectedItem as TabItem).Equals(DatasourceTabItem))
            {
                //If yes, then delete the datasources from the main sourceview
                foreach (Datasource dataSource in DatasourceListView.SelectedItems)
                {
                    modelService.RemoveDataSource(dataSource);
                }

                DatasourceListView.ItemsSource = modelService.GetDatasources();
            }
            else
            {
                //If not, then delete the datasources from the selected dashboard
                Dashboard selectedDashboard = modelService.GetDashboardByTabItem(MainTabControl.SelectedItem as TabItem);
                TabItem selectedRowTabItem = selectedDashboard.LinkedTabControl.SelectedItem as TabItem;
                Row selectedRow = modelService.GetRowByTabItem(selectedRowTabItem);

                if (selectedRow == null)
                {
                    return;
                }

                ListView listView = selectedRow.LinkedListView;

                foreach (Datasource datasource in listView.SelectedItems)
                {
                    modelService.RemoveDatasourceFromRow(selectedRow, datasource);
                }

                listView.Items.Refresh();
            }
        }

        private void CreateExportJSONButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Creates a json-text for manual export and show it in a text viewer

            //First wait for the user to select a dashboard
            SelectDashboardPopUp selectDashboardPopUp = new SelectDashboardPopUp(modelService.GetDashboards())
            {
                Owner = this
            };
            selectDashboardPopUp.ShowDialog();

            //Confirmation check
            if (!selectDashboardPopUp.SingleSelectionButtonPressed && !selectDashboardPopUp.MultiSelectionButtonPressed)
            {
                return;
            }

            //Check the user input
            List<Dashboard> selectedDashboard = selectDashboardPopUp.SelectedDashboards;

            if (selectedDashboard == null)
            {
                string title = "No dashboard selected!";
                string text = "You need to select at least one dashboard!";
                MessageBox.Show(text, title);
                return;
            }

            foreach (Dashboard dashboard in selectedDashboard)
            {
                if (dashboard == null)
                {
                    return;
                }
            }

            //Check if every datasource has a template
            bool noTemplatefound = false;
            foreach (Dashboard dashboard in selectedDashboard)
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

            //Creates the json-text and open the view
            try
            {
                foreach (Dashboard dashboard in selectedDashboard)
                {
                    JObject result = null;
                    if (selectedDashboard != null)
                    {
                        result = JSONParser.CreateNewDashboardJSON(dashboard);
                    }

                    JSONViewer viewer = new JSONViewer(result.ToString())
                    {
                        Title = dashboard.Name,
                        Owner = this
                    };
                    viewer.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        private void PostDashboardsButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Opens a window for REST-POST dashboards to Grafana
            GrafanaPOSTView view = new GrafanaPOSTView(modelService)
            {
                Owner = this
            };
            view.Show();
        }

        private void GetNewTemplateButton_OnCLick(object sender, RoutedEventArgs e)
        {
            //Button is hidden, function switched to the tempalte viewer
            //Dont remove until the button exists, otherwise the program breaks
            GetNewTemplatePopUp popUp = new GetNewTemplatePopUp
            {
                Owner = this
            };
            popUp.ShowDialog();

            if (!popUp.ButtonPressed)
            {
                return;
            }

            string templateName = popUp.TemplateName;
            string templateTitle = popUp.JSONTitle;
            string pathToJSON = popUp.PathToJSON;
            bool replaceNodeID = popUp.ReplaceNodeID;
            bool replaceResourceID = popUp.ReplaceResourceID;

            modelService.CreateTemplate(templateName, templateTitle, pathToJSON, replaceNodeID, replaceResourceID);
        }

        private void OpenTempalteViewerButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Opens a window to manage templates
            TemplateViewer templateViewer = new TemplateViewer(modelService)
            {
                Owner = this
            };
            templateViewer.ShowDialog();
        }

        private void SetTemplateForDatasourceButton_OnCLick(object sender, RoutedEventArgs e)
        {
            //Adds a template to the selcted datasources

            //Check if the selected tab is the "Datasources"-tab
            if ((MainTabControl.SelectedItem as TabItem).Equals(DatasourceTabItem))
            {
                return;
            }

            //Opens a window an wait for userinput
            SetTemplatePopUp popUp = new SetTemplatePopUp(modelService.GetTemplates())
            {
                Owner = this
            };
            popUp.ShowDialog();

            if (!popUp.ButtonPressed)
            {
                return;
            }

            //Check if the input is valid and set the tempalte for the given datasources
            Template template = popUp.SelectedTemplate;

            if (template == null)
            {
                string title = "No tempalte selected!";
                string text = "You need to select a template!";
                MessageBox.Show(text, title);
                return;
            }

            DashboardView dashboardView = ((MainTabControl.SelectedItem as TabItem).Content as Frame).Content as DashboardView;
            TabControl dashboardTabControl = dashboardView.FindName("DashboardTabControl") as TabControl;
            Row selectedRow = modelService.GetRowByTabItem(dashboardTabControl.SelectedItem as TabItem);

            ListView selectedListview = selectedRow.LinkedListView;

            if (selectedListview != null)
            {
                foreach (Datasource datasource in selectedListview.SelectedItems)
                {
                    modelService.SetTemplateForDatasource(datasource, template);
                }
            }

            selectedListview.Items.Refresh();
        }

        private void AddSpecialDatasourceButton_OnCLick(object sender, RoutedEventArgs e)
        {
            //For adding a datasource with manual properties
            AddSpecialDatasourcePopUp popUp = new AddSpecialDatasourcePopUp(modelService)
            {
                Owner = this
            };
            popUp.ShowDialog();

            //Buttoncheck
            if (!popUp.ButtonPressed)
            {
                return;
            }

            Node node = popUp.SelectedNode;
            string resourceIDString = popUp.SelectedResourceIDString;
            string name = popUp.EnteredText;

            modelService.CreateDatasource(name, resourceIDString, node);
            DatasourceListView.ItemsSource = modelService.GetDatasources();
            DatasourceListView.Items.Refresh();
        }

        private void MoveUpSelectedDatasources_OnClick(object sender, RoutedEventArgs e)
        {
            //Moving up the selected datasource
            TabItem selectedTab = MainTabControl.SelectedItem as TabItem;
            Dashboard selectedDashboard = modelService.GetDashboardByTabItem(selectedTab);

            if (selectedDashboard == null)
            {
                string title = "No dashboard selected!";
                string text = "You need to select a dashboard!";
                MessageBox.Show(text, title);
                return;
            }

            selectedTab = selectedDashboard.LinkedTabControl.SelectedItem as TabItem;
            Row selectedRow = modelService.GetRowByTabItem(selectedTab);

            if (selectedRow == null)
            {
                string title = "No row selected!";
                string text = "You need to select a row!";
                MessageBox.Show(text, title);
                return;
            }

            foreach (Datasource datasource in selectedRow.LinkedListView.SelectedItems)
            {
                modelService.SwapDatasourcesUp(datasource, selectedRow);
            }

            selectedRow.LinkedListView.Items.Refresh();
        }

        private void MoveDownSelectedDatasources_OnClick(object sender, RoutedEventArgs e)
        {
            //Moving down the selected datasource
            TabItem selectedTab = MainTabControl.SelectedItem as TabItem;
            Dashboard selectedDashboard = modelService.GetDashboardByTabItem(selectedTab);

            if (selectedDashboard == null)
            {
                string title = "No dashboard selected!";
                string text = "You need to select a dashboard!";
                MessageBox.Show(text, title);
                return;
            }

            selectedTab = selectedDashboard.LinkedTabControl.SelectedItem as TabItem;
            Row selectedRow = modelService.GetRowByTabItem(selectedTab);

            if (selectedRow == null)
            {
                string title = "No row selected!";
                string text = "You need to select a row!";
                MessageBox.Show(text, title);
                return;
            }

            foreach (Datasource datasource in selectedRow.LinkedListView.SelectedItems)
            {
                modelService.SwapDatasourcesDown(datasource, selectedRow);
            }

            selectedRow.LinkedListView.Items.Refresh();
        }

        private void MoveLeftSelectedRowButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Moveing up the selected row
            TabItem selectedTab = MainTabControl.SelectedItem as TabItem;
            Dashboard selectedDashboard = modelService.GetDashboardByTabItem(selectedTab);

            if (selectedDashboard == null)
            {
                string title = "No dashboard selected!";
                string text = "You need to select a dashboard!";
                MessageBox.Show(text, title);
                return;
            }

            selectedTab = selectedDashboard.LinkedTabControl.SelectedItem as TabItem;
            Row selectedRow = modelService.GetRowByTabItem(selectedTab);

            if (selectedRow == null)
            {
                string title = "No row selected!";
                string text = "You need to select a row!";
                MessageBox.Show(text, title);
                return;
            }

            modelService.SwapRowsLeft(selectedRow, selectedDashboard);
        }

        private void MoveRightSelectedRowButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Moveing down the selected row
            TabItem selectedTab = MainTabControl.SelectedItem as TabItem;
            Dashboard selectedDashboard = modelService.GetDashboardByTabItem(selectedTab);

            if (selectedDashboard == null)
            {
                string title = "No row selected!";
                string text = "You need to select a row!";
                MessageBox.Show(text, title);
                return;
            }

            selectedTab = selectedDashboard.LinkedTabControl.SelectedItem as TabItem;
            Row selectedRow = modelService.GetRowByTabItem(selectedTab);

            if (selectedRow == null)
            {
                string title = "No row selected!";
                string text = "You need to select a row!";
                MessageBox.Show(text, title);
                return;
            }

            modelService.SwapRowsRight(selectedRow, selectedDashboard);
        }

        private void OpenCredentialsViewer_OnClick(object sender, RoutedEventArgs e)
        {
            //Opens a window for manage credentials
            try
            {
                CredentialsViewer viewer = new CredentialsViewer(modelService)
                {
                    Owner = this
                };
                viewer.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EditDashboardsButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Opens a window for edit or delte rows/dashboards
            EditDashboardsView view = new EditDashboardsView(modelService)
            {
                Owner = this
            };
            view.ShowDialog();

            MainTabControl.Items.Refresh();

            foreach (Dashboard dashboard in modelService.GetDashboards())
            {
                dashboard.LinkedTabControl.Items.Refresh();
            }
        }

        private void ShowHideMoveButton_OnCLick(object sender, RoutedEventArgs e)
        {
            if (MoveDownSelectedDatasources.Visibility == Visibility.Visible)
            {
                MoveDownSelectedDatasources.Visibility = Visibility.Hidden;
                MoveUpSelectedDatasources.Visibility = Visibility.Hidden;
                MoveLeftSelectedRow.Visibility = Visibility.Hidden;
                MoveRightSelectedRow.Visibility = Visibility.Hidden;
            }
            else if (MoveDownSelectedDatasources.Visibility == Visibility.Hidden)
            {
                MoveDownSelectedDatasources.Visibility = Visibility.Visible;
                MoveUpSelectedDatasources.Visibility = Visibility.Visible;
                MoveLeftSelectedRow.Visibility = Visibility.Visible;
                MoveRightSelectedRow.Visibility = Visibility.Visible;
            }
        }
    }
}
