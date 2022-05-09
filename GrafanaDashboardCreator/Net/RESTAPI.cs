using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

using static GrafanaDashboardCreator.Resource.Constants;

namespace GrafanaDashboardCreator.Net
{
    internal static class RESTAPI
    {
        internal static string GETNodes()
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(RESTGETNodeURL);

            httpRequest.Headers["Authorization"] = "Basic " + RESTEncoded;

            string result = "";
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        internal static string GETResources(string nodeID)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(RESTGETResourcesURL.Replace(ReplacePatternRESTNodeID, nodeID));

            httpRequest.Headers["Authorization"] = "Basic " + RESTEncoded;

            string result = "";
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }
    }
}
