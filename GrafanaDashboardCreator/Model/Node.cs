using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaDashboardCreator.Model
{
    public class Node
    {
        private readonly string _label;
        private readonly string _nodeID;
        private readonly string _nodeForeignID;
        private readonly string _nodeForeignSource;

        public string Label { get { return _label; } }
        public string NodeID { get { return _nodeID; } }
        public string NodeForeignID { get { return _nodeForeignID; } }
        public string NodeForeignSource { get { return _nodeForeignSource; } }

        private List<Datasource> datasources = new List<Datasource>();

        public Node(string label, string nodeID, string nodeForeignID, string nodeForeignSource)
        {
            _label = label;
            _nodeID = nodeID;
            _nodeForeignID = nodeForeignID;
            _nodeForeignSource = nodeForeignSource;
        }

        public Node WithDatasources(Datasource value)
        {
            if (this.datasources == null) { this.datasources = new List<Datasource>(); }

            if (!this.datasources.Contains(value))
            {
                this.datasources.Add(value);
                value.setNode(this);
            }

            return this;
        }

        public Node WithDatasources(List<Datasource> dataSources)
        {
            foreach (Datasource dataSource in dataSources)
            {
                dataSources.Add(dataSource);
            }

            return this;
        }

        public Node WithoutDatasources(Datasource value)
        {
            if (this.datasources != null && this.datasources.Remove(value))
            {
                value.setRow(null);
            }

            return this;
        }

        public Node WithoutDatasources(List<Datasource> value)
        {
            foreach (Datasource item in value)
            {
                this.WithoutDatasources(item);
            }

            return this;
        }

        override
        public string ToString()
        {
            return _label;
        }
    }
}
