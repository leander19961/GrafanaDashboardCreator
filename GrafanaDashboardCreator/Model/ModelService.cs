using GrafanaDashboardCreator.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using static GrafanaDashboardCreator.Resource.SettingsIO;
using static GrafanaDashboardCreator.Resource.Constants;
using GrafanaDashboardCreator.Net;
using System.Collections.ObjectModel;
using System.Windows;

namespace GrafanaDashboardCreator.Model
{
    public class ModelService
    {
        //The modelservice is an "internal API" for editing the datamodel
        //Its job is to edit the model in that way, that every part of the programm works as intended

        //The modelservice knows everyone and saves the connections in some hidden properties
        private List<Row> rows;
        private List<Dashboard> dashboards;
        private List<Datasource> datasources;
        private List<Template> templates;
        private List<Node> nodes;
        private Folder standardFolder;

        public ModelService()
        {
            rows = new List<Row>();
            nodes = new List<Node>();
            dashboards = new List<Dashboard>();
            datasources = new List<Datasource>();
            templates = LoadTemplates();
            standardFolder = new Folder("General", null, null);
        }

        //ObservableCollection is a list-type that is better to track for view elements
        //If there are made changes on the elements, than the list tells it to the view
        public ObservableCollection<Dashboard> GetDashboards()
        {
            //Returns all dashboards as ObservableCollection
            ObservableCollection<Dashboard> _dashboards = new ObservableCollection<Dashboard>();

            foreach (Dashboard dashboard in dashboards)
            {
                _dashboards.Add(dashboard);
            }

            return _dashboards;
        }

        public ObservableCollection<Template> GetTemplates()
        {
            //Returns all templates as ObservableCollection
            ObservableCollection<Template> _templates = new ObservableCollection<Template>();

            foreach (Template template in templates)
            {
                _templates.Add(template);
            }

            return _templates;
        }

        public ObservableCollection<Row> GetRows()
        {
            //Returns all rows as ObservableCollection
            ObservableCollection<Row> _rows = new ObservableCollection<Row>();

            foreach (Row row in rows)
            {
                _rows.Add(row);
            }

            return _rows;
        }

        public ObservableCollection<Datasource> GetDatasources()
        {
            //Returns all datasources as ObservableCollection
            ObservableCollection<Datasource> datasourceList = new ObservableCollection<Datasource>();

            foreach (Datasource datasource in datasources)
            {
                datasourceList.Add(datasource);
            }

            return datasourceList;
        }

        public ObservableCollection<Node> GetNodes()
        {
            //Returns all nodes as ObservableCollection
            ObservableCollection<Node> nodeList = new ObservableCollection<Node>();

            foreach (Node node in nodes)
            {
                nodeList.Add(node);
            }

            return nodeList;
        }

        public ObservableCollection<Credentials> GetCredentials()
        {
            //Returns all credentials as ObservableCollection
            ObservableCollection<Credentials> credentialList = new ObservableCollection<Credentials>();

            Dictionary<string, string> openNMSCredentials = XMLParser.GetOpenNMSCredentials();
            Dictionary<string, string> grafanaCredentials = XMLParser.GetGrafanaCredentials();

            credentialList.Add(new Credentials(OpenNMSCredentailsXmlNode, openNMSCredentials["username"], openNMSCredentials["password"], "", openNMSCredentials["url"]));
            credentialList.Add(new Credentials(GrafanaCredentailsXmlNode, "", "", grafanaCredentials["token"], grafanaCredentials["url"]));

            return credentialList;
        }

        internal Node GetSpecificNode(string label, string nodeName, string nodeID)
        {
            //Check if a node with the given data already exists, else create it
            foreach (Node node in nodes)
            {
                if (node.NodeID == nodeID) { return node; }
            }

            string[] foreignDetails = nodeName.Split(':');
            return CreateNode(label, nodeID, foreignDetails[1], foreignDetails[0]);
        }

        public Dashboard CreateDashboard(string name, TabItem tabItem, TabControl tabControl)
        {
            //Creates a new dashboard in the datamodel
            Dashboard newDashBoard = new Dashboard(name, tabItem, tabControl, standardFolder);

            dashboards.Add(newDashBoard);

            return newDashBoard;
        }

        public Node CreateNode(string label, string nodeID, string nodeForeignID, string nodeForeignSource)
        {
            //Creates a new node in the datamodel
            foreach (Node node in nodes)
            {
                if (node.NodeForeignID == nodeForeignID && node.NodeForeignSource == nodeForeignSource)
                {
                    return node;
                }
            }

            Node newNode = new Node(label, nodeID, nodeForeignID, nodeForeignSource);

            nodes.Add(newNode);

            return newNode;
        }

