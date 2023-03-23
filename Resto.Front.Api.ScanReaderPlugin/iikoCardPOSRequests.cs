using Resto.Front.Api.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Resto.Front.Api.ScanReaderPlugin
{
    internal class iikoCardPOSRequests
    {
        public static void GetToken()
        {
            var request = (HttpWebRequest)WebRequest.Create($"{Immutable.url}/api/v1/getAccessToken?userName={Immutable.card5Login}&password={Immutable.card5Pass}");
            request.Method = "GET";

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var responseReader = new StreamReader(responseStream))
                {
                    Immutable.token = responseReader.ReadToEnd();
                }
                PluginContext.Log.Info($"Received token: {Immutable.token}");
            }
            catch (Exception ex)
            {
                PluginContext.Log.Error($"GetToken method error: {ex.Message}");
            }
        }

        private static void GetGuestBalance()
        {

        }
    }
}
