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
        internal static readonly string DataStoreDirectory = Path.Combine(ProgramDirectory + "\\datastore");
        internal static readonly string TemplateDirectory = Path.Combine(DataStoreDirectory + "\\Templates");
        internal static readonly string PanelTemplateDirectory = Path.Combine(TemplateDirectory + "\\PanelTemplates");

        //XML
        internal static readonly string NodeXmlFilePath = Path.Combine(DataStoreDirectory + "\\nodes.xml");
        internal static readonly string SNMPXmlFilePath = Path.Combine(DataStoreDirectory + "\\snmp.xml");
        internal static readonly string ResourcesXmlFilePath = Path.Combine(DataStoreDirectory + "\\resources.xml");
        //JSON
        internal static readonly string DashboardJSONFilePath = Path.Combine(TemplateDirectory + "\\emptydashboard.json");
        internal static readonly string RowJSONFilePath = Path.Combine(TemplateDirectory + "\\emptyrow.json");

        //Constants for REST
        //OpenNMS
        //internal static readonly string RESTOpenNMSUsername = "admin";
        //internal static readonly string RESTOpenNMSPassword = "admin";
        internal static readonly string RESTOpenNMSEncoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                                            .GetBytes("admin" + ":" + "admin"));
        internal static readonly string RESTOpenNMSBaseURL = "http://nms.dc.gls/opennms/rest";
        internal static readonly string RESTOpenNMSGETNodesURL = RESTOpenNMSBaseURL + "/nodes?limit=0";
        internal static readonly string RESTOpenNMSGETResourcesURL = RESTOpenNMSBaseURL + "/resources/fornode/§NodeID§";
        internal static readonly string RESTOpenNMSGETSNMPURL = RESTOpenNMSBaseURL + "/nodes/§NodeID§/snmpinterfaces?limit=0";
        //Grafana
        internal static readonly string RESTGrafanaToken = "";
        internal static readonly string RESTGrafanaBaseURL = "http://grafana.dc.gls:3000/api";
        internal static readonly string RESTGrafanaGETFoldersURL = RESTGrafanaBaseURL + "/folders?limit=0";
        internal static readonly string RESTGrafanaPOSTDashboardURL = RESTGrafanaBaseURL + "/dashboards/db";

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

        //JSONTemplates
        internal static readonly string EmptyRowJSON = "{  \"collapsed\": true,  \"datasource\": null,  \"gridPos\": {    \"h\": 1,    \"w\": 24,    \"x\": 0,    \"y\": 0  },  \"id\": 4,  \"panels\": [],  \"title\": \"\",  \"type\": \"row\"}";
        internal static readonly string EmptyDashboardJSON = "{  \"annotations\": {    \"list\": [      {        \"builtIn\": 1,        \"datasource\": \"-- Grafana --\",        \"enable\": true,        \"hide\": true,        \"iconColor\": \"rgba(0, 211, 255, 1)\",        \"name\": \"Annotations & Alerts\",        \"type\": \"dashboard\"      }    ]  },  \"editable\": true,  \"gnetId\": null,  \"graphTooltip\": 0,  \"id\": null,  \"links\": [],  \"panels\": [],  \"schemaVersion\": 27,  \"style\": \"dark\",  \"tags\": [],  \"templating\": {    \"list\": []  },  \"time\": {    \"from\": \"now-6h\",    \"to\": \"now\"  },  \"timepicker\": {},  \"timezone\": \"\",  \"title\": \"testdashboard\",  \"uid\": null,  \"version\": 0}";
    }
}