        public Row CreateRow(string name, TabItem tabItem, ListView listView)
        {
            //Creates a new row in the datamodel
            Row newRow = new Row(name, tabItem, listView);

            rows.Add(newRow);

            return newRow;
        }

        public void CreateDatasource(string name, string resourceID, Node node)
        {
            //Creates a new datasource in the datamodel
            Datasource newDatasource = new Datasource(name, resourceID, node);
            foreach (Datasource datasource in datasources)
            {
                if (datasource.ResourceID == newDatasource.ResourceID && datasource.Node == newDatasource.Node && newDatasource.Label == datasource.Label)
                {
                    return;
                }
            }
            datasources.Add(newDatasource);
        }

        public void CreateTemplate(string templateName, string templateTitle, string pathToJSON, bool replaceNodeID, bool replaceResourceID)
        {
            //Creates a new template in the datamodel
            string result = JSONParser.GetTemplate(templateTitle, pathToJSON, replaceNodeID, replaceResourceID);
            Template template = new Template(templateName, result, replaceNodeID, replaceResourceID);

            templates.Add(template);
            SaveTempalte(template);
        }

        internal void ReCreateTemplate(Template template, string templateName, string jSONtext, bool replaceNodeID, bool replaceResourceID)
        {
            //For editing a template
            Template newTemplate = new Template(templateName, jSONtext, replaceNodeID, replaceResourceID);

            RemoveTemplate(template);
            templates.Add(newTemplate);
            SaveTempalte(newTemplate);
        }

        internal void EditCredentialProperties(Credentials credentials, string username, string password, string token, string url)
        {
            //For editding the credentials
            credentials.Username = username;
            credentials.Password = password;
            credentials.Token = token;
            credentials.Url = url;

            if (credentials.Name == OpenNMSCredentailsXmlNode)
            {
                credentials.SaveToOpenNMSFile();
            }
            else if (credentials.Name == GrafanaCredentailsXmlNode)
            {
                credentials.SaveToGrafanaFile();
            }
        }

        public void SortDataSources()
        {
            //For better order in view
            datasources = datasources.OrderBy(x => x.ResourceID).ToList();
            datasources = datasources.OrderBy(x => x.NodeID).ToList();
        }

        public Dashboard GetDashboardByTabItem(TabItem selectedTab)
        {
            //Get the dashboard that is connected to a specific view element
            foreach (Dashboard dashboard in dashboards)
            {
                if (dashboard.LinkedTabItem == selectedTab) { return dashboard; }
            }

            return null;
        }

        public Row GetRowByTabItem(TabItem selectedRowTabItem)
        {
            //Get the row that is connected to a specific view element
            foreach (Row row in rows)
            {
                if (row.LinkedTabItem == selectedRowTabItem) { return row; }
            }

            return null;
        }

        public void RemoveDashboard(Dashboard dashboard)
        {
            //Removes the given dashboard
            while (dashboard.GetRows().Count > 0)
            {
                Row row = dashboard.GetRows().Last();
                RemoveRow(row);
            }

            dashboards.Remove(dashboard);
            dashboard = null;
        }

        public void RemoveRow(Row row)
        {
            //Removes the given row
            row.RemoveYou();
            rows.Remove(row);
            row = null;
        }

        public void RemoveDataSource(Datasource datasource)
        {
            //Removes the given datasource from the available datasources
            datasources.Remove(datasource);

            foreach (Row row in rows)
            {
                RemoveDatasourceFromRow(row, datasource);
            }
        }

        public void RemoveDatasourceFromRow(Row row, Datasource datasource)
        {
            //Removes the given datasource from the given row
            row.WithoutDatasources(datasource);
        }

        public void RemoveTemplate(Template template)
        {
            //Removes the given template
            template.RemoveYou();
            templates.Remove(template);

            try
            {
                File.Delete(PanelTemplateDirectory + $"\\{template.Name}.xml");
            }
            catch
            {
                string title = "Fileerror!";
                string text = $"There was an error while saving {template.Name}.xml";
                MessageBox.Show(text, title);
            }
        }

        public void AddDataSourceToRow(Datasource datasource, Row row)
        {
            //Adding a exact copy of the given datasource to the given row
            row.Datasources.Add(datasource.CopyYou());
        }

