using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GrafanaDashboardCreator.Helper;
using GrafanaDashboardCreator.Model;
using GrafanaDashboardCreator.View;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using static GrafanaDashboardCreator.Resource.Constants;

namespace GrafanaDashboardCreator.Parser
{
    internal static class JSONParser
    {
        internal static JObject CreateNewDashboardJSON(Dashboard dashboard)
        {
            //Creates a new json-text for the given dashboard
            int id = 1;
            int gridIncrementForX = 0;
            int gridIncrementForY = 1;
            GridPos gridPos = new GridPos();
            string dashboardJSONstring;
            using (StreamReader r = new StreamReader(DashboardJSONFilePath))
            {
                dashboardJSONstring = r.ReadToEnd();
            }

            if (string.IsNullOrEmpty(dashboardJSONstring))
            {
                return null; //TODO
            }

            //Start with dashboard-template
            JObject dashboardJSON = JObject.Parse(dashboardJSONstring);

            try
            {
                //Setting dashboard properties
                foreach (JProperty dashboardProperty in dashboardJSON.Properties())
                {
                    if (dashboardProperty.Name.Equals(JSONPanelTitlePropertyName))
                    {
                        dashboardProperty.Value = dashboard.Name;
                    }

                    //Adding panels to the panels[] from dashboard-template
                    if (dashboardProperty.Name.Equals(JSONDashboardPanelsPropertyName))
                    {
                        JArray dashboardPanels = dashboardProperty.Value as JArray;

                        //Adding free-space panels
                        foreach (Datasource datasource in dashboard.GetFreeSpaceRow().Datasources)
                        {
                            JObject panelJSON = JObject.Parse(datasource.Template.JSONtext);

                            //Setting panel properties
                            foreach (JProperty panelProperty in panelJSON.Properties())
                            {
                                //Panel ID
                                if (panelProperty.Name.Equals(JSONPanelIDPropertyName))
                                {
                                    panelProperty.Value = id;
                                    id++;
                                }

                                //Panel Title
                                if (panelProperty.Name.Equals(JSONPanelTitlePropertyName))
                                {
                                    panelProperty.Value = datasource.Label;
                                }

                                //Panel Position
                                if (panelProperty.Name.Equals(JSONRowGridPosPropertyName))
                                {
                                    JObject gridJSON = panelProperty.Value as JObject;

                                    foreach (JProperty gridProperty in gridJSON.Properties())
                                    {
                                        if (gridProperty.Name.Equals(JSONGridPosPropertyX))
                                        {
                                            gridProperty.Value = gridPos.X;
                                        }

                                        if (gridProperty.Name.Equals(JSONGridPosPropertyY))
                                        {
                                            gridProperty.Value = gridPos.Y;
                                        }

                                        if (gridProperty.Name.Equals(JSONGridPosPropertyW))
                                        {
                                            gridIncrementForX = int.Parse(gridProperty.Value.ToString());
                                        }

                                        if (gridProperty.Name.Equals(JSONGridPosPropertyH))
                                        {
                                            gridIncrementForY = int.Parse(gridProperty.Value.ToString());
                                        }
                                    }

                                    gridPos.IncrementGridPos(gridIncrementForX, gridIncrementForY);
                                }

                                //Panel Target
                                if (panelProperty.Name.Equals(JSONPanelTargetsPropertyName))
                                {
                                    foreach (JObject targets in (JArray)panelProperty.Value)
                                    {
                                        foreach (JProperty target_property in targets.Properties())
                                        {
                                            if (target_property.Name.Equals(JSONPanelResourceIDPropertyName))
                                            {
                                                if (target_property.Name.Equals(JSONPanelResourceIDPropertyName) && datasource.Template.ReplaceResourceID)
                                                {
                                                    target_property.Value = datasource.ResourceID;
                                                }

                                                if (target_property.Name.Equals(JSONPanelNodeIDPropertyName) && datasource.Template.ReplaceNodeID)
                                                {
                                                    target_property.Value = datasource.NodeID;
                                                }
                                            }

                                            if (target_property.Name.Equals(JSONPanelNodeIDPropertyName))
                                            {
                                                target_property.Value = datasource.NodeID;
                                            }
                                        }
                                    }
                                }
                            }

                            dashboardPanels.Add(panelJSON);
                        }

                        //Adding rows
                        foreach (Row row in dashboard.GetRowsWithoutFreeSpace())
                        {
                            string rowJSONstring;
                            using (StreamReader r = new StreamReader(RowJSONFilePath))
                            {
                                rowJSONstring = r.ReadToEnd();
                            }

                            JObject rowJSON = JObject.Parse(rowJSONstring);

                            //Setting row properties
                            foreach (JProperty rowProperty in rowJSON.Properties())
                            {
                                //Row ID
                                if (rowProperty.Name.Equals(JSONRowIDPropertyName))
                                {
                                    rowProperty.Value = id;
                                    id++;
                                }

                                //Row Title
                                if (rowProperty.Name.Equals(JSONRowTitlePropertyName))
                                {
                                    rowProperty.Value = row.Name;
                                }

                                //Row Position
                                if (rowProperty.Name.Equals(JSONRowGridPosPropertyName))
                                {
                                    JObject gridJSON = rowProperty.Value as JObject;

                                    gridPos.NewRow(gridIncrementForY);

                                    foreach (JProperty gridProperty in gridJSON.Properties())
                                    {
                                        if (gridProperty.Name.Equals(JSONGridPosPropertyX))
                                        {
                                            gridProperty.Value = gridPos.X;
                                        }

                                        if (gridProperty.Name.Equals(JSONGridPosPropertyY))
                                        {
                                            gridProperty.Value = gridPos.Y;
                                        }

                                        if (gridProperty.Name.Equals(JSONGridPosPropertyW))
                                        {
                                            gridIncrementForX = int.Parse(gridProperty.Value.ToString());
                                        }
                                    }

                                    gridPos.NewRowInner();
                                }
                            }

                            dashboardPanels.Add(rowJSON);

                            //Adding panels to the row
                            foreach (Datasource datasource in row.Datasources)
                            {
                                JObject panelJSON = JObject.Parse(datasource.Template.JSONtext);

                                //Setting panel properties
                                foreach (JProperty panelProperty in panelJSON.Properties())
                                {
                                    //Panel ID
                                    if (panelProperty.Name.Equals(JSONPanelIDPropertyName))
                                    {
                                        panelProperty.Value = id;
                                        id++;
                                    }

                                    //Panel Title
                                    if (panelProperty.Name.Equals(JSONPanelTitlePropertyName))
                                    {
                                        panelProperty.Value = datasource.Label;
                                    }

                                    //Panel Position
                                    if (panelProperty.Name.Equals(JSONRowGridPosPropertyName))
                                    {
                                        JObject gridJSON = panelProperty.Value as JObject;

                                        foreach (JProperty gridProperty in gridJSON.Properties())
                                        {
                                            if (gridProperty.Name.Equals(JSONGridPosPropertyX))
                                            {
                                                gridProperty.Value = gridPos.X;
                                            }

                                            if (gridProperty.Name.Equals(JSONGridPosPropertyY))
                                            {
                                                gridProperty.Value = gridPos.Y;
                                            }

                                            if (gridProperty.Name.Equals(JSONGridPosPropertyW))
                                            {
                                                gridIncrementForX = int.Parse(gridProperty.Value.ToString());
                                            }

                                            if (gridProperty.Name.Equals(JSONGridPosPropertyH))
                                            {
                                                gridIncrementForY = int.Parse(gridProperty.Value.ToString());
                                            }
                                        }

                                        gridPos.IncrementGridPos(gridIncrementForX, gridIncrementForY);
                                    }

                                    //Panel Target
                                    if (panelProperty.Name.Equals(JSONPanelTargetsPropertyName))
                                    {
                                        foreach (JObject targets in (JArray)panelProperty.Value)
                                        {
                                            foreach (JProperty target_property in targets.Properties())
                                            {
                                                if (target_property.Name.Equals(JSONPanelResourceIDPropertyName) && datasource.Template.ReplaceResourceID)
                                                {
                                                    target_property.Value = datasource.ResourceID;
                                                }

                                                if (target_property.Name.Equals(JSONPanelNodeIDPropertyName) && datasource.Template.ReplaceNodeID)
                                                {
                                                    target_property.Value = datasource.NodeID;
                                                }
                                            }
                                        }
                                    }
                                }
                                dashboardPanels.Add(panelJSON);
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

            return dashboardJSON;
        }

        internal static JObject CreateFolderUploadJSON(Dashboard dashboard)
        {
            //Creates a new json-text for the given dashboard that can be posted to Grafana via REST
            JObject folderJSON = null;

            try
            {
                string dashboardJSONstring = CreateNewDashboardJSON(dashboard).ToString();
                string folderJSONstring = "";
                using (StreamReader r = new StreamReader(FolderJSONFilePath))
                {
                    folderJSONstring = r.ReadToEnd();
                }

                JObject dashboardJSON = JObject.Parse(dashboardJSONstring);

                folderJSON = JObject.Parse(folderJSONstring);
                foreach (JProperty property in folderJSON.Properties())
                {
                    if (property.Name.Equals(JSONFolderDashboardPropertyName))
                    {
                        property.Value = dashboardJSON;
                    }

                    if (property.Name.Equals(JSONFolderFolderIDPropertyName) && dashboard.Folder.ID != null)
                    {
                        property.Value = int.Parse(dashboard.Folder.ID);
                    }

                    if (property.Name.Equals(JSONFolderFolderUIDPropertyName) && dashboard.Folder.Uid != null)
                    {
                        property.Value = dashboard.Folder.Uid;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                MessageBox.Show(ex.StackTrace, "Error!");
            }

            return folderJSON;
        }

        internal static string GetTemplate(string templateTitle, string pathToJSON, bool replaceNodeID, bool replaceResourceID)
        {
            //Parses a template with the given title from the given json file
            //If "replaceNodeID" or "replaceResourceID" then the corresponding 
            //properties are not set to "null" in the created template
            string inputJSONString;
            string path = pathToJSON.Replace("\"", String.Empty);
            using (StreamReader r = new StreamReader(pathToJSON.Replace("\"", String.Empty)))
            {
                inputJSONString = r.ReadToEnd();
            }

            JObject inputJSON = JObject.Parse(inputJSONString);
            JObject newTemplate = null;

            try
            {
                //Properties in dashboard
                foreach (JProperty property in inputJSON.Properties())
                {
                    //Panels[] in dashboard
                    if (property.Name.Equals(JSONDashboardPanelsPropertyName))
                    {
                        //JObjects in Panels[] from dashboard
                        JArray dashboardPanels = JArray.Parse(property.Value.ToString());
                        foreach (JObject row_or_panel in dashboardPanels.Children())
                        {
                            //Properties in JObjects..
                            foreach (JProperty row_or_panel_property in row_or_panel.Properties())
                            {
                                //Panel in Panels[] from dashboard
                                if (row_or_panel_property.Name.Equals(JSONPanelTypePropertyName) && row_or_panel_property.Value.ToString().Equals(JSONPanelTypePropertyValue))
                                {
                                    foreach (JProperty panel_property in row_or_panel.Properties())
                                    {
                                        //Panel with given title in Panels[] from dashboard (free space)
                                        if (panel_property.Name.Equals(JSONDashboardTitlePropertyName) && panel_property.Value.ToString().Equals(templateTitle))
                                        {
                                            newTemplate = row_or_panel;
                                        }
                                    }
                                }

                                //Row in Panels[] from dashboard
                                if (row_or_panel_property.Name.Equals(JSONRowTypePropertyName) && row_or_panel_property.Value.ToString().Equals(JSONRowTypePropertyValue))
                                {
                                    foreach (JProperty row_property in row_or_panel.Properties())
                                    {
                                        //Check for panels in Panels[] from Row
                                        if (row_property.Name.Equals(JSONRowPanelsPropertyName))
                                        {
                                            JArray row_panels = JArray.Parse(row_property.Value.ToString());

                                            if (row_panels.Count > 0)
                                            {
                                                foreach (JObject row_panel in row_panels)
                                                {
                                                    //Panels in Panels[] from row
                                                    foreach (JProperty row_panel_property in row_panel.Properties())
                                                    {
                                                        if (row_panel_property.Name.Equals(JSONPanelTypePropertyName) && row_panel_property.Value.ToString().Equals(JSONPanelTypePropertyValue))
                                                        {
                                                            foreach (JProperty _row_panel_property in row_panel.Properties())
                                                            {
                                                                //Panel with given title in Panels[] from row
                                                                if (_row_panel_property.Name.Equals(JSONDashboardTitlePropertyName) && _row_panel_property.Value.ToString().Equals(templateTitle))
                                                                {
                                                                    newTemplate = row_panel;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (newTemplate == null)
                {
                    return null; //TODO
                }

                foreach (JProperty property in newTemplate.Properties())
                {
                    if (property.Name.Equals(JSONPanelTitlePropertyName))
                    {
                        property.Value = null;
                    }

                    if (property.Name.Equals(JSONRowTitlePropertyName))
                    {
                        property.Value = null;
                    }

                    if (property.Name.Equals(JSONPanelIDPropertyName))
                    {
                        property.Value = null;
                    }

                    if (property.Name.Equals(JSONPanelTargetsPropertyName))
                    {
                        foreach (JObject targets in (JArray)property.Value)
                        {
                            foreach (JProperty target_property in targets.Properties())
                            {
                                if (target_property.Name.Equals(JSONPanelResourceIDPropertyName) && replaceResourceID)
                                {
                                    target_property.Value = null;
                                }

                                if (target_property.Name.Equals(JSONPanelNodeIDPropertyName) && replaceNodeID)
                                {
                                    target_property.Value = null;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }

            return newTemplate.ToString();
        }

        internal static List<Folder> GetFolders(string json)
        {
            //Parses the available Grafana folders out of the given json-text
            List<Folder> folders = new List<Folder>();

            try
            {
                JArray foldersJson = JArray.Parse(json);

                //Main node in the given json
                foreach (JObject folder in foldersJson.Children())
                {
                    //Single folder
                    string title = "";
                    string id = "";
                    string uid = "";
                    foreach (JProperty folder_property in folder.Properties())
                    {
                        //Properties of the current selected folder
                        if (folder_property.Name.Equals(JSONFolderTitlePropertyName))
                        {
                            title = folder_property.Value.ToString();
                        }
                        if (folder_property.Name.Equals(JSONFolderIDPropertyName))
                        {
                            id = folder_property.Value.ToString();
                        }
                        if (folder_property.Name.Equals(JSONFolderUIDPropertyName))
                        {
                            uid = folder_property.Value.ToString();
                        }
                    }

                    folders.Add(new Folder(title, id, uid));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                MessageBox.Show(ex.StackTrace, "Error!");
            }

            return folders;
        }
    }
}
