using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaDashboardCreator.Model
{
    public class Node
    {
        //Hidden properties of the Node-Class
        private readonly string _label;
        private readonly string _nodeID;
        private readonly string _nodeForeignID;
        private readonly string _nodeForeignSource;

        //Public properties of the Node-Class
        public string Label { get { return _label; } }
        public string NodeID { get { return _nodeID; } }
        public string NodeForeignID { get { return _nodeForeignID; } }
        public string NodeForeignSource { get { return _nodeForeignSource; } }

        private List<Datasource> datasources = new List<Datasource>();

        //Constructor of the Node-Class
        public Node(string label, string nodeID, string nodeForeignID, string nodeForeignSource)
        {
            _label = label;
            _nodeID = nodeID;
            _nodeForeignID = nodeForeignID;
            _nodeForeignSource = nodeForeignSource;
        }

        //Referential integrity for the Datasource-Class
        public Node WithDatasources(Datasource value)
        {
            //Add a new value to a multi-connection

            //Check if the internal list is initialized
            if (this.datasources == null) { this.datasources = new List<Datasource>(); }

            //Set the new value and tell the old value that it is no longer connected
            if (!this.datasources.Contains(value))
            {
                this.datasources.Add(value);
                value.setNode(this);
            }

            return this;
        }

        public Node WithDatasources(List<Datasource> dataSources)
        {
            //Just for the case you want to add more than one value
            //Calls the "Add single value"-Method for every item in the given list
            foreach (Datasource dataSource in dataSources)
            {
                dataSources.Add(dataSource);
            }

            return this;
        }

        public Node WithoutDatasources(Datasource value)
        {
            //Remove a value from a multi-connection

            //Check if the internal list is initialized and if the value could successfull removed (otherwise it was not in the list)
            //If the value was removed than tell him that it is no longer connected
            if (this.datasources != null && this.datasources.Remove(value))
            {
                value.setRow(null);
            }

            return this;
        }

        public Node WithoutDatasources(List<Datasource> value)
        {
            //Just for the case you want to remove more than one value
            //Calls the "Remove single value"-Method for every item in the given list
            foreach (Datasource item in value)
            {
                this.WithoutDatasources(item);
            }

            return this;
        }

        override
        public string ToString()
        {
            //ToString() gets called if the view tries to "render" an object
            //and dont know how to handle it
            return _label;
        }
    }
}
