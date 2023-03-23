using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resto.Front.Api.ScanReaderPlugin
{
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
