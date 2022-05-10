using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            catch (Exception e)
            {
                JSONViewer viewer = new JSONViewer(e.ToString() + "\n" + e.StackTrace)
                {
                    Title = "Error!",
                    Owner = App.Current.MainWindow
                };

                viewer.Show();
            }

            return dashboardJSON;
        }

        internal static string GetTemplate(string templateTitle, string pathToJSON, bool replaceNodeID, bool replaceResourceID)
        {
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
            catch (Exception e)
            {
                JSONViewer viewer = new JSONViewer(e.ToString() + "\n" + e.StackTrace)
                {
                    Title = "Error!",
                    Owner = App.Current.MainWindow
                };

                viewer.Show();
            }

            return newTemplate.ToString();
        }
    }
}
