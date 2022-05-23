using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using GrafanaDashboardCreator.Model;
using GrafanaDashboardCreator.Parser;

using static GrafanaDashboardCreator.Resource.Constants;
using System.Windows;

namespace GrafanaDashboardCreator.Resource
{
    internal static class SettingsIO
    {
        //Checks if all needed folders and files are existing
        //If not then it creates the folders and fills the files
        //with minimal data, else the program could crash if
        //it tries to access a folder/file that does not exist
        internal static void CheckForFileSystem()
        {
            if (!Directory.Exists(DataStoreDirectory))
            {
                Directory.CreateDirectory(DataStoreDirectory);
            }

            if (!Directory.Exists(PanelTemplateDirectory))
            {
                Directory.CreateDirectory(PanelTemplateDirectory);
            }

            if (!Directory.Exists(CredentialsDirectory))
            {
                Directory.CreateDirectory(CredentialsDirectory);
            }

            if (!File.Exists(DashboardJSONFilePath))
            {
                JObject emptyDashbooardJSON = JObject.Parse(EmptyDashboardJSON);
                File.WriteAllText(DashboardJSONFilePath, emptyDashbooardJSON.ToString());
            }

            if (!File.Exists(RowJSONFilePath))
            {
                JObject emptyRowJSON = JObject.Parse(EmptyRowJSON);
                File.WriteAllText(RowJSONFilePath, emptyRowJSON.ToString());
            }

            if (!File.Exists(FolderJSONFilePath))
            {
                JObject emptyRowJSON = JObject.Parse(EmptyFolderJSON);
                File.WriteAllText(FolderJSONFilePath, emptyRowJSON.ToString());
            }

            if (!File.Exists(OpenNMSCredentailsFilePath))
            {
                XmlDocument openNMSCredentials = new XmlDocument();
                XmlNode credentials = openNMSCredentials.CreateElement(OpenNMSCredentailsXmlNode);
                credentials.Attributes.Append(openNMSCredentials.CreateAttribute("url"));
                credentials.Attributes.Append(openNMSCredentials.CreateAttribute("username"));
                credentials.Attributes.Append(openNMSCredentials.CreateAttribute("password"));
                openNMSCredentials.AppendChild(credentials);
                openNMSCredentials.Save(OpenNMSCredentailsFilePath);
            }

            if (!File.Exists(GrafanaCredentailsFilePath))
            {
                XmlDocument openNMSCredentials = new XmlDocument();
                XmlNode credentials = openNMSCredentials.CreateElement(GrafanaCredentailsXmlNode);
                credentials.Attributes.Append(openNMSCredentials.CreateAttribute("url"));
                credentials.Attributes.Append(openNMSCredentials.CreateAttribute("token"));
                openNMSCredentials.AppendChild(credentials);
                openNMSCredentials.Save(GrafanaCredentailsFilePath);
            }
        }

        internal static void SaveTempalte(Template template)
        {
            //Saves the given template in a xml file
            XmlDocument templateXml = new XmlDocument();

            XmlNode templateNode = templateXml.CreateElement("Template");
            XmlAttribute name = templateXml.CreateAttribute("Name");
            XmlAttribute replaceNodeID = templateXml.CreateAttribute("ReplaceNodeID");
            XmlAttribute replaceResourceID = templateXml.CreateAttribute("ReplaceResourceID");

            name.Value = template.Name;
            replaceNodeID.Value = template.ReplaceNodeID.ToString();
            replaceResourceID.Value = template.ReplaceResourceID.ToString();

            templateNode.Attributes.Append(name);
            templateNode.Attributes.Append(replaceNodeID);
            templateNode.Attributes.Append(replaceResourceID);

            XmlNode jsonTextNode = templateXml.CreateElement("jsonText");
            jsonTextNode.InnerText = template.JSONtext.Replace(Environment.NewLine, String.Empty);

            templateNode.AppendChild(jsonTextNode);
            templateXml.AppendChild(templateNode);

            try
            {
                templateXml.Save(PanelTemplateDirectory + "\\" + template.Name + ".xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        internal static List<Template> LoadTemplates()
        {
            //Load all templates from existing xml files in the template folder
            List<Template> templates = new List<Template>();

            foreach (string file in Directory.GetFiles(PanelTemplateDirectory))
            {
                Template tmp = LoadTemplate(file);
                if (tmp != null)
                {
                    templates.Add(tmp);
                }
            }

            return templates;
        }

        private static Template LoadTemplate(string path)
        {
            //Loads a template from the given path
            try
            {
                XmlDocument templateXml = new XmlDocument();
                templateXml.Load(path);

                XmlNode xmlNode = templateXml.DocumentElement;

                string name = xmlNode.Attributes.GetNamedItem("Name").Value;
                bool replaceNodeID = bool.Parse(xmlNode.Attributes.GetNamedItem("ReplaceNodeID").Value);
                bool replaceResourceID = bool.Parse(xmlNode.Attributes.GetNamedItem("ReplaceResourceID").Value);
                string jsonText = JObject.Parse(templateXml.GetElementsByTagName("jsonText").Item(0).InnerText).ToString();

                return new Template(name, jsonText, replaceNodeID, replaceResourceID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                return null;
            }
        }
    }
}
