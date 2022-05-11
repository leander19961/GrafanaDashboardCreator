using GrafanaDashboardCreator.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings;
using System.Threading;
using System.Threading.Tasks;

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
            Dictionary<string, string> crecentials = XMLParser.GetOpenNMSCredentials();

            string baseUrl = crecentials["url"];
            string username = crecentials["username"];
            string password = crecentials["password"];
            string encodedCredentials = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(baseUrl + RESTOpenNMSGETNodesURL);

            httpRequest.Headers["Authorization"] = "Basic " + encodedCredentials;
            httpRequest.Method = "GET";

            string result = "";
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        ///<summary>
        ///Returns XML
        ///</summary>
        internal static string GETResourcesFromOpenNMS(string nodeID)
        {
            Dictionary<string, string> crecentials = XMLParser.GetOpenNMSCredentials();

            string baseUrl = crecentials["url"];
            string username = crecentials["username"];
            string password = crecentials["password"];
            string encodedCredentials = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(baseUrl + RESTOpenNMSGETResourcesURL.Replace(ReplacePatternRESTNodeID, nodeID));

            httpRequest.Headers["Authorization"] = "Basic " + encodedCredentials;
            httpRequest.Method = "GET";

            string result = "";
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        ///<summary>
        ///Returns Json
        ///</summary>
        internal static string GETFoldersFromGrafana()
        {
            Dictionary<string, string> crecentials = XMLParser.GetOpenNMSCredentials();

            string baseUrl = crecentials["url"];
            string token = crecentials["token"];
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(baseUrl + RESTGrafanaGETFoldersURL);

            httpRequest.Headers["Authorization"] = "Bearer " + token;
            httpRequest.Headers["Accept"] = "application/json";
            httpRequest.Headers["Content-Type"] = "application/json";
            httpRequest.Method = "GET";

            string result = "";
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        ///<summary>
        ///Expects Json
        ///</summary>
        internal static string POSTJsonToGrafana(string json)
        {
            Dictionary<string, string> crecentials = XMLParser.GetOpenNMSCredentials();

            string baseUrl = crecentials["url"];
            string token = crecentials["token"];
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(baseUrl + RESTGrafanaPOSTDashboardURL);

            httpRequest.Headers["Authorization"] = "Bearer " + token;
            httpRequest.Headers["Accept"] = "application/json";
            httpRequest.Headers["Content-Type"] = "application/json";
            httpRequest.Method = "POST";
            httpRequest.ContentLength = json.Length;

            string result = "";
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (Stream stream = httpResponse.GetResponseStream())
            {
                stream.Write(Encoding.Unicode.GetBytes(json), 0, json.Length);
            }

            return result;
        }
    }
}
