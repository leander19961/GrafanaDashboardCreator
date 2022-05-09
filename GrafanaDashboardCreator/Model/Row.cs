using HandlebarsDotNet.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GrafanaDashboardCreator.Model
{
    public class Row : INotifyPropertyChanged
    {
        private string _name;
        private List<Datasource> datasources;
        private readonly TabItem _linkedTabItem;
        private readonly ListView _linkedListView;
        private Dashboard _dashboard;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public TabItem LinkedTabItem { get { return _linkedTabItem; } }

        public ListView LinkedListView { get { return _linkedListView; } }

        public List<Datasource> Datasources { get { return datasources; } }

        public Dashboard Dashboard { get { return _dashboard; } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        public Row(string name, TabItem tabItem, ListView listView)
        {
            _name = name;
            _linkedTabItem = tabItem;
            _linkedListView = listView;
            datasources = new List<Datasource>();
        }

        public Row SetDashboard(Dashboard value)
        {
            if (this._dashboard == value)
            {
                return this;
            }

            Dashboard oldValue = this._dashboard;
            if (this._dashboard != null)
            {
                this._dashboard = null;
                oldValue.WithoutRows(this);
            }
            this._dashboard = value;
            if (value != null)
            {
                value.WithRows(this);
            }

            return this;
        }

        public Row WithDatasources(Datasource value)
        {
            if (this.datasources == null) { this.datasources = new List<Datasource>(); }

            if (!this.datasources.Contains(value))
            {
                this.datasources.Add(value);
                value.setRow(this);
            }

            return this;
        }

        public Row WithDatasources(List<Datasource> datasources)
        {
            foreach (Datasource datasource in datasources)
            {
                WithDatasources(datasource);
            }

            return this;
        }

        public Row WithoutDatasources(Datasource value)
        {
            if (this.datasources != null && this.datasources.Remove(value))
            {
                value.setRow(null);
            }

            return this;
        }

        public Row WithoutDatasources(List<Datasource> value)
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
            return _name;
        }

        internal void RemoveYou()
        {
            if (this.datasources != null)
            {
                foreach (Datasource dataSource in this.datasources)
                {
                    dataSource.setRow(null);
                }
            }

            if (this._dashboard != null)
            {
                _dashboard.WithoutRows(this);
                _dashboard = null;
            }
        }
    }
}
