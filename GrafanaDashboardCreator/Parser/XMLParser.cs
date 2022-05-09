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

namespace GrafanaDashboardCreator.Parser
{
    internal static class XMLParser
    {
        internal static XmlDocument GetXMLDocumentFromFile(String filepath)
        {
            XmlDocument resources = new XmlDocument();
            resources.Load(filepath);

            return resources;
        }

        internal static void GetResourcesFromXML(ModelService modelService, string resourcesXML)
        {
            //XmlDocument resources = GetXMLDocumentFromFile(ResourcesXmlFilePath);
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

                }
                else
                {
                    string label = resource.Attributes.GetNamedItem("label").Value;
                    string resourceID = resource.Attributes.GetNamedItem("id").Value.Split(seperators, 2)[1];

                    modelService.CreateDatasource(label, resourceID, modelService.GetSpecificNodeByName(nodeLabel, nodeName, nodeID));
                }
            }

            modelService.SortDataSources();
        }

        internal static void GetNodesFromXML(ModelService modelService, string nodesXML)
        {
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
            catch (Exception e)
            {
                JSONViewer viewer = new JSONViewer(e.ToString())
                {
                    Owner = App.Current.MainWindow
                };

                viewer.Show();
            }
        }

        internal static string GetNodeID(XmlNode resources)
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

            return null;
        }
    }
}
