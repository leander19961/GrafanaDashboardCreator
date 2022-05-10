using GrafanaDashboardCreator.Model;
using GrafanaDashboardCreator.Net;
using GrafanaDashboardCreator.Parser;
using GrafanaDashboardCreator.Resource;
using GrafanaDashboardCreator.View;
using HandlebarsDotNet.Collections;
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
using System.Windows.Shapes;

using static GrafanaDashboardCreator.Resource.Constants;

namespace GrafanaDashboardCreator
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly ModelService modelService;

        public MainWindow()
        {
            InitializeComponent();
            SettingsIO.CheckForFileSystem();

            modelService = new ModelService();
        }

        private void GetDatasourcesButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                XMLParser.GetNodesFromXML(modelService, RESTAPI.GETNodesFromOpenNMS());

                SelectNodePopUp popUp = new SelectNodePopUp(modelService.GetNodes())
                {
                    Owner = this
                };

                popUp.ShowDialog();

                if (!popUp.ButtonPressed || popUp.SelectedNode == null)
                {
                    return;
                }

                string resourcesXML = RESTAPI.GETResourcesFromOpenNMS(popUp.SelectedNode.NodeForeignSource + ":" + popUp.SelectedNode.NodeForeignID);

                if (resourcesXML == null)
                {
                    resourcesXML = "Null";
                }

                /* For debugging
                JSONViewer viewer = new JSONViewer(resourcesXML)
                {
                    Owner = this
                };

                viewer.Show();
                */

                XMLParser.GetResourcesFromXML(modelService, resourcesXML);
                DatasourceListView.ItemsSource = modelService.GetDatasources();
            }
            catch (Exception ex)
            {
                JSONViewer viewer = new JSONViewer(ex.ToString())
                {
                    Title = "Error!",
                    Owner = this
                };

                viewer.Show();
            }
        }

        private void CreateNewDashboardButton_OnClick(object sender, RoutedEventArgs e)
        {
            DashboardPopUp dashboardPopUp = new DashboardPopUp
            {
                Owner = this
            };
            dashboardPopUp.ShowDialog();

            if (!dashboardPopUp.ButtonPressed)
            {
                return;
            }

            string name = dashboardPopUp.EnteredText;

            if (name == "")
            {
                return; //TODO
            }

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
            TabItem selectedTab = MainTabControl.SelectedItem as TabItem;

            RowPopUp rowPopUp = new RowPopUp(modelService.GetDashboards(), modelService.GetDashboardByTabItem(selectedTab))
            {
                Owner = this
            };
            rowPopUp.ShowDialog();

            if (!rowPopUp.ButtonPressed)
            {
                return;
            }

            string name = rowPopUp.EnteredText;

            if (name == "")
            {
                return; //TODO
            }

            Dashboard dashboard = rowPopUp.SelectedDashboard;

            if (dashboard == null)
            {
                return; //TODO
            }

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
            if ((MainTabControl.SelectedItem as TabItem).Name != "DatasourceTabItem")
            {
                return;
            }

            AddToDashboardPopUp addToDashboardPopUp = new AddToDashboardPopUp(modelService.GetDashboards())
            {
                Owner = this
            };
            addToDashboardPopUp.ShowDialog();

            if (!addToDashboardPopUp.ButtonPressed)
            {
                return;
            }

            //Dashboard dashboard = addToDashboardPopUp.SelectedDashboard;
            Row row = addToDashboardPopUp.SelectedRow;

            if (row == null)
            {
                return; //TODO
            }

            foreach (Datasource dataSource in DatasourceListView.SelectedItems)
            {
                modelService.AddDataSourceToRow(dataSource, row);
            }

            row.LinkedListView.Items.Refresh();
        }

        private void RemoveSelectedDashboardButton_OnCLick(object sender, RoutedEventArgs e)
        {
            RemoveDashboardPopUp popUp = new RemoveDashboardPopUp(modelService.GetDashboards())
            {
                Owner = this
            };
            popUp.ShowDialog();

            if (!popUp.ButtonPressed)
            {
                return;
            }

            Dashboard dashboard = popUp.SelectedDashboard;

            if (dashboard == null)
            {
                return; //TODO
            }

            MainTabControl.Items.Remove(dashboard.LinkedTabItem);
            MainTabControl.Items.Refresh();
            modelService.RemoveDashboard(dashboard);
        }

        private void RemoveSelectedRowButton_OnCLick(object sender, RoutedEventArgs e)
        {
            RemoveRowPopUp popUp = new RemoveRowPopUp(modelService.GetDashboards())
            {
                Owner = this
            };
            popUp.ShowDialog();

            if (!popUp.ButtonPressed)
            {
                return;
            }

            Row row = popUp.SelectedRow;

            if (row == null)
            {
                return; //TODO
            }

            Dashboard dashboard = popUp.SelectedDashboard;
            dashboard.LinkedTabControl.Items.Remove(row.LinkedTabItem);
            dashboard.LinkedTabControl.Items.Refresh();
            modelService.RemoveRow(row);
        }

        private void RemoveDatasourceFromDashboardButton_OnCLick(object sender, RoutedEventArgs e)
        {
            if ((MainTabControl.SelectedItem as TabItem).Equals(DatasourceTabItem))
            {
                foreach (Datasource dataSource in DatasourceListView.SelectedItems)
                {
                    modelService.RemoveDataSourceFromRow(dataSource);
                }

                DatasourceListView.ItemsSource = modelService.GetDatasources();
            }
            else
            {
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
            SelectDashboardPopUp selectDashboardPopUp = new SelectDashboardPopUp(modelService.GetDashboards())
            {
                Owner = this
            };
            selectDashboardPopUp.ShowDialog();

            if (!selectDashboardPopUp.SingleSelectionButtonPressed && !selectDashboardPopUp.MultiSelectionButtonPressed)
            {
                JSONViewer viewer = new JSONViewer("Return at Buttoncheck")
                {
                    Title = "Error!",
                    Owner = this
                };
                viewer.Show();
                return;
            }

            List<Dashboard> selectedDashboard = selectDashboardPopUp.SelectedDashboard;

            if (selectedDashboard == null)
            {
                JSONViewer viewer = new JSONViewer("Return at List-Test")
                {
                    Title = "Error!",
                    Owner = this
                };
                viewer.Show();
                return;
            }

            foreach (Dashboard dashboard in selectedDashboard)
            {
                if (dashboard == null)
                {
                    JSONViewer viewer = new JSONViewer("Return at Dashboards-Test")
                    {
                        Title = "Error!",
                        Owner = this
                    };
                    viewer.Show();
                    return;
                }
            }

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
                ErrorPopUp popUp = new ErrorPopUp("Not all of your datasources does have a Template")
                {
                    Owner = this
                };
                popUp.ShowDialog();

                return;
            }

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

                    //TODO
                }
            }
            catch (Exception ex)
            {

                JSONViewer viewer = new JSONViewer(ex.StackTrace.ToString())
                {
                    Title = "Error!",
                    Owner = this
                };
                viewer.Show();
            }
        }

        private void GetNewTemplateButton_OnCLick(object sender, RoutedEventArgs e)
        {
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
            TemplateViewer templateViewer = new TemplateViewer(modelService)
            {
                Owner = this
            };
            templateViewer.ShowDialog();
        }

        private void SetTemplateForDatasourceButton_OnCLick(object sender, RoutedEventArgs e)
        {
            if ((MainTabControl.SelectedItem as TabItem).Equals(DatasourceTabItem))
            {
                return;
            }

            SetTemplatePopUp popUp = new SetTemplatePopUp(modelService.GetTemplates())
            {
                Owner = this
            };
            popUp.ShowDialog();

            if (!popUp.ButtonPressed)
            {
                return;
            }

            Template template = popUp.SelectedTemplate;

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

        private void RenameSelectedDashboardButton_OnClick(object sender, RoutedEventArgs e)
        {
            Dashboard dashboard = modelService.GetDashboardByTabItem(MainTabControl.SelectedItem as TabItem);

            if (dashboard == null)
            {
                return; //TODO
            }

            RenamePopUp popUp = new RenamePopUp
            {
                Owner = this
            };
            popUp.ShowDialog();

            if (!popUp.ButtonPressed)
            {
                return;
            }

            dashboard.Name = popUp.EnteredText;
            MainTabControl.Items.Refresh();
        }

        private void RenameSelectedRowButton_OnClick(object sender, RoutedEventArgs e)
        {
            if ((MainTabControl.SelectedItem as TabItem).Name.Equals("DatasourceTabItem"))
            {
                return;
            }

            DashboardView dashboardView = ((MainTabControl.SelectedItem as TabItem).Content as Frame).Content as DashboardView;
            TabControl dashboardViewTabControl = dashboardView.FindName("DashboardTabControl") as TabControl;

            if ((dashboardViewTabControl.SelectedItem as TabItem).Name.Equals("FreeSpaceRow"))
            {
                return;
            }

            TabItem rowTabItem = dashboardViewTabControl.SelectedItem as TabItem;
            Row row = modelService.GetRowByTabItem(rowTabItem);

            RenamePopUp popUp = new RenamePopUp
            {
                Owner = this
            };
            popUp.ShowDialog();

            if (!popUp.ButtonPressed)
            {
                return;
            }

            row.Name = popUp.EnteredText;
            dashboardViewTabControl.Items.Refresh();
        }

        private void AddSpecialDatasourceButton_OnCLick(object sender, RoutedEventArgs e)
        {
            AddSpecialDatasourcePopUp popUp = new AddSpecialDatasourcePopUp(modelService)
            {
                Owner = this
            };
            popUp.ShowDialog();

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
            TabItem selectedTab = MainTabControl.SelectedItem as TabItem;
            Dashboard selectedDashboard = modelService.GetDashboardByTabItem(selectedTab);

            if (selectedDashboard == null)
            {
                return; //TODO
            }

            selectedTab = selectedDashboard.LinkedTabControl.SelectedItem as TabItem;
            Row selectedRow = modelService.GetRowByTabItem(selectedTab);

            if (selectedRow == null)
            {
                return; //TODO
            }

            foreach (Datasource datasource in selectedRow.LinkedListView.SelectedItems)
            {
                modelService.SwapDatasourcesUp(datasource, selectedRow);
            }

            selectedRow.LinkedListView.Items.Refresh();
        }

        private void MoveDownSelectedDatasources_OnClick(object sender, RoutedEventArgs e)
        {
            TabItem selectedTab = MainTabControl.SelectedItem as TabItem;
            Dashboard selectedDashboard = modelService.GetDashboardByTabItem(selectedTab);

            if (selectedDashboard == null)
            {
                return; //TODO
            }

            selectedTab = selectedDashboard.LinkedTabControl.SelectedItem as TabItem;
            Row selectedRow = modelService.GetRowByTabItem(selectedTab);

            if (selectedRow == null)
            {
                return; //TODO
            }

            foreach (Datasource datasource in selectedRow.LinkedListView.SelectedItems)
            {
                modelService.SwapDatasourcesDown(datasource, selectedRow);
            }

            selectedRow.LinkedListView.Items.Refresh();
        }

        private void MoveLeftSelectedRowButton_OnClick(object sender, RoutedEventArgs e)
        {
            TabItem selectedTab = MainTabControl.SelectedItem as TabItem;
            Dashboard selectedDashboard = modelService.GetDashboardByTabItem(selectedTab);

            if (selectedDashboard == null)
            {
                return; //TODO
            }

            selectedTab = selectedDashboard.LinkedTabControl.SelectedItem as TabItem;
            Row selectedRow = modelService.GetRowByTabItem(selectedTab);

            if (selectedRow == null)
            {
                return; //TODO
            }

            modelService.SwapRowsLeft(selectedRow, selectedDashboard);
        }

        private void MoveRightSelectedRowButton_OnClick(object sender, RoutedEventArgs e)
        {
            TabItem selectedTab = MainTabControl.SelectedItem as TabItem;
            Dashboard selectedDashboard = modelService.GetDashboardByTabItem(selectedTab);

            if (selectedDashboard == null)
            {
                return; //TODO
            }

            selectedTab = selectedDashboard.LinkedTabControl.SelectedItem as TabItem;
            Row selectedRow = modelService.GetRowByTabItem(selectedTab);

            if (selectedRow == null)
            {
                return; //TODO
            }

            modelService.SwapRowsRight(selectedRow, selectedDashboard);
        }
    }
}
