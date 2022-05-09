﻿using System;
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

namespace GrafanaDashboardCreator.Resource
{
    internal static class SettingsIO
    {
        internal static void CheckForFileSystem()
        {
            if (!Directory.Exists(DataStoreDirectory))
            {
                Directory.CreateDirectory(DataStoreDirectory);
            }

            if (!File.Exists(NodeXmlFilePath))
            {
                XmlDocument emptyxml = new XmlDocument();
                emptyxml.Save(NodeXmlFilePath);
            }

            if (!File.Exists(SNMPXmlFilePath))
            {
                XmlDocument emptyxml = new XmlDocument();
                emptyxml.Save(SNMPXmlFilePath);
            }

            if (!File.Exists(DashboardJSONFilePath))
            {
                JObject emptyJson = new JObject();
                File.WriteAllText(DashboardJSONFilePath, emptyJson.ToString());
            }

            if (!File.Exists(RowJSONFilePath))
            {
                JObject emptyJson = new JObject();
                File.WriteAllText(RowJSONFilePath, emptyJson.ToString());
            }
        }

        internal static void SaveTempalte(Template template)
        {
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
            catch { } //TODO
        }

        internal static List<Template> LoadTemplates()
        {
            List<Template> templates = new List<Template>();

            foreach (string file in Directory.GetFiles(PanelTemplateDirectory))
            {
                templates.Add(LoadTemplate(file));
            }

            return templates;
        }

        internal static Template LoadTemplate(string path)
        {
            XmlDocument templateXml = XMLParser.GetXMLDocumentFromFile(path);
            XmlNode xmlNode = templateXml.DocumentElement;

            string name = xmlNode.Attributes.GetNamedItem("Name").Value;
            bool replaceNodeID = bool.Parse(xmlNode.Attributes.GetNamedItem("ReplaceNodeID").Value);
            bool replaceResourceID = bool.Parse(xmlNode.Attributes.GetNamedItem("ReplaceResourceID").Value);
            string jsonText = JObject.Parse(templateXml.GetElementsByTagName("jsonText").Item(0).InnerText).ToString();

            return new Template(name, jsonText, replaceNodeID, replaceResourceID);
        }
    }
}