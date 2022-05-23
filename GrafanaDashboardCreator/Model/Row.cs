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
        //Hidden properties of the Row-Class
        private string _name;
        private List<Datasource> _datasources;
        private readonly TabItem _linkedTabItem;
        private readonly ListView _linkedListView;
        private Dashboard _dashboard;

        //Public properties of the Row-Class
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

        public List<Datasource> Datasources { get { return _datasources; } }

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

        //Constructor of the Row-Class
        public Row(string name, TabItem tabItem, ListView listView)
        {
            _name = name;
            _linkedTabItem = tabItem;
            _linkedListView = listView;
            _datasources = new List<Datasource>();
        }

        //Referential integrity for the Row-Class
        public Row SetDashboard(Dashboard value)
        {
            //Set a single connection

            //Check if the given value is null
            //If you want to "delte" a connection
            if (this._dashboard == value)
            {
                return this;
            }

            //Set the new value and tell the old value that it is no longer connected
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
            //Add a new value to a multi-connection

            //Check if the internal list is initialized
            if (this._datasources == null) { this._datasources = new List<Datasource>(); }

            //Check if the value is already connected
            //If not then add the new value and tell him it is connected
            if (!this._datasources.Contains(value))
            {
                this._datasources.Add(value);
                value.setRow(this);
            }

            return this;
        }

        public Row WithDatasources(List<Datasource> datasources)
        {
            //Just for the case you want to add more than one value
            //Calls the "Add single value"-Method for every item in the given list
            foreach (Datasource datasource in datasources)
            {
                WithDatasources(datasource);
            }

            return this;
        }

        public Row WithoutDatasources(Datasource value)
        {
            //Remove a value from a multi-connection

            //Check if the internal list is initialized and if the value could successfull removed (otherwise it was not in the list)
            //If the value was removed than tell him that it is no longer connected
            if (this._datasources != null && this._datasources.Remove(value))
            {
                value.setRow(null);
            }

            return this;
        }

        public Row WithoutDatasources(List<Datasource> value)
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
            return _name;
        }

        internal void RemoveYou()
        {
            //If you want to remove an object of this class
            //Tells all connection that it no longer exists
            if (this._datasources != null)
            {
                foreach (Datasource dataSource in this._datasources)
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
