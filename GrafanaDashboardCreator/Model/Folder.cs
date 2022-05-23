using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaDashboardCreator.Model
{
    public class Folder
    {
        //Hidden properties of the Folder-Class
        private string _title;
        private string _id;
        private string _uid;

        //Public properties of the Folder-Class
        public string Title { get { return _title; } }
        public string ID { get { return _id; } }
        public string Uid { get { return _uid; } }

        //Constructor of the Folder-Class
        public Folder(string title, string id, string uid)
        {
            _title = title;
            _id = id;
            _uid = uid;
        }

        override
        public string ToString()
        {
            //ToString() gets called if the view tries to "render" an object
            //and dont know how to handle it
            return _title;
        }
    }
}
