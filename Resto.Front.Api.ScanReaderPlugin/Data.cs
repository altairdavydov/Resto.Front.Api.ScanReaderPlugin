using System;
using System.Collections.Generic;

namespace Resto.Front.Api.ScanReaderPlugin
{
    internal class Data
    {
        public static string token = "";

        public static List<UserWallets> userWallets = new List<UserWallets>(); 

        public static Immutable pluginData = new Immutable();
    }

    [Serializable]
    public class Immutable
    {
        public string card5Login { get; set; }    //LapTop Test 2177570
        public string card5Pass { get; set; }  //LapTop Test d6q%23*DEc
        public string iikoPosServerUrl { get; set; }
    }

    public class UserData
    {
        public List<UserWallets> userWallets { get; set; }
    }

    public class UserWallets
    {
        public Guid? id { get; set; }
        public string name { get; set; }
        public bool? isInteger { get; set; }
        public decimal? balance { get; set; }
    }
}
