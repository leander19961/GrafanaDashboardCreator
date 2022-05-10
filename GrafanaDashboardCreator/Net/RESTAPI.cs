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
        //Returns XML
        internal static string GETNodesFromOpenNMS()
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(RESTOpenNMSGETNodesURL);

            httpRequest.Headers["Authorization"] = "Basic " + RESTOpenNMSEncoded;
            httpRequest.Method = "GET";

            string result = "";
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        //Returns XML
        internal static string GETResourcesFromOpenNMS(string nodeID)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(RESTOpenNMSGETResourcesURL.Replace(ReplacePatternRESTNodeID, nodeID));

            httpRequest.Headers["Authorization"] = "Basic " + RESTOpenNMSEncoded;
            httpRequest.Method = "GET";

            string result = "";
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        //Returns Json
        internal static string GETFoldersFromGrafana(string nodeID)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(RESTGrafanaGETFoldersURL);

            httpRequest.Headers["Authorization"] = "Bearer " + RESTGrafanaToken;
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

        //Expects Json
        internal static string POSTJsonToGrafana(string json)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(RESTGrafanaPOSTDashboardURL);

            httpRequest.Headers["Authorization"] = "Bearer " + RESTGrafanaToken;
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
