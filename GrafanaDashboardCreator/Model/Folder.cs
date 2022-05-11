using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaDashboardCreator.Model
{
    public class Folder
    {
        private string _title;
        private string _id;
        private string _uid;

        public string Title { get { return _title; } }
        public string Id { get { return _id; } }
        public string Uid { get { return _uid; } }

        public Folder(string title, string id, string uid)
        {
            _title = title;
            _id = id;
            _uid = uid;
        }

        override
        public string ToString()
        {
            return _title;
        }
    }
}
