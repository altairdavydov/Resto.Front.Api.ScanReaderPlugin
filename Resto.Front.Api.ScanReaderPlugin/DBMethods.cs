using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Resto.Front.Api.ScanReaderPlugin
{
    internal class DBMethods
    {
        //private static SqlConnection conn;

        public static string DBCheck()
        {
            string result = "default";
            /*
            conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=.\SQLEXPRESS;
                          AttachDbFilename=C:\Windows\ServiceProfiles\iikoCard5POS\AppData\Roaming\iiko\iikoCard5\iikoCard5POS.mdf;
                          Integrated Security=True;
                          User Instance=True";
            try
            {
                conn.Open();
                conn.Close();
                result = "success";
            }
            catch (Exception ex) 
            {
                PluginContext.Log.Error($"error connecto to sql base, message:\r\n{ex}");
                result = "error";
            }*/

            return result;
        }
    }
}
