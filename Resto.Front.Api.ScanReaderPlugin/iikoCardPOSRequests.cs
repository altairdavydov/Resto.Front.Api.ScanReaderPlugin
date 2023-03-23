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
        
        public static string GetToken(IViewManager vm = null)
        {
            string result = "test";

            var request = (HttpWebRequest)WebRequest.Create($"http://localhost:7001/api/v1/getAccessToken?userName={Immutable.card5Login}&password={Immutable.card5Pass}");
            request.Method = "GET";

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var responseReader = new StreamReader(responseStream))
                {
                    result = responseReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                vm.ShowOkPopup("responce", ex.Message);
            }
            PluginContext.Log.Info(result);
            //vm.ShowOkPopup("responce", result);

            return result;
        }

        private static void GetGuestBalance()
        {

        }
    }
}
