using Resto.Front.Api.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using Newtonsoft.Json;
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
                    string result = responseReader.ReadToEnd();
                    Immutable.token = result.Substring(1, result.Length - 2);
                }
                PluginContext.Log.Info($"Received token: {Immutable.token}");
            }
            catch (Exception ex)
            {
                PluginContext.Log.Error($"GetToken method error: {ex.Message}");
            }
        }

        public static bool GetGuestBalance(string card)
        {
            bool result = true;
            Immutable.userWallets.Clear();

            var request = (HttpWebRequest)WebRequest.Create($"{Immutable.url}/api/v2/guests/userWallets");
            request.Method = "POST";
            request.Headers["AccessToken"] = Immutable.token;
            request.ContentType = "application/json";

            string postData = $"{{ \"request\": {{ \"authData\": {{ \"credential\": {card}, \"searchScope\":2 }} }} }}";
            byte[] data = Encoding.UTF8.GetBytes(postData);
            Stream webData = request.GetRequestStream();
            webData.Write(data, 0, data.Length);
            webData.Close();

            try
            {
                WebResponse webResponse = request.GetResponse();
                webData = webResponse.GetResponseStream();
                StreamReader reader = new StreamReader(webData);
                string responseFromServer = reader.ReadToEnd();

                var userData = JsonConvert.DeserializeObject<UserData>(responseFromServer);
                if(userData.userWallets.Count > 0)
                {
                    foreach (var userWallets in userData.userWallets)
                    {
                        Immutable.userWallets.Add(userWallets);
                    }
                }

                reader.Close();
                webData.Close();
                webResponse.Close();
                result = true;
            }
            catch (WebException ex)
            {
                using (WebResponse errorResponce = ex.Response)
                {
                    if (errorResponce != null)
                    {
                        HttpWebResponse httpWebResponse = (HttpWebResponse)errorResponce;
                        using (Stream errorData = errorResponce.GetResponseStream())
                        using (var reader = new StreamReader(errorData))
                        {
                            string text = reader.ReadToEnd();
                            if (text.Contains("FaultReason\":53"))
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                            PluginContext.Log.Error($"GetGuestBalance method error, message {text}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PluginContext.Log.Error($"GetGuestBalance method error, message {e}");
            }
            PluginContext.Log.Info($"GetGuestBalance for card: {card}");

            return result;
        }
    }
}
