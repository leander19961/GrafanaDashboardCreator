using GrafanaDashboardCreator.Parser;
using GrafanaDashboardCreator.View;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static GrafanaDashboardCreator.Resource.Constants;

namespace GrafanaDashboardCreator.Net
{
    internal static class RESTAPI
    {
        ///<summary>
        ///Returns XML
        ///</summary>
        internal static string GETNodesFromOpenNMS()
        {
            string result = "";

            try
            {
                Dictionary<string, string> crecentials = XMLParser.GetOpenNMSCredentials();

                string baseUrl = crecentials["url"];
                string username = crecentials["username"];
                string password = crecentials["password"];
                string encodedCredentials = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(baseUrl + RESTOpenNMSGETNodesURL);

                httpRequest.Headers["Authorization"] = "Basic " + encodedCredentials;
                httpRequest.Method = "GET";

                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                MessageBox.Show(ex.StackTrace, "Error!");
            }

            return result;
        }

        ///<summary>
        ///Returns XML
        ///</summary>
        internal static string GETResourcesFromOpenNMS(string nodeID)
        {
            string result = "";

            try
            {
                Dictionary<string, string> crecentials = XMLParser.GetOpenNMSCredentials();

                string baseUrl = crecentials["url"];
                string username = crecentials["username"];
                string password = crecentials["password"];
                string encodedCredentials = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(baseUrl + RESTOpenNMSGETResourcesURL.Replace(ReplacePatternRESTNodeID, nodeID));

                httpRequest.Headers["Authorization"] = "Basic " + encodedCredentials;
                httpRequest.Method = "GET";

                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                MessageBox.Show(ex.StackTrace, "Error!");
            }

            return result;
        }

        ///<summary>
        ///Returns Json
        ///</summary>
        internal static string GETFoldersFromGrafana()
        {
            string result = "";

            try
            {
                Dictionary<string, string> crecentials = XMLParser.GetGrafanaCredentials();

                string baseUrl = crecentials["url"];
                string token = crecentials["token"];
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(baseUrl + RESTGrafanaGETFoldersURL);

                httpRequest.Headers["Authorization"] = "Bearer " + token;
                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";
                httpRequest.Method = "GET";

                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                MessageBox.Show(ex.StackTrace, "Error!");
            }

            return result;
        }

        ///<summary>
        ///Expects Json
        ///</summary>
        internal static string POSTJsonToGrafana(string json)
        {
            string result = "";

            try
            {
                Dictionary<string, string> crecentials = XMLParser.GetGrafanaCredentials();

                string baseUrl = crecentials["url"];
                string token = crecentials["token"];
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(baseUrl + RESTGrafanaPOSTDashboardURL);

                byte[] content = Encoding.UTF8.GetBytes(json);
                httpRequest.Headers["Authorization"] = "Bearer " + token;
                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";
                httpRequest.ContentLength = content.Length;
                httpRequest.Method = "POST";

                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(content, 0, content.Length);
                }

                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                string response = "";
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                JObject responseJSON = JObject.Parse(response);
                MessageBox.Show(responseJSON.ToString(), "Response");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                MessageBox.Show(ex.StackTrace, "Error!");
            }

            return result;
        }
    }
}
