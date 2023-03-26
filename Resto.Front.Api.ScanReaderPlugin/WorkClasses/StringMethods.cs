using Resto.Front.Api.Data.Brd;
using Resto.Front.Api.Data.View;
using Resto.Front.Api.Extensions;
using Resto.Front.Api.UI;
using System;
using System.Collections.Generic;

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

        public static Guid GetCustomerId(IOperationService os, string card)
        {
            Guid result = new Guid();

            try
            {
                PhoneDto phoneDto = new PhoneDto();
                phoneDto.PhoneValue = "+71234123499";
                phoneDto.IsMain = true;
                List<PhoneDto> phones = new List<PhoneDto>();
                phones.Add(phoneDto);
                os.CreateClient(Guid.NewGuid(), "asdasd", phones, card, DateTime.Now, os.AuthenticateByPin(Immutable.pin));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("is already bound"))
                {
                    string id = ex.Message.Split('(')[1];
                    id = id.Split(')')[0];
                    result = Guid.Parse(id);
                }
            }
            return result;
        }
    }
}