        public void SetTemplateForDatasource(Datasource datasource, Template template)
        {
            //Sets the template for a given datasource
            datasource.Template = template;
        }

        //Swap methods are for manual sorting
        public void SwapDatasourcesUp(Datasource datasource, Row row)
        {
            //Moves the given datasource up in the list of the given row
            if (row.Datasources[0] == datasource)
            {
                return;
            }

            int indexOne = row.Datasources.IndexOf(datasource);
            int indexTwo = row.Datasources.IndexOf(datasource) - 1;

            if (row.LinkedListView.SelectedItems.Contains(row.Datasources[indexOne]) && row.LinkedListView.SelectedItems.Contains(row.Datasources[indexTwo]) && row.LinkedListView.SelectedItems.Contains(row.Datasources[0]))
            {
                return;
            }

            Datasource tmp = row.Datasources[indexOne];
            row.Datasources[indexOne] = row.Datasources[indexTwo];
            row.Datasources[indexTwo] = tmp;
        }

        public void SwapDatasourcesDown(Datasource datasource, Row row)
        {
            //Moves the given datasource down in the list of the given row
            if (row.Datasources[row.Datasources.Count - 1] == datasource)
            {
                return;
            }

            int indexOne = row.Datasources.IndexOf(datasource);
            int indexTwo = row.Datasources.IndexOf(datasource) + 1;

            if (row.LinkedListView.SelectedItems.Contains(row.Datasources[indexOne]) && row.LinkedListView.SelectedItems.Contains(row.Datasources[indexTwo]) && row.LinkedListView.SelectedItems.Contains(row.Datasources[row.Datasources.Count - 1]))
            {
                return;
            }

            Datasource tmp = row.Datasources[indexOne];
            row.Datasources[indexOne] = row.Datasources[indexTwo];
            row.Datasources[indexTwo] = tmp;
        }

        public void SwapRowsLeft(Row row, Dashboard dashboard)
        {
            //Moves the given row up in the list of the given dashboard
            if (dashboard.GetRows()[1] == row || row.Name == "FreeSpace")
            {
                return;
            }

            int indexOne = dashboard.GetRows().IndexOf(row);
            int indexTwo = dashboard.GetRows().IndexOf(row) - 1;

            Row tmpRow = dashboard.GetRows()[indexOne];
            dashboard.GetRows()[indexOne] = dashboard.GetRows()[indexTwo];
            dashboard.GetRows()[indexTwo] = tmpRow;

            TabControl dashboardTabControl = dashboard.LinkedTabControl;
            TabItem selectedTabItem = dashboardTabControl.SelectedItem as TabItem;

            List<TabItem> dasboardRowTabItems = new List<TabItem>();

            foreach (Row _row in dashboard.GetRows())
            {
                dasboardRowTabItems.Add(_row.LinkedTabItem);
            }

            foreach (TabItem tabItem in dasboardRowTabItems)
            {
                dashboardTabControl.Items.Remove(tabItem);
            }

            foreach (TabItem tabItem in dasboardRowTabItems)
            {
                dashboardTabControl.Items.Add(tabItem);
            }

            dashboardTabControl.SelectedItem = selectedTabItem;
        }

        public void SwapRowsRight(Row row, Dashboard dashboard)
        {
            //Moves the given row down in the list of the given dashboard
            if (dashboard.GetRows()[dashboard.GetRows().Count - 1] == row || row.Name == "FreeSpace")
            {
                return;
            }

            int indexOne = dashboard.GetRows().IndexOf(row);
            int indexTwo = dashboard.GetRows().IndexOf(row) + 1;


            Row tmp = dashboard.GetRows()[indexOne];
            dashboard.GetRows()[indexOne] = dashboard.GetRows()[indexTwo];
            dashboard.GetRows()[indexTwo] = tmp;

            TabControl dashboardTabControl = dashboard.LinkedTabControl;
            TabItem selectedTabItem = dashboardTabControl.SelectedItem as TabItem;

            List<TabItem> dasboardRowTabItems = new List<TabItem>();

            foreach (Row _row in dashboard.GetRows())
            {
                dasboardRowTabItems.Add(_row.LinkedTabItem);
            }

            foreach (TabItem tabItem in dasboardRowTabItems)
            {
                dashboardTabControl.Items.Remove(tabItem);
            }

            foreach (TabItem tabItem in dasboardRowTabItems)
            {
                dashboardTabControl.Items.Add(tabItem);
            }

            dashboardTabControl.SelectedItem = selectedTabItem;
        }
    }
}
