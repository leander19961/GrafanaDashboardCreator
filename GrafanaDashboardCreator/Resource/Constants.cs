﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaDashboardCreator.Resource
{
    internal static class Constants
    {
        //Constants for IO
        internal static readonly string ProgramDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        internal static readonly string DataStoreDirectory = ProgramDirectory + "\\datastore";
        internal static readonly string TemplateDirectory = DataStoreDirectory + "\\Templates";
        internal static readonly string PanelTemplateDirectory = TemplateDirectory + "\\PanelTemplates";

        //XML
        internal static readonly string NodeXmlFilePath = DataStoreDirectory + "\\nodes.xml";
        internal static readonly string SNMPXmlFilePath = DataStoreDirectory + "\\snmp.xml";
        internal static readonly string ResourcesXmlFilePath = DataStoreDirectory + "\\resources.xml";
        //JSON
        internal static readonly string DashboardJSONFilePath = DataStoreDirectory + "\\emptydashboard.json";
        internal static readonly string RowJSONFilePath = DataStoreDirectory + "\\emptyrow.json";

        //Constants for REST
        internal static readonly string RESTUsername = "admin";
        internal static readonly string RESTPassword = "admin";
        internal static readonly string RESTEncoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                                            .GetBytes(RESTUsername + ":" + RESTPassword));
        internal static readonly string RESTBaseURL = "http://nms.dc.gls/opennms/rest";
        internal static readonly string RESTGETNodeURL = RESTBaseURL + "/nodes?limit=0";
        internal static readonly string RESTGETResourcesURL = RESTBaseURL + "/resources/fornode/§NodeID§";
        internal static readonly string RESTGETSNMPURL = RESTBaseURL + "/nodes/§NodeID§/snmpinterfaces?limit=0";

        //Constants for JSONParse
        //Dashboard
        internal static readonly string JSONDashboardTitlePropertyName = "title";
        internal static readonly string JSONDashboardPanelsPropertyName = "panels";
        //Row
        internal static readonly string JSONRowIDPropertyName = "id";
        internal static readonly string JSONRowTitlePropertyName = "title";
        internal static readonly string JSONRowPanelsPropertyName = "panels";
        internal static readonly string JSONRowTypePropertyName = "type";
        internal static readonly string JSONRowTypePropertyValue = "row";
        internal static readonly string JSONRowGridPosPropertyName = "gridPos";
        //Panel
        internal static readonly string JSONPanelIDPropertyName = "id";
        internal static readonly string JSONPanelTitlePropertyName = "title";
        internal static readonly string JSONPanelNodeIDPropertyName = "nodeId";
        internal static readonly string JSONPanelResourceIDPropertyName = "resourceId";
        internal static readonly string JSONPanelDatasourcePropertyName = "datasource";
        internal static readonly string JSONPanelDatasourcePropertyValue = "OpenNMS Performance";
        internal static readonly string JSONPanelTargetsPropertyName = "targets";
        internal static readonly string JSONPanelTypePropertyName = "type";
        internal static readonly string JSONPanelTypePropertyValue = "graph";
        internal static readonly string JSONPanelGridPosPropertyName = "gridPos";
        //GridPos
        internal static readonly string JSONGridPosPropertyX = "x";
        internal static readonly string JSONGridPosPropertyY = "y";
        internal static readonly string JSONGridPosPropertyW = "w";
        internal static readonly string JSONGridPosPropertyH = "h";

        //ReplacePattern
        internal static readonly string ReplacePatternRESTNodeID = "§NodeID§";
    }
}
