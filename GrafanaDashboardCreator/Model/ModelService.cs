using GrafanaDashboardCreator.Parser;
using HandlebarsDotNet.Collections;
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

namespace GrafanaDashboardCreator.Model
{
    public class ModelService
    {
        private List<Row> rows;
        private List<Dashboard> dashboards;
        private List<Datasource> datasources;
        private List<Template> templates;
        private List<Node> nodes;

        public ModelService()
        {
            rows = new List<Row>();
            nodes = new List<Node>();
            dashboards = new List<Dashboard>();
            datasources = new List<Datasource>();
            templates = LoadTemplates();
        }

        public ObservableList<Dashboard> GetDashboards()
        {
            ObservableList<Dashboard> _dashboards = new ObservableList<Dashboard>();

            foreach (Dashboard dashboard in dashboards)
            {
                _dashboards.Add(dashboard);
            }

            return _dashboards;
        }

        public ObservableList<Template> GetTemplates()
        {
            ObservableList<Template> _templates = new ObservableList<Template>();

            foreach (Template template in templates)
            {
                _templates.Add(template);
            }

            return _templates;
        }

        public ObservableList<Row> GetRows()
        {
            ObservableList<Row> _rows = new ObservableList<Row>();

            foreach (Row row in rows)
            {
                _rows.Add(row);
            }

            return _rows;
        }

        public Node GetNodeByID(string nodeid)
        {
            foreach (Node node in nodes)
            {
                if (node.NodeID == nodeid) { return node; }
            }

            return null;
        }

        internal Node GetNodeByName(string nodeName)
        {
            foreach (Node node in nodes)
            {
                if ((node.NodeForeignSource + ":" + node.NodeForeignID) == nodeName) { return node; }
            }

            return null;
        }

        internal Node GetSpecificNodeByName(string label, string nodeName, string nodeID)
        {
            foreach (Node node in nodes)
            {
                if (node.NodeID == nodeID) { return node; }
            }

            string[] foreignDetails = nodeName.Split(':');
            return CreateNode(label, nodeID, foreignDetails[1], foreignDetails[0]);
        }

        public ObservableList<Datasource> GetDatasources()
        {
            ObservableList<Datasource> datasourceList = new ObservableList<Datasource>();

            foreach (Datasource datasource in datasources)
            {
                datasourceList.Add(datasource);
            }

            return datasourceList;
        }

        public ObservableList<Node> GetNodes()
        {
            ObservableList<Node> nodeList = new ObservableList<Node>();

            foreach (Node node in nodes)
            {
                nodeList.Add(node);
            }

            return nodeList;
        }

        public Dashboard CreateDashboard(string name, TabItem tabItem, TabControl tabControl)
        {
            Dashboard newDashBoard = new Dashboard(name, tabItem, tabControl);

            dashboards.Add(newDashBoard);

            return newDashBoard;
        }

        public Node CreateNode(string label, string nodeID, string nodeForeignID, string nodeForeignSource)
        {
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
            Row newRow = new Row(name, tabItem, listView);

            rows.Add(newRow);

            return newRow;
        }

        public void CreateDatasource(string name, string resourceID, Node node)
        {
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

        public void SortDataSources()
        {
            datasources = datasources.OrderBy(x => x.ResourceID).ToList();
            datasources = datasources.OrderBy(x => x.NodeID).ToList();
        }

        public void CreateTemplate(string templateName, string templateTitle, string pathToJSON, bool replaceNodeID, bool replaceResourceID)
        {
            string result = JSONParser.GetTemplate(templateTitle, pathToJSON, replaceNodeID, replaceResourceID);
            Template template = new Template(templateName, result, replaceNodeID, replaceResourceID);

            templates.Add(template);
            SaveTempalte(template);
        }

        internal void ReCreateTemplate(string templateName, string jSONtext, bool replaceNodeID, bool replaceResourceID)
        {
            Template template = new Template(templateName, jSONtext, replaceNodeID, replaceResourceID);

            templates.Add(template);
            SaveTempalte(template);
        }

        public Dashboard CheckForDashboard(string name)
        {
            foreach (Dashboard dashboard in dashboards)
            {
                if (dashboard.Name == name) { return dashboard; }
            }

            return null;
        }

        public Dashboard GetDashboardByTabItem(TabItem selectedTab)
        {
            foreach (Dashboard dashboard in dashboards)
            {
                if (dashboard.LinkedTabItem == selectedTab) { return dashboard; }
            }

            return null;
        }

        public Row GetRowByTabItem(TabItem selectedRowTabItem)
        {
            foreach (Row row in rows)
            {
                if (row.LinkedTabItem == selectedRowTabItem) { return row; }
            }

            return null;
        }

        public void RemoveDashboard(Dashboard dashboard)
        {
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
            row.RemoveYou();
            rows.Remove(row);
            row = null;
        }

        public void RemoveDataSourceFromRow(Datasource datasource)
        {
            datasources.Remove(datasource);
        }

        public void RemoveDatasourceFromRow(Row row, Datasource datasource)
        {
            row.WithoutDatasources(datasource);
        }

        public void RemoveTemplate(Template template)
        {
            template.RemoveYou();
            templates.Remove(template);

            try
            {
                File.Delete(PanelTemplateDirectory + "\\" + template.Name + ".xml");
            }
            catch { } //TODO
        }

        public void AddDataSourceToRow(Datasource datasource, Row row)
        {
            row.Datasources.Add(datasource.CopyYou());
        }

        public void SetTemplateForDatasource(Datasource datasource, Template template)
        {
            datasource.Template = template;
        }

        public void SwapDatasourcesUp(Datasource datasource, Row row)
        {
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
    }
}
