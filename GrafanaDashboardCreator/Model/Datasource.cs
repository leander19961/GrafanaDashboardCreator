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
        private readonly string _label;
        private readonly string _resourceID;

        private Node _node;
        private Template _template;

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

        public Datasource(string label, string resourceID, Node node)
        {
            _label = label;
            _resourceID = resourceID;
            _node = node;
        }

        public Datasource setRow(Row value)
        {
            if (this._row == value)
            {
                return this;
            }

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
            if (this._node == value)
            {
                return this;
            }

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
            return new Datasource(_label, _resourceID, _node);
        }

        override
        public string ToString()
        {
            return _label;
        }
    }
}
