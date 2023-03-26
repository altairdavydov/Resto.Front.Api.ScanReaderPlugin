using System;
using System.IO;
using System.Xml.Serialization;
using System.Net;

namespace Resto.Front.Api.ScanReaderPlugin
{
    internal class FileMethods
    {
        public static bool ReadConfig()
        {
            bool result = false;
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string immutablePath = "\\iiko\\CashServer\\PluginConfigs\\Resto.Front.Api.ScanReader/ScanReader.config";

            if(File.Exists(appDataPath + immutablePath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Immutable));

                using (FileStream fs = new FileStream(appDataPath + immutablePath, FileMode.OpenOrCreate))
                {
                    try
                    {
                        Data.pluginData = xmlSerializer.Deserialize(fs) as Immutable;

                        if(Data.pluginData.card5Login == "" || Data.pluginData.card5Pass == "")
                        {
                            result = false;
                        }
                        else
                        {
                            Data.pluginData.card5Pass = WebUtility.UrlEncode(Data.pluginData.card5Pass);
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        PluginContext.Log.Error($"Serialization configFile has an error, message: {ex.Message}");
                        result = false;
                    }
                }
            }
            else
            {
                try
                {
                    var configFile = new FileInfo(appDataPath + immutablePath);
                    FileStream fileStream = configFile.Create();
                    fileStream.Close();

                    StreamWriter sw = new StreamWriter(appDataPath + immutablePath, true);
                    sw.WriteLine("<?xml version=\"1.0\"?>\r\n<Immutable>\r\n    <card5Login></card5Login>\r\n    <card5Pass></card5Pass>\r\n    <iikoPosServerUrl>http://localhost:7001</iikoPosServerUrl>\r\n</Immutable>");
                    sw.Close();
                    PluginContext.Log.Error("Empty configFile created, add parameters and restart plugin");
                }
                catch (Exception)
                {
                    PluginContext.Log.Warn("File creation error");
                }
                PluginContext.Log.Error("ConfigFile not found");
            }

            return result;
        }
    }
}
