using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaDashboardCreator.Model
{
    public class Template
    {
        //Hidden properties of the Template-Class
        private readonly string _name;
        private readonly string _jsontext;
        private readonly bool _replaceNodeID;
        private readonly bool _replaceResopurceID;

        //Public properties of the Template-Class
        public string Name { get { return _name; } }
        public string JSONtext { get { return _jsontext; } }
        public string JSONText { get { return _jsontext.Replace(Environment.NewLine, String.Empty); } }
        public bool ReplaceNodeID { get { return _replaceNodeID; } }
        public bool ReplaceResourceID { get { return _replaceResopurceID; } }

        //Constructor of the Template-Class
        public Template(string name, string jsontext, bool replaceNodeID, bool replaceResourceID)
        {
            _name = name;
            _jsontext = jsontext;
            _replaceNodeID = replaceNodeID;
            _replaceResopurceID = replaceResourceID;
        }

        override
        public string ToString()
        {
            return _name;
        }

        internal void RemoveYou()
        {

        }
    }
}
