using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GrafanaDashboardCreator.Resource.Constants;

namespace GrafanaDashboardCreator.Model
{
    public class Datasource : INotifyPropertyChanged
    {
        //Hidden properties of the Datasource-Class
        private readonly string _label;
        private readonly string _resourceID;

        private Node _node;
        private Template _template;

        //Public properties of the Datasource-Class
        public string Label { get => _label; }
        public string ResourceID { get { return _resourceID; } }
        public string NodeID { get { return _node.NodeForeignSource + ':' + _node.NodeForeignID; } }

        public Node Node 
        { 
            get { return _node; }
            set { _node = value; }
        }
        public Template Template
        { 
            get { return _template; } 
            set { _template = value; }
        }

        private Row _row;

        public Row Row { get => _row; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        //Constructor of the Datasource-Class
        public Datasource(string label, string resourceID, Node node)
        {
            _label = label;
            _resourceID = resourceID;
            _node = node;
        }

        //Referential integrity for the Datasource-Class
        public Datasource setRow(Row value)
        {
            //Set a single connection

            //Check if the given value is null
            //If you want to "delte" a connection
            if (this._row == value)
            {
                return this;
            }

            //Set the new value and tell the old value that it is no longer connected
            Row oldValue = this._row;
            if (this._row != null)
            {
                this._row = null;
                oldValue.WithoutDatasources(this);
            }
            this._row = value;
            if (value != null)
            {
                value.WithDatasources(this);
            }
            return this;
        }

        public Datasource setNode(Node value)
        {
            //Set a single connection

            //Check if the given value is null
            //If you want to "delte" a connection
            if (this._node == value)
            {
                return this;
            }

            //Set the new value and tell the old value that it is no longer connected
            Node oldValue = this._node;
            if (this._node != null)
            {
                this._node = null;
                oldValue.WithoutDatasources(this);
            }
            this._node = value;
            if (value != null)
            {
                value.WithDatasources(this);
            }
            return this;
        }

        internal Datasource CopyYou()
        {
            //Returns a new datasource that is an exact clone
            return new Datasource(_label, _resourceID, _node);
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
