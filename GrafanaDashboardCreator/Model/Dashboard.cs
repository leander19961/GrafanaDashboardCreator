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
    public class Dashboard : INotifyPropertyChanged
    {
        private string _name;
        private Folder _folder;
        private List<Row> _rows;
        private readonly TabItem _linkedTabItem;
        private readonly TabControl _linkedTabControl;

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

        public Dashboard(string name, TabItem tabItem, TabControl tabControl)
        {
            _name = name;
            _rows = new List<Row>();
            _linkedTabItem = tabItem;
            _linkedTabControl = tabControl;
        }

        public Dashboard WithRows(Row value)
        {
            if (this._rows == null) { this._rows = new List<Row>(); }

            if (!this._rows.Contains(value))
            {
                this._rows.Add(value);
                value.SetDashboard(this);
            }

            return this;
        }
        public Dashboard WithRows(List<Row> rows)
        {
            foreach (Row row in rows) { WithRows(row); }

            return this;
        }

        public Dashboard WithoutRows(Row value)
        {
            if (this._rows != null && this._rows.Remove(value))
            {
                value.SetDashboard(null);
            }

            return this;
        }

        public Dashboard WithoutRows(List<Row> value)
        {
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
            return _name;
        }

        internal void RemoveYou()
        {
            while (this.GetRows().Count > 0)
            {
                this.WithoutRows(this.GetRows().Last());
            }
        }
    }
}
