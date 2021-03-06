using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.IO;
using GrafanaDashboardCreator.Model;

using static GrafanaDashboardCreator.Resource.Constants;
using GrafanaDashboardCreator.View;
using System.Windows;

namespace GrafanaDashboardCreator.Parser
{
    internal static class XMLParser
    {
        internal static void GetResourcesFromXML(ModelService modelService, string resourcesXML)
        {
            //Parses the resources from the given xml-text
            try
            {
                XmlDocument resources = new XmlDocument();
                resources.LoadXml(resourcesXML);
                XmlNode node = resources.SelectSingleNode("resource");
                XmlNode internalResources = node.SelectSingleNode("children");

                char[] seperators = { '.' };

                string nodeID = GetNodeID(internalResources);
                string nodeName = node.Attributes.GetNamedItem("name").Value;
                string nodeLabel = node.Attributes.GetNamedItem("label").Value;

                foreach (XmlNode resource in internalResources.ChildNodes)
                {
                    if (resource.Attributes.GetNamedItem("typeLabel").Value == "SNMP Node Data")
                    {
                        //The "SNMP Node Data" rescource is a special resource
                        //Can be later be used if needed, currently just for skipping this node
                    }
                    else
                    {
                        string label = resource.Attributes.GetNamedItem("label").Value;
                        string resourceID = resource.Attributes.GetNamedItem("id").Value.Split(seperators, 2)[1];

                        modelService.CreateDatasource(label, resourceID, modelService.GetSpecificNode(nodeLabel, nodeName, nodeID));
                    }
                }

                modelService.SortDataSources();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                MessageBox.Show(ex.StackTrace, "Error!");
            }
        }

        internal static void GetNodesFromXML(ModelService modelService, string nodesXML)
        {
            //Parses the nodes from the given xml-text
            try
            {
                XmlDocument nodes = new XmlDocument();
                nodes.LoadXml(nodesXML);
                XmlNode internalNodes = nodes.SelectSingleNode("nodes");

                foreach (XmlNode node in internalNodes.ChildNodes)
                {
                    string label = node.Attributes.GetNamedItem("label").Value;
                    string nodeID = node.Attributes.GetNamedItem("id").Value;
                    string nodeForeignID = node.Attributes.GetNamedItem("foreignId").Value;
                    string nodeForeignSource = node.Attributes.GetNamedItem("foreignSource").Value;

                    modelService.CreateNode(label, nodeID, nodeForeignID, nodeForeignSource);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                MessageBox.Show(ex.StackTrace, "Error!");
            }
        }

        internal static string GetNodeID(XmlNode resources)
        {
            //Parses the node id for the given resources
            //Its not an attribute, its stored in each resource, so 
            //its needed to parse it this way
            try
            {
                foreach (XmlNode resource in resources.ChildNodes)
                {
                    if (resource.Attributes.GetNamedItem("typeLabel").Value == "SNMP Interface Data")
                    {
                        XmlNode externalValueAttributes = resource.SelectSingleNode("externalValueAttributes");

                        foreach (XmlNode entry in externalValueAttributes.ChildNodes)
                        {
                            if (entry.SelectSingleNode("key").InnerText == "nodeId")
                            {
                                return entry.SelectSingleNode("value").InnerText;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                MessageBox.Show(ex.StackTrace, "Error!");
            }

            return null;
        }

        internal static Dictionary<string, string> GetOpenNMSCredentials()
        {
            //Parses the credentials for OpenNMS stored in the xml file
            XmlDocument openNMSCredentialsXml = new XmlDocument();
            openNMSCredentialsXml.Load(OpenNMSCredentailsFilePath);
            XmlNode credentials = openNMSCredentialsXml.SelectSingleNode(OpenNMSCredentailsXmlNode);

            Dictionary<string, string> result = new Dictionary<string, string>();

            result["url"] = credentials.Attributes["url"].Value;
            result["username"] = credentials.Attributes["username"].Value;
            result["password"] = credentials.Attributes["password"].Value;

            return result;
        }

        internal static Dictionary<string, string> GetGrafanaCredentials()
        {
            //Parses the credentials for Grafana stored in the xml file
            XmlDocument openNMSCredentialsXml = new XmlDocument();
            openNMSCredentialsXml.Load(GrafanaCredentailsFilePath);
            XmlNode credentials = openNMSCredentialsXml.SelectSingleNode(GrafanaCredentailsXmlNode);

            Dictionary<string, string> result = new Dictionary<string, string>();

            result["url"] = credentials.Attributes["url"].Value;
            result["token"] = credentials.Attributes["token"].Value;

            return result;
        }
    }
}
