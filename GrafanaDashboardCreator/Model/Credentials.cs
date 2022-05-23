using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using static GrafanaDashboardCreator.Resource.Constants;

namespace GrafanaDashboardCreator.Model
{
    public class Credentials
    {
        //Hidden properties of the Credentials-Class
        private readonly string _name;
        private string _username;
        private string _password;
        private string _token;
        private string _url;

        //Public properties of the Credentials-Class
        public string Name { get { return _name; } }
        public string Username { get { return _username; } set { _username = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        public string Token { get { return _token; } set { _token = value; } }
        public string Url { get { return _url; } set { _url = value; } }

        //Constructor of the Credentials-Class
        public Credentials(string name, string username, string password, string token, string url)
        {
            this._name = name;
            this._username = username;
            this._password = password;
            this._token = token;
            this._url = url;
        }

        public void SaveToOpenNMSFile()
        {
            XmlDocument openNMSCredentials = new XmlDocument();
            openNMSCredentials.Load(OpenNMSCredentailsFilePath);
            XmlNode credentials = openNMSCredentials.SelectSingleNode(OpenNMSCredentailsXmlNode);
            credentials.Attributes["url"].Value = _url;
            credentials.Attributes["username"].Value = _username;
            credentials.Attributes["password"].Value = _password;
            openNMSCredentials.Save(OpenNMSCredentailsFilePath);
        }

        public void SaveToGrafanaFile()
        {
            XmlDocument grafanaCredentials = new XmlDocument();
            grafanaCredentials.Load(GrafanaCredentailsFilePath);
            XmlNode credentials = grafanaCredentials.SelectSingleNode(GrafanaCredentailsXmlNode);
            credentials.Attributes["url"].Value = _url;
            credentials.Attributes["token"].Value = _token;
            grafanaCredentials.Save(GrafanaCredentailsFilePath);
        }
    }
}
