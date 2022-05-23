using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GrafanaDashboardCreator.Model
{
    public class Dashboard : INotifyPropertyChanged
    {
        //Hidden properties of the Dashboard-Class
        private string _name;
        private Folder _folder;
        private List<Row> _rows;
        private readonly TabItem _linkedTabItem;
        private readonly TabControl _linkedTabControl;

        //Public properties of the Dashboard-Class
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

        public TabControl LinkedTabControl { get { return _linkedTabControl; } }

        public Folder Folder { get { return _folder; } set { _folder = value; } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        //Constructor of the Dashboard-Class
        public Dashboard(string name, TabItem tabItem, TabControl tabControl, Folder folder)
        {
            _name = name;
            _rows = new List<Row>();
            _linkedTabItem = tabItem;
            _linkedTabControl = tabControl;
            _folder = folder;
        }

        //Referential integrity for the Dashboard-Class
        public Dashboard WithRows(Row value)
        {
            //Add a new value to a multi-connection

            //Check if the internal list is initialized
            if (this._rows == null) { this._rows = new List<Row>(); }

            //Check if the value is already connected
            //If not then add the new value and tell him it is connected
            if (!this._rows.Contains(value))
            {
                this._rows.Add(value);
                value.SetDashboard(this);
            }

            return this;
        }
        public Dashboard WithRows(List<Row> rows)
        {
            //Just for the case you want to add more than one value
            //Calls the "Add single value"-Method for every item in the given list
            foreach (Row row in rows) { WithRows(row); }

            return this;
        }

        public Dashboard WithoutRows(Row value)
        {
            //Remove a value from a multi-connection

            //Check if the internal list is initialized and if the value could successfull removed (otherwise it was not in the list)
            //If the value was removed than tell him that it is no longer connected
            if (this._rows != null && this._rows.Remove(value))
            {
                value.SetDashboard(null);
            }

            return this;
        }

        public Dashboard WithoutRows(List<Row> value)
        {
            //Just for the case you want to remove more than one value
            //Calls the "Remove single value"-Method for every item in the given list
            foreach (Row item in value)
            {
                this.WithoutRows(item);
            }

            return this;
        }

        public List<Row> GetRows()
        {
            return _rows;
        }

        public List<Row> GetRowsWithoutFreeSpace()
        {
            //Returns all "true" rows
            //The "FreeSpace" above the first row is handled as row
            List<Row> rows = new List<Row>();

            foreach (Row row in _rows)
            {
                if (!row.Name.Equals("FreeSpace"))
                {
                    rows.Add(row);
                }
            }

            return rows;
        }

        public Row GetFreeSpaceRow()
        {
            //If you want just the "FreeSpace"
            foreach (Row row in _rows)
            {
                if (row.Name.Equals("FreeSpace"))
                {
                    return row;
                }
            }

            return null;
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
            while (this.GetRows().Count > 0)
            {
                this.WithoutRows(this.GetRows().Last());
            }
        }
    }
}
