using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Resto.Front.Api.ScanReaderPlugin
{
    internal class StringMethods
    {
        public static string stringTransform(string data)
        {
            string result = "";

            try
            {
                char[] separators = new char[] { ' ' };
                string firstString = data.Substring(39, 5);
                string secondString = data.Substring(44, 3);
                string thirdString = data.Substring(52, 5);

                firstString = modifiedString(firstString, 5);
                secondString = modifiedString(secondString, 2);
                thirdString = modifiedString(thirdString, 5);

                result = firstString + secondString + thirdString;
            }
            catch (Exception ex)
            {
                PluginContext.Log.Error($"stringTransform error: message: {ex.Message}");
            }

            return result;
        }

        private static string modifiedString(string str, int strLenght)
        {
            str = str.Replace(" ", "");

            if (str.Length < strLenght)
            {
                while (str.Length < strLenght)
                {
                    str = '0' + str;
                }
            }

            return str;
        }
    }
}
